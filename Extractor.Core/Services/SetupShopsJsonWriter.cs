using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;

namespace Extractor.Core.Services;

public class SetupShopsJsonWriter(string filePath) : JsonWriter(filePath), IWriter<SetupShopsData>
{
    public void SaveData(SetupShopsData data)
    {
        SaveToFile(data);
    }
}