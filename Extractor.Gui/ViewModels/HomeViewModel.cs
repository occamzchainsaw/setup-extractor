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
using Extractor.Gui.Services;
using Extractor.Gui.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ShadUI;

namespace Extractor.Gui.ViewModels;

public partial class HomeViewModel(
    [FromKeyedServices("zip")] IFilePicker zipFilePicker,
    TempDataJsonRepository tempDataRepository,
    IArchiveHandler archiveHandler,
    IPathContextComposer pathContextComposer,
    IPathComposer pathComposer,
    IExceptionHandler exceptionHandler,
    ToastManager toastManager) 
    : ViewModelBase, INavigable
{
    private bool _isInitilized;
    private const string TempDataFileName = "lastInputs.json";
    public static string Route => "home";
    [ObservableProperty] public partial bool IsBusy { get; private set; }
    [ObservableProperty] public partial int Year { get; set; } = DateTime.Now.Year;
    [ObservableProperty] public partial int SeasonNumber { get; set; } = 1;
    [ObservableProperty] public partial int WeekNumber { get; set; } = 1;
    private string SeasonString => $"{Year}S{SeasonNumber}";
    public ObservableCollection<ArchivePathDto> LoadedArchives { get; set; } = [];
    public ObservableCollection<TargetTreeNode> TargetTree { get; set; } = [];

    private readonly Dictionary<string, List<(string InternalPath, string FullTargetPath)>> _extractionPlan = [];

    public async Task InitializeAsync()
    {
        if (_isInitilized) return;

        var tempDto = new UserInputDto();
        try
        {
            await Task.Run(() =>
            {
                tempDto = tempDataRepository.ReadData<UserInputDto>(TempDataFileName);
            });
        }
        catch (Exception ex)
        {
            exceptionHandler.ShowDialog(ex, "Failed to load previous input data");
        }
        finally
        {
            if (tempDto is not null)
            {
                Year = tempDto.SelectedYear;
                SeasonNumber = tempDto.SelectedSeason;
                WeekNumber = tempDto.SelectedWeek;
            }
            _isInitilized = true;
        }
    }
    
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
    
    private void ProduceTargetPaths()
    {
        TargetTree.Clear();
        _extractionPlan.Clear();

        foreach (var archive in LoadedArchives)
        {
            var archiveNode = new TargetTreeNode { Name = Path.GetFileName(archive.Path) };
            TargetTree.Add(archiveNode);

            var planForArchive = new List<(string InternalPath, string FullTargetPath)>();
            _extractionPlan[archive.Path] = planForArchive;

            var archiveItemPaths = 
                archiveHandler.GetAllPathsFromArchive(archive.Path, ".sto");
            foreach (var path in archiveItemPaths)
            {
                var pathTemplateContext = pathContextComposer.ComposePathTemplateContext(path, SeasonString, WeekNumber.ToString());
                var setupFilePath = Path.GetFileName(path);

                var fullTargetPath = pathComposer.GenerateFullPath(pathTemplateContext, setupFilePath);
                planForArchive.Add((path, fullTargetPath));
                
                var relativePath = pathComposer.GenerateRelativePath(pathTemplateContext, setupFilePath);
                var parts = relativePath.Split(
                    [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], 
                    StringSplitOptions.RemoveEmptyEntries);

                var currentNode = archiveNode;
                foreach (var part in parts)
                {
                    var existingChild = currentNode.Children
                        .FirstOrDefault(c => c.Name.Equals(part, StringComparison.OrdinalIgnoreCase));
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
    private async Task Extract()
    {
        if (_extractionPlan.Count == 0) return;

        IsBusy = true;
        try
        {
            await Task.Run(() =>
            {
                foreach (var (archivePath, mapping) in _extractionPlan)
                {
                    archiveHandler.ExtractFiles(archivePath, mapping);
                }
            });
            
            toastManager.CreateToast("Success")
                .WithContent("The setup files have been extracted")
                .DismissOnClick()
                .WithDelay(5)
                .OnBottomCenter()
                .ShowSuccess();
        }
        catch (Exception ex)
        {
            exceptionHandler.ShowDialog(ex, "Failed to extract setups");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    public void SaveTempData()
    {
        try
        {
            var tempDto = new UserInputDto
            {
                SelectedYear = Year,
                SelectedSeason = SeasonNumber,
                SelectedWeek = WeekNumber
            };
            tempDataRepository.SaveData(tempDto, TempDataFileName);
        }
        catch (Exception ex)
        {
            exceptionHandler.ShowDialog(ex, "Failed to save last input data");
        }
    }
}