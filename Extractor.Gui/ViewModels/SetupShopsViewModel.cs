using System;
using System.Threading.Tasks;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using Extractor.Gui.Models;
using Microsoft.Extensions.Options;
using Extractor.Gui.Services.Interfaces;

namespace Extractor.Gui.ViewModels;

public partial class SetupShopsViewModel(
    IMapper mapper,
    IOptionsMonitor<SetupShopsData> setupShopsMonitor,
    IWriter<SetupShopsData> setupShopsWriter,
    IExceptionHandler exceptionHandler) 
    : ViewModelBase
{
    private bool _isInitialized;
    [ObservableProperty] public partial bool IsLoading { get; set; } = false;
    [ObservableProperty] public partial SetupShopsDataDto ShopsDto { get; set; } = new();

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        IsLoading = true;
        var tempDto = new SetupShopsDataDto();
        try
        {
            await Task.Run(() =>
            {
                var shops = setupShopsMonitor.CurrentValue;
                mapper.Map(shops, tempDto);
            });
        }
        catch (Exception e)
        {
            await exceptionHandler.ShowAsync(e, "Failed to load setup shops", "Could not load setup shops from setupShopsData.json.");
        }
        finally
        {
            ShopsDto = tempDto;
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
                var shops = new SetupShopsData();
                mapper.Map(ShopsDto, shops);
                setupShopsWriter.SaveData(shops);
            });
        }
        catch (Exception ex)
        {
            await exceptionHandler.ShowAsync(ex, "Failed to save setup shops", "Could not save setup shops to setupShopsData.json.");
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
        var tempDto = new SetupShopsDataDto();
        try
        {
            await Task.Run(() =>
            {
                var shops = setupShopsMonitor.CurrentValue;
                mapper.Map(shops, tempDto);
            });
        }
        catch (Exception ex)
        {
            await exceptionHandler.ShowAsync(ex, "Failed to reload setup shops", "Could not reload setup shops from setupShopsData.json.");
        }
        finally
        {
            ShopsDto = tempDto;
            IsLoading = false;
        }
    }
}
