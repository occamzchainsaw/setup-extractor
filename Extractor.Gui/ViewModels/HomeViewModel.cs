using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using Extractor.Gui.Models;
using Extractor.Gui.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Extractor.Gui.ViewModels;

public partial class HomeViewModel(
    [FromKeyedServices("zip")] IFilePicker zipFilePicker,
    IArchiveHandler archiveHandler,
    IPathContextComposer pathContextComposer,
    IPathComposer pathComposer) 
    : ViewModelBase
{
    public ObservableCollection<ArchivePathDto> LoadedArchives { get; set; } = [];
    public ObservableCollection<string> TargetPaths { get; set; } = [];
    
    [RelayCommand]
    private async Task BrowseSetupsArchiveAsync()
    {
        LoadedArchives.Clear();
        var selectedPaths = await zipFilePicker.PickMultipleItemsAsync("Select setups archives");
        foreach (var path in selectedPaths)
        {
            LoadedArchives.Add(new ArchivePathDto { Path = path });
        }
    }

    [RelayCommand]
    private void ProduceTargetPaths()
    {
        foreach (var archive in LoadedArchives)
        {
            var archiveItemPaths = 
                archiveHandler.GetAllPathsFromArchive(archive.Path, ".sto");
            foreach (var path in archiveItemPaths)
            {
                var pathTemplateContext = pathContextComposer.ComposePathTemplateContext(path, "2026S2", "8");
                var setupFilePath = Path.GetFileName(path);
                TargetPaths.Add(pathComposer.GenerateFullPath(pathTemplateContext, setupFilePath));
            }
        }
    }
}