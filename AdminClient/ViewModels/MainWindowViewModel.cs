using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

            var orgViewModel = new OrganizationCollectionViewModel(_apiService);
            // Listen for events (from OrganizationCollectionViewModel)
            orgViewModel.OrganizationsCollectionUpdated += async (s, e) => await LoadTreeAsync();
            orgViewModel.OrganizationDeleted += OnOrganizationDeleted;

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

        // Handler for organization deletion
        private void OnOrganizationDeleted(object sender, Organization org)
        {
            var rootNode = TreeNodes.FirstOrDefault();
            if (rootNode != null)
            {
                // Debug: Log what we're looking for
                System.Diagnostics.Debug.WriteLine($"Looking to delete org with ID: {org.Id}");

                foreach (var node in rootNode.Children)
                {
                    System.Diagnostics.Debug.WriteLine($"Checking node: {node.Name}, ModelObject: {node.ModelObject?.GetType().Name ?? "null"}");
                }

                var orgNode = rootNode.Children.FirstOrDefault(n =>
                    n.ModelObject != null &&
                    n.ModelObject is Organization orgModel &&
                    orgModel.Id == org.Id);

                if (orgNode != null)
                {
                    rootNode.Children.Remove(orgNode);
                    System.Diagnostics.Debug.WriteLine("Node removed successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Could not find matching node to remove");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Root node not found");
            }
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
                // Clean up current view model if needed
                if (CurrentViewModel is OrganizationCollectionViewModel currentOrgViewModel)
                {
                    currentOrgViewModel.OrganizationDeleted -= OnOrganizationDeleted;
                }

                // Wire up events for the view model we're navigating back to
                if (viewModel is OrganizationCollectionViewModel newOrgViewModel)
                {
                    newOrgViewModel.OrganizationDeleted += OnOrganizationDeleted;
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
                if (disposing)
                {
                    if (CurrentViewModel is OrganizationViewModel viewModel)
                    {
                        viewModel.OrganizationSelected -= OnOrganizationSelected;
                    }
                    if (CurrentViewModel is OrganizationCollectionViewModel collectionViewModel)
                    {
                        collectionViewModel.OrganizationDeleted -= OnOrganizationDeleted;
                    }
                    // Add this cleanup
                    if (CurrentViewModel is ProgramCollectionViewModel programViewModel)
                    {
                        programViewModel.ProgramsCollectionUpdated -= (s, updatedProgram) =>
                            UpdateProgramNodeInTree(updatedProgram);
                        programViewModel.ProgramDeleted -= (s, deletedProgram) =>
                            RemoveProgramNodeFromTree(deletedProgram);
                        programViewModel.ProgramsCollectionUpdated -= (s, newProgram) =>
                            AddProgramNodeToTree(newProgram);
                    }
                    _disposed = true;
                }
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
                    rootNode.AddChild(orgNode);

                    var programsNode = new TreeNodeViewModel("Programs", TreeNodeType.Programs);
                    orgNode.AddChild(programsNode);

                    var programs = await _apiService.GetProgramsForOrganizationAsync(org.Id);
                    foreach (var program in programs)
                    {
                        await AddProgramNode(programsNode, program);
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
            programsNode.AddChild(programNode);

            // ADD Operating Units with real data
            var operatingUnitsNode = new TreeNodeViewModel("Operating Units", TreeNodeType.OperatingUnits);
            programNode.AddChild(operatingUnitsNode);

            var operatingUnits = await _apiService.GetOperatingUnitsForProgramAsync(program.Id);
            foreach (var unit in operatingUnits)
            {
                var unitNode = new TreeNodeViewModel(unit.Name, TreeNodeType.OperatingUnit, unit);
                operatingUnitsNode.AddChild(unitNode);
            }

            var bundleDefsNode = new TreeNodeViewModel("Bundle Definitions", TreeNodeType.BundleDefinitions);
            programNode.AddChild(bundleDefsNode);

            var bundles = await _apiService.GetBundleDefinitionsForProgramAsync(program.Id);
            foreach (var bundle in bundles)
            {
                var bundleNode = new TreeNodeViewModel(bundle.Name, TreeNodeType.BundleDefinition, bundle);
                bundleDefsNode.AddChild(bundleNode);
            }
        }

        public async Task HandleTreeNodeSelectionAsync(TreeNodeViewModel selectedNode)
        {
            if (selectedNode == null) return;

            try
            {
                if (selectedNode.IsCollectionNode)
                {
                    switch (selectedNode.NodeType)
                    {
                        case TreeNodeType.Root:
                            _navigationStack.Push((CurrentViewModel, CurrentViewTitle));
                            var orgCollectionViewModel = new OrganizationCollectionViewModel(_apiService);
                            CurrentViewModel = orgCollectionViewModel;
                            CurrentViewTitle = orgCollectionViewModel.CollectionTitle;
                            CanNavigateBack = true;
                            break;
                        case TreeNodeType.Programs:
                            if (selectedNode.Parent?.ModelObject is Organization parentOrg)
                            {
                                _navigationStack.Push((CurrentViewModel, CurrentViewTitle));
                                var programCollectionViewModel = new ProgramCollectionViewModel(_apiService, parentOrg);
                                // Wire up event handlers (events from CollectionViewModels)
                                programCollectionViewModel.ProgramsCollectionUpdated += (s, updatedProgram) =>
                                                    UpdateProgramNodeInTree(updatedProgram);
                                programCollectionViewModel.ProgramDeleted += (s, deletedProgram) =>
                                                    RemoveProgramNodeFromTree(deletedProgram);
                                programCollectionViewModel.ProgramsCollectionUpdated += (s, newProgram) =>
                                                    AddProgramNodeToTree(newProgram);
                                CurrentViewModel = programCollectionViewModel;
                                CurrentViewTitle = programCollectionViewModel.CollectionTitle;
                                CanNavigateBack = true;
                            }
                            break;
                        case TreeNodeType.OperatingUnits:
                            if (selectedNode.Parent?.ModelObject is Program parentProgramForOU)
                            {
                                _navigationStack.Push((CurrentViewModel, CurrentViewTitle));
                                var operatingUnitCollectionViewModel = new OperatingUnitCollectionViewModel(_apiService, parentProgramForOU);
                                CurrentViewModel = operatingUnitCollectionViewModel;
                                CurrentViewTitle = operatingUnitCollectionViewModel.CollectionTitle;
                                CanNavigateBack = true;
                            }
                            break;
                        case TreeNodeType.BundleDefinitions:
                            if (selectedNode.Parent?.ModelObject is Program parentProgramForBD)
                            {
                                _navigationStack.Push((CurrentViewModel, CurrentViewTitle));
                                var bundleCollectionViewModel = new BundleDefinitionCollectionViewModel(_apiService, parentProgramForBD);
                                // Wire up event handlers (events from CollectionViewModel)
                                bundleCollectionViewModel.BundlesCollectionUpdated += (s, newBundle) =>
                                {
                                    // Add new bundle to the tree
                                    var bundleNode = new TreeNodeViewModel(newBundle.Name, TreeNodeType.BundleDefinition, newBundle);
                                    selectedNode.AddChild(bundleNode);
                                };
                                CurrentViewModel = bundleCollectionViewModel;
                                CurrentViewTitle = bundleCollectionViewModel.CollectionTitle;
                                CanNavigateBack = true;
                            }
                            break;
                        // Add other collection cases
                        default:
                            // Handle unknown collection type
                            break;
                    }
                    return;
                }
                // MODIFY switch statement to handle all node types
                switch (selectedNode.NodeType)
                {
                    // Keep existing Organization handling
                    case TreeNodeType.Organization:
                        if (selectedNode.ModelObject is Organization org)
                        {
                            _navigationStack.Push((CurrentViewModel, CurrentViewTitle));
                            var detailViewModel = new OrganizationDetailViewModel(_apiService, org);
                            CurrentViewModel = detailViewModel;
                            CurrentViewTitle = $"Organization - {org.Name}";
                            CanNavigateBack = true;
                        }
                        break;

                    // ADD Program handling
                    case TreeNodeType.Program:
                        if (selectedNode.ModelObject is Program program)
                        {
                            try
                            {
                                // Fetch the complete program details including organization.
                                var completeProgram = await _apiService.GetProgramAsync(program.Id);

                                _navigationStack.Push((CurrentViewModel, CurrentViewTitle));
                                var programViewModel = new ProgramViewModel(_apiService, completeProgram.Organization)
                                {
                                    Program = completeProgram
                                };
                                CurrentViewModel = programViewModel;
                                CurrentViewTitle = $"Program - {completeProgram.Name}";
                                CanNavigateBack = true;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error loading program details: {ex.Message}");
                            }
                        }
                        break;
                    case TreeNodeType.OperatingUnit:
                        if (selectedNode.ModelObject is OperatingUnit unit)
                        {
                            _navigationStack.Push((CurrentViewModel, CurrentViewTitle));

                            var detailViewModel = new OperatingUnitDetailsDialogModel(_apiService, unit);
                            CurrentViewModel = detailViewModel;
                            CurrentViewTitle = $"Operating Unit - {unit.Name}";
                            CanNavigateBack = true;
                        }
                        break;

                    case TreeNodeType.BundleDefinition:
                        if (selectedNode.ModelObject is BundleDefinition bundle)
                        {
                            _navigationStack.Push((CurrentViewModel, CurrentViewTitle));

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

        // In MainWindowViewModel.cs
        private void UpdateProgramNodeInTree(Program updatedProgram)
        {
            try
            {
                if (updatedProgram == null) return;

                var rootNode = TreeNodes.FirstOrDefault();
                if (rootNode == null) return;

                // Find the organization containing this program
                var orgNode = rootNode.Children
                    .FirstOrDefault(n => n.ModelObject is Organization org
                        && org.Id == updatedProgram.Organization?.Id);
                if (orgNode == null) return;

                // Find the Programs container node
                var programsNode = orgNode.Children
                    .FirstOrDefault(n => n.NodeType == TreeNodeType.Programs);
                if (programsNode == null) return;

                // Find the program node
                var programNode = programsNode.Children
                    .FirstOrDefault(n => n.ModelObject is Program prog
                        && prog.Id == updatedProgram.Id);
                if (programNode != null)
                {
                    // We can only update the Name property
                    programNode.Name = updatedProgram.Name;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating program node: {ex.Message}");
            }
        }

        // In MainWindowViewModel.cs
        private void RemoveProgramNodeFromTree(Program deletedProgram)
        {
            try
            {
                if (deletedProgram == null) return;

                var rootNode = TreeNodes.FirstOrDefault();
                if (rootNode == null) return;

                // Find the organization containing this program
                var orgNode = rootNode.Children
                    .FirstOrDefault(n => n.ModelObject is Organization org
                        && org.Id == deletedProgram.Organization?.Id);
                if (orgNode == null) return;

                // Find the Programs container node
                var programsNode = orgNode.Children
                    .FirstOrDefault(n => n.NodeType == TreeNodeType.Programs);
                if (programsNode == null) return;

                // Find and remove the program node
                var programNode = programsNode.Children
                    .FirstOrDefault(n => n.ModelObject is Program prog
                        && prog.Id == deletedProgram.Id);
                if (programNode != null)
                {
                    programsNode.Children.Remove(programNode);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error removing program node: {ex.Message}");
            }
        }

        // In MainWindowViewModel.cs
        private void AddProgramNodeToTree(Program newProgram)
        {
            try
            {
                if (newProgram == null) return;

                var rootNode = TreeNodes.FirstOrDefault();
                if (rootNode == null) return;

                // Find the organization containing this program
                var orgNode = rootNode.Children
                    .FirstOrDefault(n => n.ModelObject is Organization org
                        && org.Id == newProgram.Organization?.Id);
                if (orgNode == null) return;

                // Find the Programs container node
                var programsNode = orgNode.Children
                    .FirstOrDefault(n => n.NodeType == TreeNodeType.Programs);
                if (programsNode == null) return;

                // Create and add the new program node
                var newProgramNode = new TreeNodeViewModel(
                    newProgram.Name,
                    TreeNodeType.Program,
                    newProgram);
                programsNode.Children.Add(newProgramNode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding program node: {ex.Message}");
            }
        }
    }
}