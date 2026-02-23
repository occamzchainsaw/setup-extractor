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

        // setup local listener
        var loopbackAddress = IPAddress.Parse("127.0.0.1");
        var listener = new TcpListener(loopbackAddress, 0);
        listener.Start();

        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        string redirectUri = $"http://127.0.0.1:{port}/oauth/redirect";
        Console.WriteLine($"Listening on {redirectUri}");

        // Prepare PKCE
        string codeVerifier = GenerateCodeVerifier();
        string codeChallenge = GenerateCodeChallenge(codeVerifier);

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
        var authResult = await WaitForCallbackAsync(listener, state);
        listener.Stop();

        if (authResult.IsError)
        {
            Console.WriteLine($"Error: {authResult.ErrorMessage}");
            return;
        }

        Console.WriteLine($"Authorization code: {authResult.Code[..10]}...");

        // exchange code for tokens
        Console.WriteLine("Exchanging obtained code for tokens");
        var tokenData = await ExchangeCodeForTokensAsync(
            authResult.Code,
            codeVerifier,
            redirectUri
        );

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
    private class AuthResult
    {
        public string Code { get; set; } = string.Empty;
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    private static async Task<AuthResult> WaitForCallbackAsync(
        TcpListener listener,
        string expectedState
    )
    {
        using var client = await listener.AcceptTcpClientAsync();
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.ASCII);
        using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        // read the request line
        if (await reader.ReadLineAsync() is not string requestLine)
            return new AuthResult { IsError = true, ErrorMessage = "Could not read request" };

        // consume headers, but we don't care about them
        while (!string.IsNullOrEmpty(await reader.ReadLineAsync())) { }

        if (string.IsNullOrEmpty(requestLine))
            return new AuthResult { IsError = true, ErrorMessage = "Empty request" };

        // parse query string
        string[] parts = requestLine.Split(' ');
        if (parts.Length < 2)
            return new AuthResult { IsError = true, ErrorMessage = "Ivalid request" };

        string url = parts[1];
        if (!url.Contains("?"))
            return new AuthResult { IsError = true, ErrorMessage = "No query parameters" };

        string queryString = url.Substring(url.IndexOf('?') + 1);
        string? code = null;
        string? incomingState = null;

        foreach (var param in queryString.Split('&'))
        {
            var pair = param.Split('=');
            if (pair.Length == 2)
            {
                if (pair[0] == "code")
                    code = pair[1];
                if (pair[0] == "state")
                    incomingState = pair[1];
            }
        }

        // send success response to browser
        string responseHtml =
            "<html>"
            + "<body style='font-family:sans-serif;'>"
            + "<h1>Authorization Successful</h1>"
            + "<p>You can close the browser window</p>"
            + "</body>"
            + "</html>";

        await writer.WriteAsync("HTTP/1.1 200 OK\r\n");
        await writer.WriteAsync("Content-Type: text/html\r\n");
        await writer.WriteAsync("Connection: close\r\n\r\n");
        await writer.WriteAsync(responseHtml);

        if (incomingState != expectedState)
            return new AuthResult { IsError = true, ErrorMessage = "State Mismatch" };
        if (string.IsNullOrEmpty(code))
            return new AuthResult { IsError = true, ErrorMessage = "Code not found" };

        return new AuthResult { Code = code, IsError = false };
    }

    private static async Task<JsonDocument?> ExchangeCodeForTokensAsync(
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
