namespace Extractor.Core.Model;

public class CoreConfig
{
    public string SetupsBasePath { get; set; } = string.Empty;
    public string PathTemplate { get; set; } = string.Empty;
    public int MinConfidenceScore { get; set; }
}
