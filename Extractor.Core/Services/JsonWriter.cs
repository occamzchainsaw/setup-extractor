using System.Text.Encodings.Web;
using System.Text.Json;

namespace Extractor.Core.Services;

public class JsonWriter(string filePath)
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    protected void SaveToFile<T>(T data)
    {
        var serializedData = JsonSerializer.Serialize(data, _serializerOptions);
        File.WriteAllText(filePath, serializedData);
    }
}