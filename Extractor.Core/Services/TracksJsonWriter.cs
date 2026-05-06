using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;

namespace Extractor.Core.Services;

public class TracksJsonWriter(string filePath) : JsonWriter(filePath), IWriter<TracksData>
{
    public void SaveData(TracksData data)
    {
        SaveToFile(data);
    }
}