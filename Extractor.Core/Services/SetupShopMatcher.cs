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
        var setupShops = setupShopsMonitor.CurrentValue.Shops;
        var minConfidenceScore = configMonitor.CurrentValue.MinConfidenceScore;

        if (setupShops.Count == 0)
            return SetupShopMatchResult.NotFound();
        
        var bestMatch = SetupShopMatchResult.NotFound();

        foreach (var setupShop in setupShops)
        {
            var segments = path.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in segments)
            {
                var sanitizedSegment = segment.SanitizeSpecialChars();
                var cardinalScore = Fuzz.PartialRatio(setupShop.Cardinal.SanitizeSpecialChars(), sanitizedSegment);
                MatchScoreHelper.CheckScore(ref bestMatch, minConfidenceScore, setupShop.Cardinal, segment,
                    cardinalScore);

                foreach (var alias in setupShop.Aliases)
                {
                    var aliasScore = Fuzz.PartialRatio(alias.SanitizeSpecialChars(), sanitizedSegment);
                    MatchScoreHelper.CheckScore(ref bestMatch, minConfidenceScore, alias, segment, aliasScore);
                }
            }
        }
        
        return bestMatch;
    }
}