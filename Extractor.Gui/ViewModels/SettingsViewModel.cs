using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using Extractor.Gui.Models;
using Extractor.Gui.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Extractor.Gui.ViewModels;

public partial class SettingsViewModel(
    IMapper mapper,
    IOptionsMonitor<CoreConfig> configMonitor,
    IWriter<CoreConfig> configWriter,
    [FromKeyedServices("folder")] IFilePicker folderPicker,
    IPathComposer pathComposer,
    IExceptionHandler exceptionHandler)
    : ViewModelBase
{
    private bool _isInitialized;

    public PathTemplateBuilderViewModel PathTemplateBuilder { get; } = new();

    [ObservableProperty] public partial bool IsLoading { get; set; } = false;
    [ObservableProperty] public partial CoreConfigDto ConfigDto { get; set; } = new();

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        IsLoading = true;
        var tempDto = new CoreConfigDto();
        List<PathElement> existingTemplateElements = [];
        try
        {
            await Task.Run(() =>
            {
                var config = configMonitor.CurrentValue;
                mapper.Map(config, tempDto);
                existingTemplateElements = 
                    pathComposer.DeconstructTemplateElementsFromSettings(config.PathTemplate);
            });
        }
        catch (Exception ex)
        {
            await exceptionHandler.ShowAsync(ex, "Failed to load settings", "Could not load settings from coreConfig.json.");
        }
        finally
        {
            ConfigDto = tempDto;
            PathTemplateBuilder.Initialize(existingTemplateElements);
            _isInitialized = true;
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        IsLoading = true;
        try
        {
            await Task.Run(() =>
            {
                var config = new CoreConfig();
                ConfigDto.PathTemplate = 
                    pathComposer.GenerateTemplateStringFromEnums(PathTemplateBuilder.SelectedElementsValues);
                mapper.Map(ConfigDto, config);
                configWriter.SaveData(config);
            });
        }
        catch (Exception ex)
        {
            await exceptionHandler.ShowAsync(ex, "Failed to save settings", "Could not save settings to coreConfig.json.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Reload()
    {
        IsLoading = true;
        var tempDto = new CoreConfigDto();
        List<PathElement> existingTemplateElements = [];
        try
        {
            await Task.Run(() =>
            {
                var config = configMonitor.CurrentValue;
                mapper.Map(config, tempDto);
                existingTemplateElements = 
                    pathComposer.DeconstructTemplateElementsFromSettings(config.PathTemplate);
            });
        }
        catch (Exception ex)
        {
            await exceptionHandler.ShowAsync(ex, "Failed to reload settings", "Could not reload settings from coreConfig.json.");
        }
        finally
        {
            ConfigDto = tempDto;
            PathTemplateBuilder.Initialize(existingTemplateElements);
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task BrowseSetupsFolder()
    {
        var selectedPath = await folderPicker.PickSingleItemAsync("Select iRacing Setups Folder");
        if (string.IsNullOrEmpty(selectedPath)) return;
        ConfigDto.SetupsBasePath = selectedPath;
    }
}
