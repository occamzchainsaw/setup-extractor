using System.Threading.Tasks;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using Extractor.Gui.Models;
using Extractor.Gui.Services.Interfaces;

namespace Extractor.Gui.ViewModels;

public partial class SettingsViewModel(IMapper mapper, ISettingsRepostory settingsRepository, IFolderPicker folderPicker) : ViewModelBase
{
    private bool _isInitialized;
    [ObservableProperty] public partial bool IsLoading { get; set; } = false;
    [ObservableProperty] public partial CoreConfigDto ConfigDto { get; set; } = new();

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        IsLoading = true;
        try
        {
            await Task.Run(() =>
            {
                var config = settingsRepository.ReadSettings() ?? new CoreConfig();
                mapper.Map(config, ConfigDto);
            });
        }
        catch
        {
        }
        finally
        {
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
                mapper.Map(ConfigDto, config);
                settingsRepository.SaveSettings(config);
            });
        }
        catch
        {
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
        try
        {
            await Task.Run(() =>
            {
                var config = settingsRepository.ReadSettings() ?? new CoreConfig();
                mapper.Map(config, ConfigDto);
            });
        }
        catch
        {
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task BrowseSetupsFolder()
    {
        var selectedPath = await folderPicker.PickFolderAsync("Select iRacing Setups Folder");
        if (string.IsNullOrEmpty(selectedPath)) return;
        ConfigDto.SetupsBasePath = selectedPath;
    }
}