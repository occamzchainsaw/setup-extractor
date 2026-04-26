namespace Extractor.Core.Model;

public sealed class TracksData
{
    public List<TrackDenomination> Tracks { get; set; } = [];
}

public sealed class TrackDenomination
{
    public string Cardinal { get; set; } = string.Empty;
    public List<string> Aliases { get; set; } = [];
}
