using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Dao;
using DelftTools.Shell.Core.Services;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Utils;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Globalization;
using DelftTools.Utils.Reflection;
using DelftTools.Utils.RegularExpressions;
using DelftTools.Utils.Threading;
using DeltaShell.Core.Services;
using log4net;

namespace DeltaShell.Core
{
    public class DeltaShellApplication : IApplication
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DeltaShellApplication));

        private PluginConfigurationLoader pluginConfigurationLoader;
        private IList<ApplicationPlugin> plugins;

        private Project project;
        private NameValueCollection settings;
        private DeltaShellApplicationSettings userSettings;
        private ResourceManager resources;
        private IProjectService projectService;
        private IProjectRepositoryFactory projectRepositoryFactory;
        private NotifyingThreadQueue<IActivity> activities;
        private readonly IList<IFileImporter> fileImporters = new List<IFileImporter>();
        private readonly IList<IFileExporter> fileExporters = new List<IFileExporter>();

        private string defaultRepositoryTypeName;

        private bool isRunning;

        public Action WaitMethod;

        public string PluginVersions
        {
            get { return String.Join("\n", plugins.Select(p => p.Name + "  " + p.Version)); }
        }

        public event Action AfterRun;
        
        public bool IsDataAccessSynchronizationDisabled { get; set; }

        public PluginConfigurationLoader PluginConfigurationLoader
        {
            get { return pluginConfigurationLoader; }
        }

        // TODO: migrate into ProjectService
        public IProjectRepositoryFactory ProjectRepositoryFactory
        {
            get { return projectRepositoryFactory; }
            set { projectRepositoryFactory = value; }
        }

        public DeltaShellApplication()
        {
            projectRepositoryFactory = new ProjectRepositoryFactory<InMemoryProjectRepository>();

            Settings = ConfigurationManager.AppSettings;
            UserSettings = Properties.Settings.Default;

            ProjectService = new ProjectService(projectRepositoryFactory);

            plugins = new List<ApplicationPlugin>();

            ActivityRunner = new ActivityRunner();
            ActivityRunner.IsRunningChanged += ActivityRunnerIsRunningChanged;

            if (RunningActivityLogAppender.Instance != null)
            {
                RunningActivityLogAppender.Instance.ActivityRunner = ActivityRunner;
            }
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
            
            InitializeSettingsHelper();
			
            Settings = ConfigurationManager.AppSettings;
			UserSettings = Properties.Settings.Default;
        }

        static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            //HACK : this is needed because of issue 4382...the black boxes in PG. It seem like the assembly for 
            //enum types like AggregationOptions cannot be found without this 
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.FullName == args.Name);
        }

        void ActivityRunnerIsRunningChanged(object sender, EventArgs e)
        {
            //don't fire events while running models
            DelayedEventHandlerController.FireEvents = !ActivityRunner.IsRunning;
        }

        public ApplicationPlugin GetPluginForType(Type type)
        {
            foreach (var plugin in plugins)
            {
                if (plugin.GetType().Assembly.Equals(type.Assembly))
                {
                    return plugin;
                }

                if (plugin.GetDataItemInfos().Any(dii => dii.ValueType == type))
                {
                    return plugin;
                }
            }

            return null;
        }

        public Project Project
        {
            get { return project; }
            
            [InvokeRequired]
            set
            {
                if (project != null)
                {
                    if (ProjectClosing != null)
                    {
                        ProjectClosing(project);
                    }

                    ProjectService.Close(project);
                }

                if (value != null && ProjectOpening != null)
                {
                    ProjectOpening(value);
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

        [InvokeRequired]
        private void ProjectServiceProjectSaving(object sender, EventArgs e)
        {
            if (ProjectSaving != null)
            {
                ProjectSaving(Project);
            }
        }

        [InvokeRequired]
        void ProjectServiceProjectSaveFailed(object sender, EventArgs e)
        {
            if (ProjectSaveFailed != null)
            {
                ProjectSaveFailed(Project);
            }
        }

        [InvokeRequired]
        void ProjectServiceProjectSaved(object sender, EventArgs e)
        {
            if (ProjectSaved != null)
            {
                ProjectSaved(Project);
            }
        }

        public IList<ApplicationPlugin> Plugins
        {
            get { return plugins; }
            set { plugins = value; }
        }



        // TODO: migrate into ProjectService
        public event Action<Project> ProjectOpening;
        public event Action<Project> ProjectOpened;

        // TODO: migrate into ProjectService
        public event Action<Project> ProjectClosing;
        public event Action<Project> ProjectSaving;
        public event Action<Project> ProjectSaveFailed;
        public event Action<Project> ProjectSaved;

        public IActivityRunner ActivityRunner { get; set; }

        public ApplicationSettingsBase UserSettings
        {
            get { return userSettings; }
            set
            {
                userSettings = new DeltaShellApplicationSettings(value); // small hack, wrap settings so that we will know when they are changed.
            }
        }

        public string GetUserSettingsDirectoryPath()
        {
            return SettingsHelper.GetApplicationLocalUserSettingsDirectory();
        }

        public NameValueCollection Settings
        {
            get { return this.settings; }
            set { this.settings = value; }
        }

        public string Version
        {
            get
            {
                return SettingsHelper.ApplicationVersion;
                /*var assemblyInfo = AssemblyUtils.GetAssemblyInfo(Assembly.GetExecutingAssembly());
                return assemblyInfo.Version;*/
            }
        }

        public string ApplicationNameAndVersion
        {
            get { return SettingsHelper.ApplicationNameAndVersion; }
        }

        public bool IsProjectCreatedInTemporaryDirectory { get; set; }

        public void Run(string projectPath)
        {
            Run();
            OpenProject(projectPath);
        }

        private bool running;
        public void Run()
        {
            if(isRunning)
            {
                throw new InvalidOperationException(Properties.Resources.DeltaShellApplication_Run_You_can_call_Run___only_once_per_application);
            }

            initializing = true;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            log.Info(Properties.Resources.DeltaShellApplication_Run_Starting_Delta_Shell____);

            if(running)
            {
                throw new InvalidOperationException(Properties.Resources.DeltaShellApplication_Run_Application_is_already_running);
            }

            running = true;

            // load all assemblies from current assembly directory
            AssemblyUtils.LoadAllAssembliesFromDirectory(Path.GetFullPath(Path.GetDirectoryName(GetType().Assembly.Location))).ToList();

            SetLanguageAndRegionalSettions();

            ProjectService = new ProjectService();

            //Disabled trace logging this causes focus bugs combined with avalon dock (KeyPreview debug messages)
            //InitializeLogging();

            LogSystemInfo();

            InitializeLicense();

            log.Info(Properties.Resources.DeltaShellApplication_Run_Initializing_plugins____); 
            InitializePlugins();
            
            log.Info(Properties.Resources.DeltaShellApplication_Run_Initializing_project_repository____);
            InitializeProjectRepositoryFactory();

            isRunning = true;

            log.Info(Properties.Resources.DeltaShellApplication_Run_Creating_new_project____);
            CreateNewProject();

            log.Info(Properties.Resources.DeltaShellApplication_Run_Activating_plugins____);
            InitializePluginResources();

            ActivatePlugins();

            RegisterDataTypes();
            RegisterImporters();
            RegisterExporters();

            log.Info(Properties.Resources.DeltaShellApplication_Run_Waiting_until_all_plugins_are_activated____);

            if (!TemporaryProjectSavedAsynchroneously)
            {
                // wait until all plugins are activated
                while(Plugins.Any(p => !p.IsActive))
                {
                    Thread.Sleep(250);
                }
            }

            Project = projectBeingCreated; // opens project in application

            initializing = false;

            stopwatch.Stop();

            log.InfoFormat(Properties.Resources.DeltaShellApplication_Run_Delta_Shell_is_ready__started_in__0_F3__sec_, stopwatch.ElapsedMilliseconds / 1000.0);

            if (AfterRun != null)
            {
                AfterRun();
            }
        }

        private void InitializeSettingsHelper()
        {
            //read settings from app.config and update the settings helper.
            if (Settings.AllKeys.Contains("applicationName"))
            {
                SettingsHelper.ApplicationName = Settings["applicationName"];
                SettingsHelper.ApplicationVersion = Settings["fullVersion"];
            }
        }

        public static void SetLanguageAndRegionalSettions(ApplicationSettingsBase tempUserSettings = null)
        {
            var settings = ConfigurationManager.AppSettings;
            
            var language = settings["language"];

            if (language != null)
            {
                RegionalSettingsManager.Language = language;
            }

            if (tempUserSettings != null && tempUserSettings.Properties.Count > 0)
            {
                var realNumberFormat = tempUserSettings["realNumberFormat"];
                if (realNumberFormat != null)
                {
                    RegionalSettingsManager.RealNumberFormat = (string)realNumberFormat;
                }
            }

            var culture = Thread.CurrentThread.CurrentCulture.ToString();
            if (culture == "tr-TR" || culture == "az")
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            }
        }


        public void CloseProject()
        {
            Project = null;
        }

        public void SaveProjectAs(string path)
        {
            ProjectService.SaveProjectAs(Project, path);
        }

        public void SaveProject()
        {
            ProjectService.Save(Project);
        }

        public void CreateNewProject()
        {
            if (!isRunning)
            {
                throw new InvalidOperationException(Properties.Resources.DeltaShellApplication_CreateNewProject_Run___must_be_called_first_before_project_can_be_opened);
            }

            projectBeingCreated = new Project();

            if (!initializing) // open in app
            {
                Project = projectBeingCreated;
            }

            if (IsProjectCreatedInTemporaryDirectory)
            {
                TemporaryProjectBeingSaved = true;

                // for now enable async save only during start-up
                if (initializing && TemporaryProjectSavedAsynchroneously)
                {
                    var saveProjectThread = new Thread(SaveTemporaryProjectThread)
                    {
                        CurrentCulture = CultureInfo.CurrentCulture,
                        CurrentUICulture = CultureInfo.CurrentUICulture
                    };
                    saveProjectThread.Start();
                }
                else
                {
                    SaveTemporaryProjectThread();
                }
            }
        }

        private Project projectBeingCreated;
        private bool initializing;

        private void SaveTemporaryProjectThread()
        {
            ProjectService.SaveProjectInTemporaryFolder(projectBeingCreated);
            TemporaryProjectSaved = true;
            TemporaryProjectBeingSaved = false;
        }

        public static bool TemporaryProjectBeingSaved;
        public static bool TemporaryProjectSaved;
        public static bool TemporaryProjectSavedAsynchroneously;

        public bool OpenProject(string path)
        {
            if(!isRunning)
            {
                throw new InvalidOperationException(Properties.Resources.DeltaShellApplication_CreateNewProject_Run___must_be_called_first_before_project_can_be_opened);
            }

            if(Project != null)
            {
                CloseProject();
            }

            var retrievedProject = ProjectService.Open(path);
            if (retrievedProject != null)
            {
                Project = retrievedProject;    
            }
            return retrievedProject != null;
        }
        
        private static void LogSystemInfo()
        {
            log.DebugFormat(Properties.Resources.DeltaShellApplication_LogSystemInfo_Environmental_variables_);

            var culture = Thread.CurrentThread.CurrentCulture;
            log.DebugFormat("{0} = {1}", "CURRENT_THREAD_CULTURE" , culture.EnglishName);
            log.DebugFormat("{0} = {1}", "NUMBER_DECIMAL_DIGITS" , culture.NumberFormat.NumberDecimalDigits);
            log.DebugFormat("{0} = {1}", "NUMBER_DECIMAL_SEPARATOR" , culture.NumberFormat.NumberDecimalSeparator);
            log.DebugFormat("{0} = {1}", "FULL_DATE_TIME_PATTERN" , culture.DateTimeFormat.FullDateTimePattern);
            log.DebugFormat("{0} = {1}", "DATE_SEPARATOR" , culture.DateTimeFormat.DateSeparator);
            log.DebugFormat("{0} = {1}", "OS_VERSION" , Environment.OSVersion);
            log.DebugFormat("{0} = {1}", "OS_VERSION_NUMBER" , Environment.Version);

            foreach(DictionaryEntry pair in Environment.GetEnvironmentVariables())
            {
                log.DebugFormat("{0} = {1}", pair.Key, pair.Value);
            }
        }

        protected void ActivatePlugins()
        {
            log.Debug(Properties.Resources.DeltaShellApplication_Run_Activating_plugins____);

            /*
            // activate all plugins (parallel)
            Parallel.For(0, plugins.Count, delegate(int i)
            {
                log.InfoFormat("Activating plugin {0} ...", plugins[i].Name);
                plugins[i].Activate();
                log.InfoFormat("Activating plugin {0} done.", plugins[i].Name);
            }
            );
            */

            // activate all plugins
            foreach (var plugin in plugins)
            {
                log.DebugFormat(Properties.Resources.DeltaShellApplication_ActivatePlugins_Activating_plugin__0_____, plugin.Name);
                string cwdOld = ".";

                var assembly = plugin.GetType().Assembly;
                if(!assembly.IsDynamic())
                {
                    cwdOld = Path.GetDirectoryName(assembly.Location);
                }

                var cwd = Environment.CurrentDirectory;
                Environment.CurrentDirectory = cwdOld;

                plugin.Activate();

                Environment.CurrentDirectory = cwd;
            }

            log.Debug(Properties.Resources.DeltaShellApplication_ActivatePlugins_All_plugins_were_activated_);
        }

        /// <summary>
        /// Set license environmental variables to path of the license file. 
        /// </summary>
        private void InitializeLicense()
        {
            string licenseFilePath = null;

            if (Settings != null && Settings["licenseFilePath"] != null)
            {
                licenseFilePath = Settings["licenseFilePath"];
            }

            if(string.IsNullOrEmpty(licenseFilePath))
            {
                return;
            }

            log.Debug(Properties.Resources.DeltaShellApplication_InitializeLicense_Initializing_license____);

            if (!File.Exists(licenseFilePath))
            {
                log.WarnFormat(Properties.Resources.DeltaShellApplication_InitializeLicense_License_file_does_not_exist___0_, licenseFilePath);
            }
            else
            {
                Environment.SetEnvironmentVariable("DHSDELFT_LICENSE_FILE", Path.GetFullPath(licenseFilePath));
            }

            log.Debug(Properties.Resources.DeltaShellApplication_InitializeLicense_License_is_initialized_);
        }

        protected void InitializePlugins()
        {
            log.Debug(Properties.Resources.DeltaShellApplication_InitializePlugins_Searching_for_plugins____);

            var pluginsDirectory = GetPluginDirectory();
            
            PluginManager.RegisterAdditionalPlugins(Plugins);
            PluginManager.Initialize(pluginsDirectory);
            
            var newApplicationPlugins = PluginManager.GetPlugins<ApplicationPlugin>().Except(Plugins).ToList();
            foreach (var plugin in newApplicationPlugins)
            {
                plugins.Add(plugin);
            }
            Plugins.ForEach(p => p.Application = this);
            
            log.InfoFormat(Properties.Resources.DeltaShellApplication_InitializePlugins__0__plugin_s__were_loaded, Plugins.Count);
        }

        private string GetPluginDirectory()
        {
            // load plugins from a configured folder
            if (Settings != null)
            {
                string pluginsDirectory = Settings["pluginsDirectory"];

                if (Directory.Exists(pluginsDirectory))
                {
                    return pluginsDirectory;
                }
            }
            return null;
        }

        private void InitializePluginResources()
        {
            if (plugins.Count > 0)
            {
                string s = "";
                foreach (var p in plugins)
                {
                    InitializePluginResources(p);

                    s += p.Name + ", ";
                }
                log.Debug(Properties.Resources.DeltaShellApplication_InitializePluginResources_DeltaShell_application_started__active_plugins__ + s.Substring(0, s.Length - 2));
            }
            else
            {
                log.Warn(Properties.Resources.DeltaShellApplication_InitializePluginResources_No_plugins_found__most_probably_there_is_a_configuration_problem_);
                log.Warn(Properties.Resources.DeltaShellApplication_InitializePluginResources_Please_check_your_configuration_and_plugins_folder_);
            }
        }

        /// <summary>
        /// Sets plugin resources, resource manager will automatically select culture-aware resource file.
        /// </summary>
        /// <param name="plugin"></param>
        public static void InitializePluginResources(IPlugin plugin)
        {
            var resourceName = plugin.GetType().Assembly.FullName.Split(',')[0] + ".Properties.Resources";
            plugin.Resources = new ResourceManager(resourceName, plugin.GetType().Assembly);
        }

        /// <summary>
        /// Initialize the log4net part
        /// </summary>
        protected static void InitializeLogging()
        {
            if (!Trace.Listeners.Cast<TraceListener>().Any(tl => tl is DeltaShellTraceListener))
            {
                Trace.Listeners.Add(new DeltaShellTraceListener());
            }
        }

        #region Nested type: DeltaShellTraceListener

        internal class DeltaShellTraceListener : TraceListener
        {
            private static ILog log;
            private static bool logTraceMessages = true;

            public DeltaShellTraceListener()
            {
                log = LogManager.GetLogger(GetType());
            }

            public static bool LogTraceMessages
            {
                get { return logTraceMessages; }
                set { logTraceMessages = value; }
            }

            public override void Write(string message)
            {
                if (logTraceMessages)
                {
                    WriteLine(message);
                }
            }

            public override void WriteLine(string message)
            {
                if (logTraceMessages)
                {
                    log.Debug(message);
                }
            }
        }

        #endregion

        public void Exit()
        {
            Trace.Listeners.Clear();

            if (Project != null && Project.IsChanged)
            {
                CloseProject();
            }

            if (userSettings.IsDirty)
            {
                UserSettings.Save();
            }
        }

        // TODO: hide it
        public IProjectService ProjectService
        {
            get { return projectService; }
            set
            {
                if(projectService != null)
                {
                    projectService.ProjectSaving -= ProjectServiceProjectSaving;
                    projectService.ProjectSaved -= ProjectServiceProjectSaved;
                    projectService.ProjectSaveFailed -= ProjectServiceProjectSaveFailed;
                }

                projectService = value;

                if (projectService != null)
                {
                    projectService.ProjectSaving += ProjectServiceProjectSaving;
                    projectService.ProjectSaved += ProjectServiceProjectSaved;
                    projectService.ProjectSaveFailed += ProjectServiceProjectSaveFailed;
                }
            }
        }

        // TODO: migrate into ProjectService
        private void InitializeProjectRepositoryFactory()
        {
            //File.WriteAllText(@"d:\check.graphml", new PluginPersistencyGraphMlExporter().GetGraphML(Plugins));
            RegisterPersistentAssemblies(ProjectRepositoryFactory);
            
            // add data access listeners from plugins
            foreach (var plugin in Plugins)
            {
                var dataAccessListenersProvider = plugin as IDataAccessListenersProvider;
                if(dataAccessListenersProvider != null)
                {
                    foreach (var listener in dataAccessListenersProvider.CreateDataAccessListeners())
                    {
                        ProjectRepositoryFactory.AddDataAccessListener(listener);
                    }
                }
            }

            projectService.ProjectRepositoryFactory = ProjectRepositoryFactory;
        }

        private void RegisterDataTypes()
        {
            log.Debug(Properties.Resources.DeltaShellApplication_RegisterDataTypes_Registering_persistent_data_types____);

            foreach (var dataName in plugins.SelectMany(plugin => plugin.GetDataItemInfos().Select(dii => dii.Name)))
            {
                log.DebugFormat(Properties.Resources.DeltaShellApplication_RegisterDataTypes_Registering_data_type__0_, dataName);
            }
        }

        private void RegisterImporters()
        {
            log.Debug(Properties.Resources.DeltaShellApplication_RegisterImporters_Registering_importers____);

            foreach (var fileImporter in plugins.SelectMany(plugin => plugin.GetFileImporters()))
            {
                var projectImporter = fileImporter as IProjectImporter;
                if (projectImporter != null)
                {
                    projectImporter.ProjectService = ProjectService;
                }

                fileImporters.Add(fileImporter);

                log.DebugFormat(Properties.Resources.DeltaShellApplication_RegisterImporters_Registering_importer__0_, fileImporter.Name);
            }
        }

        private void RegisterExporters()
        {
            log.Debug(Properties.Resources.DeltaShellApplication_RegisterExporters_Registering_exporters____);

            foreach (var fileExporter in plugins.SelectMany(plugin => plugin.GetFileExporters()))
            {
                var projectExporter = fileExporter as IProjectItemExporter;
                if (projectExporter != null)
                {
                    projectExporter.ProjectService = ProjectService;
                }

                fileExporters.Add(fileExporter);

                log.DebugFormat(Properties.Resources.DeltaShellApplication_RegisterExporters_Registering_exporter__0_, fileExporter.Name);
            }
        }

        private static void RegisterPersistentAssemblies(IProjectRepositoryFactory projectRepositoryFactory)
        {
            log.Debug(Properties.Resources.DeltaShellApplication_RegisterPersistentAssemblies_Registering_assemblies_containing_persistent_data_types____);

            foreach (var plugin in PluginManager.GetPlugins<IPlugin>())
            {
                projectRepositoryFactory.AddPlugin(plugin);
            }
        }

        public string ProjectDataDirectory
        {
            get { return projectService.ProjectDataDirectory; }
        }

        public string ProjectFilePath
        {
            get { return projectService.ProjectRepository.Path; }
        }

        public IEnumerable<IFileImporter> FileImporters
        {
            get { return fileImporters; }
        }

        public IEnumerable<IFileExporter> FileExporters
        {
            get { return fileExporters; }
        }

        public ResourceManager Resources
        {
            get { return this.resources; }
            set { this.resources = value; }
        }

        public bool IsActivityRunning()
        {
            return this.ActivityRunner.IsRunning;
        }

        public void RunActivity(IActivity activity)
        {
            if (WaitMethod == null) //typically in console
            {
                DelftTools.Shell.Core.Workflow.ActivityRunner.RunActivity(activity); //run sync
                return;
            }

            RunActivityInBackground(activity);
            while(ActivityRunner.IsRunningActivity(activity))
            {
                WaitMethod();
            }
        }

        public void RunActivityInBackground(IActivity activity)
        {
            ActivityRunner.Enqueue(activity); 
        }

        public void StopActivity(IActivity activity)
        {
            ActivityRunner.Cancel(activity);
            //CurrentActivities.Abort(activity);
            activity.Cancel();
        }

        public bool IsActivityRunningOrWaiting(IActivity activity)
        {
            return ActivityRunner.Activities.Contains(activity);
        }

        private bool disposed;
        
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ActivityRunner.IsRunningChanged -= ActivityRunnerIsRunningChanged;

                    CloseProject();  
                    
                    projectBeingCreated = null;

                    RegularExpression.ClearExpressionsCache();

                    //make sure we close our repository avoiding memory leaks
                    if (projectService != null)
                    {
                        projectService.Dispose();
                        projectService = null;
                    }

                    foreach (var plugin in Plugins)
                    {
                        plugin.Deactivate();
                    }

                    foreach (var disposable in Plugins.OfType<IDisposable>())
                    {
                        disposable.Dispose();
                    }

                    plugins.Clear();

                    plugins = null;

                    PluginManager.Reset();

                    if (RunningActivityLogAppender.Instance != null)
                    {
                        RunningActivityLogAppender.Instance.ActivityRunner = null;
                        RunningActivityLogAppender.Instance = null;
                    }
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DeltaShellApplication()
        {
            Dispose(false);
        }
    }
}