using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Extractor.Gui.Services.Interfaces;

namespace Extractor.Gui.Services;

public class FolderPicker : IFolderPicker
{
    public async Task<string?> PickFolderAsync(string title)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return null;

        var window = desktop.MainWindow;
        if (window is null) return null;

        var result = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = title,
            AllowMultiple = false
        });

        return result.Count == 0 ? null : result[0].TryGetLocalPath();
    }
}