namespace Extractor.Core.Model;

public class SetupShopMatchResult : IMatchResult
{
    public bool Success { get; init; }
    public string CardinalName { get; init; } = string.Empty;
    public string MatchedAlias { get; init; } = string.Empty;
    public int ConfidenceScore { get; init; }

    public static SetupShopMatchResult NotFound() => new() { Success = false };
}