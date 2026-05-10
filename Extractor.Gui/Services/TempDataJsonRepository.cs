using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Extractor.Gui.Services;

public class TempDataJsonRepository
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public T? ReadData<T>(string fileName) where T : class
    {
        var fullFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        if (!File.Exists(fullFilePath))
            return null;
        var rawData = File.ReadAllText(fullFilePath);
        return JsonSerializer.Deserialize<T>(rawData, _serializerOptions);
    }

    public void SaveData<T>(T data, string fileName) where T : class
    {
        var serializedData = JsonSerializer.Serialize(data, _serializerOptions);
        var fullFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        File.WriteAllText(fullFilePath, serializedData);
    }
}