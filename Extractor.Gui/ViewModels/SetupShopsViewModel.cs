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
    : ViewModelBase, INavigable
{
    private bool _isInitialized;
    public static string Route => "setupShops";
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
        catch (Exception ex)
        {
            exceptionHandler.ShowDialog(ex, "Failed to load setup shops");
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
            exceptionHandler.ShowDialog(ex, "Failed to save setup shops");
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
            exceptionHandler.ShowDialog(ex, "Failed to reload setup shops");
        }
        finally
        {
            ShopsDto = tempDto;
            IsLoading = false;
        }
    }
}
