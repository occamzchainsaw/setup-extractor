using CommunityToolkit.Mvvm.ComponentModel;
using ShadUI;

namespace Extractor.Gui.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty] public partial DialogManager DialogManager { get; set; }
}
