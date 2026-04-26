using Extractor.Core.Extensions;
using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using FuzzySharp;
using Microsoft.Extensions.Options;

namespace Extractor.Core.Services;

public class SetupShopMatcher(IOptionsMonitor<SetupShopsData> setupShopsMonitor, IOptionsMonitor<CoreConfig> configMonitor) 
    : IComponentMatcher<SetupShopMatchResult>
{
    public SetupShopMatchResult TryMatchComponentFromPath(string path)
    {
        var setupShopNames = setupShopsMonitor.CurrentValue.Names;
        var minConfidenceScore = configMonitor.CurrentValue.MinConfidenceScore;

        if (setupShopNames.Count == 0)
            return SetupShopMatchResult.NotFound();
        
        var bestMatch = SetupShopMatchResult.NotFound();

        foreach (var setupShopName in setupShopNames)
        {
            var sanitizedShopName = setupShopName.SanitizeSpecialChars();
            var segments = path.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in segments)
            {
                var sanitizedSegment = segment.SanitizeSpecialChars();
                
                if (sanitizedSegment.Equals(setupShopName, StringComparison.OrdinalIgnoreCase)      )
                    MatchScoreHelper.CheckScore(ref bestMatch, minConfidenceScore, setupShopName, segment, 100);

                var fuzzySegmentScore = Fuzz.PartialRatio(sanitizedShopName, sanitizedSegment);
                MatchScoreHelper.CheckScore(ref bestMatch, minConfidenceScore, setupShopName, segment, fuzzySegmentScore);
            }
        }
        
        return bestMatch;
    }
}