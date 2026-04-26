using Extractor.Core.Extensions;
using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using FuzzySharp;
using Microsoft.Extensions.Options;

namespace Extractor.Core.Services;

public class TrackMatcher(IOptionsMonitor<TracksData> tracksMonitor, IOptionsMonitor<CoreConfig> configMonitor)
    : IComponentMatcher<TrackMatchResult>
{
    public TrackMatchResult TryMatchComponentFromPath(string path)
    {
        var tracks = tracksMonitor.CurrentValue.Tracks;
        var minConfidenceScore = configMonitor.CurrentValue.MinConfidenceScore;

        if (tracks.Count == 0)
            return TrackMatchResult.NotFound();
        
        var bestMatch = TrackMatchResult.NotFound();

        foreach (var track in tracks)
        {
            var segments = path.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in segments)
            {
                var sanitizedSegment = segment.SanitizeSpecialChars();
                var cardinalScore = Fuzz.PartialRatio(track.Cardinal.SanitizeSpecialChars(), sanitizedSegment);
                MatchScoreHelper.CheckScore(ref bestMatch, minConfidenceScore, track.Cardinal, segment, cardinalScore);

                foreach (var alias in track.Aliases)
                {
                    var aliasScore = Fuzz.PartialRatio(alias.SanitizeSpecialChars(), sanitizedSegment);
                    MatchScoreHelper.CheckScore(ref bestMatch, minConfidenceScore, alias, segment,  aliasScore);
                }
            }
        }

        return bestMatch;
    }
}
