using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;

namespace Extractor.Core.Services;

public class PathContextComposer(IComponentMatcher<CarMatchResult> carMatcher, 
    IComponentMatcher<TrackMatchResult> trackMatcher,
    IComponentMatcher<SetupShopMatchResult> setupShopMatcher) : IPathContextComposer
{
    public PathTemplateContext ComposePathTemplateContext(string archivePath, string season, string week)
    {
        var bestCarMatch = carMatcher.TryMatchComponentFromPath(archivePath);
        var bestTrackMatch = trackMatcher.TryMatchComponentFromPath(archivePath);
        var bestSetupShopMatch = setupShopMatcher.TryMatchComponentFromPath(archivePath);

        return new PathTemplateContext()
        {
            Car = bestCarMatch.Success ? bestCarMatch.CardinalName : string.Empty,
            Track = bestTrackMatch.Success ? bestTrackMatch.CardinalName : string.Empty,
            SetupShop = bestSetupShopMatch.Success ? bestSetupShopMatch.CardinalName : string.Empty,
            Season = $"S{season}",
            SeasonAndWeek = $"S{season}W{week}",
        };
    }
}