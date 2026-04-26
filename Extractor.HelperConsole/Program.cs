using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Extractor.Core.Model;

namespace Extractor.HelperConsole;

class Program
{
    static async Task Main(string[] args)
    {
        var rawJsonText = await File.ReadAllTextAsync("/home/paul/Documents/iracing-tracks.json");
        using var doc = JsonDocument.Parse(rawJsonText);
        List<TrackDenomination> trackDenominations = [];
        foreach (var element in doc.RootElement.EnumerateArray())
        {
            if (!element.TryGetProperty("track_name", out var trackNameProp)) continue;
            
            var trackName = trackNameProp.GetString();
            if (!string.IsNullOrWhiteSpace(trackName) && !trackName.Contains("[Retired]"))
                trackDenominations.Add(new TrackDenomination
                {
                    Cardinal = trackName,
                    Aliases = [ trackName ]
                });
        }
        
        var tracksData = new TracksData()
        {
            Tracks = [.. trackDenominations
                .DistinctBy(x => x.Cardinal)
                .OrderBy(x => x.Cardinal)]
        };
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var jsonString = JsonSerializer.Serialize(tracksData, options);
        await File.WriteAllTextAsync("/home/paul/dev/setup-extractor/tracksData.json", jsonString);
    }
}