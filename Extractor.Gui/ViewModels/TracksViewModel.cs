using System;
using System.Threading.Tasks;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Extractor.Core.Model;
using Extractor.Core.Services.Interfaces;
using Extractor.Gui.Models;

namespace Extractor.Gui.ViewModels;

public partial class TracksViewModel(IMapper mapper, ITracksRepository tracksRepository) : ViewModelBase
{
    private bool _isInitialized;
    [ObservableProperty] public partial bool IsLoading { get; set; } = false;
    [ObservableProperty] public partial TracksDataDto TracksDto { get; set; } = new();

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        IsLoading = true;
        try
        {
            await Task.Run(() =>
            {
                var tracks = tracksRepository.ReadTracks();
                mapper.Map(tracks, TracksDto);
            });
        }
        catch (Exception e)
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
                var tracks = new TracksData();
                mapper.Map(TracksDto, tracks);
                tracksRepository.SaveTracks(tracks);
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
                var tracks = tracksRepository.ReadTracks();
                mapper.Map(tracks, TracksDto);
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
}