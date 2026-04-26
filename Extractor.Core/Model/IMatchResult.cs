namespace Extractor.Core.Model;

public interface IMatchResult
{
    bool Success { get; init; }
    string CardinalName { get; init; }
    string MatchedAlias { get; init; }
    int ConfidenceScore { get; init; }
}
