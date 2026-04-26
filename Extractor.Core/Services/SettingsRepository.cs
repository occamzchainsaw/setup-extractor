using System.Text.Encodings.Web;
using System.Text.Json;
using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;

namespace Extractor.Core.Services;

public class SettingsRepository : ISettingsRepostory
{
    private const string ConfigFileName = "coreCnfig.json";
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    public void SaveSettings(CoreConfig config)
    {
        var serializedOptions = JsonSerializer.Serialize(config, _jsonSerializerOptions);
        File.WriteAllText(ConfigFileName, serializedOptions);
    }
}