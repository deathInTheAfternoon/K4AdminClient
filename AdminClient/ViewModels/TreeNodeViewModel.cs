// NEW FILE: ViewModels/TreeNodeViewModel.cs
using AdminClient.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace AdminClient.ViewModels
{
    // This class represents a node in our domain tree
    public partial class TreeNodeViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private ObservableCollection<TreeNodeViewModel> _children = new();

        [ObservableProperty]
        private TreeNodeType _nodeType;

        // Reference to the actual domain model object
        public object ModelObject { get; }

        public TreeNodeViewModel(string name, TreeNodeType nodeType, object modelObject = null)
        {
            Name = name;
            NodeType = nodeType;
            ModelObject = modelObject;
        }
    }

    // Enum to identify node types in our domain
    public enum TreeNodeType
    {
        Root,
        Organization,
        Programs,
        Program,
        OperatingUnits,
        OperatingUnit,
        BundleDefinitions,
        BundleDefinition,
        ActivityDefinitions,
        ActivityDefinition
    }
}