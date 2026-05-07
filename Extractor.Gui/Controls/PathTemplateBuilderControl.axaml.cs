using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Extractor.Gui.Models;
using Extractor.Gui.ViewModels;

namespace Extractor.Gui.Controls;

public partial class PathTemplateBuilderControl : UserControl
{
    public static readonly DataFormat<PathElementDto> PathElementFormat =
        DataFormat.CreateInProcessFormat<PathElementDto>("application/x-pathelement");

    public PathTemplateBuilderControl()
    {
        InitializeComponent();
    }

    private async void OnElementPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border { DataContext: PathElementDto element }) return;
        
        var item = new DataTransferItem();
        item.Set(PathElementFormat, element);

        var dragData = new DataTransfer();
        dragData.Add(item);
            
        await DragDrop.DoDragDropAsync(e, dragData, DragDropEffects.Move);
    }

    private void OnDropZoneDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Formats.Contains(PathElementFormat) ? DragDropEffects.Move : DragDropEffects.None;
    }

    [SuppressMessage("Performance", "CA1868:Unnecessary call to \'Contains(item)\'")]
    private void OnDropZoneDrop(object? sender, DragEventArgs e)
    {
        if (!e.DataTransfer.Formats.Contains(PathElementFormat) ||
            DataContext is not PathTemplateBuilderViewModel vm) return;
        
        if (e.DataTransfer.Items.FirstOrDefault(i =>
                i.Formats.Contains(PathElementFormat)) is DataTransferItem item)
        {
            var droppedData = item.TryGetRaw(PathElementFormat);
            if (droppedData is PathElementDto droppedElement)
            {
                if (vm.AvailableElements.Contains(droppedElement))
                {
                    vm.AvailableElements.Remove(droppedElement);
                    vm.SelectedElements.Add(droppedElement);
                }
                else if (vm.SelectedElements.Contains(droppedElement))
                {
                    vm.SelectedElements.Remove(droppedElement);
                    vm.SelectedElements.Add(droppedElement);
                }
            }
        }
            
        e.DragEffects = DragDropEffects.Move;
        e.Handled = true;
    }

    [SuppressMessage("Performance", "CA1868:Unnecessary call to \'Contains(item)\'")]
    private void OnTargetItemDrop(object? sender, DragEventArgs e)
    {
        if (sender is not Border { DataContext: PathElementDto targetElement } ||
            DataContext is not PathTemplateBuilderViewModel vm ||
            !e.DataTransfer.Formats.Contains(PathElementFormat))
            return;

        if (e.DataTransfer.Items.FirstOrDefault(i => 
                i.Contains(PathElementFormat)) is DataTransferItem item)
        {
            var droppedData = item.TryGetRaw(PathElementFormat);
            if (droppedData is PathElementDto droppedElement && !droppedElement.Equals(targetElement))
            {
                var insertIndex = vm.SelectedElements.IndexOf(targetElement);

                if (vm.AvailableElements.Contains(droppedElement))
                    vm.AvailableElements.Remove(droppedElement);
                else if (vm.SelectedElements.Contains(droppedElement))
                    vm.SelectedElements.Remove(droppedElement);
                
                vm.SelectedElements.Insert(insertIndex, droppedElement);
            }
        }
        
        e.DragEffects = DragDropEffects.Move;
        e.Handled = true;
    }
    
    private void OnRemoveClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: PathElementDto element } ||
            DataContext is not PathTemplateBuilderViewModel vm)
            return;
        
        vm.SelectedElements.Remove(element);
        vm.AvailableElements.Add(element);
    }
}