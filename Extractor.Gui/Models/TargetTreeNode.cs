using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Extractor.Gui.Models;

public partial class TargetTreeNode : ObservableObject
{
    [ObservableProperty] public partial string Name { get; set; } = string.Empty;
    
    public ObservableCollection<TargetTreeNode> Children { get; } = [];
}
