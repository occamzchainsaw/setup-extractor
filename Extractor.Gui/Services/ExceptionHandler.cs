using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Extractor.Gui.Services.Interfaces;
using Extractor.Gui.Views;

namespace Extractor.Gui.Services;

public sealed class ExceptionHandler : IExceptionHandler
{
    public Task ShowAsync(Exception exception, string title, string? message = null)
    {
        if (Dispatcher.UIThread.CheckAccess())
            return ShowDialogAsync(exception, title, message);

        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(async () =>
        {
            try
            {
                await ShowDialogAsync(exception, title, message);
                completion.TrySetResult();
            }
            catch (Exception ex)
            {
                completion.TrySetException(ex);
            }
        });

        return completion.Task;
    }

    private static Window? GetOwnerWindow()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return null;

        return desktop.Windows.Count > 0
            ? desktop.Windows[^1]
            : desktop.MainWindow;
    }

    private static Task ShowDialogAsync(Exception exception, string title, string? message)
    {
        var dialog = new ExceptionDialog(title, message, exception);
        var owner = GetOwnerWindow();
        return owner is null
            ? dialog.ShowStandaloneAsync()
            : dialog.ShowDialog(owner);
    }
}
