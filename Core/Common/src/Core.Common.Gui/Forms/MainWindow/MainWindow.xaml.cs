﻿// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Core.Common.Gui.Forms.MainWindow.Interop;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Gui.Forms.Options;
using Core.Common.Gui.Properties;
using Core.Common.Gui.Theme;
using Core.Common.Utils;
using Core.Common.Utils.Events;
using Fluent;
using log4net;
using Microsoft.Win32;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Xceed.Wpf.AvalonDock.Themes;
using Button = Fluent.Button;
using Cursors = System.Windows.Input.Cursors;
using WindowsFormApplication = System.Windows.Forms.Application;

namespace Core.Common.Gui.Forms.MainWindow
{
    /// <summary>
    /// Main windows of Ringtoets
    /// </summary>
    public partial class MainWindow : IMainWindow, IDisposable, ISynchronizeInvoke
    {
        private const string dockingLayoutFileName = "WindowLayout_normal.xml";
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        /// <summary>
        /// Remember last active contextual tab per view.
        /// </summary>
        private readonly IDictionary<Type, string> lastActiveContextTabNamePerViewType = new Dictionary<Type, string>();

        private IToolViewController toolViewController;
        private IDocumentViewController documentViewController;
        private ICommandsOwner commands;
        private ISettingsOwner settings;
        private IApplicationSelection applicationSelection;

        private bool resetDefaultLayout;

        private MessageWindow.MessageWindow messageWindow;
        private PropertyGridView.PropertyGridView propertyGrid;
        private WindowInteropHelper windowInteropHelper;

        private IEnumerable<IRibbonCommandHandler> ribbonCommandHandlers;

        /// <summary>
        /// This is used when user selects non-contextual tab explicitly. Then we won't activate contextual tab on the next view activation.
        /// </summary>
        private bool activateContextualTab;

        /// <summary>
        /// Used when contextual tab was activated and we switch back to view which does not support contextual tabs.
        /// </summary>
        private string lastNonContextualTab;

        private IGui gui;

        public MainWindow()
        {
            InitializeComponent();

            windowInteropHelper = new WindowInteropHelper(this);

            log.Info(Properties.Resources.MainWindow_MainWindow_Main_window_created_);
        }

        public IGui Gui
        {
            get
            {
                return gui;
            }
            set
            {
                gui = value;

                toolViewController = gui;
                documentViewController = gui;
                settings = gui;
                commands = gui;
                applicationSelection = gui;
            }
        }

        public bool IsWindowDisposed { get; private set; }

        public bool Enabled
        {
            get
            {
                return IsEnabled;
            }
            set
            {
                IsEnabled = value;
                NativeWin32.EnableWindow(new HandleRef(this, windowInteropHelper.Handle), value); // prevents resize etc
            }
        }

        public DockingManager DockingManager
        {
            get
            {
                return dockingManager;
            }
        }

        public IMessageWindow MessageWindow
        {
            get
            {
                return messageWindow;
            }
        }

        public bool Visible
        {
            get
            {
                return IsVisible;
            }
            set
            {
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

        public void SubscribeToGui()
        {
            if (toolViewController != null && toolViewController.ToolWindowViews != null)
            {
                toolViewController.ToolWindowViews.CollectionChanged += ToolWindowViews_CollectionChanged;
            }
            if (documentViewController != null && documentViewController.DocumentViews != null)
            {
                documentViewController.DocumentViews.ActiveViewChanged += DocumentViewsOnActiveViewChanged;
                documentViewController.DocumentViews.ActiveViewChanging += DocumentViewsOnActiveViewChanging;
            }
        }

        public void UnsubscribeFromGui()
        {
            if (toolViewController != null && toolViewController.ToolWindowViews != null)
            {
                toolViewController.ToolWindowViews.CollectionChanged -= ToolWindowViews_CollectionChanged;
            }
            if (documentViewController != null && documentViewController.DocumentViews != null)
            {
                documentViewController.DocumentViews.ActiveViewChanged -= DocumentViewsOnActiveViewChanged;
                documentViewController.DocumentViews.ActiveViewChanging -= DocumentViewsOnActiveViewChanging;
            }
        }

        public void LoadLayout()
        {
            LoadDockingLayout();
            RestoreWindowAppearance();
        }

        public void SaveLayout()
        {
            SaveWindowAppearance();
            SaveDockingLayout();
        }

        public void InitializeToolWindows()
        {
            InitMessagesWindowOrActivate();
            InitPropertiesWindowAndActivate();
        }

        public void SuspendLayout() {}

        public void ResumeLayout() {}

        public void InitPropertiesWindowAndActivate()
        {
            if ((propertyGrid == null) || (propertyGrid.IsDisposed))
            {
                propertyGrid = new PropertyGridView.PropertyGridView(applicationSelection, gui.PropertyResolver);
            }

            propertyGrid.Text = Properties.Resources.Properties_Title;
            propertyGrid.Data = propertyGrid.GetObjectProperties(applicationSelection.Selection);

            toolViewController.ToolWindowViews.Add(propertyGrid, ViewLocation.Right | ViewLocation.Bottom);

            toolViewController.ToolWindowViews.ActiveView = null;
            toolViewController.ToolWindowViews.ActiveView = propertyGrid;
        }

        public void ShowStartPage(bool checkUseSettings = true)
        {
            if (!checkUseSettings || Convert.ToBoolean(settings.UserSettings["showStartPage"], CultureInfo.InvariantCulture))
            {
                log.Info(Properties.Resources.MainWindow_ShowStartPage_Adding_welcome_page_);
                OpenStartPage();
            }
        }

        public void ClearDocumentTabs()
        {
            foreach (var contentToClose in dockingManager.Layout.Descendents().OfType<LayoutContent>().Where(d => (d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow)).ToArray())
            {
                if (!contentToClose.CanClose)
                {
                    continue;
                }

                if (contentToClose is LayoutDocument)
                {
                    CloseContent(contentToClose as LayoutDocument);
                }
                else if (contentToClose is LayoutAnchorable)
                {
                    CloseContent(contentToClose as LayoutAnchorable);
                }
            }

            foreach (var child in LayoutDocumentPaneGroup.Children.OfType<LayoutDocumentPane>())
            {
                child.Children.Clear();
            }

            while (LayoutDocumentPaneGroup.Children.Count != 1)
            {
                LayoutDocumentPaneGroup.Children.RemoveAt(0);
            }
        }

        public void Dispose()
        {
            Close();
            Close();

            if (IsWindowDisposed)
            {
                return;
            }

            // TODO: add dispose code

            IsWindowDisposed = true;

            if (dockingManager.AutoHideWindow != null)
            {
                var m = typeof(LayoutAutoHideWindowControl).GetField("_manager", BindingFlags.Instance | BindingFlags.NonPublic);
                m.SetValue(dockingManager.AutoHideWindow, null);
                dockingManager.AutoHideWindow.Dispose();
            }

            Content = null;

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

            dockingManager = null;

            if (propertyGrid != null)
            {
                propertyGrid.Dispose();
                propertyGrid = null;
            }

            if (messageWindow != null)
            {
                messageWindow.Dispose();
                messageWindow = null;
            }

            // I pulled this code from some internet sources combined with the reflector to remove a well-known leak
            var handlers = typeof(SystemEvents).GetField("_handlers", BindingFlags.Static | BindingFlags.NonPublic)
                                               .GetValue(null);
            if (handlers != null)
            {
                var upcHandler = typeof(SystemEvents).GetField("OnUserPreferenceChangedEvent", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                var eventLockObject = typeof(SystemEvents).GetField("eventLockObject", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

                lock (eventLockObject)
                {
                    var upcHandlerList = (IList)((IDictionary)handlers)[upcHandler];
                    for (int i = upcHandlerList.Count - 1; i >= 0; i--)
                    {
                        var target = (Delegate)upcHandlerList[i].GetType().GetField("_delegate", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(upcHandlerList[i]);
                        upcHandlerList.RemoveAt(i);
                    }
                }
            }
            
            ribbonCommandHandlers = null;
            windowInteropHelper = null;

            Gui = null;

            // Dispatcher.InvokeShutdown();
            //System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        public void ValidateItems()
        {
            if (Gui == null)
            {
                return;
            }

            ValidateMainWindowRibbonItems();

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

        private void ToolWindowViews_CollectionChanged(object sender, NotifyCollectionChangeEventArgs e)
        {
            if (e.Action != NotifyCollectionChangeAction.Remove)
            {
                return;
            }

            if (e.Item == propertyGrid)
            {
                propertyGrid = null;
            }

            if (e.Item == MessageWindow)
            {
                messageWindow = null;
            }
        }

        private void DocumentViewsOnActiveViewChanging(object sender, ActiveViewChangeEventArgs e)
        {
            if (Ribbon.SelectedTabItem != null && !Ribbon.SelectedTabItem.IsContextual)
            {
                lastNonContextualTab = Ribbon.SelectedTabItem.Header.ToString();
            }

            // remember active contextual tab per view type, when view is activated back - activate contextual item
            if (Ribbon.SelectedTabItem != null && e.OldView != null)
            {
                if (Ribbon.SelectedTabItem.IsContextual)
                {
                    lastActiveContextTabNamePerViewType[e.OldView.GetType()] = Ribbon.SelectedTabItem.Header.ToString();
                    activateContextualTab = true;
                }
                else
                {
                    // user has clicked on non-contextual tab before switching active view
                    if (lastActiveContextTabNamePerViewType.ContainsKey(e.OldView.GetType()))
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

        private void DocumentViewsOnActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            // activate contextual tab which was active for this view type
            if (activateContextualTab && Ribbon.SelectedTabItem != null && e.View != null &&
                Ribbon.Tabs.Any(t => t.IsContextual && t.Visibility == Visibility.Visible))
            {
                string lastActiveTabForActiveView;
                if (lastActiveContextTabNamePerViewType.TryGetValue(e.View.GetType(), out lastActiveTabForActiveView))
                {
                    var tab = Ribbon.Tabs.First(t => t.Header.ToString() == lastActiveTabForActiveView);
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

        private void AddRecentlyOpenedProjectsToFileMenu()
        {
            var mruList = Settings.Default["mruList"] as StringCollection;

            foreach (var recent in mruList)
            {
                AddNewMruItem(recent, false);
            }
        }

        private void AddNewMruItem(string path, bool putOnTop = true)
        {
            var newItem = new TabItem
            {
                Header = path
            };
            newItem.MouseDoubleClick += (sender, args) =>
            {
                try
                {
                    commands.StorageCommands.OpenExistingProject(path);
                    RecentProjectsTabControl.Items.Remove(newItem);
                    RecentProjectsTabControl.Items.Insert(1, newItem);
                }
                catch (Exception)
                {
                    //remove item from the list if it cannot be retrieved from file
                    RecentProjectsTabControl.Items.Remove(newItem);
                    log.WarnFormat("{0} {1}", Properties.Resources.MainWindow_AddNewMruItem_Can_t_open_project, path);
                }
                finally
                {
                    CommitMruToSettings();
                    ValidateItems();
                    Menu.IsOpen = false;
                }
            };
            if (RecentProjectsTabControl.Items.OfType<TabItem>().Any(i => Equals(i.Header, path)))
            {
                return;
            }

            if (putOnTop)
            {
                RecentProjectsTabControl.Items.Insert(1, newItem);
            }
            else
            {
                RecentProjectsTabControl.Items.Add(newItem);
            }
        }

        private void CommitMruToSettings()
        {
            var mruList = (StringCollection) Settings.Default["mruList"];

            mruList.Clear();

            foreach (TabItem item in RecentProjectsTabControl.Items)
            {
                if (item is SeparatorTabItem) //header
                {
                    continue;
                }

                mruList.Add(item.Header.ToString());
            }
        }

        private void OnLayoutRootPropertyChanged(object sender, PropertyChangedEventArgs e) {}

        private void ValidateMainWindowRibbonItems()
        {
            if (toolViewController.ToolWindowViews != null)
            {
                ButtonShowMessages.IsChecked = toolViewController.ToolWindowViews.Contains(MessageWindow);
                ButtonShowProperties.IsChecked = toolViewController.ToolWindowViews.Contains(PropertyGrid);
            }
        }

        private void InitMessagesWindowOrActivate()
        {
            if (messageWindow == null || messageWindow.IsDisposed)
            {
                messageWindow = new MessageWindow.MessageWindow(this)
                {
                    Text = Properties.Resources.Messages
                };
            }

            if (toolViewController.ToolWindowViews == null)
            {
                return;
            }

            if (toolViewController.ToolWindowViews.Contains(messageWindow))
            {
                toolViewController.ToolWindowViews.ActiveView = messageWindow;
                return;
            }

            toolViewController.ToolWindowViews.Add(messageWindow, ViewLocation.Bottom);
        }

        private void OnFileSaveClicked(object sender, RoutedEventArgs e)
        {
            var saveProject = commands.StorageCommands.SaveProject();
            OnAfterProjectSaveOrOpen(saveProject);
        }

        private void OnFileSaveAsClicked(object sender, RoutedEventArgs e)
        {
            var saveProject = commands.StorageCommands.SaveProjectAs();
            OnAfterProjectSaveOrOpen(saveProject);
        }

        private void OnAfterProjectSaveOrOpen(bool actionSuccesful)
        {
            //TODO: Implement

            // Original code:
            /*
            if (actionSuccesful)
            {
                AddNewMruItem(Gui.ApplicationCore.ProjectFilePath);
                CommitMruToSettings();
            }
            ValidateItems();
            */
        }

        private void OnFileOpenClicked(object sender, RoutedEventArgs e)
        {
            var succesful = commands.StorageCommands.OpenExistingProject();
            OnAfterProjectSaveOrOpen(succesful);
        }

        private void OnFileNewClicked(object sender, RoutedEventArgs e)
        {
            // Original code:
            commands.StorageCommands.CreateNewProject();
            ValidateItems();
        }

        private string GetLayoutFilePath()
        {
            string localUserSettingsDirectory = SettingsHelper.GetApplicationLocalUserSettingsDirectory();
            return Path.Combine(localUserSettingsDirectory, dockingLayoutFileName);
        }

        private static string GetLocalLayoutFilePath()
        {
            var uri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string assemblyDir = Path.GetDirectoryName(path);
            return Path.Combine(assemblyDir, dockingLayoutFileName);
        }

        private void LoadDockingLayout()
        {
            string layoutFilePath = GetLayoutFilePath();

            if (!File.Exists(layoutFilePath))
            {
                layoutFilePath = GetLocalLayoutFilePath();
            }
            if (!File.Exists(layoutFilePath))
            {
                return;
            }

            try
            {
                new XmlLayoutSerializer(dockingManager).Deserialize(layoutFilePath);
            }
            catch (InvalidOperationException)
            {
                log.Warn(Properties.Resources.MainWindow_OnLoadLayout_Could_not_load_the_requested_dock_layout_The_settings_are_invalid_and_will_be_reset_to_the_default_state_);
                return;
            }

            var ribbonSettingsPath = layoutFilePath + ".ribbon";

            if (!File.Exists(ribbonSettingsPath))
            {
                return;
            }

            try
            {
                var settings = File.ReadAllText(ribbonSettingsPath).Split('|');
                if (settings.Length != 2)
                {
                    return;
                }

                var ribbonStateSettings = settings[0].Split(',');
                Ribbon.IsMinimized = bool.Parse(ribbonStateSettings[0]);
                Ribbon.ShowQuickAccessToolBarAboveRibbon = bool.Parse(ribbonStateSettings[1]);
            }
            catch (Exception)
            {
                log.Warn(Properties.Resources.MainWindow_OnLoadLayout_Could_not_restore_the_ribbon_state_The_settings_are_invalid_and_will_be_reset_to_the_default_state_);
            }
        }

        private void SaveDockingLayout()
        {
            string layoutFilePath = GetLayoutFilePath();
            var ribbonLayoutFilePath = layoutFilePath + ".ribbon";
            if (resetDefaultLayout)
            {
                if (File.Exists(layoutFilePath))
                {
                    File.Delete(layoutFilePath);
                }
                if (File.Exists(ribbonLayoutFilePath))
                {
                    File.Delete(ribbonLayoutFilePath);
                }
                return; // Do not save when resetting
            }

            var layoutSerializer = new XmlLayoutSerializer(dockingManager);
            layoutSerializer.Serialize(layoutFilePath);

            var ribbonStream = new FileStream(ribbonLayoutFilePath, FileMode.Create);
            Ribbon.SaveState(ribbonStream);
        }

        private void RestoreWindowAppearance()
        {
            WindowStartupLocation = WindowStartupLocation.Manual;
            var x = Settings.Default.MainWindow_X;
            var y = Settings.Default.MainWindow_Y;
            var width = Settings.Default.MainWindow_Width;
            var height = Settings.Default.MainWindow_Height;
            var rec = new Rectangle(x, y, width, height);

            if (!IsVisibleOnAnyScreen(rec))
            {
                width = Screen.PrimaryScreen.Bounds.Width - 200;
                height = Screen.PrimaryScreen.Bounds.Height - 200;
            }

            var fs = Settings.Default.MainWindow_FullScreen;
            Width = width;
            Height = height;
            if (fs)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private bool IsVisibleOnAnyScreen(Rectangle rect)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(rect))
                {
                    return true;
                }
            }

            return false;
        }

        private void SaveWindowAppearance()
        {
            if (WindowState == WindowState.Maximized)
            {
                Settings.Default.MainWindow_FullScreen = true;
            }
            else
            {
                Settings.Default.MainWindow_Width = (int) Width;
                Settings.Default.MainWindow_Height = (int) Height;
                Settings.Default.MainWindow_FullScreen = false;
            }
            Settings.Default.Save();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            AddRecentlyOpenedProjectsToFileMenu();

            SetColorTheme((ColorTheme) settings.UserSettings["colorTheme"]);

            FileManualButton.IsEnabled = File.Exists(settings.FixedSettings.ManualFilePath);

            LicenseButton.IsEnabled = File.Exists(settings.FixedSettings.LicenseFilePath);
            FeedbackButton.IsEnabled = false;

            ButtonQuickAccessOpenProject.IsEnabled = ButtonMenuFileOpenProject.IsEnabled;
            ButtonQuickAccessSaveProject.IsEnabled = ButtonMenuFileSaveProject.IsEnabled;

            UpdateMainWindowRibbonElements();
            UpdateRibbonExtensions(gui);

            ValidateItems();
        }

        private void UpdateMainWindowRibbonElements()
        {
            resetUIButton.ToolTip = new ScreenTip
            {
                Title = Properties.Resources.MainWindow_UpdateMainWindowRibbonElements_Reset_layout_restart,
                Text = Properties.Resources.MainWindow_UpdateMainWindowRibbonElements_When_this_option_is_turned_on_the_default_layout_will_be_used_when_restarting_Ringtoets,
                MaxWidth = 250
            };
        }

        private void UpdateRibbonExtensions(IGuiPluginsHost guiPluginsHost)
        {
            // get all ribbon controls
            ribbonCommandHandlers = guiPluginsHost.Plugins.Where(p => p.RibbonCommandHandler != null).Select(p => p.RibbonCommandHandler).ToArray();

            foreach (var ribbonExtension in ribbonCommandHandlers)
            {
                var ribbonControl = ribbonExtension.GetRibbonControl();

                // fill contextual groups from plugins
                foreach (var group in ribbonControl.ContextualGroups)
                {
                    if (Ribbon.ContextualGroups.Any(g => g.Name == group.Name))
                    {
                        continue;
                    }

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

        private void OnFileOptionsClicked(object sender, RoutedEventArgs e)
        {
            using (var optionsDialog = new OptionsDialog(this, settings.UserSettings))
            {
                if (optionsDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetColorTheme((ColorTheme) settings.UserSettings["colorTheme"]);
                }
            }
        }

        private void SetColorTheme(ColorTheme colorTheme)
        {
            if (colorTheme == ColorTheme.Dark && !(DockingManager.Theme is ExpressionDarkTheme))
            {
                DockingManager.Theme = new ExpressionDarkTheme();
            }
            else if (colorTheme == ColorTheme.Light && !(DockingManager.Theme is ExpressionLightTheme))
            {
                DockingManager.Theme = new ExpressionLightTheme();
            }
            else if (colorTheme == ColorTheme.Metro && !(DockingManager.Theme is MetroTheme))
            {
                DockingManager.Theme = new MetroTheme();
            }
            else if (colorTheme == ColorTheme.Aero && !(DockingManager.Theme is AeroTheme))
            {
                DockingManager.Theme = new AeroTheme();
            }
            else if (colorTheme == ColorTheme.VS2010 && !(DockingManager.Theme is VS2010Theme))
            {
                DockingManager.Theme = new VS2010Theme();
            }
            else if (colorTheme == ColorTheme.Generic && !(DockingManager.Theme is GenericTheme))
            {
                DockingManager.Theme = new GenericTheme();
            }

            settings.UserSettings["colorTheme"] = colorTheme;
        }

        private void ButtonShowProperties_Click(object sender, RoutedEventArgs e)
        {
            var active = toolViewController.ToolWindowViews.Contains(PropertyGrid);

            if (active)
            {
                toolViewController.ToolWindowViews.Remove(PropertyGrid);
                ButtonShowProperties.IsChecked = false;
            }
            else
            {
                InitPropertiesWindowAndActivate();
                ButtonShowProperties.IsChecked = true;
            }
        }

        private void ButtonShowMessages_Click(object sender, RoutedEventArgs e)
        {
            var active = toolViewController.ToolWindowViews.Contains(MessageWindow);

            if (active)
            {
                toolViewController.ToolWindowViews.Remove(MessageWindow);
                ButtonShowMessages.IsChecked = false;
            }
            else
            {
                InitMessagesWindowOrActivate();
                ButtonShowMessages.IsChecked = true;
            }
        }

        private void OnFileHelpStartPage_Clicked(object sender, RoutedEventArgs e)
        {
            ShowStartPage(false);
        }

        private void OnFileHelpLicense_Clicked(object sender, RoutedEventArgs e)
        {
            var licensePageName = Properties.Resources.MainWindow_LicenseView_Name;

            foreach (var view in documentViewController.DocumentViews.OfType<RichTextView>())
            {
                if (view.Text == licensePageName)
                {
                    documentViewController.DocumentViews.ActiveView = view;

                    return;
                }
            }

            documentViewController.DocumentViewsResolver.OpenViewForData(new RichTextFile
            {
                Name = licensePageName,
                FilePath = settings.FixedSettings.LicenseFilePath
            });
        }

        private void OnFileHelpSubmitFeedback_Clicked(object sender, RoutedEventArgs e) {}

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

        private void OpenStartPage()
        {
            var welcomePageName = (string) settings.UserSettings["startPageName"];
            var welcomePageUrl = settings.FixedSettings.StartPageUrl;
            if (string.IsNullOrEmpty(welcomePageUrl))
            {
                welcomePageUrl = "about:blank";
            }

            var url = new WebLink(welcomePageName, new Uri(welcomePageUrl));

            commands.ViewCommands.OpenView(url);
        }

        private void CloseContent(LayoutContent c)
        {
            c.Close();
        }

        private void CloseDocumentTab(object sender, ExecutedRoutedEventArgs e)
        {
            documentViewController.DocumentViews.Remove(documentViewController.DocumentViews.ActiveView);
        }

        private void CanCloseDocumentTab(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = documentViewController.DocumentViews.Any();
        }

        private void ButtonResetUILayout_Click(object sender, RoutedEventArgs e)
        {
            resetDefaultLayout = resetUIButton.IsChecked ?? false;
        }

        private void OnAboutDialog_Clicked(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new SplashScreen.SplashScreen
            {
                HasProgress = false,
                VersionText = SettingsHelper.ApplicationVersion,
                CopyrightText = settings.FixedSettings.Copyright,
                LicenseText = settings.FixedSettings.LicenseDescription,
                CompanyText = SettingsHelper.ApplicationCompany,
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
    }
}