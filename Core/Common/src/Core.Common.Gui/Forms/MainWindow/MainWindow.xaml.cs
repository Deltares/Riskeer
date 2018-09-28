// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Fluent;

namespace Core.Common.Gui.Forms.MainWindow
{
    /// <summary>
    /// Main user interface of the application.
    /// </summary>
    public partial class MainWindow : IMainWindow, IDisposable, ISynchronizeInvoke
    {
        /// <summary>
        /// Remember last active contextual tab per view.
        /// </summary>
        private readonly IDictionary<Type, string> lastActiveContextTabNamePerViewType = new Dictionary<Type, string>();

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

        private IEnumerable<IRibbonCommandHandler> ribbonCommandHandlers;

        /// <summary>
        /// This is used when user selects non-contextual tab explicitly. Then we won't 
        /// activate contextual tab on the next view activation.
        /// </summary>
        private bool activateContextualTab;

        /// <summary>
        /// Used when contextual tab was activated and we switch back to view which does 
        /// not support contextual tabs.
        /// </summary>
        private string lastNonContextualTab;

        private IGui gui;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            windowInteropHelper = new WindowInteropHelper(this);
        }

        /// <summary>
        /// Gets a value indicating whether this window is disposed.
        /// </summary>
        public bool IsWindowDisposed { get; private set; }

        /// <summary>
        /// Gets the log messages tool window.
        /// </summary>
        public IMessageWindow MessageWindow
        {
            get
            {
                return messageWindow;
            }
        }

        /// <summary>
        /// Gets the view host.
        /// </summary>
        public IViewHost ViewHost
        {
            get
            {
                return AvalonDockViewHost;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the main user interface is visible.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no gui has been set using <see cref="SetGui"/>.</exception>
        public bool Visible
        {
            get
            {
                return IsVisible;
            }
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

        public IView PropertyGrid
        {
            get
            {
                return propertyGrid;
            }
        }

        public IntPtr Handle
        {
            get
            {
                return windowInteropHelper.Handle;
            }
        }

        public bool InvokeRequired
        {
            get
            {
                return !Dispatcher.CheckAccess();
            }
        }

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
                viewController.ViewHost.ActiveDocumentViewChanged += OnActiveDocumentViewChanged;
                viewController.ViewHost.ActiveDocumentViewChanging += OnActiveDocumentViewChanging;
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
                viewController.ViewHost.ActiveDocumentViewChanged -= OnActiveDocumentViewChanged;
                viewController.ViewHost.ActiveDocumentViewChanging -= OnActiveDocumentViewChanging;
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
            InitMessagesWindowOrActivate();
            InitPropertiesWindowAndActivate();
        }

        public void ValidateItems()
        {
            if (gui == null)
            {
                return;
            }

            UpdateToolWindowButtonState();

            if (ribbonCommandHandlers == null)
            {
                return;
            }

            foreach (IRibbonCommandHandler ribbonCommandHandler in ribbonCommandHandlers)
            {
                ribbonCommandHandler.ValidateItems();
            }
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
        public void InitPropertiesWindowAndActivate()
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

            if (Ribbon != null)
            {
                foreach (RibbonTabItem tab in Ribbon.Tabs)
                {
                    foreach (RibbonGroupBox group in tab.Groups)
                    {
                        group.Items.Clear();
                    }

                    tab.Groups.Clear();
                }

                Ribbon.Tabs.Clear();
                Ribbon = null;
            }

            ribbonCommandHandlers = null;

            SetGui(null);
        }

        private void OnActiveDocumentViewChanging(object sender, EventArgs e)
        {
            if (Ribbon.SelectedTabItem != null && !Ribbon.SelectedTabItem.IsContextual)
            {
                lastNonContextualTab = Ribbon.SelectedTabItem.Header.ToString();
            }

            // remember active contextual tab per view type, when view is activated back - activate contextual item
            IView activeDocumentView = viewController.ViewHost.ActiveDocumentView;
            if (Ribbon.SelectedTabItem != null && activeDocumentView != null)
            {
                if (Ribbon.SelectedTabItem.IsContextual)
                {
                    lastActiveContextTabNamePerViewType[activeDocumentView.GetType()] = Ribbon.SelectedTabItem.Header.ToString();
                    activateContextualTab = true;
                }
                else
                {
                    // user has clicked on non-contextual tab before switching active view
                    if (lastActiveContextTabNamePerViewType.ContainsKey(activeDocumentView.GetType()))
                    {
                        activateContextualTab = false;
                    }
                }
            }
            else
            {
                activateContextualTab = true;
            }
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
        }

        private void OnActiveDocumentViewChanged(object sender, EventArgs e)
        {
            // activate contextual tab which was active for this view type
            IView activeDocumentView = viewController.ViewHost.ActiveDocumentView;
            if (activateContextualTab && Ribbon.SelectedTabItem != null && activeDocumentView != null &&
                Ribbon.Tabs.Any(t => t.IsContextual && t.Visibility == Visibility.Visible))
            {
                string lastActiveTabForActiveDocumentView;
                if (lastActiveContextTabNamePerViewType.TryGetValue(activeDocumentView.GetType(), out lastActiveTabForActiveDocumentView))
                {
                    RibbonTabItem tab = Ribbon.Tabs.First(t => t.Header.ToString() == lastActiveTabForActiveDocumentView);
                    if (tab.IsVisible)
                    {
                        Ribbon.SelectedTabItem = tab;
                    }
                }
                else // activate first contextual group tab
                {
                    RibbonTabItem tab = Ribbon.Tabs.FirstOrDefault(t => t.IsContextual && t.Visibility == Visibility.Visible);
                    Ribbon.SelectedTabItem = tab;
                }
            }
            else if (!string.IsNullOrEmpty(lastNonContextualTab)) // reactivate last non-contextual tab
            {
                RibbonTabItem tab = Ribbon.Tabs.First(t => t.Header.ToString() == lastNonContextualTab);
                Ribbon.SelectedTabItem = tab;
            }
        }

        private void UpdateToolWindowButtonState()
        {
            if (viewController.ViewHost != null)
            {
                ButtonShowMessages.IsChecked = viewController.ViewHost.ToolViews.Contains(MessageWindow);
                ButtonShowProperties.IsChecked = viewController.ViewHost.ToolViews.Contains(PropertyGrid);
            }
        }

        private void InitMessagesWindowOrActivate()
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

            ButtonQuickAccessOpenProject.IsEnabled = ButtonMenuFileOpenProject.IsEnabled;
            ButtonQuickAccessSaveProject.IsEnabled = ButtonMenuFileSaveProject.IsEnabled;

            UpdateRibbonExtensions(gui);

            ValidateItems();
        }

        private void UpdateRibbonExtensions(IPluginsHost pluginsHost)
        {
            // get all ribbon controls
            ribbonCommandHandlers = pluginsHost.Plugins.Where(p => p.RibbonCommandHandler != null)
                                               .Select(p => p.RibbonCommandHandler);

            foreach (Ribbon ribbonControl in ribbonCommandHandlers.Select(rch => rch.GetRibbonControl()))
            {
                FillContextualGroupsFromRibbonComponent(ribbonControl);

                FillRibbonTabsFromRibbonComponent(ribbonControl);
            }
        }

        private void FillContextualGroupsFromRibbonComponent(Ribbon ribbonControl)
        {
            foreach (RibbonContextualTabGroup group in ribbonControl.ContextualGroups.Where(
                group => Ribbon.ContextualGroups.All(g => g.Name != group.Name)))
            {
                Ribbon.ContextualGroups.Add(group);
            }
        }

        private void FillRibbonTabsFromRibbonComponent(Ribbon ribbonControl)
        {
            foreach (RibbonTabItem sourceTab in ribbonControl.Tabs)
            {
                RibbonTabItem existingTab = Ribbon.Tabs.FirstOrDefault(t => t.Header.Equals(sourceTab.Header));
                if (existingTab == null) // add new tab
                {
                    Ribbon.Tabs.Add(sourceTab);
                    existingTab = sourceTab;
                }
                else
                {
                    SetReduceOrder(existingTab, sourceTab);

                    CopyRibbonGroupBoxes(existingTab, sourceTab);
                }

                UpdateContextualTabBindings(existingTab);
            }
        }

        private static void SetReduceOrder(RibbonTabItem targetTab, RibbonTabItem sourceTab)
        {
            if (!string.IsNullOrEmpty(sourceTab.ReduceOrder))
            {
                targetTab.ReduceOrder += "," + sourceTab.ReduceOrder; // Naive implementation; Can cause duplicates to occur
            }
        }

        private static void CopyRibbonGroupBoxes(RibbonTabItem targetTab, RibbonTabItem sourceTab)
        {
            foreach (RibbonGroupBox sourceGroup in sourceTab.Groups)
            {
                RibbonGroupBox existingGroup = targetTab.Groups.FirstOrDefault(g => g.Header.Equals(sourceGroup.Header));
                if (existingGroup == null) // add new group
                {
                    RibbonGroupBox newGroup = CreateRibbonGroupBox(sourceGroup);
                    targetTab.Groups.Add(newGroup);
                    existingGroup = newGroup;
                }

                InitializeRibbonGroupBoxIcon(existingGroup, sourceGroup.Icon);

                CopyGroupBoxItems(existingGroup, sourceGroup);
            }
        }

        private static RibbonGroupBox CreateRibbonGroupBox(RibbonGroupBox sourceGroup)
        {
            var newGroup = new RibbonGroupBox
            {
                Header = sourceGroup.Header,
                Name = sourceGroup.Name // Ensure ReduceOrder is working properly
            };

            // Set KeyTip for keyboard navigation:
            string keys = KeyTip.GetKeys(sourceGroup);
            if (!string.IsNullOrEmpty(keys))
            {
                KeyTip.SetKeys(newGroup, keys);
            }

            return newGroup;
        }

        private static void InitializeRibbonGroupBoxIcon(RibbonGroupBox existingGroup, object icon)
        {
            if (existingGroup.Icon == null && icon != null)
            {
                existingGroup.Icon = icon;
            }
        }

        private static void CopyGroupBoxItems(RibbonGroupBox targetGroupBox, RibbonGroupBox sourceGroupBox)
        {
            foreach (object item in sourceGroupBox.Items.Cast<object>().ToArray())
            {
                // HACK: remember and restore button size (looks like a bug in Fluent)
                var iconSize = RibbonControlSize.Small;
                var buttonItem = item as Button;
                if (buttonItem != null)
                {
                    Button button = buttonItem;
                    iconSize = button.Size;
                }

                sourceGroupBox.Items.Remove(item);
                targetGroupBox.Items.Add(item);

                if (buttonItem != null)
                {
                    Button button = targetGroupBox.Items.OfType<Button>().Last();
                    button.Size = iconSize;
                }
            }
        }

        private void UpdateContextualTabBindings(RibbonTabItem existingTab)
        {
            if (existingTab.Group != null)
            {
                RibbonContextualTabGroup newGroup = Ribbon.ContextualGroups.First(g => g.Name == existingTab.Group.Name);
                existingTab.Group = newGroup;
            }
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
                InitPropertiesWindowAndActivate();
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
                InitMessagesWindowOrActivate();
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
                Title = Properties.Resources.Ribbon_About,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Core.Common.Gui;component/Resources/information.png", UriKind.Absolute)),
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