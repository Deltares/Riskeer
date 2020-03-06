// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.Settings;
using Core.Common.Util.Extensions;
using Core.Common.Util.Settings;
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

        private readonly Observer projectObserver;
        private ISelectionProvider currentSelectionProvider;

        private bool isExiting;
        private bool runFinished;
        private SplashScreen splashScreen;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiCore"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="projectStore">The project store.</param>
        /// <param name="projectMigrator">The project migrator.</param>
        /// <param name="projectFactory">The project factory.</param>
        /// <param name="fixedSettings">The fixed settings.</param>
        /// <exception cref="InvalidOperationException">Thrown when another <see cref="GuiCore"/>
        /// instance is running.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GuiCore(IMainWindow mainWindow, IStoreProject projectStore, IMigrateProject projectMigrator, IProjectFactory projectFactory, GuiCoreSettings fixedSettings)
        {
            // error detection code, make sure we use only a single instance of GuiCore at a time
            if (isAlreadyRunningInstanceOfIGui)
            {
                isAlreadyRunningInstanceOfIGui = false; // reset to that the consecutive creations won't fail.
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                                  Resources.GuiCore_Only_a_single_instance_of_Riskeer_is_allowed_at_the_same_time_per_process_Make_sure_that_the_previous_instance_was_disposed_correctly_stack_trace_0,
                                  instanceCreationStackTrace));
            }

            if (mainWindow == null)
            {
                throw new ArgumentNullException(nameof(mainWindow));
            }

            if (projectStore == null)
            {
                throw new ArgumentNullException(nameof(projectStore));
            }

            if (projectMigrator == null)
            {
                throw new ArgumentNullException(nameof(projectMigrator));
            }

            if (projectFactory == null)
            {
                throw new ArgumentNullException(nameof(projectFactory));
            }

            if (fixedSettings == null)
            {
                throw new ArgumentNullException(nameof(fixedSettings));
            }

            MainWindow = mainWindow;
            FixedSettings = fixedSettings;

            isAlreadyRunningInstanceOfIGui = true;
            instanceCreationStackTrace = new StackTrace().ToString();

            Plugins = new List<PluginBase>();

            viewCommandHandler = new ViewCommandHandler(this, this, this);

            StorageCommands = new StorageCommandHandler(projectStore, projectMigrator, projectFactory,
                                                        this, dialogBasedInquiryHelper, MainWindow);

            importCommandHandler = new GuiImportHandler(MainWindow, Plugins.SelectMany(p => p.GetImportInfos()), dialogBasedInquiryHelper);
            exportCommandHandler = new GuiExportHandler(MainWindow, Plugins.SelectMany(p => p.GetExportInfos()));
            updateCommandHandler = new GuiUpdateHandler(MainWindow, Plugins.SelectMany(p => p.GetUpdateInfos()), dialogBasedInquiryHelper);

            WindowsApplication.EnableVisualStyles();
            ViewPropertyEditor.ViewCommands = ViewCommands;

            ProjectOpened += ApplicationProjectOpened;
            BeforeProjectOpened += ApplicationBeforeProjectOpened;
            projectObserver = new Observer(UpdateTitle);

            Project = projectFactory.CreateNewProject();
        }

        public IPropertyResolver PropertyResolver { get; private set; }

        #region Implementation: ISettingsOwner

        public GuiCoreSettings FixedSettings { get; }

        #endregion

        /// <summary>
        /// Runs the user interface, causing all user interface components to initialize, 
        /// loading plugins, opening a saved project and displaying the main window.
        /// </summary>
        /// <param name="projectPath">Path to the project to be opened. (optional)</param>
        public void Run(string projectPath = null)
        {
            DateTime startTime = DateTime.Now;

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
            GC.SuppressFinalize(this);
        }

        public void ExitApplication()
        {
            if (isExiting)
            {
                return; //already got here before
            }

            // Store project?
            if (!StorageCommands.HandleUnsavedChanges())
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

        public IContextMenuBuilder Get(object value, TreeViewControl treeViewControl)
        {
            if (applicationFeatureCommands == null)
            {
                throw new InvalidOperationException("Call IGui.Run in order to initialize dependencies before getting the ContextMenuBuilder.");
            }

            return new ContextMenuBuilder(applicationFeatureCommands,
                                          importCommandHandler,
                                          exportCommandHandler,
                                          updateCommandHandler,
                                          ViewCommands,
                                          value,
                                          treeViewControl);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                projectObserver.Dispose();

                if (Plugins != null)
                {
                    foreach (PluginBase plugin in Plugins.ToArray())
                    {
                        DeactivatePlugin(plugin);
                    }
                }

                isExiting = true;

                mainWindow?.UnsubscribeFromGui();

                Selection = null;

                if (ViewHost != null)
                {
                    ViewHost.Dispose();
                    ViewHost.ViewClosed -= OnViewClosed;
                    ViewHost.ViewClosed -= OnActiveDocumentViewChanged;
                    ViewHost.ActiveDocumentViewChanged -= OnActiveDocumentViewChanged;
                    ViewHost.ActiveViewChanged -= OnActiveViewChanged;
                }

                if (currentSelectionProvider != null)
                {
                    currentSelectionProvider.SelectionChanged -= OnSelectionChanged;
                }

                // Dispose managed resources. TODO: double check if we need to dispose managed resources?
                if (mainWindow != null && !mainWindow.IsWindowDisposed)
                {
                    mainWindow.Dispose();
                    mainWindow = null;
                }

                if (DocumentViewController != null)
                {
                    DocumentViewController.Dispose();
                    DocumentViewController = null;
                }

                splashScreen = null;

                MessageWindowLogAppender.Instance.MessageWindow = null;

                RemoveLogging();
                Plugins = null;
            }

            #region Prevent nasty Windows.Forms memory leak (keeps references to databinding objects / controls

            Assembly systemAssembly = typeof(Component).Assembly;
            Type reflectTypeDescriptionProviderType =
                systemAssembly.GetType("System.ComponentModel.ReflectTypeDescriptionProvider");
            FieldInfo propertyCacheInfo = reflectTypeDescriptionProviderType.GetField("_propertyCache",
                                                                                      BindingFlags.Static |
                                                                                      BindingFlags.NonPublic);
            var propertyCache = (Hashtable) propertyCacheInfo?.GetValue(null);
            propertyCache?.Clear();

            FieldInfo extendedPropertyCacheInfo = reflectTypeDescriptionProviderType.GetField(
                "_extendedPropertyCache", BindingFlags.Static | BindingFlags.NonPublic);
            var extendedPropertyCache = extendedPropertyCacheInfo?.GetValue(null) as Hashtable;
            extendedPropertyCache?.Clear();

            FieldInfo eventCacheInfo = reflectTypeDescriptionProviderType.GetField("_eventCache",
                                                                                   BindingFlags.Static |
                                                                                   BindingFlags.NonPublic);
            var eventCache = eventCacheInfo?.GetValue(null) as Hashtable;
            eventCache?.Clear();

            FieldInfo attributeCacheInfo = reflectTypeDescriptionProviderType.GetField("_attributeCache",
                                                                                       BindingFlags.Static |
                                                                                       BindingFlags.NonPublic);
            var attributeCache = attributeCacheInfo?.GetValue(null) as Hashtable;
            attributeCache?.Clear();

            Type typeDescriptorType = systemAssembly.GetType("System.ComponentModel.TypeDescriptor");
            FieldInfo providerTableInfo = typeDescriptorType.GetField("_providerTable",
                                                                      BindingFlags.Static | BindingFlags.NonPublic);
            var providerTable = providerTableInfo?.GetValue(null) as Hashtable;
            providerTable?.Clear();

            FieldInfo providerTypeTableInfo = typeDescriptorType.GetField("_providerTypeTable",
                                                                          BindingFlags.Static | BindingFlags.NonPublic);
            var providerTypeTable = providerTypeTableInfo?.GetValue(null) as Hashtable;
            providerTypeTable?.Clear();

            FieldInfo defaultProvidersInfo = typeDescriptorType.GetField("_defaultProviders",
                                                                         BindingFlags.Static | BindingFlags.NonPublic);
            var defaultProviders = defaultProvidersInfo?.GetValue(null) as Hashtable;
            defaultProviders?.Clear();

            #endregion

            GC.Collect();

            instanceCreationStackTrace = "";
            isAlreadyRunningInstanceOfIGui = false;
        }

        private IProjectExplorer ProjectExplorer
        {
            get
            {
                return ViewHost.ToolViews.OfType<IProjectExplorer>().FirstOrDefault();
            }
        }

        private void InitializeProjectFromPath(string projectPath)
        {
            bool isPathGiven = !string.IsNullOrWhiteSpace(projectPath);
            if (isPathGiven)
            {
                StorageCommands.OpenExistingProject(projectPath);
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
            mainWindow?.ValidateItems();

            projectObserver.Observable = newProject;
            UpdateTitle();
        }

        private void ApplicationBeforeProjectOpened(IProject oldProject)
        {
            ViewCommands.RemoveAllViewsForItem(project);
        }

        private static void ConfigureLogging()
        {
            // configure logging
            Logger rootLogger = ((Hierarchy) LogManager.GetRepository()).Root;

            if (!rootLogger.Appenders.Cast<IAppender>().Any(a => a is MessageWindowLogAppender))
            {
                rootLogger.AddAppender(new MessageWindowLogAppender());
                rootLogger.Repository.Configured = true;
            }
        }

        private static void RemoveLogging()
        {
            Logger rootLogger = ((Hierarchy) LogManager.GetRepository()).Root;
            MessageWindowLogAppender messageWindowLogAppender = rootLogger.Appenders.Cast<IAppender>().OfType<MessageWindowLogAppender>().FirstOrDefault();
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
                VersionText = SettingsHelper.Instance.ApplicationVersion
            };

            splashScreen.IsVisibleChanged += delegate
            {
                if (splashScreen.IsVisible)
                {
                    return;
                }

                if (!runFinished) // splash screen was closed before gui started.
                {
                    log.Info(Resources.GuiCore_ShowSplashScreen_User_has_canceled_start_Exiting);
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
                if (ViewHost.ToolViews.Contains(mainWindow.PropertyGrid))
                {
                    ViewHost.BringToFront(mainWindow.PropertyGrid);
                }

                if (ViewHost.ToolViews.Contains(mainWindow.MessageWindow))
                {
                    ViewHost.BringToFront(mainWindow.MessageWindow);
                }

                if (ViewHost.ToolViews.Contains(ProjectExplorer))
                {
                    ViewHost.BringToFront(ProjectExplorer);
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
            ViewHost.ViewClosed += OnViewClosed;
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

        private void OnViewClosed(object sender, ViewChangeEventArgs e)
        {
            if (ReferenceEquals(currentSelectionProvider, e.View))
            {
                currentSelectionProvider.SelectionChanged -= OnSelectionChanged;
                currentSelectionProvider = null;

                Selection = null;
            }
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
            if (HandleActivatingCurrentSelectionProvidingDocumentView(e.View))
            {
                return;
            }

            var selectionProvider = e.View as ISelectionProvider;
            if (selectionProvider == null)
            {
                HandleDeactivatingCurrentSelectionProvidingDocumentView();

                return;
            }

            if (currentSelectionProvider != null)
            {
                currentSelectionProvider.SelectionChanged -= OnSelectionChanged;
            }

            currentSelectionProvider = selectionProvider;
            currentSelectionProvider.SelectionChanged += OnSelectionChanged;

            Selection = currentSelectionProvider.Selection;
        }

        private bool HandleActivatingCurrentSelectionProvidingDocumentView(IView activeView)
        {
            var handled = false;

            if (ReferenceEquals(currentSelectionProvider, activeView) && ViewHost.DocumentViews.Contains(activeView))
            {
                currentSelectionProvider.SelectionChanged += OnSelectionChanged;

                handled = true;
            }

            return handled;
        }

        private void HandleDeactivatingCurrentSelectionProvidingDocumentView()
        {
            if (currentSelectionProvider != null && ViewHost.DocumentViews.Contains((IView) currentSelectionProvider))
            {
                currentSelectionProvider.SelectionChanged -= OnSelectionChanged;
            }
        }

        private void OnSelectionChanged(object sender, EventArgs eventArgs)
        {
            Selection = currentSelectionProvider.Selection;
        }

        private void InitializePlugins()
        {
            Plugins.ForEachElementDo(p => p.Gui = this);

            var problematicPlugins = new List<PluginBase>();

            // Try to activate all plugins
            foreach (PluginBase plugin in Plugins)
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
            foreach (PluginBase problematicPlugin in problematicPlugins)
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
        public event Action<IProject> BeforeProjectOpened;
        public event Action<IProject> ProjectOpened;

        public string ProjectFilePath { get; private set; }

        public IProject Project
        {
            get
            {
                return project;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), @"There should always be a project.");
                }

                if (!ReferenceEquals(project, value))
                {
                    OnBeforeProjectOpened();
                    project = value;
                    OnProjectOpened();
                }
            }
        }

        private void OnBeforeProjectOpened()
        {
            BeforeProjectOpened?.Invoke(project);
        }

        private void OnProjectOpened()
        {
            ProjectOpened?.Invoke(project);
        }

        public void SetProject(IProject newProject, string projectPath)
        {
            Project = newProject;
            ProjectFilePath = projectPath;
        }

        #endregion

        #region Implementation: IApplicationSelection

        private object selection;

        public object Selection
        {
            get
            {
                return selection;
            }
            set
            {
                if (selection == value)
                {
                    return;
                }

                selection = value;

                if (mainWindow == null)
                {
                    return;
                }

                if (mainWindow.PropertyGrid != null)
                {
                    mainWindow.PropertyGrid.Data = selection;
                }

                if (!isExiting && !mainWindow.IsWindowDisposed)
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
        private readonly GuiUpdateHandler updateCommandHandler;

        public IApplicationFeatureCommands ApplicationCommands
        {
            get
            {
                return applicationFeatureCommands;
            }
        }

        public IStorageCommands StorageCommands { get; }

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

        public List<PluginBase> Plugins { get; private set; }

        public IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            return Plugins.SelectMany(pluginGui => pluginGui.GetTreeNodeInfos());
        }

        public IEnumerable GetAllDataWithViewDefinitionsRecursively(object rootValue)
        {
            var resultSet = new HashSet<object>();
            foreach (object childDataInstance in Plugins.SelectMany(p => p.GetChildDataWithViewDefinitions(rootValue)).Distinct())
            {
                resultSet.Add(childDataInstance);

                if (!ReferenceEquals(rootValue, childDataInstance))
                {
                    foreach (object dataWithViewDefined in GetAllDataWithViewDefinitionsRecursively(childDataInstance))
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
        private DialogBasedInquiryHelper dialogBasedInquiryHelper;

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
                dialogBasedInquiryHelper = new DialogBasedInquiryHelper(MainWindow);
            }
        }

        private void UpdateTitle()
        {
            if (mainWindow != null)
            {
                mainWindow.Title = string.Format(CultureInfo.CurrentCulture,
                                                 "{0} - {1} {2}",
                                                 Project.Name,
                                                 FixedSettings.MainWindowTitle,
                                                 SettingsHelper.Instance.ApplicationVersion);
            }
        }

        #endregion
    }
}