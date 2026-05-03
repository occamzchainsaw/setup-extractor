using System.Threading.Tasks;

namespace Extractor.Gui.Services.Interfaces;

public interface IFolderPicker
{
    Task<string?> PickFolderAsync(string title);
}