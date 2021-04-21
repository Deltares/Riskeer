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
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Selection;
using Core.Common.Gui.Settings;
using Core.Common.Util.Settings;

namespace Core.Common.Gui.Forms.MainWindow
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

        private MessageWindow.MessageWindow messageWindow;
        private PropertyGridView.PropertyGridView propertyGrid;

        private IGui gui;
        private ProjectExplorer.ProjectExplorer projectExplorer;

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
        /// Gets the log messages tool window.
        /// </summary>
        public IMessageWindow MessageWindow => messageWindow;

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

        public IView PropertyGrid => propertyGrid;

        public IntPtr Handle => windowInteropHelper.Handle;

        public bool InvokeRequired => !Dispatcher.CheckAccess();

        public ProjectExplorer.ProjectExplorer ProjectExplorer => projectExplorer;

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
                viewController.ViewHost.ViewClosed += OnViewClosed;
            }
        }

        /// <summary>
        /// Unsubscribes the main user interface from the <see cref="IGui"/> provided by <see cref="SetGui"/>.
        /// </summary>
        public void UnsubscribeFromGui()
        {
            if (viewController?.ViewHost != null)
            {
                viewController.ViewHost.ViewClosed -= OnViewClosed;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
                viewController.ViewHost.SetImage(propertyGrid, Properties.Resources.PropertiesHS);
            }
            else
            {
                viewController.ViewHost.BringToFront(propertyGrid);
            }
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

        private void OnViewClosed(object sender, ViewChangeEventArgs e)
        {
            if (ReferenceEquals(e.View, propertyGrid))
            {
                propertyGrid = null;
            }

            if (ReferenceEquals(e.View, messageWindow))
            {
                messageWindow = null;
            }

            if (ReferenceEquals(e.View, projectExplorer))
            {
                projectExplorer = null;
            }
        }

        private void UpdateToolWindowButtonState()
        {
            if (viewController.ViewHost != null)
            {
                ButtonShowProjectExplorer.IsChecked = viewController.ViewHost.ToolViews.Contains(ProjectExplorer);
                ButtonShowMessages.IsChecked = viewController.ViewHost.ToolViews.Contains(MessageWindow);
                ButtonShowProperties.IsChecked = viewController.ViewHost.ToolViews.Contains(PropertyGrid);
            }
        }
        
        private void InitProjectExplorerWindowOrBringToFront()
        {
            if (gui == null)
            {
                throw new InvalidOperationException("Must call 'SetGui(IGui)' before calling 'InitMessagesWindowOrActivate'.");
            }

            if (projectExplorer == null)
            {
                projectExplorer = new ProjectExplorer.ProjectExplorer(gui.ViewCommands, gui.GetTreeNodeInfos())
                {
                    Data = gui.Project
                };
                viewController.ViewHost.AddToolView(projectExplorer, ToolViewLocation.Left);
                viewController.ViewHost.SetImage(messageWindow, Properties.Resources.application_view_list);
            }
            else
            {
                viewController.ViewHost.BringToFront(projectExplorer);
            }
        }
        
        private void InitMessagesWindowOrBringToFront()
        {
            if (gui == null)
            {
                throw new InvalidOperationException("Must call 'SetGui(IGui)' before calling 'InitMessagesWindowOrActivate'.");
            }

            if (messageWindow == null)
            {
                messageWindow = new MessageWindow.MessageWindow(this)
                {
                    Text = Properties.Resources.Messages
                };
                viewController.ViewHost.AddToolView(messageWindow, ToolViewLocation.Bottom);
                viewController.ViewHost.SetImage(messageWindow, Properties.Resources.application_view_list);
            }
            else
            {
                viewController.ViewHost.BringToFront(messageWindow);
            }
        }

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