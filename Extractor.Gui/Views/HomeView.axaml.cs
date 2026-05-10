using Avalonia.Controls;
using Avalonia.Interactivity;
using Extractor.Gui.ViewModels;

namespace Extractor.Gui.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not HomeViewModel viewModel)
            return;

        await viewModel.InitializeAsync();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not HomeViewModel viewModel)
            return;
        
        viewModel.SaveTempData();
    }
}