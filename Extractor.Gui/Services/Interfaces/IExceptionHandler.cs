using System;
using System.Threading.Tasks;

namespace Extractor.Gui.Services.Interfaces;

public interface IExceptionHandler
{
    Task ShowAsync(Exception exception, string title, string? message = null);
}
