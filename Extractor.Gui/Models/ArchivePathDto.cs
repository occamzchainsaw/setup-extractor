using CommunityToolkit.Mvvm.ComponentModel;

namespace Extractor.Gui.Models;

public partial class ArchivePathDto : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayPath))]
    public partial string Path { get; set; } = string.Empty;

    public string DisplayPath =>
        string.IsNullOrWhiteSpace(Path) ? string.Empty : $".../{System.IO.Path.GetFileName(Path)}";
}