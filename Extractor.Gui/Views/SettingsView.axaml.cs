using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Extractor.Gui.ViewModels;

namespace Extractor.Gui.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewModel viewModel)
            return;

        await viewModel.InitializeAsync();
    }
}