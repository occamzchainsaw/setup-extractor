using Extractor.Core.Model;

namespace Extractor.Core.Services.Interfaces;

public interface ITrackMatcher
{
    TrackMatchResult TryMatchTrack(string path);
}
