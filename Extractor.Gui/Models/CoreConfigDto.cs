using System.ComponentModel.DataAnnotations;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Extractor.Gui.Models;

public partial class CoreConfigDto : ObservableObject
{
    [ObservableProperty]
    public partial string SetupsBasePath { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PathTemplateDisplay))]
    public partial string PathTemplate { get; set; } = string.Empty;

    public string PathTemplateDisplay
    {
        get
        {
            var separator = Path.DirectorySeparatorChar;
            var split = PathTemplate.Split(separator);
            return string.Join(" / ", split);
        }
    }

    [ObservableProperty]
    public partial int MinConfidenceScore { get; set; }
}