using Extractor.Core.Model;

namespace Extractor.Core.Services.Interfaces;

public interface ITracksRepository
{
    void SaveTracks(TracksData tracksData);
    TracksData? ReadTracks();
}