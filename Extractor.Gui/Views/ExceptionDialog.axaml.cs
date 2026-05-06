using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace Extractor.Gui.Views;

public partial class ExceptionDialog : Window
{
    public Exception Exception { get; set; }
    public string Message { get; set; }
    public string ExceptionType { get; set; }
    public string ExceptionDetails { get; set; }

    public ExceptionDialog()
    {
        InitializeComponent();
        Exception = new Exception();
        Message = string.Empty;
        ExceptionType = string.Empty;
        ExceptionDetails = string.Empty;
    }
    
    public ExceptionDialog(string title, string? message, Exception exception)
    {
        InitializeComponent();
        Title = title;
        Message = message ?? "Unexpected error";
        Exception = exception;
        ExceptionType = $"Type: {exception.GetType().FullName}";
        ExceptionDetails = $"Details: {exception}";
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

    private void CloseOnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
