using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Extractor.Core.Model;
using Extractor.Gui.Models;

namespace Extractor.Gui.ViewModels;

public partial class PathTemplateBuilderViewModel : ViewModelBase
{
    public ObservableCollection<PathElementDto> AvailableElements { get; } = [];
    public ObservableCollection<PathElementDto> SelectedElements { get; set; } = [];
    public IEnumerable<PathElement> SelectedElementsValues => SelectedElements.Select(e => e.Value);

    public void Initialize(IEnumerable<PathElement> existingTemplate)
    {
        AvailableElements.Clear();
        SelectedElements.Clear();
        
        foreach (var element in existingTemplate)
            SelectedElements.Add(element.ToDto());

        var allElements = Enum.GetValues<PathElement>().Select(e => e.ToDto()).ToList();
        var comparer = new PathElementDtoComparer();
        
        foreach (var element in allElements.Where(e => !SelectedElements.Contains(e, comparer)))
            AvailableElements.Add(element);
    }
}