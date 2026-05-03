using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Extractor.Gui.ViewModels;

namespace Extractor.Gui.Views;

public partial class TracksView : UserControl
{
    public TracksView()
    {
        InitializeComponent();
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not TracksViewModel viewModel)
            return;

        await viewModel.InitializeAsync();
    }
}