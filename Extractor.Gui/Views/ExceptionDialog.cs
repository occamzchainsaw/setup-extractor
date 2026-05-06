using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Extractor.Gui.Views;

public sealed class ExceptionDialog : Window
{
    public ExceptionDialog(string title, string? message, Exception exception)
    {
        Title = title;
        Width = 720;
        Height = 480;
        MinWidth = 480;
        MinHeight = 320;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        var messageBlock = new TextBlock
        {
            Text = message ?? "Unexpected error.",
            TextWrapping = TextWrapping.Wrap
        };

        var exceptionTypeBlock = new TextBlock
        {
            Text = $"Type: {exception.GetType().FullName}",
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            Margin = new Thickness(0, 8, 0, 0)
        };

        var detailsBox = new TextBox
        {
            Text = exception.ToString(),
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var closeButton = new Button
        {
            Content = "Close",
            HorizontalAlignment = HorizontalAlignment.Right,
            MinWidth = 96
        };
        closeButton.Click += (_, _) => Close();

        Content = new Grid
        {
            Margin = new Thickness(16),
            RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto"),
            RowSpacing = 12,
            Children =
            {
                messageBlock,
                exceptionTypeBlock,
                new ScrollViewer
                {
                    Content = detailsBox
                },
                closeButton
            }
        };

        Grid.SetRow(messageBlock, 0);
        Grid.SetRow(exceptionTypeBlock, 1);
        Grid.SetRow((Control)((Grid)Content).Children[2], 2);
        Grid.SetRow(closeButton, 3);
    }

    public Task ShowStandaloneAsync()
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Closed += HandleClosed;
        Show();
        return completion.Task;

        void HandleClosed(object? sender, EventArgs args)
        {
            Closed -= HandleClosed;
            completion.TrySetResult();
        }
    }
}
