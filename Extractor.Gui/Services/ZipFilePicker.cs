using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Extractor.Gui.Services.Interfaces;

namespace Extractor.Gui.Services;

public class ZipFilePicker : IFilePicker
{
    private readonly FilePickerFileType _archiveType = new("zip archive")
    {
        Patterns = ["*.zip"]
    };
    
    public async Task<IEnumerable<string>> PickMultipleItemsAsync(string title)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return [];

        var window = desktop.MainWindow;
        if (window is null) return [];
        
        var result = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = title,
            AllowMultiple = true,
            FileTypeFilter = [_archiveType]
        });

        if (result.Count == 0) return [];

        List<IStorageFile> selectedItems =
        [
            .. result
                .Where(item => !string.IsNullOrWhiteSpace(item.TryGetLocalPath()))
        ];
        return [..selectedItems.Select(item => item.TryGetLocalPath()!)];
    }

    public async Task<string?> PickSingleItemAsync(string title)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return null;

        var window = desktop.MainWindow;
        if (window is null) return null;
        
        var result = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = [_archiveType]
        });
        
        return result.Count == 0 ? null : result[0].TryGetLocalPath();
    }
}