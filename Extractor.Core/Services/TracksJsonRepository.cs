using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;

namespace Extractor.Core.Services;

public class TracksJsonRepository(string filePath) : JsonRepository(filePath), ITracksRepository
{
    public void SaveTracks(TracksData tracksData)
    {
        SaveToFile(tracksData);
    }

    public TracksData? ReadTracks()
    {
        return ReadFromFile<TracksData>();
    }
}