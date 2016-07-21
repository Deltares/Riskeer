﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Selection;
using Core.Common.Gui.Settings;
using Fluent;
using Button = Fluent.Button;
using Cursors = System.Windows.Input.Cursors;
using WindowsFormApplication = System.Windows.Forms.Application;

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
        private RichTextFile richTextFile;

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

        public IMessageWindow MessageWindow
        {
            get
            {
                return messageWindow;
            }
        }

        public IViewHost ViewHost
        {
            get
            {
                return avalonDockViewHost;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the main user interface is visible.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">When no gui has been set using <see cref="SetGui"/>.</exception>
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

        public string StatusBarMessage
        {
            get
            {
                return StatusMessageTextBlock.Text;
            }
            set
            {
                StatusMessageTextBlock.Text = value;
            }
        }

        public IPropertyGrid PropertyGrid
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
            if (viewController != null && viewController.ViewHost != null)
            {
                viewController.ViewHost.ActiveDocumentViewChanged += OnActiveDocumentViewChanged;
                viewController.ViewHost.ActiveDocumentViewChanging += OnActiveDocumentViewChanging;
            }
        }

        /// <summary>
        /// Unsubscribes the main user interface from the <see cref="IGui"/> provided by <see cref="SetGui"/>.
        /// </summary>
        public void UnsubscribeFromGui()
        {
            if (viewController != null && viewController.ViewHost != null)
            {
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

        public void Dispose()
        {
            if (IsWindowDisposed)
            {
                return;
            }

            IsWindowDisposed = true;

            Close();

            if (Ribbon != null)
            {
                foreach (var tab in Ribbon.Tabs)
                {
                    foreach (var group in tab.Groups)
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

            if (propertyGrid == null || propertyGrid.IsDisposed)
            {
                propertyGrid = new PropertyGridView.PropertyGridView(applicationSelection, gui.PropertyResolver);
            }

            propertyGrid.Text = Properties.Resources.Properties_Title;
            propertyGrid.Data = propertyGrid.GetObjectProperties(applicationSelection.Selection);

            viewController.ViewHost.AddToolView(propertyGrid, ToolViewLocation.Right);
            viewController.ViewHost.SetImage(propertyGrid, Properties.Resources.PropertiesHS);
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

            // activate all context-specific groups (ValidateItems implementations should activate them)
            foreach (var tabGroup in Ribbon.ContextualGroups)
            {
                var showGroup = false;
                foreach (var tabItem in tabGroup.Items)
                {
                    var tabItemVisible = ribbonCommandHandlers.Any(h => h.IsContextualTabVisible(tabGroup.Name, tabItem.Name));
                    tabItem.Visibility = tabItemVisible ? Visibility.Visible : Visibility.Collapsed;

                    if (tabItemVisible && !showGroup)
                    {
                        showGroup = true;
                    }
                }

                tabGroup.Visibility = showGroup ? Visibility.Visible : Visibility.Collapsed;
            }

            foreach (var ribbonCommandHandler in ribbonCommandHandlers)
            {
                ribbonCommandHandler.ValidateItems();
            }

            foreach (var ribbonGroupBox in Ribbon.Tabs.SelectMany(tab => tab.Groups))
            {
                // Colapse all groups without visible items
                ribbonGroupBox.Visibility = ribbonGroupBox.Items.OfType<UIElement>().All(e => e.Visibility == Visibility.Collapsed || e is Separator)
                                                ? Visibility.Collapsed
                                                : Visibility.Visible;
            }
        }

        public void SetWaitCursorOn()
        {
            Mouse.OverrideCursor = Cursors.Wait;
        }

        public void SetWaitCursorOff()
        {
            Mouse.OverrideCursor = null;
        }

        private void OnActiveDocumentViewChanging(object sender, EventArgs e)
        {
            if (Ribbon.SelectedTabItem != null && !Ribbon.SelectedTabItem.IsContextual)
            {
                lastNonContextualTab = Ribbon.SelectedTabItem.Header.ToString();
            }

            // remember active contextual tab per view type, when view is activated back - activate contextual item
            var activeDocumentView = viewController.ViewHost.ActiveDocumentView;
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

        private void OnActiveDocumentViewChanged(object sender, EventArgs e)
        {
            // activate contextual tab which was active for this view type
            var activeDocumentView = viewController.ViewHost.ActiveDocumentView;
            if (activateContextualTab && Ribbon.SelectedTabItem != null && activeDocumentView != null &&
                Ribbon.Tabs.Any(t => t.IsContextual && t.Visibility == Visibility.Visible))
            {
                string lastActiveTabForActiveDocumentView;
                if (lastActiveContextTabNamePerViewType.TryGetValue(activeDocumentView.GetType(), out lastActiveTabForActiveDocumentView))
                {
                    var tab = Ribbon.Tabs.First(t => t.Header.ToString() == lastActiveTabForActiveDocumentView);
                    if (tab.IsVisible)
                    {
                        Ribbon.SelectedTabItem = tab;
                    }
                }
                else // activate first contextual group tab
                {
                    var tab = Ribbon.Tabs.FirstOrDefault(t => t.IsContextual && t.Visibility == Visibility.Visible);
                    Ribbon.SelectedTabItem = tab;
                }
            }
            else if (!string.IsNullOrEmpty(lastNonContextualTab)) // reactivate last non-contextual tab
            {
                var tab = Ribbon.Tabs.First(t => t.Header.ToString() == lastNonContextualTab);
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

            if (messageWindow == null || messageWindow.IsDisposed)
            {
                messageWindow = new MessageWindow.MessageWindow(this)
                {
                    Text = Properties.Resources.Messages
                };
            }

            if (viewController.ViewHost != null)
            {
                viewController.ViewHost.AddToolView(messageWindow, ToolViewLocation.Bottom);
                viewController.ViewHost.SetImage(messageWindow, Properties.Resources.application_view_list);
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
            commands.StorageCommands.OpenExistingProject();
        }

        private void OnFileNewClicked(object sender, RoutedEventArgs e)
        {
            // Original code:
            commands.StorageCommands.CreateNewProject();
            ValidateItems();
        }

        private void OnFileExitClicked(object sender, RoutedEventArgs e)
        {
            gui.Exit();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;

            FileManualButton.IsEnabled = File.Exists(settings.FixedSettings.ManualFilePath);

            LicenseButton.IsEnabled = File.Exists(settings.FixedSettings.LicenseFilePath);

            ButtonQuickAccessOpenProject.IsEnabled = ButtonMenuFileOpenProject.IsEnabled;
            ButtonQuickAccessSaveProject.IsEnabled = ButtonMenuFileSaveProject.IsEnabled;

            UpdateRibbonExtensions(gui);

            richTextFile = new RichTextFile
            {
                Name = Properties.Resources.MainWindow_LicenseView_Name,
                FilePath = settings.FixedSettings.LicenseFilePath
            };

            ValidateItems();
        }

        private void UpdateRibbonExtensions(IPluginsHost pluginsHost)
        {
            // get all ribbon controls
            ribbonCommandHandlers = pluginsHost.Plugins.Where(p => p.RibbonCommandHandler != null).Select(p => p.RibbonCommandHandler).ToArray();

            foreach (var ribbonControl in ribbonCommandHandlers.Select(rch => rch.GetRibbonControl())) 
            {
                // fill contextual groups from plugins
                foreach (var group in ribbonControl.ContextualGroups.Where(group => Ribbon.ContextualGroups.All(g => g.Name != group.Name)))
                {
                    Ribbon.ContextualGroups.Add(group);
                }

                // fill tabs from plugins
                foreach (var tab in ribbonControl.Tabs)
                {
                    var existingTab = Ribbon.Tabs.FirstOrDefault(t => t.Header.Equals(tab.Header));

                    if (existingTab == null) // add new tab
                    {
                        Ribbon.Tabs.Add(tab);
                        existingTab = tab;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(tab.ReduceOrder))
                        {
                            existingTab.ReduceOrder += "," + tab.ReduceOrder; // Naive implementation; Can cause duplicates to occur
                        }

                        foreach (var group in tab.Groups)
                        {
                            var existingGroup = existingTab.Groups.FirstOrDefault(g => g.Header.Equals(group.Header));

                            if (existingGroup == null) // add new group
                            {
                                var newGroup = new RibbonGroupBox
                                {
                                    Header = group.Header,
                                    Name = group.Name // Ensure ReduceOrder is working properly
                                };

                                // Set KeyTip for keyboard navigation:
                                var keys = KeyTip.GetKeys(group);
                                if (!string.IsNullOrEmpty(keys))
                                {
                                    KeyTip.SetKeys(newGroup, keys);
                                }

                                existingTab.Groups.Add(newGroup);
                                existingGroup = newGroup;
                            }

                            // Set group icon if not yet set:
                            if (existingGroup.Icon == null && group.Icon != null)
                            {
                                existingGroup.Icon = group.Icon;
                            }

                            foreach (var item in group.Items.Cast<object>().ToArray())
                            {
                                // HACK: remember and restore button size (looks like a bug in Fluent)
                                var iconSize = RibbonControlSize.Small;
                                if (item is Button)
                                {
                                    var button = (Button) item;
                                    iconSize = button.Size;
                                }

                                group.Items.Remove(item);
                                existingGroup.Items.Add(item);

                                if (item is Button)
                                {
                                    var button = existingGroup.Items.OfType<Button>().Last();
                                    button.Size = iconSize;
                                }
                            }
                        }
                    }

                    // update contextual tab bindings
                    if (existingTab.Group != null)
                    {
                        var newGroup = Ribbon.ContextualGroups.First(g => g.Name == existingTab.Group.Name);
                        existingTab.Group = newGroup;
                    }
                }
            }
        }

        private void ButtonShowProperties_Click(object sender, RoutedEventArgs e)
        {
            var active = viewController.ViewHost.ToolViews.Contains(PropertyGrid);

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
            var active = viewController.ViewHost.ToolViews.Contains(MessageWindow);

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

        private void OnFileHelpLicense_Clicked(object sender, RoutedEventArgs e)
        {
            viewController.DocumentViewController.OpenViewForData(richTextFile);
        }

        private void OnFileHelpShowLog_Clicked(object sender, RoutedEventArgs e)
        {
            commands.ApplicationCommands.OpenLogFileExternal();
        }

        private void OnFileManual_Clicked(object sender, RoutedEventArgs e)
        {
            var manualFileName = settings.FixedSettings.ManualFilePath;

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
                HasProgress = false,
                VersionText = SettingsHelper.ApplicationVersion,
                CopyrightText = settings.FixedSettings.Copyright,
                LicenseText = settings.FixedSettings.LicenseDescription,
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