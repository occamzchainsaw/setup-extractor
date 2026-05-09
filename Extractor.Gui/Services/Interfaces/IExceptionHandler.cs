using System;
using System.Threading.Tasks;

namespace Extractor.Gui.Services.Interfaces;

public interface IExceptionHandler
{
    void ShowDialog(Exception exception, string title);
}
