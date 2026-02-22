using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Extractor.ApiClient;

class Program
{
    private const string ClientId = "cartrack-agg";
    private const string Scope = "iracing.auth";

    private const string BaseAuthUrl = "https://oauth.iracing.com/oauth2";
    private const string AuthEndpoint = $"{BaseAuthUrl}/authorize";
    private const string TokenEndpoint = $"{BaseAuthUrl}/token";
    private const string AuthRedirectUri = "http://127.0.0.1:0/oauth/redirect";

    private const string BaseDataApiUrl = "https://members-ng.iracing.com/data";

    static async Task Main(string[] args)
    {
        Console.WriteLine("--- iRacing OAuth Console Test ---");

        // Prepare PKCE
        string codeVerifier = GenerateCodeVerifier();
        string codeChallenge = GenerateCodeChallenge(codeVerifier);

        // setup local listener
        int port = GetFreePort();
        string redirectUri = AuthRedirectUri.Replace(":0", $":{port}");
        using var listener = new HttpListener();
        listener.Prefixes.Add(redirectUri + "/");
        listener.Start();
        Console.WriteLine($"Listening on {redirectUri}");

        // setup authorize url
        string state = Guid.NewGuid().ToString("N");
        string authorizeUrl =
            $"{AuthEndpoint}"
            + $"?client_id={ClientId}"
            + $"&redirect_uri={redirectUri}"
            + $"&response_type=code"
            + $"&code_challenge={codeChallenge}"
            + $"&code_challenge_method=S256"
            + $"&state={state}"
            + $"&scope={Scope}";

        // open browser
        Console.WriteLine("Opening browser for authentication...");
        OpenBrowser(authorizeUrl);

        // wait for redirect
        var context = await listener.GetContextAsync();
        var request = context.Request;
        var response = context.Response;

        string? code = request.QueryString.Get("code");
        string? incomingState = request.QueryString.Get("state");

        if (string.IsNullOrEmpty(code) || incomingState != state)
        {
            Console.WriteLine("Error: Invalid state or missing code");
            await SendResponseAsync(response, "Error: Authenticaiton failed or invalid state");
            return;
        }

        // send Success message to browser
        await SendResponseAsync(
            response,
            "<html><body><h1>Authorization Successful</h1><p>You can now close the browser window</p></body></html>"
        );
        listener.Stop();
        Console.WriteLine("Authorization code received");

        // exchange code for tokens
        Console.WriteLine("Exchanging obtained code for tokens");
        var tokenData = await ExchangeCodeForTokensAsync(code, codeVerifier, AuthRedirectUri);

        if (tokenData == null)
        {
            Console.WriteLine("Failed to obtain tokens");
            return;
        }

        if (tokenData.RootElement.GetProperty("access_token").GetString() is not string accessToken)
        {
            Console.WriteLine("Failed to read access token");
            return;
        }
        if (
            tokenData.RootElement.GetProperty("refresh_token").GetString()
            is not string refreshToken
        )
        {
            Console.WriteLine(
                "Failed to read refresh token. Proceeding, but you won't get a refreshed access token if it times out"
            );
        }

        Console.WriteLine($"\nAcessToken: {accessToken[..10]}... (truncated)");

        // try calling the data api
        Console.WriteLine("/nCalling the /data API (/data/doc)...");
        await CallDataApiAsync(accessToken, BaseDataApiUrl + "/doc");
    }

    #region Helpers
    private static async Task<JsonDocument> ExchangeCodeForTokensAsync(
        string code,
        string codeVerifier,
        string redirectUri
    )
    {
        using var client = new HttpClient();

        var content = new FormUrlEncodedContent(
            new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("code_verifier", codeVerifier),
            }
        );

        var response = await client.PostAsync(TokenEndpoint, content);
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Token Error: {response.StatusCode} - {json}");
            return null;
        }

        return JsonDocument.Parse(json);
    }

    private static async Task CallDataApiAsync(string accessToken, string url)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            accessToken
        );

        var response = await client.GetAsync(url);
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"API call failed: {response.StatusCode}");
            Console.WriteLine(json);
            return;
        }

        Console.WriteLine("API call successful");
        try
        {
            var doc = JsonDocument.Parse(json);
            string pretty = JsonSerializer.Serialize(
                doc,
                new JsonSerializerOptions() { WriteIndented = true }
            );
            Console.WriteLine(pretty.Length > 1000 ? pretty[..1000] + "/n..." : pretty);
        }
        catch
        {
            Console.WriteLine(json);
        }
    }
    #endregion

    #region Infrastructure
    private static int GetFreePort()
    {
        using var socket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        );
        socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        return ((IPEndPoint)socket.LocalEndPoint!).Port;
    }

    private static void OpenBrowser(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch
        {
            if (
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                    System.Runtime.InteropServices.OSPlatform.Windows
                )
            )
                Process.Start(
                    new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}")
                    {
                        CreateNoWindow = true,
                    }
                );
            else if (
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                    System.Runtime.InteropServices.OSPlatform.Linux
                )
            )
                Process.Start("xdg-open", url);
            else if (
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                    System.Runtime.InteropServices.OSPlatform.OSX
                )
            )
                Process.Start("open", url);
            else
                Console.WriteLine($"Please open this URL manually: {url}");
        }
    }

    private static async Task SendResponseAsync(
        HttpListenerResponse response,
        string responseString
    )
    {
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        using var output = response.OutputStream;
        await output.WriteAsync(buffer, 0, buffer.Length);
    }

    #endregion

    #region PKCE
    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string GenerateCodeVerifier()
    {
        var rng = RandomNumberGenerator.Create();
        byte[] bytes = new byte[32];
        rng.GetBytes(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        return Base64UrlEncode(challengeBytes);
    }
    #endregion
}
