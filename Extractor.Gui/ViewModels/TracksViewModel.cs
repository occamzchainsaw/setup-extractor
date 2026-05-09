using System;
using System.Threading.Tasks;
using AutoMapper;
using Avalonia.Collections;
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
    : ViewModelBase, INavigable
{
    private bool _isInitialized;
    public static string Route => "tracks";
    [ObservableProperty] public partial bool IsLoading { get; set; } = false;
    [ObservableProperty] public partial bool IsSearching { get; set; } = false;
    [ObservableProperty] public partial TracksDataDto TracksDto { get; private set; } = new();
    [ObservableProperty] public partial DataGridCollectionView TracksView { get; private set; }
    [ObservableProperty] public partial string SearchText { get; set; } = string.Empty;

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
        catch (Exception ex)
        {
            exceptionHandler.ShowDialog(ex, "Failed to load tracks");
        }
        finally
        {
            TracksDto = tempDto;
            TracksView = new DataGridCollectionView(TracksDto.Tracks) { Filter = FilterTracks };
            SearchText = string.Empty;
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
            exceptionHandler.ShowDialog(ex, "Failed to save tracks");
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
            exceptionHandler.ShowDialog(ex, "Failed to reload tracks");
        }
        finally
        {
            TracksDto = tempDto;
            TracksView?.Refresh();
            SearchText = string.Empty;
            IsLoading = false;
        }
    }
    
    private bool FilterTracks(object item)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return true;

        if (item is not TrackDenominationDto track)
            return false;

        return track.Cardinal?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true;
    }

    partial void OnSearchTextChanged(string value)
    {
        IsSearching = true;
        TracksView?.Refresh();
        IsSearching = false;
    }
}
