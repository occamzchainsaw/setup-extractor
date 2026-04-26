using System;
using Extractor.Core.Model;

namespace Extractor.Core.Services;

public static class MatchScoreHelper
{
    public static void CheckScore<T>(ref T bestMatch, int minConfidenceScore, string cardinal, string matchedText, int score)
        where T : IMatchResult, new()
    {
        if (score >= minConfidenceScore && score > bestMatch.ConfidenceScore)
        {
            bestMatch = new T
            {
                Success = true,
                CardinalName = cardinal,
                MatchedAlias = matchedText,
                ConfidenceScore = score,
            };
        }
    }
}
