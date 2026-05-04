using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Extractor.Gui.ViewModels;

namespace Extractor.Gui.Views;

public partial class SetupShopsView : UserControl
{
    public SetupShopsView()
    {
        InitializeComponent();
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SetupShopsViewModel viewModel)
            return;

        await viewModel.InitializeAsync();
    }
}