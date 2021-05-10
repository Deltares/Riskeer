// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Core.Common.Util.Settings;
using Core.Components.Chart.Forms;
using Core.Components.Gis.Forms;
using Core.Gui.Commands;
using Core.Gui.Forms.Backstage;
using Core.Gui.Forms.Chart;
using Core.Gui.Forms.Map;
using Core.Gui.Forms.MessageWindow;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Selection;
using Core.Gui.Settings;

namespace Core.Gui.Forms.MainWindow
{
    /// <summary>
    /// Main user interface of the application.
    /// </summary>
    public partial class MainWindow : IMainWindow, IDisposable, ISynchronizeInvoke
    {
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

        private PropertyGridView.PropertyGridView propertyGrid;
        private IMapView currentMapView;
        private IChartView currentChartView;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            windowInteropHelper = new WindowInteropHelper(this);
            Name = "RiskeerMainWindow";

            NewProjectCommand = new RelayCommand(OnNewProject);
            SaveProjectCommand = new RelayCommand(OnSaveProject);
            SaveProjectAsCommand = new RelayCommand(OnSaveProjectAs);
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
            OpenUserManualCommand = new RelayCommand(OnOpenUserManual, CanOpenUserManual);
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
        /// Gets the <see cref="Core.Gui.Forms.ProjectExplorer.ProjectExplorer"/>.
        /// </summary>
        public ProjectExplorer.ProjectExplorer ProjectExplorer { get; private set; }

        /// <summary>
        /// Gets the log messages tool window.
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
        /// <exception cref="InvalidOperationException">
        /// When a <see cref="IGui"/> hasn't been set with <see cref="SetGui"/>.
        /// </exception>
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
        }

        public void ValidateItems()
        {
            if (gui == null)
            {
                return;
            }

            UpdateToolWindowButtonState();
        }

        /// <summary>
        /// Updates the data of the <see cref="ProjectExplorer"/>.
        /// </summary>
        public void UpdateProjectExplorer()
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

        internal void AddStateButton(string text, string symbol, Func<IProject, object> getRootData)
        {
            var stateToggleButton = new ToggleButton
            {
                Tag = text,
                Style = (Style) FindResource("MainButtonBarToggleButtonStyle"),
                Content = new TextBlock
                {
                    Style = (Style) FindResource("ButtonLargeIconStyle"),
                    Text = symbol
                }
            };

            stateToggleButton.Click += (sender, e) => { HandleStateButtonClick((ToggleButton) sender); };

            MainButtonStackPanel.Children.Insert(MainButtonStackPanel.Children.Count - 1, stateToggleButton);

            stateToggleButtonLookup.Add(stateToggleButton, getRootData);

            if (stateToggleButtonLookup.Count == 1)
            {
                stateToggleButton.IsChecked = true;

                UpdateProjectExplorer();
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

            gui.DocumentViewController.CloseAllViews();

            UpdateProjectExplorer();
        }

        #region OnClick events

        private void OnAboutDialog_Clicked(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new SplashScreen.SplashScreen
            {
                VersionText = SettingsHelper.Instance.ApplicationVersion,
                SupportEmail = settings.FixedSettings.SupportEmailAddressUrl,
                SupportPhoneNumber = settings.FixedSettings.SupportPhoneNumberUrl,
                AllowsTransparency = false,
                WindowStyle = WindowStyle.SingleBorderWindow,
                Title = Properties.Resources.About_DisplayName,
                Icon = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.information.GetHbitmap(),
                                                             IntPtr.Zero,
                                                             Int32Rect.Empty,
                                                             BitmapSizeOptions.FromEmptyOptions()),
                ShowInTaskbar = false,
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            aboutDialog.PreviewKeyDown += (s, ev) =>
            {
                if (ev.Key == Key.Escape)
                {
                    ev.Handled = true;
                    aboutDialog.Shutdown();
                }
            };

            aboutDialog.ShowDialog();
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command to start a new project.
        /// </summary>
        public ICommand NewProjectCommand { get; }

        /// <summary>
        /// Gets the command to save a project.
        /// </summary>
        public ICommand SaveProjectCommand { get; }

        /// <summary>
        /// Gets the command to save a project as.
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

        /// <summary>
        /// Gets the command to open the user manual.
        /// </summary>
        public ICommand OpenUserManualCommand { get; }

        private void OnNewProject(object obj)
        {
            commands.StorageCommands.CreateNewProject();
            ValidateItems();
        }

        private void OnSaveProject(object obj)
        {
            commands.StorageCommands.SaveProject();
        }

        private void OnSaveProjectAs(object obj)
        {
            commands.StorageCommands.SaveProjectAs();
        }

        private void OnOpenProject(object obj)
        {
            string projectPath = commands.StorageCommands.GetExistingProjectFilePath();
            if (!string.IsNullOrEmpty(projectPath))
            {
                commands.StorageCommands.OpenExistingProject(projectPath);
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

        private bool CanOpenUserManual(object obj)
        {
            return File.Exists(settings?.FixedSettings.ManualFilePath);
        }

        private void OnOpenUserManual(object obj)
        {
            Process.Start(settings.FixedSettings.ManualFilePath);
        }

        #endregion

        #region ToolWindows

        /// <summary>
        /// Initializes and shows the property grid tool window.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// When a <see cref="IGui"/> hasn't been set with <see cref="SetGui"/>.
        /// </exception>
        public void InitPropertiesWindowOrBringToFront()
        {
            if (gui == null)
            {
                throw new InvalidOperationException("Must call 'SetGui(IGui)' before calling 'InitPropertiesWindowAndActivate'.");
            }

            if (propertyGrid == null)
            {
                propertyGrid = new PropertyGridView.PropertyGridView(gui.PropertyResolver)
                {
                    Text = Properties.Resources.Properties_Title,
                    Data = applicationSelection.Selection
                };

                viewController.ViewHost.AddToolView(propertyGrid, ToolViewLocation.Right, "\uE0945");
            }
            else
            {
                viewController.ViewHost.BringToFront(propertyGrid);
            }
        }

        private void UpdateToolWindowButtonState()
        {
            if (viewController.ViewHost != null)
            {
                ButtonShowProjectExplorer.IsChecked = viewController.ViewHost.ToolViews.Contains(ProjectExplorer);
                ButtonShowMessages.IsChecked = viewController.ViewHost.ToolViews.Contains(MessageWindow);
                ButtonShowProperties.IsChecked = viewController.ViewHost.ToolViews.Contains(PropertyGrid);
                ButtonShowMapLegendView.IsChecked = viewController.ViewHost.ToolViews.Contains(MapLegendView);
                ButtonShowChartLegendView.IsChecked = viewController.ViewHost.ToolViews.Contains(ChartLegendView);
            }
        }

        private void InitProjectExplorerWindow()
        {
            ProjectExplorer = new ProjectExplorer.ProjectExplorer(gui.ViewCommands, gui.GetTreeNodeInfos());

            viewController.ViewHost.AddToolView(ProjectExplorer, ToolViewLocation.Left, "\uE95B");

            UpdateProjectExplorer();
        }

        private void InitMessagesWindow()
        {
            MessageWindow = new MessageWindow.MessageWindow(this)
            {
                Text = Properties.Resources.Messages
            };
            viewController.ViewHost.AddToolView(MessageWindow, ToolViewLocation.Bottom, "\uE942");
        }

        private void InitMapLegendWindow()
        {
            MapLegendView = new MapLegendView(gui);

            viewController.ViewHost.AddToolView(MapLegendView, ToolViewLocation.Left, "\uE94C");
        }

        private void InitChartLegendWindow()
        {
            ChartLegendView = new ChartLegendView(gui);

            viewController.ViewHost.AddToolView(ChartLegendView, ToolViewLocation.Left, "\uE943");
        }

        #endregion

        #region Events

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;

            ValidateItems();
        }

        private void OnViewOpened(object sender, ViewChangeEventArgs e)
        {
            var mapView = e.View as IMapView;
            mapView?.Map.ZoomToVisibleLayers();
            UpdateComponentsForMapView(mapView);
            currentMapView = mapView;

            var chartView = e.View as IChartView;
            UpdateComponentsForChartView(chartView);
            currentChartView = chartView;

            if (e.View is MapLegendView || e.View is ChartLegendView)
            {
                UpdateComponentsForView(viewController.ViewHost.ActiveDocumentView);
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
            if (MapLegendView == null)
            {
                return;
            }

            currentMapView = mapView;
            MapLegendView.MapControl = mapView?.Map;
        }

        private void UpdateComponentsForChartView(IChartView chartView)
        {
            if (ChartLegendView == null)
            {
                return;
            }

            currentChartView = chartView;
            ChartLegendView.ChartControl = chartView?.Chart;
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