using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Extractor.Gui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IServiceProvider _services;
    [ObservableProperty] private ObservableObject _currentPage;

    public MainWindowViewModel(IServiceProvider services)
    {
        _services = services;
        _currentPage = _services.GetRequiredService<HomeViewModel>();
    }

    [RelayCommand]
    private void GoHome() => CurrentPage = _services.GetRequiredService<HomeViewModel>();

    [RelayCommand]
    private void GoSettings() => CurrentPage = _services.GetRequiredService<SettingsViewModel>();
    
    [RelayCommand]
    private void GoTracks() => CurrentPage = _services.GetRequiredService<TracksViewModel>();

    [RelayCommand]
    private void GoSetupShops() => CurrentPage = _services.GetRequiredService<SetupShopsViewModel>();
}
