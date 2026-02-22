namespace Extractor.Core.Model;

public sealed class TracksData
{
    public List<TrackDenomination> Tracks { get; init; } = [];
}

public sealed class TrackDenomination
{
    public string Cardinal { get; init; } = string.Empty;
    public List<string> Aliases { get; set; } = [];
}
