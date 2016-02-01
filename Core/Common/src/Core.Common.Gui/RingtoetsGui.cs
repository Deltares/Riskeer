// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Events;
using Core.Common.Utils.Extensions;
using Core.Common.Utils.Reflection;

using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

using SplashScreen = Core.Common.Gui.Forms.SplashScreen.SplashScreen;
using Application = System.Windows.Application;
using WindowsApplication = System.Windows.Forms.Application;

namespace Core.Common.Gui
{
    /// <summary>
    /// Gui class provides graphical user functionality for a given IApplication.
    /// </summary>
    public class RingtoetsGui : IGui
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RingtoetsGui));

        private static bool isAlreadyRunningInstanceOfIGui;
        private static string instanceCreationStackTrace;

        private AvalonDockDockingManager toolWindowViewsDockingManager;

        private SplashScreen splashScreen;

        private bool runFinished;
        private bool isExiting;

        public RingtoetsGui(IMainWindow mainWindow, IStoreProject projectStore, ApplicationCore applicationCore = null, GuiCoreSettings fixedSettings = null)
        {
            // error detection code, make sure we use only a single instance of RingtoetsGui at a time
            if (isAlreadyRunningInstanceOfIGui)
            {
                isAlreadyRunningInstanceOfIGui = false; // reset to that the consequent creations won't fail.
                throw new InvalidOperationException(Resources.RingtoetsGui_Only_a_single_instance_of_Ringtoets_is_allowed_at_the_same_time_per_process_Make_sure_that_the_previous_instance_was_disposed_correctly_stack_trace + instanceCreationStackTrace);
            }

            MainWindow = mainWindow;
            Storage = projectStore;
            ApplicationCore = applicationCore ?? new ApplicationCore();
            FixedSettings = fixedSettings ?? new GuiCoreSettings();

            isAlreadyRunningInstanceOfIGui = true;
            instanceCreationStackTrace = new StackTrace().ToString();

            Plugins = new List<GuiPlugin>();

            UserSettings = Settings.Default;

            viewCommandHandler = new ViewCommandHandler(this, this, this, this);
            storageCommandHandler = new StorageCommandHandler(projectStore, this, this, this, this, viewCommandHandler);
            exportImportCommandHandler = new ExportImportCommandHandler(MainWindow, ApplicationCore, this);
            projectCommandsHandler = new ProjectCommandsHandler(this, MainWindow, ApplicationCore, this, this);

            WindowsApplication.EnableVisualStyles();
            ViewPropertyEditor.ViewCommands = ViewCommands;

            ProjectOpened += ApplicationProjectOpened;
        }

        public ApplicationCore ApplicationCore { get; private set; }

        public IPropertyResolver PropertyResolver { get; private set; }

        public IStoreProject Storage { get; private set; }

        private IProjectExplorer ProjectExplorer
        {
            get
            {
                return ToolWindowViews.OfType<IProjectExplorer>().FirstOrDefault();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Run()
        {
            Run("");
        }

        public void Run(string projectPath)
        {
            var startTime = DateTime.Now;

            ConfigureLogging();

            ShowSplashScreen();

            log.Info(Resources.RingtoetsGui_Run_Creating_new_project);

            if (!string.IsNullOrEmpty(projectPath))
            {
                // TODO: Implement logic for opening the project from the provided file path
                Project = new Project();
            }
            else
            {
                log.Info(Resources.RingtoetsGui_Run_Starting_application);

                Project = new Project();
            }

            log.Info(Resources.RingtoetsGui_Run_Initializing_graphical_user_interface);

            Initialize();

            log.InfoFormat(Resources.RingtoetsGui_Run_Started_in_0_f2_sec, (DateTime.Now - startTime).TotalSeconds);

            runFinished = true;

            HideSplashScreen();

            MessageWindowLogAppender.Enabled = true;
        }

        public void Exit()
        {
            if (isExiting)
            {
                return; //already got here before
            }

            isExiting = true;

            CopyDefaultViewsToUserSettings();

            mainWindow.ClearDocumentTabs();

            mainWindow.SaveLayout(); // save before ApplicationCore.Exit

            if (userSettingsDirty)
            {
                UserSettings.Save();
            }

            UserSettings = null;

            // close faster (hide main window)
            mainWindow.Visible = false;

            if (Application.Current != null)
            {
                Application.Current.Shutdown();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                isExiting = true;

                if (mainWindow != null)
                {
                    mainWindow.UnsubscribeFromGui();
                }

                Selection = null;

                Project = null;

                if (ToolWindowViews != null)
                {
                    ToolWindowViews.Clear();
                }

                if (storageCommandHandler != null)
                {
                    storageCommandHandler.Dispose();
                    storageCommandHandler = null;
                }

                if (Plugins != null)
                {
                    foreach (var plugin in Plugins.ToList())
                    {
                        DeactivatePlugin(plugin);
                    }

                    Plugins = null;
                }

                if (toolWindowViews != null)
                {
                    toolWindowViews.Clear();
                    toolWindowViews.Dispose();
                    toolWindowViews = null;
                }

                if (documentViews != null)
                {
                    documentViews.Clear();
                    documentViews.Dispose();
                    documentViews = null;
                }

                if (toolWindowViewsDockingManager != null)
                {
                    toolWindowViewsDockingManager.Dispose();
                    toolWindowViewsDockingManager = null;
                }

                // Dispose managed resources. TODO: double check if we need to dispose managed resources?
                if (mainWindow != null && !mainWindow.IsWindowDisposed)
                {
                    mainWindow.Dispose();
                    mainWindow = null;
                }

                DocumentViewsResolver = null;

                splashScreen = null;

                MessageWindowLogAppender.MessageWindow = null;

                if (ApplicationCore != null)
                {
                    ApplicationCore.Dispose();
                }

                RemoveLogging();
            }

            WindowsApplication.ApplicationExit -= HandleApplicationExit;

            // prevent nasty Windows.Forms memory leak (keeps references to databinding objects / controls
            var systemAssembly = typeof(Component).Assembly;
            var reflectTypeDescriptionProviderType =
                systemAssembly.GetType("System.ComponentModel.ReflectTypeDescriptionProvider");
            var propertyCacheInfo = reflectTypeDescriptionProviderType.GetField("_propertyCache",
                                                                                BindingFlags.Static |
                                                                                BindingFlags.NonPublic);
            var propertyCache = (Hashtable)propertyCacheInfo.GetValue(null);
            if (propertyCache != null)
            {
                propertyCache.Clear();
            }

            var extendedPropertyCacheInfo = reflectTypeDescriptionProviderType.GetField(
                "_extendedPropertyCache", BindingFlags.Static | BindingFlags.NonPublic);
            var extendedPropertyCache = extendedPropertyCacheInfo.GetValue(null) as Hashtable;
            if (extendedPropertyCache != null)
            {
                extendedPropertyCache.Clear();
            }

            var eventCacheInfo = reflectTypeDescriptionProviderType.GetField("_eventCache",
                                                                             BindingFlags.Static |
                                                                             BindingFlags.NonPublic);
            var eventCache = eventCacheInfo.GetValue(null) as Hashtable;
            if (eventCache != null)
            {
                eventCache.Clear();
            }

            var attributeCacheInfo = reflectTypeDescriptionProviderType.GetField("_attributeCache",
                                                                                 BindingFlags.Static |
                                                                                 BindingFlags.NonPublic);
            var attributeCache = attributeCacheInfo.GetValue(null) as Hashtable;
            if (attributeCache != null)
            {
                attributeCache.Clear();
            }

            var typeDescriptorType = systemAssembly.GetType("System.ComponentModel.TypeDescriptor");
            var providerTableInfo = typeDescriptorType.GetField("_providerTable",
                                                                BindingFlags.Static | BindingFlags.NonPublic);
            var providerTable = providerTableInfo.GetValue(null) as Hashtable;
            if (providerTable != null)
            {
                providerTable.Clear();
            }

            var providerTypeTableInfo = typeDescriptorType.GetField("_providerTypeTable",
                                                                    BindingFlags.Static | BindingFlags.NonPublic);
            var providerTypeTable = providerTypeTableInfo.GetValue(null) as Hashtable;
            if (providerTypeTable != null)
            {
                providerTypeTable.Clear();
            }

            var defaultProvidersInfo = typeDescriptorType.GetField("_defaultProviders",
                                                                   BindingFlags.Static | BindingFlags.NonPublic);
            var defaultProviders = defaultProvidersInfo.GetValue(null) as Hashtable;
            if (defaultProviders != null)
            {
                defaultProviders.Clear();
            }

            GC.Collect();

            instanceCreationStackTrace = "";
            isAlreadyRunningInstanceOfIGui = false;
        }

        private void DeactivatePlugin(GuiPlugin plugin)
        {
            try
            {
                plugin.Deactivate();
            }
            catch (Exception exception)
            {
                log.Error(Resources.RingtoetsGui_ActivatePlugins_Exception_during_plugin_gui_deactivation, exception);
            }

            plugin.Dispose();

            Plugins.Remove(plugin);
        }

        private void ResumeUI()
        {
            if (mainWindow != null)
            {
                mainWindow.ValidateItems();
            }
        }

        private void ApplicationProjectOpened(Project project)
        {
            ResumeUI();
        }

        // Sets the tooltip for given view, assuming that ProjectExplorer is not null.
        private void SetToolTipForView(IView view)
        {
            if (mainWindow == null || ProjectExplorer == null)
            {
                return;
            }

            var node = ProjectExplorer.TreeView.TreeViewController.GetNodeByTag(view.Data);
            if (node != null)
            {
                DocumentViews.SetTooltip(view, node.FullPath);
            }
        }

        private void ConfigureLogging()
        {
            // configure logging
            var rootLogger = ((Hierarchy)LogManager.GetRepository()).Root;

            if (!rootLogger.Appenders.Cast<IAppender>().Any(a => a is MessageWindowLogAppender))
            {
                rootLogger.AddAppender(new MessageWindowLogAppender());
                rootLogger.Repository.Configured = true;
            }
        }

        private void RemoveLogging()
        {
            var rootLogger = ((Hierarchy)LogManager.GetRepository()).Root;
            var messageWindowLogAppender = rootLogger.Appenders.Cast<IAppender>().OfType<MessageWindowLogAppender>().FirstOrDefault();
            if (messageWindowLogAppender != null)
            {
                rootLogger.RemoveAppender(messageWindowLogAppender);
            }
        }

        private void Initialize()
        {
            InitializeWindows();

            Plugins.ForEachElementDo(p => p.Gui = this);

            ActivatePlugins();

            InitializeMenusAndToolbars();

            WindowsApplication.ApplicationExit += HandleApplicationExit;

            CopyDefaultViewsFromUserSettings();

            //enable activation AFTER initialization
            documentViews.IgnoreActivation = false;
            toolWindowViews.IgnoreActivation = false;

            if (Settings.Default.SettingsKey.Contains("mruList"))
            {
                if (Settings.Default["mruList"] == null)
                {
                    Settings.Default["mruList"] = new StringCollection();
                }
            }
        }

        private void HandleApplicationExit(object sender, EventArgs e)
        {
            Exit();
        }

        private void ShowSplashScreen()
        {
            splashScreen = new SplashScreen
            {
                VersionText = SettingsHelper.ApplicationVersion,
                CopyrightText = FixedSettings.Copyright,
                LicenseText = FixedSettings.LicenseDescription,
                CompanyText = SettingsHelper.ApplicationCompany
            };

            splashScreen.IsVisibleChanged += delegate
            {
                if (splashScreen.IsVisible)
                {
                    return;
                }

                if (!runFinished) // splash screen was closed before gui started.
                {
                    log.Info(Resources.RingtoetsGui_ShowSplashScreen_User_has_cancelled_start_Exiting);
                    Environment.Exit(1);
                }
            };

            object showSplashScreen = UserSettings["showSplashScreen"];
            if (showSplashScreen != null && bool.Parse(showSplashScreen.ToString()))
            {
                splashScreen.Show();
            }
        }

        private void HideSplashScreen()
        {
            splashScreen.Shutdown();
        }

        private void InitializeMainWindow()
        {
            mainWindow.Loaded += delegate
            {
                mainWindow.LoadLayout();

                toolWindowViewsDockingManager.UpdateLayout();

                // bug in Fluent ribbon (views removed during load layout are not cleared - no events), synchronize them manually
                toolWindowViews.SynchronizeViews(toolWindowViewsDockingManager.Views.ToArray());

                // make sure these windows come on top
                if (ToolWindowViews.Contains(mainWindow.PropertyGrid))
                {
                    ToolWindowViews.ActiveView = mainWindow.PropertyGrid;
                }

                if (ToolWindowViews.Contains(mainWindow.MessageWindow))
                {
                    ToolWindowViews.ActiveView = mainWindow.MessageWindow;
                }

                if (ToolWindowViews.Contains(ProjectExplorer))
                {
                    ToolWindowViews.ActiveView = ProjectExplorer;
                }

                mainWindow.ValidateItems();

                mainWindow.ShowStartPage();
            };

            mainWindow.Closing += delegate(object sender, CancelEventArgs e)
            {
                if (isExiting)
                {
                    return;
                }

                e.Cancel = true; //cancel closing: let Exit handle it
                Exit();
            };
        }

        private void InitializeWindows()
        {
            log.Info(Resources.RingtoetsGui_InitializeWindows_Initializing_windows);

            InitializeMainWindow();

            log.Info(Resources.RingtoetsGui_InitializeWindows_Creating_default_tool_windows);
            InitToolWindows();

            UpdateTitle();

            log.Info(Resources.RingtoetsGui_InitializeWindows_All_windows_are_created);
        }

        private void ActiveViewChanging(object sender, ActiveViewChangeEventArgs e)
        {
            if (e.View == null || mainWindow == null || mainWindow.IsWindowDisposed)
            {
                return;
            }

            if (ProjectExplorer != null)
            {
                SetToolTipForView(e.View);
            }
        }

        // TODO: incapsulate any knowledge of the plugin XML inside plugin configurator, the rest of the system should not know about it!
        private void InitToolWindows()
        {
            log.Info(Resources.RingtoetsGui_InitToolWindows_Creating_document_window_manager);

            var allowedDocumentWindowLocations = new[]
            {
                ViewLocation.Document,
                ViewLocation.Floating
            };

            var documentDockingManager = new AvalonDockDockingManager(mainWindow.DockingManager, allowedDocumentWindowLocations);

            var documentViewManager = new ViewList(documentDockingManager, ViewLocation.Document)
            {
                IgnoreActivation = true,
                UpdateViewNameAction = v => UpdateViewName(v),
            };

            documentViewManager.EnableTabContextMenus();

            documentViewManager.ActiveViewChanging += ActiveViewChanging;
            documentViewManager.ActiveViewChanged += OnActiveViewChanged;
            documentViewManager.CollectionChanged += DocumentViewsCollectionChanged;

            documentViews = documentViewManager;

            DocumentViewsResolver = new ViewResolver(documentViews, Plugins.SelectMany(p => p.GetViewInfoObjects()), mainWindow);
            PropertyResolver = new PropertyResolver(Plugins.SelectMany(p => p.GetPropertyInfos()));
            appFeatureApplicationCommands = new ApplicationFeatureCommandHandler(PropertyResolver, MainWindow, this);

            var allowedToolWindowLocations = new[]
            {
                ViewLocation.Left,
                ViewLocation.Right,
                ViewLocation.Top,
                ViewLocation.Bottom,
                ViewLocation.Floating
            };

            toolWindowViewsDockingManager = new AvalonDockDockingManager(mainWindow.DockingManager, allowedToolWindowLocations);

            toolWindowViews = new ViewList(toolWindowViewsDockingManager, ViewLocation.Left)
            {
                IgnoreActivation = true,
            };

            toolWindowViews.CollectionChanged += ToolWindowViewsOnCollectionChanged;

            log.Info(Resources.RingtoetsGui_InitToolWindows_Creating_tool_window_manager);

            mainWindow.InitializeToolWindows();

            log.Debug(Resources.RingtoetsGui_InitToolWindows_Finished_InitToolWindows);

            mainWindow.SubscribeToGui();
        }

        private void OnActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            if (mainWindow == null || mainWindow.IsWindowDisposed)
            {
                return;
            }

            mainWindow.ValidateItems();
            FireActiveViewChanged(e);
        }

        private void FireActiveViewChanged(ActiveViewChangeEventArgs e)
        {
            if (ActiveViewChanged != null)
            {
                ActiveViewChanged(null, e);
            }
        }

        private void UpdateViewName(IView view)
        {
            view.Text = DocumentViewsResolver.GetViewName(view);
            SetToolTipForView(view);
        }

        private void ToolWindowViewsOnCollectionChanged(object sender, NotifyCollectionChangeEventArgs notifyCollectionChangeEventArgs)
        {
            if (isExiting)
            {
                return;
            }
            mainWindow.ValidateItems();
        }

        private void DocumentViewsCollectionChanged(object sender, NotifyCollectionChangeEventArgs e)
        {
            if (isExiting || documentViews.Count != 0)
            {
                return;
            }

            // if no new active view is set update toolbars
            mainWindow.ValidateItems();
        }

        private void InitializeMenusAndToolbars()
        {
            log.Info(Resources.RingtoetsGui_InitializeMenusAndToolbars_Setting_up_menus_and_toolbars);
            mainWindow.SuspendLayout();

            // Validate once when loading is completed
            mainWindow.ValidateItems();

            mainWindow.ResumeLayout();
            log.Info(Resources.RingtoetsGui_InitializeMenusAndToolbars_Menus_and_toolbars_are_ready);
        }

        private void ActivatePlugins()
        {
            var problematicPlugins = new List<GuiPlugin>();

            mainWindow.SuspendLayout();

            // Try to activate all plugins
            foreach (var plugin in Plugins)
            {
                try
                {
                    plugin.Activate();
                }
                catch (Exception)
                {
                    problematicPlugins.Add(plugin);
                }
            }

            // Deactivate and remove all problematic plugins
            foreach (var problematicPlugin in problematicPlugins)
            {
                DeactivatePlugin(problematicPlugin);
            }

            mainWindow.ResumeLayout();
        }

        private void CopyDefaultViewsFromUserSettings()
        {
            StringCollection defaultViews;
            StringCollection defaultViewDataTypes;
            if (UserSettings["defaultViews"] != null)
            {
                defaultViews = (StringCollection)UserSettings["defaultViews"];
                defaultViewDataTypes = (StringCollection)UserSettings["defaultViewDataTypes"];
            }
            else
            {
                return;
            }

            for (int i = 0; i < defaultViews.Count; i++)
            {
                string skey = defaultViewDataTypes[i];
                string sview = defaultViews[i];
                if (AssemblyUtils.GetTypeByName(skey) != null)
                {
                    DocumentViewsResolver.DefaultViewTypes.Add(AssemblyUtils.GetTypeByName(skey), AssemblyUtils.GetTypeByName(sview));
                }
            }
        }

        private void CopyDefaultViewsToUserSettings()
        {
            StringCollection defaultViews = new StringCollection();
            StringCollection defaultViewDataTypes = new StringCollection();

            foreach (Type objectType in DocumentViewsResolver.DefaultViewTypes.Keys)
            {
                if (DocumentViewsResolver.DefaultViewTypes[objectType] == null)
                {
                    continue;
                }
                defaultViews.Add(DocumentViewsResolver.DefaultViewTypes[objectType].ToString());
                defaultViewDataTypes.Add(objectType.ToString());
            }

            UserSettings["defaultViews"] = defaultViews;
            UserSettings["defaultViewDataTypes"] = defaultViewDataTypes;
        }

        ~RingtoetsGui()
        {
            Dispose(false);
        }

        #region Implementation: IContextMenuBuilderProvider

        public IContextMenuBuilder Get(TreeNode treeNode, TreeNodeInfo treeNodeInfo)
        {
            return new ContextMenuBuilder(appFeatureApplicationCommands, exportImportCommandHandler, ViewCommands, treeNode, treeNodeInfo);
        }

        #endregion

        #region Implementation: IProjectOwner

        private Project project;

        public event Action<Project> ProjectOpened;
        public event Action<Project> ProjectClosing;

        public string ProjectFilePath { get; set; }

        public Project Project
        {
            get
            {
                return project;
            }
            set
            {
                if (project != null)
                {
                    if (ProjectClosing != null)
                    {
                        ProjectClosing(project);
                    }
                }

                project = value;

                if (project != null)
                {
                    if (ProjectOpened != null)
                    {
                        ProjectOpened(project);
                    }
                }
            }
        }

        #endregion

        #region Implementation: IApplicationSelection

        private object selection;

        private bool settingSelection;

        public event EventHandler<SelectedItemChangedEventArgs> SelectionChanged; // TODO: make it weak

        public object Selection
        {
            get
            {
                return selection;
            }
            set
            {
                if (selection == value || settingSelection)
                {
                    return;
                }

                if (null != DocumentViews)
                {
                    DocumentViews.IgnoreActivation = true;
                }

                selection = value;

                settingSelection = true;

                try
                {
                    if (SelectionChanged != null)
                    {
                        SelectionChanged(selection, new SelectedItemChangedEventArgs(value));
                    }
                }
                finally
                {
                    settingSelection = false;
                }

                if (!isExiting && mainWindow != null && !mainWindow.IsWindowDisposed)
                {
                    mainWindow.ValidateItems();
                }

                if (null != DocumentViews)
                {
                    DocumentViews.IgnoreActivation = false;
                }
            }
        }

        #endregion

        #region Implementation: ICommandsOwner

        private ApplicationFeatureCommandHandler appFeatureApplicationCommands;
        private readonly ViewCommandHandler viewCommandHandler;
        private readonly ProjectCommandsHandler projectCommandsHandler;
        private readonly ExportImportCommandHandler exportImportCommandHandler;
        private StorageCommandHandler storageCommandHandler;

        public IApplicationFeatureCommands ApplicationCommands
        {
            get
            {
                return appFeatureApplicationCommands;
            }
        }

        public IStorageCommands StorageCommands
        {
            get
            {
                return storageCommandHandler;
            }
        }

        public IProjectCommands ProjectCommands
        {
            get
            {
                return projectCommandsHandler;
            }
        }

        public IViewCommands ViewCommands
        {
            get
            {
                return viewCommandHandler;
            }
        }

        #endregion

        #region Implementation: IDocumentViewController

        private ViewList documentViews;

        public event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanged;

        public bool IsViewRemoveOnItemDeleteSuspended { get; set; }

        public IView ActiveView
        {
            get
            {
                return DocumentViews.ActiveView;
            }
        }

        public IViewList DocumentViews
        {
            get
            {
                return documentViews;
            }
        }

        public IViewResolver DocumentViewsResolver { get; private set; }

        public void UpdateToolTips()
        {
            foreach (var view in DocumentViews.AllViews)
            {
                SetToolTipForView(view);
            }
        }

        #endregion

        #region Implementation: ISettingsOwner

        private bool userSettingsDirty;
        private ApplicationSettingsBase userSettings;

        public GuiCoreSettings FixedSettings { get; private set; }

        public ApplicationSettingsBase UserSettings
        {
            get
            {
                return userSettings;
            }
            private set
            {
                if (userSettings != null)
                {
                    userSettings.PropertyChanged -= UserSettingsPropertyChanged;
                }

                userSettings = value;

                if (userSettings != null)
                {
                    userSettings.PropertyChanged += UserSettingsPropertyChanged;
                }

                userSettingsDirty = false;
            }
        }

        private void UserSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            userSettingsDirty = true;
        }

        #endregion

        #region Implementation: IGuiPluginHost

        public IList<GuiPlugin> Plugins { get; private set; }

        public GuiPlugin GetPluginGuiForType(Type type)
        {
            foreach (var plugin in Plugins)
            {
                Type[] pluginTypes = plugin.GetType().Assembly.GetTypes();
                foreach (Type pluginType in pluginTypes)
                {
                    if (pluginType == type)
                    {
                        return plugin;
                    }
                }
            }

            return null;
        }

        public IEnumerable GetAllDataWithViewDefinitionsRecursively(object rootDataObject)
        {
            var resultSet = new HashSet<object>();
            foreach (var childDataInstance in Plugins.SelectMany(p => p.GetChildDataWithViewDefinitions(rootDataObject)).Distinct())
            {
                resultSet.Add(childDataInstance);

                if (!ReferenceEquals(rootDataObject, childDataInstance))
                {
                    foreach (var dataWithViewDefined in GetAllDataWithViewDefinitionsRecursively(childDataInstance))
                    {
                        resultSet.Add(dataWithViewDefined);
                    }
                }
            }
            return resultSet;
        }

        #endregion

        #region Implementation: IMainWindowController

        private MainWindow mainWindow;

        public IMainWindow MainWindow
        {
            get
            {
                return mainWindow;
            }
            private set
            {
                mainWindow = (MainWindow)value;
                mainWindow.Gui = this;
            }
        }

        public void RefreshGui()
        {
            // Set the gui selection to the current project
            Selection = Project;

            // Update the window title
            UpdateTitle();
        }

        public void UpdateTitle()
        {
            if (mainWindow != null)
            {
                mainWindow.Title = string.Format("{0} - {1} {2}",
                                                 Project != null ? Project.Name : Resources.RingtoetsGui_UpdateTitle_Unknown,
                                                 FixedSettings.MainWindowTitle,
                                                 SettingsHelper.ApplicationVersion);
            }
        }

        #endregion

        #region Implementation: IToolViewController

        private ViewList toolWindowViews;

        public IViewList ToolWindowViews
        {
            get
            {
                return toolWindowViews;
            }
        }

        public void CloseToolView(IView toolView)
        {
            ToolWindowViews.Remove(toolView);
        }

        public void OpenToolView(IView toolView)
        {
            ToolWindowViews.Add(toolView);
            ToolWindowViews.ActiveView = toolView;
        }

        #endregion
    }
}