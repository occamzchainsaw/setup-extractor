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

public partial class TracksViewModel(
    IMapper mapper,
    IOptionsMonitor<TracksData> tracksMonitor,
    IWriter<TracksData> tracksWriter,
    IExceptionHandler exceptionHandler) 
    : ViewModelBase
{
    private bool _isInitialized;
    [ObservableProperty] public partial bool IsLoading { get; set; } = false;
    [ObservableProperty] public partial TracksDataDto TracksDto { get; set; } = new();

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        IsLoading = true;
        var tempDto = new TracksDataDto();
        try
        {
            await Task.Run(() =>
            {
                var tracks = tracksMonitor.CurrentValue;
                mapper.Map(tracks, tempDto);
            });
        }
        catch (Exception e)
        {
            await exceptionHandler.ShowAsync(e, "Failed to load tracks", "Could not load tracks from tracksData.json.");
        }
        finally
        {
            TracksDto = tempDto;
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
                var tracks = new TracksData();
                mapper.Map(TracksDto, tracks);
                tracksWriter.SaveData(tracks);
            });
        }
        catch (Exception ex)
        {
            await exceptionHandler.ShowAsync(ex, "Failed to save tracks", "Could not save tracks to tracksData.json.");
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
        var tempDto = new TracksDataDto();
        try
        {
            await Task.Run(() =>
            {
                var tracks = tracksMonitor.CurrentValue;
                mapper.Map(tracks, tempDto);
            });
        }
        catch (Exception ex)
        {
            await exceptionHandler.ShowAsync(ex, "Failed to reload tracks", "Could not reload tracks from tracksData.json.");
        }
        finally
        {
            TracksDto = tempDto;
            IsLoading = false;
        }
    }
}
