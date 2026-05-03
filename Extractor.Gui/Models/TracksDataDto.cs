using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Extractor.Gui.Models;

public partial class TracksDataDto : ObservableObject
{
    public ObservableCollection<TrackDenominationDto> Tracks { get; set; } = [];
}

public partial class TrackDenominationDto : ObservableObject
{
    [ObservableProperty] public partial string Cardinal { get; set; } = string.Empty;
    public ObservableCollection<string> Aliases { get; set; } = [];
    [ObservableProperty] public partial string AliasesJoined { get; set; } = string.Empty;
}