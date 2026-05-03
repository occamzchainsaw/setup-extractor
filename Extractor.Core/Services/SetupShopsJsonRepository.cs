using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;

namespace Extractor.Core.Services;

public class SetupShopsJsonRepository(string filePath) : JsonRepository(filePath), ISetupShopsRepository
{
    public void SaveSetupShops(SetupShopsData setupShopsData)
    {
        SaveToFile(setupShopsData);
    }

    public SetupShopsData? ReadSetupShops()
    {
        return ReadFromFile<SetupShopsData>();
    }
}