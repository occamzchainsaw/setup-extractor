using CommunityToolkit.Mvvm.ComponentModel;

namespace Extractor.Gui.Models;

public partial class CoreConfigDto : ObservableObject
{
    [ObservableProperty]
    public partial string SetupsBasePath { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PathTemplate { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int MinConfidenceScore { get; set; }
}