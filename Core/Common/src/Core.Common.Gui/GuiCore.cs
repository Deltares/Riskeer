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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Common.Gui.Selection;
using Core.Common.Gui.Settings;
using Core.Common.Utils.Extensions;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using SplashScreen = Core.Common.Gui.Forms.SplashScreen.SplashScreen;
using WindowsApplication = System.Windows.Forms.Application;

namespace Core.Common.Gui
{
    /// <summary>
    /// Gui class provides graphical user functionality for the application.
    /// </summary>
    public class GuiCore : IGui
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiCore));

        private static bool isAlreadyRunningInstanceOfIGui;
        private static string instanceCreationStackTrace;

        private readonly IProjectFactory projectFactory;

        private SplashScreen splashScreen;

        private bool runFinished;
        private bool isExiting;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiCore"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="projectStore">The project store.</param>
        /// <param name="projectFactory">The project factory.</param>
        /// <param name="fixedSettings">The fixed settings.</param>
        /// <exception cref="System.InvalidOperationException">Thrown when another <see cref="GuiCore"/>
        /// instance is running.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GuiCore(IMainWindow mainWindow, IStoreProject projectStore, IProjectFactory projectFactory, GuiCoreSettings fixedSettings)
        {
            // error detection code, make sure we use only a single instance of GuiCore at a time
            if (isAlreadyRunningInstanceOfIGui)
            {
                isAlreadyRunningInstanceOfIGui = false; // reset to that the consecutive creations won't fail.
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                                  Resources.GuiCore_Only_a_single_instance_of_Ringtoets_is_allowed_at_the_same_time_per_process_Make_sure_that_the_previous_instance_was_disposed_correctly_stack_trace_0,
                                  instanceCreationStackTrace));
            }

            if (mainWindow == null)
            {
                throw new ArgumentNullException("mainWindow");
            }
            if (projectStore == null)
            {
                throw new ArgumentNullException("projectStore");
            }
            if (projectFactory == null)
            {
                throw new ArgumentNullException("projectFactory");
            }
            if (fixedSettings == null)
            {
                throw new ArgumentNullException("fixedSettings");
            }
            this.projectFactory = projectFactory;

            MainWindow = mainWindow;
            FixedSettings = fixedSettings;

            isAlreadyRunningInstanceOfIGui = true;
            instanceCreationStackTrace = new StackTrace().ToString();

            Plugins = new List<PluginBase>();

            viewCommandHandler = new ViewCommandHandler(this, this, this);
            storageCommandHandler = new StorageCommandHandler(projectStore, this, MainWindow);
            importCommandHandler = new GuiImportHandler(MainWindow, Plugins.SelectMany(p => p.GetImportInfos()));
            exportCommandHandler = new GuiExportHandler(MainWindow, Plugins.SelectMany(p => p.GetExportInfos()));

            WindowsApplication.EnableVisualStyles();
            ViewPropertyEditor.ViewCommands = ViewCommands;

            ProjectOpened += ApplicationProjectOpened;
            projectObserver = new Observer(UpdateTitle);
        }

        public IPropertyResolver PropertyResolver { get; private set; }

        #region Implementation: ISettingsOwner

        public GuiCoreSettings FixedSettings { get; private set; }

        #endregion

        /// <summary>
        /// Runs the user interface, causing all user interface components to initialize, 
        /// loading plugins, opening a saved project and displaying the main window.
        /// </summary>
        /// <param name="projectPath">Path to the project to be opened. (optional)</param>
        public void Run(string projectPath = null)
        {
            var startTime = DateTime.Now;

            ConfigureLogging();

            ShowSplashScreen();

            InitializeProjectFromPath(projectPath);

            Initialize();

            log.InfoFormat(Resources.GuiCore_Run_Started_in_0_f2_sec, (DateTime.Now - startTime).TotalSeconds);

            runFinished = true;

            HideSplashScreen();

            MessageWindowLogAppender.Instance.Enabled = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void ExitApplication()
        {
            if (isExiting)
            {
                return; //already got here before
            }

            // Store project?
            if (!storageCommandHandler.ContinueIfHasChanges())
            {
                // User pressed cancel
                return;
            }

            isExiting = true;

            if (Application.Current != null)
            {
                Application.Current.Shutdown();
            }
        }

        #region Implementation: IContextMenuBuilderProvider

        public IContextMenuBuilder Get(object dataObject, TreeViewControl treeViewControl)
        {
            if (applicationFeatureCommands == null)
            {
                throw new InvalidOperationException("Call IGui.Run in order to initialize dependencies before getting the ContextMenuBuilder.");
            }

            return new ContextMenuBuilder(applicationFeatureCommands,
                                          importCommandHandler,
                                          exportCommandHandler,
                                          ViewCommands,
                                          dataObject,
                                          treeViewControl);
        }

        #endregion

        private IProjectExplorer ProjectExplorer
        {
            get
            {
                return ViewHost.ToolViews.OfType<IProjectExplorer>().FirstOrDefault();
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                projectObserver.Dispose();

                if (Plugins != null)
                {
                    foreach (var plugin in Plugins.ToArray())
                    {
                        DeactivatePlugin(plugin);
                    }
                }

                isExiting = true;

                if (mainWindow != null)
                {
                    mainWindow.UnsubscribeFromGui();
                }

                Selection = null;
                Project = null;

                if (ViewHost != null)
                {
                    ViewHost.Dispose();
                    ViewHost.ViewClosed -= OnActiveDocumentViewChanged;
                    ViewHost.ActiveDocumentViewChanged -= OnActiveDocumentViewChanged;
                    ViewHost.ActiveViewChanged -= OnActiveViewChanged;
                }

                // Dispose managed resources. TODO: double check if we need to dispose managed resources?
                if (mainWindow != null && !mainWindow.IsWindowDisposed)
                {
                    mainWindow.Dispose();
                    mainWindow = null;
                }

                DocumentViewController = null;

                splashScreen = null;

                MessageWindowLogAppender.Instance.MessageWindow = null;

                RemoveLogging();
                Plugins = null;
            }

            #region prevent nasty Windows.Forms memory leak (keeps references to databinding objects / controls

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

            #endregion

            GC.Collect();

            instanceCreationStackTrace = "";
            isAlreadyRunningInstanceOfIGui = false;
        }

        private void InitializeProjectFromPath(string projectPath)
        {
            var setDefaultProject = string.IsNullOrWhiteSpace(projectPath);
            if (!setDefaultProject)
            {
                setDefaultProject = !storageCommandHandler.OpenExistingProject(projectPath);
            }
            if (setDefaultProject)
            {
                log.Info(Resources.GuiCore_Run_Creating_new_project);
                CreateNewProject();
            }
        }

        private void DeactivatePlugin(PluginBase plugin)
        {
            try
            {
                plugin.Deactivate();
            }
            catch (Exception exception)
            {
                log.Error(Resources.GuiCore_ActivatePlugins_Exception_during_plugin_gui_deactivation, exception);
            }

            plugin.Dispose();

            Plugins.Remove(plugin);
        }

        private void ApplicationProjectOpened(IProject newProject)
        {
            if (mainWindow != null)
            {
                mainWindow.ValidateItems();
            }
        }

        private static void ConfigureLogging()
        {
            // configure logging
            var rootLogger = ((Hierarchy) LogManager.GetRepository()).Root;

            if (!rootLogger.Appenders.Cast<IAppender>().Any(a => a is MessageWindowLogAppender))
            {
                rootLogger.AddAppender(new MessageWindowLogAppender());
                rootLogger.Repository.Configured = true;
            }
        }

        private static void RemoveLogging()
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

            InitializePlugins();
        }

        private void ShowSplashScreen()
        {
            splashScreen = new SplashScreen
            {
                VersionText = SettingsHelper.ApplicationVersion
            };

            splashScreen.IsVisibleChanged += delegate
            {
                if (splashScreen.IsVisible)
                {
                    return;
                }

                if (!runFinished) // splash screen was closed before gui started.
                {
                    log.Info(Resources.GuiCore_ShowSplashScreen_User_has_cancelled_start_Exiting);
                    Environment.Exit(1);
                }
            };

            splashScreen.Show();
        }

        private void HideSplashScreen()
        {
            splashScreen.Shutdown();
        }

        private void InitializeMainWindow()
        {
            mainWindow.Loaded += delegate
            {
                // make sure these windows come on top
                if (ViewHost.ToolViews.Contains(mainWindow.PropertyGrid))
                {
                    ViewHost.SetFocusToView(mainWindow.PropertyGrid);
                }

                if (ViewHost.ToolViews.Contains(mainWindow.MessageWindow))
                {
                    ViewHost.SetFocusToView(mainWindow.MessageWindow);
                }

                if (ViewHost.ToolViews.Contains(ProjectExplorer))
                {
                    ViewHost.SetFocusToView(ProjectExplorer);
                }

                mainWindow.ValidateItems();
            };

            mainWindow.Closing += delegate(object sender, CancelEventArgs e)
            {
                if (isExiting)
                {
                    return;
                }

                e.Cancel = true; //cancel closing: let Exit handle it
                ExitApplication();
            };
        }

        private void InitializeWindows()
        {
            InitializeMainWindow();

            ViewHost = mainWindow.ViewHost;
            ViewHost.ViewClosed += OnActiveDocumentViewChanged;
            ViewHost.ActiveDocumentViewChanged += OnActiveDocumentViewChanged;
            ViewHost.ActiveViewChanged += OnActiveViewChanged;

            DocumentViewController = new DocumentViewController(ViewHost, Plugins.SelectMany(p => p.GetViewInfos()), mainWindow);

            PropertyResolver = new PropertyResolver(Plugins.SelectMany(p => p.GetPropertyInfos()));
            applicationFeatureCommands = new ApplicationFeatureCommandHandler(PropertyResolver, mainWindow);

            mainWindow.InitializeToolWindows();

            mainWindow.SubscribeToGui();

            UpdateTitle();
        }

        private void OnActiveDocumentViewChanged(object sender, EventArgs e)
        {
            if (mainWindow != null && !mainWindow.IsWindowDisposed)
            {
                mainWindow.ValidateItems();
            }
        }

        private void OnActiveViewChanged(object sender, ViewChangeEventArgs e)
        {
            var selectionProvider = e.View as ISelectionProvider;
            if (selectionProvider != null)
            {
                Selection = selectionProvider.Selection ?? Selection;
            }
        }

        private void InitializePlugins()
        {
            Plugins.ForEachElementDo(p => p.Gui = this);

            var problematicPlugins = new List<PluginBase>();

            // Try to activate all plugins
            foreach (var plugin in Plugins)
            {
                try
                {
                    plugin.Activate();
                }
                catch
                {
                    problematicPlugins.Add(plugin);
                }
            }

            // Deactivate and remove all problematic plugins
            foreach (var problematicPlugin in problematicPlugins)
            {
                DeactivatePlugin(problematicPlugin);
            }
        }

        ~GuiCore()
        {
            Dispose(false);
        }

        #region Implementation: IProjectOwner

        private IProject project;
        private readonly Observer projectObserver;

        public event Action<IProject> ProjectOpened;

        public string ProjectFilePath { get; set; }

        public IProject Project
        {
            get
            {
                return project;
            }
            set
            {
                if (project != null)
                {
                    ViewCommands.RemoveAllViewsForItem(project);
                }

                project = value;
                projectObserver.Observable = project;
                UpdateTitle();

                if (project != null)
                {
                    if (ProjectOpened != null)
                    {
                        ProjectOpened(project);
                    }
                }
            }
        }

        public bool IsCurrentNew()
        {
            return Project != null && Project.Equals(projectFactory.CreateNewProject());
        }

        public void CreateNewProject()
        {
            Project = projectFactory.CreateNewProject();
        }

        public void CloseProject()
        {
            Project = null;
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
            }
        }

        #endregion

        #region Implementation: ICommandsOwner

        private ApplicationFeatureCommandHandler applicationFeatureCommands;
        private readonly ViewCommandHandler viewCommandHandler;
        private readonly GuiImportHandler importCommandHandler;
        private readonly GuiExportHandler exportCommandHandler;
        private readonly StorageCommandHandler storageCommandHandler;

        public IApplicationFeatureCommands ApplicationCommands
        {
            get
            {
                return applicationFeatureCommands;
            }
        }

        public IStorageCommands StorageCommands
        {
            get
            {
                return storageCommandHandler;
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

        #region Implementation: IViewController

        public IViewHost ViewHost { get; private set; }

        public IDocumentViewController DocumentViewController { get; private set; }

        #endregion

        #region Implementation: IPluginHost

        public IList<PluginBase> Plugins { get; private set; }

        public IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            return Plugins.SelectMany(pluginGui => pluginGui.GetTreeNodeInfos());
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
                mainWindow = (MainWindow) value;
                mainWindow.SetGui(this);
            }
        }

        private void UpdateTitle()
        {
            if (mainWindow != null)
            {
                mainWindow.Title = string.Format(CultureInfo.CurrentCulture,
                                                 "{0} - {1} {2}",
                                                 Project != null ? Project.Name : Resources.GuiCore_UpdateTitle_Unknown,
                                                 FixedSettings.MainWindowTitle,
                                                 SettingsHelper.ApplicationVersion);
            }
        }

        #endregion
    }
}