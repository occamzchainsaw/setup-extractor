using System.Text.Encodings.Web;
using System.Text.Json;

namespace Extractor.Core.Services;

public class JsonRepository(string filePath)
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    protected T? ReadFromFile<T>()
    {
        var rawData = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(rawData, _serializerOptions);
    }

    protected void SaveToFile<T>(T data)
    {
        var serializedData = JsonSerializer.Serialize(data, _serializerOptions);
        File.WriteAllText(filePath, serializedData);
    }
}