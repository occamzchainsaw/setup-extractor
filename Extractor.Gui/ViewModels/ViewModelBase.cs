using CommunityToolkit.Mvvm.ComponentModel;
using ShadUI;

namespace Extractor.Gui.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty] public partial DialogManager DialogManager { get; set; }
    [ObservableProperty] public partial ToastManager ToastManager { get; set; }
}
