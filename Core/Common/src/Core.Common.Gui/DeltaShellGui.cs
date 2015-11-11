using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using Core.Common.Base;
using Core.Common.Base.Workflow;
using Core.Common.Controls;
using Core.Common.Controls.Swf;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Properties;
using Core.Common.Utils;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Reflection;
using Core.GIS.SharpMap.UI.Helpers;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using MainWindow = Core.Common.Gui.Forms.MainWindow.MainWindow;
using SplashScreen = Core.Common.Gui.Forms.SplashScreen.SplashScreen;

namespace Core.Common.Gui
{
    /// <summary>
    /// Gui class provides graphical user functionality for a given IApplication.
    /// </summary>
    public class DeltaShellGui : IGui, IDisposable
    {
        public event EventHandler<SelectedItemChangedEventArgs> SelectionChanged; // TODO: make it weak
        private static readonly ILog log = LogManager.GetLogger(typeof(DeltaShellGui));

        private static DeltaShellGui instance;
        private static string instanceCreationStackTrace;

        private IApplication application;
        private MainWindow mainWindow;

        private object selection;

        private ViewList documentViews;
        private ViewList toolWindowViews;
        private AvalonDockDockingManager toolWindowViewsDockingManager;

        private readonly IList<IGuiCommand> commands = new List<IGuiCommand>();

        private SplashScreen splashScreen;
        private ProgressDialog progressDialog;

        private bool settingSelection;
        private bool runFinished;
        private bool isExiting;

        public DeltaShellGui()
        {
            // error detection code, make sure we use only a single instance of DeltaShellGui at a time
            if (instance != null)
            {
                instance = null; // reset to that the consequent creations won't fail.
                throw new InvalidOperationException(Resources.DeltaShellGui_DeltaShellGui_Only_a_single_instance_of_DeltaShelGui_is_allowed_at_the_same_time_per_process__make_sure_that_the_previous_instance_was_disposed_correctly__stack_trace__ + instanceCreationStackTrace);
            }

            instance = this;
            instanceCreationStackTrace = new StackTrace().ToString();
            ViewPropertyEditor.Gui = this;

            Application = new DeltaShellApplication
            {
                IsProjectCreatedInTemporaryDirectory = true,
                WaitMethod = () => System.Windows.Forms.Application.DoEvents()
            };

            Plugins = new List<GuiPlugin>();

            application.UserSettings = Settings.Default;
            application.Settings = ConfigurationManager.AppSettings;

            application.Resources = new ResourceManager(typeof(Resources));

            CommandHandler = new GuiCommandHandler(this);

            System.Windows.Forms.Application.EnableVisualStyles();
        }

        public bool SkipDialogsOnExit { get; set; }

        public Action OnMainWindowLoaded { get; set; }

        public IApplication Application
        {
            get
            {
                return application;
            }
            set
            {
                if (application != null)
                {
                    Application.ProjectClosing -= ApplicationProjectClosing;
                    Application.ProjectOpened -= ApplicationProjectOpened;
                    Application.ProjectSaved -= ApplicationProjectSaved;
                    Application.ActivityRunner.IsRunningChanged -= ActivityRunnerIsRunningChanged;
                    Application.ActivityRunner.ActivityCompleted -= ActivityRunnerActivityCompleted;
                }

                application = value;

                if (application != null)
                {
                    // subscribe to application events so that we can handle opening, closing, renamig of views on project changes
                    Application.ProjectClosing += ApplicationProjectClosing;
                    Application.ProjectOpened += ApplicationProjectOpened;
                    Application.ProjectSaved += ApplicationProjectSaved;
                    Application.ActivityRunner.IsRunningChanged += ActivityRunnerIsRunningChanged;
                    Application.ActivityRunner.ActivityCompleted += ActivityRunnerActivityCompleted;
                }
            }
        }

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

                if (Selection is IProjectItem && !ReferenceEquals(Selection, SelectedProjectItem))
                {
                    SelectedProjectItem = (IProjectItem) Selection;
                }

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

        /// <summary>
        /// TODO: add body to setter and set Selection to a correct item. And vice-versa.
        /// </summary>
        public IProjectItem SelectedProjectItem { get; set; }

        public IList<IGuiCommand> Commands
        {
            get
            {
                return commands;
            }
        }

        public IGuiCommandHandler CommandHandler { get; set; }

        public IViewList DocumentViews
        {
            get
            {
                return documentViews;
            }
        }

        public IViewResolver DocumentViewsResolver { get; private set; }

        public IViewList ToolWindowViews
        {
            get
            {
                return toolWindowViews;
            }
        }

        public IMainWindow MainWindow
        {
            get
            {
                return mainWindow;
            }
            set
            {
                mainWindow = (MainWindow) value;
                mainWindow.Gui = this;
            }
        }

        public IList<GuiPlugin> Plugins { get; private set; }

        public bool IsViewRemoveOnItemDeleteSuspended { get; set; }

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

            if (!String.IsNullOrEmpty(projectPath))
            {
                application.Run(projectPath);
            }
            else
            {
                log.Info(Resources.DeltaShellGui_Run_Starting_application____);

                application.Run();
            }

            log.Info(Resources.DeltaShellGui_Run_Initializing_graphical_user_interface____);

            Initialize();

            log.InfoFormat(Resources.DeltaShellGui_Run_Started_in__0_f2__sec, (DateTime.Now - startTime).TotalSeconds);

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

            if (!SkipDialogsOnExit && !CommandHandler.TryCloseWTIProject())
            {
                // user cancelled exit:
                isExiting = false;
                return;
            }

            ViewList.DoNotDisposeViewsOnRemove = true; // persormance optimization

            CopyDefaultViewsToUserSettings();

            mainWindow.ClearDocumentTabs();

            mainWindow.SaveLayout(); // save before Application.Exit

            Application.Exit();

            // close faster (hide main window)
            mainWindow.Visible = false;

            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                isExiting = true;

                if (mainWindow != null)
                {
                    mainWindow.UnsubscribeFromGui();
                }

                MapCursors.DisposeCursors();

                TypeUtils.ClearCaches();

                SelectedProjectItem = null;
                Selection = null;

                try
                {
                    if (Application != null)
                    {
                        if (Application.ActivityRunner.IsRunning)
                        {
                            Application.ActivityRunner.CancelAll();
                            while (Application.ActivityRunner.IsRunning)
                            {
                                System.Windows.Forms.Application.DoEvents();
                            }
                        }
                        Application.CloseProject(); // lots of unsubscribe logic in plugins reacts on this
                    }
                }
                finally
                {
                    if (ToolWindowViews != null)
                    {
                        ToolWindowViews.Clear();
                    }

                    if (CommandHandler != null)
                    {
                        CommandHandler.Dispose();
                        CommandHandler = null;
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

                    if (progressDialog != null)
                    {
                        progressDialog.Dispose();
                        progressDialog = null;
                    }

                    splashScreen = null;

                    MessageWindowLogAppender.MessageWindow = null;

                    if (Application != null)
                    {
                        Application.Dispose();
                    }

                    RemoveLogging();

                    Application = null;
                }
            }

            System.Windows.Forms.Application.ApplicationExit -= HandleApplicationExit;

            // prevent nasty Windows.Forms memory leak (keeps references to databinding objects / controls
            var systemAssembly = typeof(Component).Assembly;
            var reflectTypeDescriptionProviderType =
                systemAssembly.GetType("System.ComponentModel.ReflectTypeDescriptionProvider");
            var propertyCacheInfo = reflectTypeDescriptionProviderType.GetField("_propertyCache",
                                                                                BindingFlags.Static |
                                                                                BindingFlags.NonPublic);
            var propertyCache = (Hashtable) propertyCacheInfo.GetValue(null);
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
            instance = null;
        }

        private void DeactivatePlugin(GuiPlugin plugin)
        {
            try
            {
                plugin.Deactivate();
            }
            catch (Exception exception)
            {
                log.Error(Resources.DeltaShellGui_ActivatePlugins_Exception_during_plugin_gui_deactivation, exception);
            }

            plugin.Dispose();

            Plugins.Remove(plugin);
        }

        private void ApplicationProjectSaved(Project obj)
        {
            ResumeUI();
        }

        private void ResumeUI()
        {
            if (mainWindow != null)
            {
                mainWindow.ValidateItems();
            }
        }

        [InvokeRequired]
        private void ActivityRunnerIsRunningChanged(object sender, EventArgs e)
        {
            if (isExiting)
            {
                return;
            }

            if (!Application.IsActivityRunning())
            {
                ResumeUI();
            }

            UpdateProgressDialog();
        }

        [InvokeRequired]
        private void ActivityRunnerActivityCompleted(object sender, ActivityEventArgs e)
        {
            if (MainWindow == null || MainWindow.PropertyGrid == null)
            {
                return;
            }

            // Force refresh of propertygrid (not done automaticly because events are disabled during import)
            MainWindow.PropertyGrid.Data = MainWindow.PropertyGrid.GetObjectProperties(Selection);
        }

        private void UpdateProgressDialog()
        {
            //popping a dialog on buildserver causes error
            //Showing a modal dialog box or form when the application is not running in UserInteractive mode is not a valid operation.
            //hence this check
            if (!Environment.UserInteractive)
            {
                return;
            }

            if (isExiting)
            {
                return;
            }

            if (progressDialog == null || progressDialog.IsDisposed)
            {
                progressDialog = new ProgressDialog();
                progressDialog.CancelClicked += delegate
                {
                    Application.ActivityRunner.CancelAll();

                    // wait until all import activities are finished
                    while (Application.IsActivityRunning())
                    {
                        System.Windows.Forms.Application.DoEvents();
                    }
                };
                progressDialog.Data = Application.ActivityRunner.Activities;
            }

            if (Application.ActivityRunner.IsRunning)
            {
                if (!progressDialog.Visible)
                {
                    mainWindow.Enabled = false;
                    progressDialog.Show(mainWindow);
                    progressDialog.CenterToParent();
                }
            }
            else
            {
                mainWindow.Enabled = true;
                progressDialog.Hide();
                progressDialog.Visible = false;
            }
        }

        private void ApplicationProjectOpened(Project project)
        {
            ResumeUI();
        }

        private void ApplicationProjectClosing(Project project)
        {
            ClonableToolStripMenuItem.ClearCache();
        }

        // Sets the tooltip for given view, assuming that ProjectExplorer is not null.
        private void SetToolTipForView(IView view)
        {
            if (mainWindow == null || mainWindow.ProjectExplorer == null)
            {
                return;
            }

            var node = mainWindow.ProjectExplorer.TreeView.GetNodeByTag(view.Data);
            if (node != null)
            {
                DocumentViews.SetTooltip(view, node.FullPath);
            }
        }

        private void ConfigureLogging()
        {
            // configure logging
            var rootLogger = ((Hierarchy) LogManager.GetRepository()).Root;

            if (!rootLogger.Appenders.Cast<IAppender>().Any(a => a is MessageWindowLogAppender))
            {
                rootLogger.AddAppender(new MessageWindowLogAppender());
                rootLogger.Repository.Configured = true;
            }
        }

        private void RemoveLogging()
        {
            var rootLogger = ((Hierarchy) LogManager.GetRepository()).Root;
            var messageWindowLogAppender = rootLogger.Appenders.Cast<IAppender>().OfType<MessageWindowLogAppender>().FirstOrDefault();
            if (messageWindowLogAppender != null)
            {
                rootLogger.RemoveAppender(messageWindowLogAppender);
            }
        }

        private void Initialize()
        {
            InitializeWindows();

            Plugins.ForEach(p => p.Gui = this);

            ActivatePlugins();

            InitializeMenusAndToolbars();

            System.Windows.Forms.Application.ApplicationExit += HandleApplicationExit;

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
            splashScreen = new SplashScreen()
            {
                VersionText = SettingsHelper.ApplicationVersion,
                CopyrightText = Application.Settings["copyright"],
                LicenseText = Application.Settings["license"],
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
                    log.Info(Resources.DeltaShellGui_ShowSplashScreen_User_has_cancelled_start__exiting____);
                    SkipDialogsOnExit = true;
                    Environment.Exit(1);
                }
            };

            object showSplashScreen = Application.UserSettings["showSplashScreen"];
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
            if (MainWindow == null)
            {
                MainWindow = new MainWindow(this);
            }

            mainWindow.Loaded += delegate
            {
                mainWindow.RestoreLayout();

                toolWindowViewsDockingManager.OnLayoutChange();

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

                if (ToolWindowViews.Contains(mainWindow.ProjectExplorer))
                {
                    ToolWindowViews.ActiveView = mainWindow.ProjectExplorer;
                }

                mainWindow.ValidateItems();

                mainWindow.ShowStartPage();

                if (OnMainWindowLoaded != null)
                {
                    OnMainWindowLoaded();
                }
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
            log.Info(Resources.DeltaShellGui_InitializeWindows_Initializing_windows____);

            InitializeMainWindow();

            log.Info(Resources.DeltaShellGui_InitializeWindows_Creating_default_tool_windows____);
            InitToolWindows();

            UpdateTitle();

            log.Info(Resources.DeltaShellGui_InitializeWindows_All_windows_are_created_);
        }

        private void UpdateTitle()
        {
            //NOTE: DUPLICATED BY GuiCommandHandler::UpdateGui --> todo, remove the duplication

            string mainWindowTitle = Application.Settings["mainWindowTitle"];

            string projectTitle = "<None>";
            if (Application.Project != null)
            {
                projectTitle = Application.Project.Name;
            }

            // TODO: this must be moved to MainWindow which should listen to project changes
            if (mainWindow != null)
            {
                mainWindow.Title = projectTitle + " - " + mainWindowTitle;
            }
        }

        private void ActiveViewChanging(object sender, ActiveViewChangeEventArgs e)
        {
            if (e.View == null || mainWindow == null || mainWindow.IsWindowDisposed)
            {
                return;
            }

            if (mainWindow.ProjectExplorer != null)
            {
                SetToolTipForView(e.View);
            }
        }

        // TODO: incapsulate any knowledge of the plugin XML inside plugin configurator, the rest of the system should not know about it!
        private void InitToolWindows()
        {
            log.Info(Resources.DeltaShellGui_InitToolWindows_Creating_document_window_manager____);

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
                Gui = this
            };

            documentViewManager.EnableTabContextMenus();

            documentViewManager.ActiveViewChanging += ActiveViewChanging;
            documentViewManager.ActiveViewChanged += OnActiveViewChanged;
            documentViewManager.CollectionChanged += DocumentViewsCollectionChanged;
            documentViewManager.ChildViewChanged += DocumentViewManagerOnChildViewChanged;

            documentViews = documentViewManager;

            DocumentViewsResolver = new ViewResolver(documentViews, Plugins.SelectMany(p => p.GetViewInfoObjects()));

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
                Gui = this
            };

            toolWindowViews.CollectionChanged += ToolWindowViewsOnCollectionChanged;

            log.Info(Resources.DeltaShellGui_InitToolWindows_Creating_tool_window_manager____);

            mainWindow.InitializeToolWindows();

            log.Debug(Resources.DeltaShellGui_InitToolWindows_Finished_InitToolWindows);

            mainWindow.SubscribeToGui();
        }

        private void OnActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            if (e.View == null || mainWindow == null || mainWindow.IsWindowDisposed)
            {
                return;
            }

            mainWindow.ValidateItems();
        }

        private void DocumentViewManagerOnChildViewChanged(object sender, NotifyCollectionChangingEventArgs notifyCollectionChangingEventArgs)
        {
            if (isExiting)
            {
                return;
            }

            mainWindow.ValidateItems();
        }

        private static string GetViewName(IView view)
        {
            var name = view.ViewInfo != null ? view.ViewInfo.GetViewName(view, view.Data) : null;
            if (name != null)
            {
                return name;
            }

            var enumerable = view.Data as IEnumerable;
            var data = enumerable == null ? view.Data : enumerable.Cast<object>().FirstOrDefault();
            
            return view.Data == null ? "" : view.Data.ToString();
        }

        private void UpdateViewName(IView view)
        {
            view.Text = GetViewName(view);
            SetToolTipForView(view);
        }

        private void ToolWindowViewsOnCollectionChanged(object sender, NotifyCollectionChangingEventArgs notifyCollectionChangingEventArgs)
        {
            if (isExiting)
            {
                return;
            }
            mainWindow.ValidateItems();
        }

        private void DocumentViewsCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
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
            log.Info(Resources.DeltaShellGui_InitializeMenusAndToolbars_Setting_up_menus_and_toolbars____);
            mainWindow.SuspendLayout();

            // Validate once when loading is completed
            mainWindow.ValidateItems();

            mainWindow.ResumeLayout();
            log.Info(Resources.DeltaShellGui_InitializeMenusAndToolbars_Menus_and_toolbars_are_ready_);
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
            if (Application.UserSettings["defaultViews"] != null)
            {
                defaultViews = (StringCollection) Application.UserSettings["defaultViews"];
                defaultViewDataTypes = (StringCollection) Application.UserSettings["defaultViewDataTypes"];
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

            Application.UserSettings["defaultViews"] = defaultViews;
            Application.UserSettings["defaultViewDataTypes"] = defaultViewDataTypes;
        }

        ~DeltaShellGui()
        {
            Dispose(false);
        }
    }
}