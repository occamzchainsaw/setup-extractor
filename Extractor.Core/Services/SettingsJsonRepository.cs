using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;

namespace Extractor.Core.Services;

public class SettingsJsonRepository(string filePath) : JsonRepository(filePath), ISettingsRepostory
{
    public void SaveSettings(CoreConfig config)
    {
        SaveToFile(config);
    }

    public CoreConfig? ReadSettings()
    {
        return ReadFromFile<CoreConfig>();
    }
}