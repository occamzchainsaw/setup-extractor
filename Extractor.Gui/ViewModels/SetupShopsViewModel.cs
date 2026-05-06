using System;
using System.Threading.Tasks;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using Extractor.Gui.Models;
using Microsoft.Extensions.Options;

namespace Extractor.Gui.ViewModels;

public partial class SetupShopsViewModel(
    IMapper mapper,
    IOptionsMonitor<SetupShopsData> setupShopsMonitor,
    IWriter<SetupShopsData> setupShopsWriter) 
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
        {}
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
        catch
        {}
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
        catch
        {}
        finally
        {
            ShopsDto = tempDto;
            IsLoading = false;
        }
    }
}