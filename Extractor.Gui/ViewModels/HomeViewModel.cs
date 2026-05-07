using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Extractor.Gui.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Extractor.Gui.ViewModels;

public partial class HomeViewModel(
    [FromKeyedServices("zip")] IFilePicker zipFilePicker) 
    : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ArchivePathDisplay))]
    public partial string ArchivePath { get; set; } = string.Empty;

    public string ArchivePathDisplay =>
        string.IsNullOrWhiteSpace(ArchivePath) ? string.Empty : $".../{Path.GetFileName(ArchivePath)}";
    
    [RelayCommand]
    private async Task BrowseSetupsArchiveAsync()
    {
        var selectedPath = await zipFilePicker.PickSingleItemAsync("Select a setups archive");
        if (string.IsNullOrEmpty(selectedPath)) return;
        ArchivePath = selectedPath;
    }
}