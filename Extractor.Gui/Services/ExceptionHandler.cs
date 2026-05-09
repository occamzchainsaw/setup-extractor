using System;
using Extractor.Gui.Services.Interfaces;
using ShadUI;

namespace Extractor.Gui.Services;

public sealed class ExceptionHandler(DialogManager dialogManager) : IExceptionHandler
{
    public void ShowDialog(Exception exception, string title)
    {
        var dialogMessage = $"Exception Type: {exception.GetType().Name}\n" +
                            $"Message: {exception.Message}";
        dialogManager
            .CreateDialog(title, dialogMessage)
            .WithPrimaryButton("OK", null)
            .Show();
    }
}
