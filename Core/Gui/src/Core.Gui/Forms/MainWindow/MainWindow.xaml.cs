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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Core.Common.Controls.Views;
using Core.Common.Util.Settings;
using Core.Components.Gis.Forms;
using Core.Gui.Commands;
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

        private IViewController viewController;
        private ICommandsOwner commands;
        private ISettingsOwner settings;
        private IApplicationSelection applicationSelection;

        private IGui gui;

        private PropertyGridView.PropertyGridView propertyGrid;
        private IMapView currentMapView;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            windowInteropHelper = new WindowInteropHelper(this);
            Name = "RiskeerMainWindow";
        }

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
            InitProjectExplorerWindowOrBringToFront();
            InitMapLegendWindowOrBringToFront();
            InitChartLegendWindowOrBringToFront();
            InitMessagesWindowOrBringToFront();
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
            if (ProjectExplorer != null)
            {
                ProjectExplorer.Data = gui.Project;
            }
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

            SetGui(null);
        }

        #region OnClick events

        private void OnFileSaveClicked(object sender, RoutedEventArgs e)
        {
            commands.StorageCommands.SaveProject();
        }

        private void OnFileSaveAsClicked(object sender, RoutedEventArgs e)
        {
            commands.StorageCommands.SaveProjectAs();
        }

        private void OnFileOpenClicked(object sender, RoutedEventArgs e)
        {
            string projectPath = commands.StorageCommands.GetExistingProjectFilePath();
            if (!string.IsNullOrEmpty(projectPath))
            {
                commands.StorageCommands.OpenExistingProject(projectPath);
            }
        }

        private void OnFileNewClicked(object sender, RoutedEventArgs e)
        {
            commands.StorageCommands.CreateNewProject();
            ValidateItems();
        }

        private void OnFileExitClicked(object sender, RoutedEventArgs e)
        {
            gui.ExitApplication();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;

            FileManualButton.IsEnabled = File.Exists(settings.FixedSettings.ManualFilePath);

            ValidateItems();
        }

        private void ButtonShowProjectExplorer_Click(object sender, RoutedEventArgs e)
        {
            bool active = viewController.ViewHost.ToolViews.Contains(ProjectExplorer);

            if (active)
            {
                viewController.ViewHost.Remove(ProjectExplorer);
            }
            else
            {
                InitProjectExplorerWindowOrBringToFront();
            }

            ButtonShowProjectExplorer.IsChecked = !active;
        }

        private void ButtonShowProperties_Click(object sender, RoutedEventArgs e)
        {
            bool active = viewController.ViewHost.ToolViews.Contains(PropertyGrid);

            if (active)
            {
                viewController.ViewHost.Remove(PropertyGrid);
            }
            else
            {
                InitPropertiesWindowOrBringToFront();
            }

            ButtonShowProperties.IsChecked = !active;
        }

        private void ButtonShowMessages_Click(object sender, RoutedEventArgs e)
        {
            bool active = viewController.ViewHost.ToolViews.Contains(MessageWindow);

            if (active)
            {
                viewController.ViewHost.Remove(MessageWindow);
            }
            else
            {
                InitMessagesWindowOrBringToFront();
            }

            ButtonShowMessages.IsChecked = !active;
        }

        private void ButtonShowMapLegendView_Click(object sender, RoutedEventArgs e)
        {
            bool active = viewController.ViewHost.ToolViews.Contains(MapLegendView);

            if (active)
            {
                viewController.ViewHost.Remove(MapLegendView);
            }
            else
            {
                InitMapLegendWindowOrBringToFront();
            }

            ButtonShowMapLegendView.IsChecked = !active;
        }

        private void ButtonShowChartLegendView_Click(object sender, RoutedEventArgs e)
        {
            bool active = viewController.ViewHost.ToolViews.Contains(ChartLegendView);

            if (active)
            {
                viewController.ViewHost.Remove(ChartLegendView);
            }
            else
            {
                InitChartLegendWindowOrBringToFront();
            }

            ButtonShowChartLegendView.IsChecked = !active;
        }

        private void OnFileHelpShowLog_Clicked(object sender, RoutedEventArgs e)
        {
            commands.ApplicationCommands.OpenLogFileExternal();
        }

        private void OnFileManual_Clicked(object sender, RoutedEventArgs e)
        {
            string manualFileName = settings.FixedSettings.ManualFilePath;

            if (File.Exists(manualFileName))
            {
                Process.Start(manualFileName);
            }
        }

        private void CloseDocumentTab(object sender, ExecutedRoutedEventArgs e)
        {
            viewController.ViewHost.Remove(viewController.ViewHost.ActiveDocumentView);
        }

        private void CanCloseDocumentTab(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewController.ViewHost.DocumentViews.Any();
        }

        private void OnAboutDialog_Clicked(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new SplashScreen.SplashScreen
            {
                VersionText = SettingsHelper.Instance.ApplicationVersion,
                SupportEmail = settings.FixedSettings.SupportEmailAddress,
                SupportPhoneNumber = settings.FixedSettings.SupportPhoneNumber,
                AllowsTransparency = false,
                WindowStyle = WindowStyle.SingleBorderWindow,
                Title = Properties.Resources.ViewStateBar_About_ToolTip,
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

                viewController.ViewHost.AddToolView(propertyGrid, ToolViewLocation.Right);
                viewController.ViewHost.SetImage(propertyGrid, Properties.Resources.PropertiesPanelIcon);
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

        private void InitProjectExplorerWindowOrBringToFront()
        {
            if (gui == null)
            {
                throw new InvalidOperationException("Must call 'SetGui(IGui)' before calling 'InitMessagesWindowOrActivate'.");
            }

            if (ProjectExplorer == null)
            {
                ProjectExplorer = new ProjectExplorer.ProjectExplorer(gui.ViewCommands, gui.GetTreeNodeInfos())
                {
                    Data = gui.Project
                };
                viewController.ViewHost.AddToolView(ProjectExplorer, ToolViewLocation.Left);
                viewController.ViewHost.SetImage(ProjectExplorer, Properties.Resources.ProjectExplorerIcon);
            }
            else
            {
                viewController.ViewHost.BringToFront(ProjectExplorer);
            }
        }

        private void InitMessagesWindowOrBringToFront()
        {
            if (gui == null)
            {
                throw new InvalidOperationException("Must call 'SetGui(IGui)' before calling 'InitMessagesWindowOrActivate'.");
            }

            if (MessageWindow == null)
            {
                MessageWindow = new MessageWindow.MessageWindow(this)
                {
                    Text = Properties.Resources.Messages
                };
                viewController.ViewHost.AddToolView(MessageWindow, ToolViewLocation.Bottom);
                viewController.ViewHost.SetImage(MessageWindow, Properties.Resources.application_view_list);
            }
            else
            {
                viewController.ViewHost.BringToFront(MessageWindow);
            }
        }

        private void InitMapLegendWindowOrBringToFront()
        {
            if (gui == null)
            {
                throw new InvalidOperationException("Must call 'SetGui(IGui)' before calling 'InitMessagesWindowOrActivate'.");
            }

            if (MapLegendView == null)
            {
                MapLegendView = new MapLegendView(gui);

                viewController.ViewHost.AddToolView(MapLegendView, ToolViewLocation.Left);
                viewController.ViewHost.SetImage(MapLegendView, Properties.Resources.application_view_list);
            }
            else
            {
                viewController.ViewHost.BringToFront(MapLegendView);
            }
        }

        private void InitChartLegendWindowOrBringToFront()
        {
            if (gui == null)
            {
                throw new InvalidOperationException("Must call 'SetGui(IGui)' before calling 'InitMessagesWindowOrActivate'.");
            }

            if (ChartLegendView == null)
            {
                ChartLegendView = new ChartLegendView(gui);

                viewController.ViewHost.AddToolView(ChartLegendView, ToolViewLocation.Left);
                viewController.ViewHost.SetImage(ChartLegendView, Properties.Resources.application_view_list);
            }
            else
            {
                viewController.ViewHost.BringToFront(ChartLegendView);
            }
        }

        #endregion

        #region Events

        private void OnViewOpened(object sender, ViewChangeEventArgs e)
        {
            if (e.View is IMapView mapView)
            {
                mapView.Map.ZoomToVisibleLayers();
                UpdateComponentsForMapView(mapView);
            }
        }

        private void OnViewBroughtToFront(object sender, ViewChangeEventArgs e)
        {
            UpdateComponentsForMapView(e.View as IMapView);
        }

        private void OnActiveDocumentViewChanged(object sender, EventArgs e)
        {
            UpdateComponentsForMapView(viewController.ViewHost.ActiveDocumentView as IMapView);
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
        }

        private void UpdateComponentsForMapView(IMapView mapView)
        {
            if (ReferenceEquals(currentMapView, mapView))
            {
                return;
            }

            currentMapView = mapView;

            MapLegendView.MapControl = mapView?.Map;
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