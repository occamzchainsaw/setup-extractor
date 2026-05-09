using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Extractor.Gui.Services.Interfaces;
using Extractor.Gui.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Extractor.Gui.Services;

public partial class PageManager(IServiceProvider services) : ObservableObject
{
    [ObservableProperty] public partial ViewModelBase? CurrentPage { get; private set; }
    [ObservableProperty] public partial string CurrentRoute { get; private set; } = string.Empty;
    
    public void NavigateTo<T>() where T : ViewModelBase, INavigable
    {
        CurrentPage = services.GetRequiredService<T>();
        CurrentRoute = T.Route;
    }
}