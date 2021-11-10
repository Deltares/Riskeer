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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Core.Common.Util.Settings;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Log;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.PropertyView;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using Core.Gui.Plugin.Chart;
using Core.Gui.Plugin.Map;
using Core.Gui.Properties;
using Core.Gui.Settings;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using WindowsApplication = System.Windows.Forms.Application;

namespace Core.Gui
{
    /// <summary>
    /// Gui class that provides graphical user functionality for the application.
    /// </summary>
    public class GuiCore : IGui
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiCore));

        private readonly string applicationTitle;
        private readonly Observer projectObserver;
        private readonly IDictionary<ToggleButton, StateInfo> stateInfoLookup = new Dictionary<ToggleButton, StateInfo>();

        private ISelectionProvider currentSelectionProvider;

        private bool isExiting;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiCore"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="projectStore">The project store.</param>
        /// <param name="projectMigrator">The project migrator.</param>
        /// <param name="projectFactory">The project factory.</param>
        /// <param name="fixedSettings">The fixed settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GuiCore(IMainWindow mainWindow, IStoreProject projectStore, IMigrateProject projectMigrator, IProjectFactory projectFactory, GuiCoreSettings fixedSettings)
        {
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

            ProjectStore = projectStore;
            FixedSettings = fixedSettings;
            MainWindow = mainWindow;

            Plugins = new List<PluginBase>();

            viewCommandHandler = new ViewCommandHandler(this, this, this);

            StorageCommands = new StorageCommandHandler(projectStore, projectMigrator, projectFactory,
                                                        this, dialogBasedInquiryHelper, this);

            importCommandHandler = new GuiImportHandler(MainWindow, Plugins.SelectMany(p => p.GetImportInfos())
                                                                           .Concat(MapImportInfoFactory.Create()),
                                                        dialogBasedInquiryHelper);
            exportCommandHandler = new GuiExportHandler(MainWindow, Plugins.SelectMany(p => p.GetExportInfos()));
            updateCommandHandler = new GuiUpdateHandler(MainWindow, Plugins.SelectMany(p => p.GetUpdateInfos()), dialogBasedInquiryHelper);

            WindowsApplication.EnableVisualStyles();
            ViewPropertyEditor.ViewCommands = ViewCommands;

            ProjectOpened += ApplicationProjectOpened;
            BeforeProjectOpened += ApplicationBeforeProjectOpened;
            projectObserver = new Observer(UpdateProjectData);

            applicationTitle = string.Format(CultureInfo.CurrentCulture, "{0} {1}",
                                             FixedSettings.ApplicationName,
                                             SettingsHelper.Instance.ApplicationVersion);

            SetTitle();
        }

        public IPropertyResolver PropertyResolver { get; private set; }

        public IStoreProject ProjectStore { get; }

        #region Implementation: ISettingsOwner

        public GuiCoreSettings FixedSettings { get; }

        #endregion

        /// <summary>
        /// Runs the user interface, causing all user interface components to initialize, 
        /// loading plugins, opening a saved project and displaying the main window.
        /// </summary>
        /// <param name="projectPath">Optional: path to the project to be opened.</param>
        public void Run(string projectPath = null)
        {
            DateTime startTime = DateTime.Now;

            ConfigureLogging();

            Initialize();

            log.InfoFormat(Resources.GuiCore_Run_Started_in_0_f2_sec, (DateTime.Now - startTime).TotalSeconds);

            mainWindow.Show();

            bool isPathGiven = !string.IsNullOrWhiteSpace(projectPath);
            if (isPathGiven)
            {
                StorageCommands.OpenExistingProject(projectPath);
            }

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
                return;
            }

            // Store project or handle cancel action by the user
            if (!StorageCommands.HandleUnsavedChanges())
            {
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

                foreach (PluginBase plugin in Plugins.ToArray())
                {
                    DeactivatePlugin(plugin);
                }

                isExiting = true;

                mainWindow.UnsubscribeFromGui();

                Selection = null;

                if (ViewHost != null)
                {
                    ViewHost.Dispose();
                    ViewHost.ViewClosed -= OnViewClosed;
                    ViewHost.ActiveViewChanged -= OnActiveViewChanged;
                }

                if (currentSelectionProvider != null)
                {
                    currentSelectionProvider.SelectionChanged -= OnSelectionChanged;
                }

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

                MessageWindowLogAppender.Instance.MessageWindow = null;

                RemoveLogging();
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
            projectObserver.Observable = newProject;
            UpdateProjectData();
            mainWindow.ResetState();
        }

        private void ApplicationBeforeProjectOpened(IProject oldProject)
        {
            ViewCommands.RemoveAllViewsForItem(project);
        }

        private static void ConfigureLogging()
        {
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

                if (ViewHost.ToolViews.Contains(mainWindow.ProjectExplorer))
                {
                    ViewHost.BringToFront(mainWindow.ProjectExplorer);
                }
            };

            mainWindow.Closing += delegate(object sender, CancelEventArgs e)
            {
                if (isExiting)
                {
                    return;
                }

                e.Cancel = true; // Handle exit manually
                ExitApplication();
            };
        }

        private void InitializeWindows()
        {
            InitializeMainWindow();

            ViewHost = mainWindow.ViewHost;
            ViewHost.ViewClosed += OnViewClosed;
            ViewHost.ActiveViewChanged += OnActiveViewChanged;

            DocumentViewController = new DocumentViewController(ViewHost, Plugins.SelectMany(p => p.GetViewInfos()), mainWindow);

            PropertyResolver = new PropertyResolver(Plugins.SelectMany(p => p.GetPropertyInfos())
                                                           .Concat(ChartPropertyInfoFactory.Create())
                                                           .Concat(MapPropertyInfoFactory.Create()));
            applicationFeatureCommands = new ApplicationFeatureCommandHandler(PropertyResolver, mainWindow);

            mainWindow.InitializeToolWindows();

            foreach (StateInfo stateInfo in Plugins.SelectMany(pluginGui => pluginGui.GetStateInfos()))
            {
                stateInfoLookup[mainWindow.AddStateButton(stateInfo.Name, stateInfo.Symbol, stateInfo.FontFamily, stateInfo.GetRootData)] = stateInfo;
            }

            mainWindow.SubscribeToGui();
        }

        private void OnViewClosed(object sender, ViewChangeEventArgs e)
        {
            if (ReferenceEquals(currentSelectionProvider, e.View))
            {
                currentSelectionProvider.SelectionChanged -= OnSelectionChanged;
                currentSelectionProvider = null;

                Selection = null;
            }

            if (!mainWindow.IsWindowDisposed)
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

        private void SetTitle()
        {
            mainWindow.Title = Project != null
                                   ? string.Format(CultureInfo.CurrentCulture,
                                                   "{0} - {1}", Project.Name, applicationTitle)
                                   : applicationTitle;
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
            get => project;
            private set
            {
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
            get => selection;
            set
            {
                if (selection == value)
                {
                    return;
                }

                selection = value;

                if (mainWindow?.PropertyGrid != null)
                {
                    mainWindow.PropertyGrid.Data = selection;
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

        public IApplicationFeatureCommands ApplicationCommands => applicationFeatureCommands;

        public IStorageCommands StorageCommands { get; }

        public IViewCommands ViewCommands => viewCommandHandler;

        #endregion

        #region Implementation: IViewController

        public IViewHost ViewHost { get; private set; }

        public IDocumentViewController DocumentViewController { get; private set; }

        #endregion

        #region Implementation: IPluginHost

        public List<PluginBase> Plugins { get; }

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
            get => mainWindow;
            private set
            {
                mainWindow = (MainWindow) value;
                mainWindow.SetGui(this);
                dialogBasedInquiryHelper = new DialogBasedInquiryHelper(MainWindow);
            }
        }

        private void UpdateProjectData()
        {
            SetTitle();
            mainWindow.BackstageViewModel.InfoViewModel.SetProject(project);
        }

        #endregion
    }
}