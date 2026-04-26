using Extractor.Core.Extensions;
using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using FuzzySharp;
using Microsoft.Extensions.Options;

namespace Extractor.Core.Services;

public class CarMatcher(IOptionsMonitor<CoreConfig> configMonitor) : IComponentMatcher<CarMatchResult>
{
    public CarMatchResult TryMatchComponentFromPath(string path)
    {
        var minConfidenceScore = configMonitor.CurrentValue.MinConfidenceScore;
        var carFolders = Directory.GetDirectories(configMonitor.CurrentValue.SetupsBasePath)
            .Select(Path.GetFileName)
            .OfType<string>()
            .ToList();

        var bestMatch = CarMatchResult.NotFound();

        foreach (var carFolder in carFolders)
        {
            var sanitizedCarFolder = carFolder.SanitizeSpecialChars();
            var segments = path.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in segments)
            {
                var sanitizedSegment = segment.SanitizeSpecialChars();

                if (sanitizedSegment.Equals(sanitizedCarFolder, StringComparison.OrdinalIgnoreCase))
                {
                    MatchScoreHelper.CheckScore(ref bestMatch, minConfidenceScore, carFolder, segment, 100);
                }

                var fuzzySegmentScore = Fuzz.PartialRatio(sanitizedCarFolder, sanitizedSegment);
                MatchScoreHelper.CheckScore(ref bestMatch, minConfidenceScore, carFolder, segment, fuzzySegmentScore);
            }
        }

        return bestMatch;
    }
}
