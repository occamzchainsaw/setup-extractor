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

public partial class SetupShopsViewModel(
    IMapper mapper,
    IOptionsMonitor<SetupShopsData> setupShopsMonitor,
    IWriter<SetupShopsData> setupShopsWriter,
    IExceptionHandler exceptionHandler) 
    : ViewModelBase, INavigable
{
    private bool _isInitialized;
    public static string Route => "setupShops";
    public bool IsBusy => IsLoading || IsSearching;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBusy))]
    public partial bool IsLoading { get; set; } = false;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBusy))]
    public partial bool IsSearching { get; set; } = false;
    [ObservableProperty] public partial SetupShopsDataDto ShopsDto { get; private set; } = new();
    [ObservableProperty] public required partial DataGridCollectionView ShopsView { get; set; }
    [ObservableProperty] public partial string SearchText { get; set; } = string.Empty;

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
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                ShopsDto = tempDto;
                ShopsView = new DataGridCollectionView(ShopsDto.Shops) { Filter = FilterShops };
                SearchText = string.Empty; 
                IsLoading = false;
            });
            _isInitialized = true;
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
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                ShopsDto = tempDto;
                ShopsView?.Refresh();
                SearchText = string.Empty;
                IsLoading = false;
            });
        }
    }
    
    private bool FilterShops(object item)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return true;

        if (item is not SetupShopDenominationDto shop)
            return false;

        return shop.Cardinal?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true;
    }

    partial void OnSearchTextChanged(string value)
    {
        IsSearching = true;
        
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            ShopsView?.Refresh();
            IsSearching = false;
        });
    }
}
