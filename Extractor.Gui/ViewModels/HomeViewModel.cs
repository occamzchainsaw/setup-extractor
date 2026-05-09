using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Extractor.Core.Services.Interfaces;
using Extractor.Gui.Models;
using Extractor.Gui.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Extractor.Gui.ViewModels;

public partial class HomeViewModel(
    [FromKeyedServices("zip")] IFilePicker zipFilePicker,
    IArchiveHandler archiveHandler,
    IPathContextComposer pathContextComposer,
    IPathComposer pathComposer,
    IExceptionHandler exceptionHandler) 
    : ViewModelBase, INavigable
{
    public static string Route => "home";
    [ObservableProperty] public partial bool IsBusy { get; private set; }
    [ObservableProperty] public partial int Year { get; set; } = DateTime.Now.Year;
    [ObservableProperty] public partial int SeasonNumber { get; set; } = 1;
    [ObservableProperty] public partial int WeekNumber { get; set; } = 1;
    public ObservableCollection<ArchivePathDto> LoadedArchives { get; set; } = [];
    public ObservableCollection<TargetTreeNode> TargetTree { get; set; } = [];
    
    [RelayCommand]
    private async Task BrowseSetupsArchiveAsync()
    {
        IsBusy = true;
        try
        {
            List<string> selectedPaths = [.. await zipFilePicker.PickMultipleItemsAsync("Select setups archives")];
            if (selectedPaths.Count == 0)
                return;
            
            LoadedArchives.Clear();
            foreach (var path in selectedPaths)
            {
                LoadedArchives.Add(new ArchivePathDto { Path = path });
            }
            
            ProduceTargetPaths();
        }
        catch (Exception ex)
        {
            exceptionHandler.ShowDialog(ex, "Failed to parse paths in the archives");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ProduceTargetPaths()
    {
        TargetTree.Clear();
        foreach (var archive in LoadedArchives)
        {
            var archiveNode = new TargetTreeNode { Name = Path.GetFileName(archive.Path) };
            TargetTree.Add(archiveNode);
            
            var archiveItemPaths = 
                archiveHandler.GetAllPathsFromArchive(archive.Path, ".sto");
            foreach (var path in archiveItemPaths)
            {
                var seasonString = $"{Year}S{SeasonNumber}";
                var weekString = $"{WeekNumber}";
                var pathTemplateContext = pathContextComposer.ComposePathTemplateContext(path, seasonString, weekString);
                var setupFilePath = Path.GetFileName(path);
                
                var relativePath = pathComposer.GenerateRelativePath(pathTemplateContext, setupFilePath);
                
                var parts = relativePath.Split(
                    [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], 
                    StringSplitOptions.RemoveEmptyEntries);
                
                var currentNode = archiveNode;
                foreach (var part in parts)
                {
                    var existingChild = currentNode.Children.FirstOrDefault(c => c.Name.Equals(part, StringComparison.OrdinalIgnoreCase));
                    if (existingChild == null)
                    {
                        existingChild = new TargetTreeNode { Name = part };
                        currentNode.Children.Add(existingChild);
                    }
                    currentNode = existingChild;
                }
            }
        }
    }

    [RelayCommand]
    private void Extract()
    {
        try
        {
            throw new NotImplementedException("Not implemented yet");
        }
        catch (Exception ex)
        {
            exceptionHandler.ShowDialog(ex, "Unexpected error");
        }
    }
}