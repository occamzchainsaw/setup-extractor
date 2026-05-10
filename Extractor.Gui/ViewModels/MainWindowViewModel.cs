using CommunityToolkit.Mvvm.Input;
using Extractor.Gui.Services;
using ShadUI;

namespace Extractor.Gui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public PageManager PageManager { get; }

    public MainWindowViewModel(
        PageManager pageManager, 
        DialogManager dialogManager,
        ToastManager toastManager)
    {
        PageManager = pageManager;
        DialogManager = dialogManager;
        ToastManager = toastManager;
        
        PageManager.NavigateTo<HomeViewModel>();
    }

    [RelayCommand]
    private void GoHome() => PageManager.NavigateTo<HomeViewModel>();

    [RelayCommand]
    private void GoSettings() => PageManager.NavigateTo<SettingsViewModel>();
    
    [RelayCommand]
    private void GoTracks() => PageManager.NavigateTo<TracksViewModel>();

    [RelayCommand]
    private void GoSetupShops() => PageManager.NavigateTo<SetupShopsViewModel>();
}
