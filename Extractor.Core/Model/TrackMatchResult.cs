namespace Extractor.Core.Model;

public record TrackMatchResult
{
    public bool Success { get; init; }
    public string CardinalName { get; init; } = string.Empty;
    public string MatchedAlias { get; init; } = string.Empty;
    public int ConfidenceScore { get; init; }

    public static TrackMatchResult NotFound() => new() { Success = false };
}
