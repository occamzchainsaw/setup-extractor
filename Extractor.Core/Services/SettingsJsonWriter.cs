using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;

namespace Extractor.Core.Services;

public class SettingsJsonWriter(string filePath) : JsonWriter(filePath), IWriter<CoreConfig>
{
    public void SaveData(CoreConfig data)
    {
        SaveToFile(data);
    }
}