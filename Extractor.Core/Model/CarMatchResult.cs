namespace Extractor.Core.Model;

public record CarMatchResult : IMatchResult
{
    public bool Success { get; init; }
    public string CardinalName { get; init; } = string.Empty;
    public string MatchedAlias { get; init; } = string.Empty;
    public int ConfidenceScore { get; init; }

    public static CarMatchResult NotFound() => new() { Success = false };
}
