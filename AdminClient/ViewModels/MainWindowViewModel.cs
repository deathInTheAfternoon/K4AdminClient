using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using AdminClient.Models;
using AdminClient.Services;
using AdminClient.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdminClient.ViewModels
{
    // The partial keyword is needed for the source generators from CommunityToolkit.Mvvm
    public partial class MainWindowViewModel : ObservableObject
    {
        // TreeView model for the navigation pane
        public ObservableCollection<TreeNodeViewModel> TreeNodes { get; } = new();
        [ObservableProperty]
        private TreeNodeViewModel _selectedNode;
        [ObservableProperty]
        private bool _isTreeLoading;

        // Current ViewModel
        [ObservableProperty]
        private object _currentViewModel;
        // Current view's title
        [ObservableProperty]
        private string _currentViewTitle = "Welcome";

        // Add navigation stack and 'back' command.
        [ObservableProperty]
        private bool _canNavigateBack;
        private readonly Stack<(object ViewModel, string Title)> _navigationStack = new();
        private bool _disposed;

        // Reference to the API service that we will pass down to other ViewModels
        private readonly ApiService _apiService;

        // Dictionary to map TreeNodeType to ViewModel type
        private readonly Dictionary<TreeNodeType, Func<TreeNodeViewModel, object>> _viewModelFactories;
        public MainWindowViewModel(ApiService apiService)
        {
            _apiService = apiService;

            // Initialize the ViewModel factories
            _viewModelFactories = new Dictionary<TreeNodeType, Func<TreeNodeViewModel, object>>
            {
                { TreeNodeType.Root, _ => new CollectionViewModel("Organizations") }, // Placeholder
                { TreeNodeType.Organization, node => new OrganizationViewModel(_apiService) },
                { TreeNodeType.Programs, _ => new CollectionViewModel("Programs") }, // Placeholder
                { TreeNodeType.Program, node => new ProgramViewModel(_apiService, (Organization)node.Parent.ModelObject) { Program = (Models.Program)node.ModelObject } },
                { TreeNodeType.OperatingUnits, _ => new CollectionViewModel("Operating Units") }, // Placeholder
                { TreeNodeType.OperatingUnit, node => new OperatingUnitViewModel(_apiService, (OperatingUnit)node.ModelObject) },
                { TreeNodeType.BundleDefinitions, _ => new CollectionViewModel("Bundle Definitions") }, // Placeholder
                { TreeNodeType.BundleDefinition, node => new BundleDefinitionViewModel(_apiService, (BundleDefinition)node.ModelObject) },
                { TreeNodeType.ActivityDefinitions, _ => new CollectionViewModel("Activity Definitions") }, // Placeholder
                { TreeNodeType.ActivityDefinition, node => new LeafView() } // Placeholder for future ActivityDefinitionViewModel
            };

            var orgViewModel = new OrganizationViewModel(_apiService);
            // Add handler for when an organization is selected
            orgViewModel.OrganizationSelected += OnOrganizationSelected;
            CurrentViewModel = orgViewModel;
            CurrentViewTitle = "Organizations";
            // load tree view's model from db
            LoadTreeAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private void Navigate(Type viewModelType)
        {
            if (viewModelType == typeof(OrganizationViewModel))
            {
                if (CurrentViewModel is OrganizationViewModel oldViewModel)
                {
                    oldViewModel.OrganizationSelected -= OnOrganizationSelected;
                }

                CurrentViewTitle = "Organizations";
                var orgViewModel = new OrganizationViewModel(_apiService);
                orgViewModel.OrganizationSelected += OnOrganizationSelected;
                CurrentViewModel = orgViewModel;
            }
        }

        private void OnOrganizationSelected(object sender, Organization org)
        {
            if (CurrentViewModel is OrganizationViewModel oldViewModel)
            {
                oldViewModel.OrganizationSelected -= OnOrganizationSelected;
            }

            _navigationStack.Push((CurrentViewModel, CurrentViewTitle));

            // Create dummy program for navigation
            var program = new Program
            {
                Organization = org,
                Name = "Default Program" // You may want to load real program data here
            };

            var programViewModel = new ProgramViewModel(_apiService, org) { Program = program };
            CurrentViewModel = programViewModel;
            CurrentViewTitle = $"Programs - {org.Name}";
            CanNavigateBack = true;
        }

        [RelayCommand]
        private void NavigateBack()
        {
            if (_navigationStack.Count > 0)
            {
                var (viewModel, title) = _navigationStack.Pop();

                if (viewModel is OrganizationViewModel newViewModel)
                {
                    newViewModel.OrganizationSelected += OnOrganizationSelected;
                }

                CurrentViewModel = viewModel;
                CurrentViewTitle = title;
                CanNavigateBack = _navigationStack.Count > 0;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && CurrentViewModel is OrganizationViewModel viewModel)
                {
                    viewModel.OrganizationSelected -= OnOrganizationSelected;
                }
                _disposed = true;
            }
        }

        private async Task LoadTreeAsync()
        {
            try
            {
                IsTreeLoading = true;
                await InitializeTreeAsync();
            }
            finally
            {
                IsTreeLoading = false;
            }
        }

        private async Task InitializeTreeAsync()
        {
            try
            {
                // Clear existing nodes before loading new data
                TreeNodes.Clear();

                // Create and add root organizations node
                var rootNode = new TreeNodeViewModel("Organizations", TreeNodeType.Root);
                TreeNodes.Add(rootNode);

                var orgs = await _apiService.GetOrganizationsForRegionAsync("us");

                foreach (var org in orgs)
                {
                    var orgNode = new TreeNodeViewModel(org.Name, TreeNodeType.Organization, org);
                    rootNode.Children.Add(orgNode);

                    var programsNode = new TreeNodeViewModel("Programs", TreeNodeType.Programs);
                    orgNode.Children.Add(programsNode);

                    var programs = await _apiService.GetProgramsForOrganizationAsync(org.Id);
                    foreach (var program in programs)
                    {
                        AddProgramNode(programsNode, program);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tree data: {ex.Message}");
            }
        }

        // Helper method for creating nodes from db data
        private async Task AddProgramNode(TreeNodeViewModel programsNode, Program program)
        {
            var programNode = new TreeNodeViewModel(program.Name, TreeNodeType.Program, program);
            programsNode.Children.Add(programNode);

            // ADD Operating Units with real data
            var operatingUnitsNode = new TreeNodeViewModel("Operating Units", TreeNodeType.OperatingUnits);
            programNode.Children.Add(operatingUnitsNode);

            var operatingUnits = await _apiService.GetOperatingUnitsForProgramAsync(program.Id);
            foreach (var unit in operatingUnits)
            {
                var unitNode = new TreeNodeViewModel(unit.Name, TreeNodeType.OperatingUnit, unit);
                operatingUnitsNode.Children.Add(unitNode);
            }

            var bundleDefsNode = new TreeNodeViewModel("Bundle Definitions", TreeNodeType.BundleDefinitions);
            programNode.Children.Add(bundleDefsNode);

            var bundles = await _apiService.GetBundleDefinitionsForProgramAsync(program.Id);
            foreach (var bundle in bundles)
            {
                var bundleNode = new TreeNodeViewModel(bundle.Name, TreeNodeType.BundleDefinition, bundle);
                bundleDefsNode.Children.Add(bundleNode);
            }
        }

        public async Task HandleTreeNodeSelectionAsync(TreeNodeViewModel selectedNode)
        {
            if (selectedNode == null) return;

            try
            {
                // MODIFY switch statement to handle all node types
                switch (selectedNode.NodeType)
                {
                    // Keep existing Organization handling
                    case TreeNodeType.Organization:
                        if (selectedNode.ModelObject is Organization org)
                        {
                            // Existing organization navigation remains unchanged
                            OnOrganizationSelected(this, org);
                        }
                        break;

                    // ADD Program handling
                    case TreeNodeType.Program:
                        if (selectedNode.ModelObject is Program program)
                        {
                            // Save current state to navigation stack
                            _navigationStack.Push((CurrentViewModel, CurrentViewTitle));

                            // Create and set new program view model
                            var programViewModel = new ProgramViewModel(_apiService, program.Organization)
                            {
                                Program = program
                            };
                            CurrentViewModel = programViewModel;
                            CurrentViewTitle = $"Program - {program.Name}";
                            CanNavigateBack = true;
                        }
                        break;
                    case TreeNodeType.OperatingUnit:
                        if (selectedNode.ModelObject is OperatingUnit unit)
                        {
                            _navigationStack.Push((CurrentViewModel, CurrentViewTitle));

                            // We'll need to create OperatingUnitViewModel in the next micro-feature
                            var operatingUnitViewModel = new OperatingUnitViewModel(_apiService, unit);
                            CurrentViewModel = operatingUnitViewModel;
                            CurrentViewTitle = $"Operating Unit - {unit.Name}";
                            CanNavigateBack = true;
                        }
                        break;

                    case TreeNodeType.BundleDefinition:
                        if (selectedNode.ModelObject is BundleDefinition bundle)
                        {
                            _navigationStack.Push((CurrentViewModel, CurrentViewTitle));

                            // We'll need to create BundleDefinitionViewModel in the next micro-feature
                            var bundleViewModel = new BundleDefinitionViewModel(_apiService, bundle);
                            CurrentViewModel = bundleViewModel;
                            CurrentViewTitle = $"Bundle - {bundle.Name}";
                            CanNavigateBack = true;
                        }
                        break;

                    // Container nodes don't need navigation
                    case TreeNodeType.Root:
                    case TreeNodeType.Programs:
                    case TreeNodeType.OperatingUnits:
                    case TreeNodeType.BundleDefinitions:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error handling selection: {ex.Message}");
            }
        }
    }
}