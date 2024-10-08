// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Core.Common.Util.Settings;
using Core.Components.Chart.Forms;
using Core.Components.Gis.Forms;
using Core.Gui.Commands;
using Core.Gui.Forms.Backstage;
using Core.Gui.Forms.Chart;
using Core.Gui.Forms.Log;
using Core.Gui.Forms.Map;
using Core.Gui.Forms.Project;
using Core.Gui.Forms.PropertyView;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Selection;
using Core.Gui.Settings;
using Application = System.Windows.Forms.Application;
using FontFamily = System.Windows.Media.FontFamily;
using UserControl = System.Windows.Forms.UserControl;

namespace Core.Gui.Forms.Main
{
    /// <summary>
    /// Main user interface of the application.
    /// </summary>
    public partial class MainWindow : IMainWindow, IDisposable, ISynchronizeInvoke
    {
        private static readonly FontFamily toolViewFontFamily = new FontFamily(
            new Uri($"{PackUriHelper.UriSchemePack}://application:,,,/Core.Gui;component/Style/Symbols/"),
            "./#CoreSymbols");

        /// <summary>
        /// Class to help with hybrid winforms - WPF applications. Provides UI handle to
        /// ensure common UI functionality such as maximizing works as expected.
        /// </summary>
        private readonly WindowInteropHelper windowInteropHelper;

        private readonly IDictionary<ToggleButton, Func<IProject, object>> stateToggleButtonLookup = new Dictionary<ToggleButton, Func<IProject, object>>();

        private IViewController viewController;
        private ICommandsOwner commands;
        private ISettingsOwner settings;
        private IApplicationSelection applicationSelection;

        private IGui gui;

        private PropertyGridView propertyGrid;
        private IMapView currentMapView;
        private IChartView currentChartView;

        /// <summary>
        /// Creates a new instance of <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            windowInteropHelper = new WindowInteropHelper(this);
            Name = "RiskeerMainWindow";

            NewProjectCommand = new RelayCommand(OnNewProject);
            SaveProjectCommand = new RelayCommand(OnSaveProject, CanExecuteSaveCommand);
            SaveProjectAsCommand = new RelayCommand(OnSaveProjectAs, CanExecuteSaveCommand);
            OpenProjectCommand = new RelayCommand(OnOpenProject);
            CloseApplicationCommand = new RelayCommand(OnCloseApplication);
            CloseViewTabCommand = new RelayCommand(OnCloseViewTab, CanCloseViewTab);

            ToggleBackstageCommand = new RelayCommand(OnToggleBackstage);
            ToggleProjectExplorerCommand = new RelayCommand(OnToggleProjectExplorer);
            ToggleMapLegendViewCommand = new RelayCommand(OnToggleMapLegendView);
            ToggleChartLegendViewCommand = new RelayCommand(OnToggleChartLegendView);
            TogglePropertyGridViewCommand = new RelayCommand(OnTogglePropertyGridView);
            ToggleMessageWindowCommand = new RelayCommand(OnToggleMessageWindow);
            OpenLogFileCommand = new RelayCommand(OnOpenLogFile);
        }

        /// <summary>
        /// Gets the <see cref="BackstageViewModel"/>.
        /// </summary>
        public BackstageViewModel BackstageViewModel { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this window is disposed.
        /// </summary>
        public bool IsWindowDisposed { get; private set; }

        /// <summary>
        /// Gets the view host.
        /// </summary>
        public IViewHost ViewHost => AvalonDockViewHost;

        /// <summary>
        /// Gets or sets a value indicating whether or not the main user interface is visible.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no gui has been set using <see cref="SetGui"/>.</exception>
        public bool Visible
        {
            get => IsVisible;
            set
            {
                if (gui == null)
                {
                    throw new InvalidOperationException("First call 'SetGui(IGui)' before setting a value on this property.");
                }

                if (value)
                {
                    if (IsVisible)
                    {
                        return;
                    }

                    Show();
                }
                else
                {
                    if (!IsVisible)
                    {
                        return;
                    }

                    Hide();
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Project.ProjectExplorer"/>.
        /// </summary>
        public ProjectExplorer ProjectExplorer { get; private set; }

        /// <summary>
        /// Gets the <see cref="Log.MessageWindow"/>.
        /// </summary>
        public IMessageWindow MessageWindow { get; private set; }

        /// <summary>
        /// Gets the <see cref="Core.Gui.Forms.Map.MapLegendView"/>.
        /// </summary>
        public MapLegendView MapLegendView { get; private set; }

        /// <summary>
        /// Gets the <see cref="Core.Gui.Forms.Chart.ChartLegendView"/>
        /// </summary>
        public ChartLegendView ChartLegendView { get; private set; }

        public IView PropertyGrid => propertyGrid;

        public IntPtr Handle => windowInteropHelper.Handle;

        public Icon ApplicationIcon => gui.FixedSettings.ApplicationIcon;

        public bool InvokeRequired => !Dispatcher.CheckAccess();

        /// <summary>
        /// Sets the <see cref="IGui"/> and dependencies.
        /// </summary>
        public void SetGui(IGui value)
        {
            gui = value;

            viewController = gui;
            settings = gui;
            commands = gui;
            applicationSelection = gui;

            BackstageViewModel = new BackstageViewModel(settings.FixedSettings, SettingsHelper.Instance.ApplicationVersion);
        }

        /// <summary>
        /// Subscribes the main user interface to the <see cref="IGui"/> provided by <see cref="SetGui"/>.
        /// </summary>
        public void SubscribeToGui()
        {
            if (viewController?.ViewHost != null)
            {
                viewController.ViewHost.ViewOpened += OnViewOpened;
                viewController.ViewHost.ViewBroughtToFront += OnViewBroughtToFront;
                viewController.ViewHost.ViewClosed += OnViewClosed;
                viewController.ViewHost.ActiveDocumentViewChanged += OnActiveDocumentViewChanged;
            }
        }

        /// <summary>
        /// Unsubscribes the main user interface from the <see cref="IGui"/> provided by <see cref="SetGui"/>.
        /// </summary>
        public void UnsubscribeFromGui()
        {
            if (viewController?.ViewHost != null)
            {
                viewController.ViewHost.ViewOpened -= OnViewOpened;
                viewController.ViewHost.ViewBroughtToFront -= OnViewBroughtToFront;
                viewController.ViewHost.ViewClosed -= OnViewClosed;
                viewController.ViewHost.ActiveDocumentViewChanged -= OnActiveDocumentViewChanged;
            }
        }

        /// <summary>
        /// Initializes and shows all the tool windows.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when an <see cref="IGui"/>
        /// hasn't been set with <see cref="SetGui"/>.</exception>
        public void InitializeToolWindows()
        {
            if (gui == null)
            {
                throw new InvalidOperationException("Must call 'SetGui(IGui)' before calling 'InitializeToolWindows'.");
            }

            InitProjectExplorerWindow();
            InitMapLegendWindow();
            InitChartLegendWindow();
            InitMessagesWindow();
            InitPropertiesWindowOrBringToFront();

            UpdateToolWindowButtonState();
        }

        public void ValidateItems()
        {
            if (gui == null)
            {
                return;
            }

            UpdateToolWindowButtonState();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsWindowDisposed || !disposing)
            {
                return;
            }

            IsWindowDisposed = true;

            Close();

            gui = null;
            viewController = null;
            settings = null;
            commands = null;
            applicationSelection = null;
        }

        #region State

        internal ToggleButton AddStateButton(string text, string symbol, FontFamily fontFamily, Func<IProject, object> getRootData)
        {
            var stateToggleButton = new ToggleButton
            {
                Tag = text,
                IsEnabled = false,
                Style = (Style) FindResource("MainButtonBarToggleButtonStyle"),
                Content = new TextBlock
                {
                    Style = (Style) FindResource("ButtonLargeIconStyle"),
                    Text = symbol,
                    FontFamily = fontFamily
                }
            };

            stateToggleButton.Click += (sender, e) =>
            {
                HandleStateButtonClick((ToggleButton) sender);
            };

            MainButtonStackPanel.Children.Insert(MainButtonStackPanel.Children.Count - 1, stateToggleButton);

            stateToggleButtonLookup.Add(stateToggleButton, getRootData);

            return stateToggleButton;
        }

        internal void ResetState()
        {
            if (!stateToggleButtonLookup.Any())
            {
                return;
            }

            gui.DocumentViewController.CloseAllViews();

            Application.DoEvents();

            if (gui.Project == null)
            {
                stateToggleButtonLookup.Keys.ForEachElementDo(stb =>
                {
                    stb.IsChecked = false;
                    stb.IsEnabled = false;
                });

                UpdateProjectExplorer();
            }
            else
            {
                stateToggleButtonLookup.Keys.ForEachElementDo(stb =>
                {
                    stb.IsEnabled = true;
                });

                ToggleButton firstStateToggleButton = stateToggleButtonLookup.First().Key;

                firstStateToggleButton.IsChecked = true;

                HandleStateButtonClick(firstStateToggleButton);

                gui.DocumentViewController.OpenViewForData(stateToggleButtonLookup[firstStateToggleButton](gui.Project));
            }
        }

        private void HandleStateButtonClick(ToggleButton clickedStateToggleButton)
        {
            if (clickedStateToggleButton.IsChecked != null && !clickedStateToggleButton.IsChecked.Value)
            {
                clickedStateToggleButton.IsChecked = true;

                return;
            }

            foreach (ToggleButton stateToggleButton in stateToggleButtonLookup.Keys.Except(new[]
            {
                clickedStateToggleButton
            }))
            {
                stateToggleButton.IsChecked = false;
            }

            UpdateProjectExplorer();
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command to create a new project.
        /// </summary>
        public ICommand NewProjectCommand { get; }

        /// <summary>
        /// Gets the command to save a project.
        /// </summary>
        public ICommand SaveProjectCommand { get; }

        /// <summary>
        /// Gets the command to save a project at a new location.
        /// </summary>
        public ICommand SaveProjectAsCommand { get; }

        /// <summary>
        /// Gets the command to open a project.
        /// </summary>
        public ICommand OpenProjectCommand { get; }

        /// <summary>
        /// Gets the command to close the application.
        /// </summary>
        public ICommand CloseApplicationCommand { get; }

        /// <summary>
        /// Gets the command to close a view tab.
        /// </summary>
        public ICommand CloseViewTabCommand { get; }

        /// <summary>
        /// Gets the command to toggle the backstage.
        /// </summary>
        public ICommand ToggleBackstageCommand { get; }

        /// <summary>
        /// Gets the command to toggle the <see cref="ProjectExplorer"/>.
        /// </summary>
        public ICommand ToggleProjectExplorerCommand { get; }

        /// <summary>
        /// Gets the command to toggle the <see cref="MapLegendView"/>.
        /// </summary>
        public ICommand ToggleMapLegendViewCommand { get; }

        /// <summary>
        /// Gets the command to toggle the <see cref="ChartLegendView"/>.
        /// </summary>
        public ICommand ToggleChartLegendViewCommand { get; }

        /// <summary>
        /// Gets the command to toggle the <see cref="PropertyGridView"/>.
        /// </summary>
        public ICommand TogglePropertyGridViewCommand { get; }

        /// <summary>
        /// Gets the command to toggle the <see cref="MessageWindow"/>.
        /// </summary>
        public ICommand ToggleMessageWindowCommand { get; }

        /// <summary>
        /// Gets the command to open the log file.
        /// </summary>
        public ICommand OpenLogFileCommand { get; }

        private void OnNewProject(object obj)
        {
            commands.StorageCommands.CreateNewProject();
            CloseBackstage();
        }

        private bool CanExecuteSaveCommand(object obj)
        {
            return gui?.Project != null;
        }

        private void OnSaveProject(object obj)
        {
            commands.StorageCommands.SaveProject();
            CloseBackstage();
        }

        private void OnSaveProjectAs(object obj)
        {
            commands.StorageCommands.SaveProjectAs();
            CloseBackstage();
        }

        private void OnOpenProject(object obj)
        {
            string projectPath = commands.StorageCommands.GetExistingProjectFilePath();

            if (!string.IsNullOrEmpty(projectPath))
            {
                commands.StorageCommands.OpenExistingProject(projectPath);
            }

            CloseBackstage();
        }

        private void CloseBackstage()
        {
            if (BackstageDockPanel.Visibility == Visibility.Visible)
            {
                MainDockPanel.Visibility = Visibility.Visible;
                BackstageDockPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void OnCloseApplication(object obj)
        {
            gui.ExitApplication();
        }

        private bool CanCloseViewTab(object arg)
        {
            return viewController.ViewHost.DocumentViews.Any();
        }

        private void OnCloseViewTab(object obj)
        {
            viewController.ViewHost.Remove(viewController.ViewHost.ActiveDocumentView);
        }

        private void OnToggleBackstage(object obj)
        {
            if (MainDockPanel.Visibility == Visibility.Visible)
            {
                MainDockPanel.Visibility = Visibility.Collapsed;
                BackstageDockPanel.Visibility = Visibility.Visible;
            }
            else
            {
                MainDockPanel.Visibility = Visibility.Visible;
                BackstageDockPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void OnToggleProjectExplorer(object obj)
        {
            ToggleToolWindow(ProjectExplorer, InitProjectExplorerWindow, ButtonShowProjectExplorer);
        }

        private void OnToggleMapLegendView(object obj)
        {
            ToggleToolWindow(MapLegendView, InitMapLegendWindow, ButtonShowMapLegendView);
        }

        private void OnToggleChartLegendView(object obj)
        {
            ToggleToolWindow(ChartLegendView, InitChartLegendWindow, ButtonShowChartLegendView);
        }

        private void OnTogglePropertyGridView(object obj)
        {
            ToggleToolWindow(PropertyGrid, InitPropertiesWindowOrBringToFront, ButtonShowProperties);
        }

        private void OnToggleMessageWindow(object obj)
        {
            ToggleToolWindow(MessageWindow, InitMessagesWindow, ButtonShowMessages);
        }

        private void ToggleToolWindow(IView toolView, Action initializeToolWindowAction, ToggleButton toggleButton)
        {
            bool active = viewController.ViewHost.ToolViews.Contains(toolView);

            if (active)
            {
                viewController.ViewHost.Remove(toolView);
            }
            else
            {
                initializeToolWindowAction();
            }

            toggleButton.IsChecked = !active;
        }

        private void OnOpenLogFile(object obj)
        {
            commands.ApplicationCommands.OpenLogFileExternal();
        }

        #endregion

        #region ToolWindows

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Thrown when an <see cref="IGui"/>
        /// hasn't been set with <see cref="SetGui"/>.</exception>
        public void InitPropertiesWindowOrBringToFront()
        {
            if (gui == null)
            {
                throw new InvalidOperationException($"Must call '{nameof(SetGui)}' before calling '{nameof(InitPropertiesWindowOrBringToFront)}'.");
            }

            if (propertyGrid == null)
            {
                propertyGrid = new PropertyGridView(gui.PropertyResolver)
                {
                    Data = applicationSelection.Selection
                };

                viewController.ViewHost.AddToolView(propertyGrid, ToolViewLocation.Right, Properties.Resources.Properties_DisplayName, "\uE905", toolViewFontFamily);
            }
            else
            {
                viewController.ViewHost.BringToFront(propertyGrid);
            }
        }

        private void UpdateToolWindowButtonState()
        {
            ButtonShowProjectExplorer.IsChecked = viewController.ViewHost.ToolViews.Contains(ProjectExplorer);
            ButtonShowMessages.IsChecked = viewController.ViewHost.ToolViews.Contains(MessageWindow);
            ButtonShowProperties.IsChecked = viewController.ViewHost.ToolViews.Contains(PropertyGrid);
            ButtonShowMapLegendView.IsChecked = viewController.ViewHost.ToolViews.Contains(MapLegendView);
            ButtonShowChartLegendView.IsChecked = viewController.ViewHost.ToolViews.Contains(ChartLegendView);
        }

        private void InitProjectExplorerWindow()
        {
            ProjectExplorer = new ProjectExplorer(gui.ViewCommands, gui.GetTreeNodeInfos());

            viewController.ViewHost.AddToolView(ProjectExplorer, ToolViewLocation.Left, Properties.Resources.ProjectExplorer_DisplayName, "\uE904", toolViewFontFamily);

            UpdateProjectExplorer();
        }

        private void UpdateProjectExplorer()
        {
            if (ProjectExplorer == null)
            {
                return;
            }

            ToggleButton checkedStateToggleButton = stateToggleButtonLookup.Keys.FirstOrDefault(stb => stb.IsChecked.HasValue && stb.IsChecked.Value);

            ProjectExplorer.Data = checkedStateToggleButton != null
                                       ? stateToggleButtonLookup[checkedStateToggleButton](gui.Project)
                                       : null;
        }

        private void InitMessagesWindow()
        {
            MessageWindow = new MessageWindow(this);

            viewController.ViewHost.AddToolView(MessageWindow, ToolViewLocation.Bottom, Properties.Resources.Messages, "\uE906", toolViewFontFamily);
        }

        private void InitMapLegendWindow()
        {
            MapLegendView = new MapLegendView(gui);

            viewController.ViewHost.AddToolView(MapLegendView, ToolViewLocation.Left, Properties.Resources.MapLegendView_Map_DisplayName, "\uE907", toolViewFontFamily);
        }

        private void InitChartLegendWindow()
        {
            ChartLegendView = new ChartLegendView(gui);

            viewController.ViewHost.AddToolView(ChartLegendView, ToolViewLocation.Left, Properties.Resources.ChartLegendView_Chart_DisplayName, "\uE908", toolViewFontFamily);
        }

        #endregion

        #region Events

        private void OnViewOpened(object sender, ViewChangeEventArgs e)
        {
            var mapView = e.View as IMapView;
            if (mapView != null && e.View is UserControl mapUserControl)
            {
                mapUserControl.VisibleChanged += MapViewVisibleChanged;
            }

            UpdateComponentsForMapView(mapView);

            var chartView = e.View as IChartView;
            UpdateComponentsForChartView(chartView);

            if (e.View is MapLegendView || e.View is ChartLegendView)
            {
                UpdateComponentsForView(viewController.ViewHost.ActiveDocumentView);
            }

            UpdateToolWindowButtonState();
        }

        /// <summary>
        /// Fix for getting around the latency within AvalonDock; ensure any opened map view is zoomed to its extents, but first wait until it is completely visible.
        /// </summary>
        private static void MapViewVisibleChanged(object sender, EventArgs e)
        {
            var control = (UserControl) sender;
            if (control.Width > 0.0 && control.Height > 0.0)
            {
                IMapControl mapControl = ((IMapView) control).Map;
                mapControl.ZoomToVisibleLayers();

                control.VisibleChanged -= MapViewVisibleChanged;
            }
        }

        private void OnViewBroughtToFront(object sender, ViewChangeEventArgs e)
        {
            UpdateComponentsForView(e.View);
        }

        private void OnActiveDocumentViewChanged(object sender, EventArgs e)
        {
            UpdateComponentsForView(viewController.ViewHost.ActiveDocumentView);
        }

        private void UpdateComponentsForView(IView view)
        {
            var mapView = view as IMapView;
            if (!ReferenceEquals(currentMapView, mapView))
            {
                UpdateComponentsForMapView(mapView);
            }

            var chartView = view as IChartView;
            if (!ReferenceEquals(currentChartView, chartView))
            {
                UpdateComponentsForChartView(chartView);
            }
        }

        private void OnViewClosed(object sender, ViewChangeEventArgs e)
        {
            if (ReferenceEquals(e.View, propertyGrid))
            {
                propertyGrid = null;
            }

            if (ReferenceEquals(e.View, MessageWindow))
            {
                MessageWindow = null;
            }

            if (ReferenceEquals(e.View, ProjectExplorer))
            {
                ProjectExplorer = null;
            }

            if (ReferenceEquals(e.View, MapLegendView))
            {
                MapLegendView = null;
            }

            if (ReferenceEquals(e.View, ChartLegendView))
            {
                ChartLegendView = null;
            }

            if (ReferenceEquals(e.View, currentMapView))
            {
                UpdateComponentsForMapView(null);
            }

            if (ReferenceEquals(e.View, currentChartView))
            {
                UpdateComponentsForChartView(null);
            }
        }

        private void UpdateComponentsForMapView(IMapView mapView)
        {
            currentMapView = mapView;

            if (MapLegendView != null)
            {
                MapLegendView.MapControl = mapView?.Map;
            }
        }

        private void UpdateComponentsForChartView(IChartView chartView)
        {
            currentChartView = chartView;

            if (ChartLegendView != null)
            {
                ChartLegendView.ChartControl = chartView?.Chart;
            }
        }

        #endregion

        #region Implementation: ISynchronizeInvoke

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            Dispatcher.BeginInvoke(method, args);
            return null;
        }

        public object EndInvoke(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public object Invoke(Delegate method, object[] args)
        {
            return Dispatcher.Invoke(method, args);
        }

        #endregion
    }
}