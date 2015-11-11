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
using Core.Common.Base.Workflow;
using Core.Common.Utils;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Globalization;
using Core.Common.Utils.Reflection;
using log4net;

namespace Core.Common.Base
{
    public class RingtoetsApplication : IApplication
    {
        public event Action AfterRun;

        // TODO: migrate into ProjectService
        public event Action<Project> ProjectOpening;
        public event Action<Project> ProjectOpened;
        public event Action<Project> ProjectClosing;
        public event Action<Project> ProjectSaving;
        public event Action<Project> ProjectSaveFailed;
        public event Action<Project> ProjectSaved;

        private static readonly ILog log = LogManager.GetLogger(typeof(RingtoetsApplication));
        private readonly IList<IFileImporter> fileImporters = new List<IFileImporter>();
        private readonly IList<IFileExporter> fileExporters = new List<IFileExporter>();

        public Action WaitMethod;

        private Project project;
        private RingtoetsApplicationSettings userSettings;

        private bool isRunning;

        private bool running;

        private Project projectBeingCreated;
        private bool initializing;

        private bool disposed;

        public RingtoetsApplication()
        {
            Settings = ConfigurationManager.AppSettings;
            UserSettings = Properties.Settings.Default;

            Plugins = new List<ApplicationPlugin>();

            ActivityRunner = new ActivityRunner();

            if (RunningActivityLogAppender.Instance != null)
            {
                RunningActivityLogAppender.Instance.ActivityRunner = ActivityRunner;
            }
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;

            Settings = ConfigurationManager.AppSettings;
            UserSettings = Properties.Settings.Default;
        }

        public bool IsProjectCreatedInTemporaryDirectory { get; set; }

        public Project Project
        {
            get
            {
                return project;
            }

            [InvokeRequired]
            set
            {
                if (project != null)
                {
                    if (ProjectClosing != null)
                    {
                        ProjectClosing(project);
                    }
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

        public IList<ApplicationPlugin> Plugins { get; set; }

        public IActivityRunner ActivityRunner { get; set; }

        public ApplicationSettingsBase UserSettings
        {
            get
            {
                return userSettings;
            }
            set
            {
                userSettings = new RingtoetsApplicationSettings(value); // small hack, wrap settings so that we will know when they are changed.
            }
        }

        public NameValueCollection Settings { get; set; }

        public string ProjectFilePath{ get; private set; }

        public IEnumerable<IFileImporter> FileImporters
        {
            get
            {
                return fileImporters;
            }
        }

        public IEnumerable<IFileExporter> FileExporters
        {
            get
            {
                return fileExporters;
            }
        }

        public ResourceManager Resources { get; set; }

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
                    RegionalSettingsManager.RealNumberFormat = (string) realNumberFormat;
                }
            }

            var culture = Thread.CurrentThread.CurrentCulture.ToString();
            if (culture == "tr-TR" || culture == "az")
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            }
        }

        public string GetUserSettingsDirectoryPath()
        {
            return SettingsHelper.GetApplicationLocalUserSettingsDirectory();
        }

        public void Run(string projectPath)
        {
            Run();
            OpenProject(projectPath);
        }

        public void Run()
        {
            if (isRunning)
            {
                throw new InvalidOperationException(Properties.Resources.DeltaShellApplication_Run_You_can_call_Run___only_once_per_application);
            }

            initializing = true;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            log.Info(Properties.Resources.DeltaShellApplication_Run_Starting_Delta_Shell____);

            if (running)
            {
                throw new InvalidOperationException(Properties.Resources.DeltaShellApplication_Run_Application_is_already_running);
            }

            running = true;

            // load all assemblies from current assembly directory
            AssemblyUtils.LoadAllAssembliesFromDirectory(Path.GetFullPath(Path.GetDirectoryName(GetType().Assembly.Location))).ToList();

            SetLanguageAndRegionalSettions();

            //Disabled trace logging this causes focus bugs combined with avalon dock (KeyPreview debug messages)
            //InitializeLogging();

            LogSystemInfo();

            InitializeLicense();

            Plugins.ForEach(p => p.Application = this);

            isRunning = true;

            log.Info(Properties.Resources.DeltaShellApplication_Run_Creating_new_project____);
            CreateNewProject();

            log.Info(Properties.Resources.DeltaShellApplication_Run_Activating_plugins____);
            ActivatePlugins();

            RegisterDataTypes();
            RegisterImporters();
            RegisterExporters();

            log.Info(Properties.Resources.DeltaShellApplication_Run_Waiting_until_all_plugins_are_activated____);

            Project = projectBeingCreated; // opens project in application

            initializing = false;

            stopwatch.Stop();

            log.InfoFormat(Properties.Resources.DeltaShellApplication_Run_Delta_Shell_is_ready__started_in__0_F3__sec_, stopwatch.ElapsedMilliseconds/1000.0);

            if (AfterRun != null)
            {
                AfterRun();
            }
        }

        public void CloseProject()
        {
            Project = null;
        }

        public void SaveProjectAs(string path)
        {
            // TODO: implement
        }

        public void SaveProject()
        {
            // TODO: implement
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
        }

        public bool OpenProject(string path)
        {
            if (!isRunning)
            {
                throw new InvalidOperationException(Properties.Resources.DeltaShellApplication_CreateNewProject_Run___must_be_called_first_before_project_can_be_opened);
            }

            // TODO: implement and remove Project = new Project();
            Project = new Project();
            return false;
        }

        public void Exit()
        {
            Trace.Listeners.Clear();

            if (Project != null)
            {
                CloseProject();
            }

            if (userSettings.IsDirty)
            {
                UserSettings.Save();
            }
        }

        public bool IsActivityRunning()
        {
            return ActivityRunner.IsRunning;
        }

        public void RunActivity(IActivity activity)
        {
            if (WaitMethod == null) //typically in console
            {
                Workflow.ActivityRunner.RunActivity(activity); //run sync
                return;
            }

            RunActivityInBackground(activity);
            while (ActivityRunner.IsRunningActivity(activity))
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ActivatePlugins()
        {
            log.Debug(Properties.Resources.DeltaShellApplication_Run_Activating_plugins____);

            // Activate all plugins
            foreach (var plugin in Plugins)
            {
                string cwdOld = ".";

                var assembly = plugin.GetType().Assembly;
                if (!assembly.IsDynamic())
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
        /// Initialize the log4net part
        /// </summary>
        protected static void InitializeLogging()
        {
            if (!Trace.Listeners.Cast<TraceListener>().Any(tl => tl is RingtoetsTraceListener))
            {
                Trace.Listeners.Add(new RingtoetsTraceListener());
            }
        }

        private static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            //HACK : this is needed because of issue 4382...the black boxes in PG. It seem like the assembly for 
            //enum types like AggregationOptions cannot be found without this 
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.FullName == args.Name);
        }

        private static void LogSystemInfo()
        {
            log.DebugFormat(Properties.Resources.DeltaShellApplication_LogSystemInfo_Environmental_variables_);

            var culture = Thread.CurrentThread.CurrentCulture;
            log.DebugFormat("{0} = {1}", "CURRENT_THREAD_CULTURE", culture.EnglishName);
            log.DebugFormat("{0} = {1}", "NUMBER_DECIMAL_DIGITS", culture.NumberFormat.NumberDecimalDigits);
            log.DebugFormat("{0} = {1}", "NUMBER_DECIMAL_SEPARATOR", culture.NumberFormat.NumberDecimalSeparator);
            log.DebugFormat("{0} = {1}", "FULL_DATE_TIME_PATTERN", culture.DateTimeFormat.FullDateTimePattern);
            log.DebugFormat("{0} = {1}", "DATE_SEPARATOR", culture.DateTimeFormat.DateSeparator);
            log.DebugFormat("{0} = {1}", "OS_VERSION", Environment.OSVersion);
            log.DebugFormat("{0} = {1}", "OS_VERSION_NUMBER", Environment.Version);

            foreach (DictionaryEntry pair in Environment.GetEnvironmentVariables())
            {
                log.DebugFormat("{0} = {1}", pair.Key, pair.Value);
            }
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

            if (string.IsNullOrEmpty(licenseFilePath))
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

        private void RegisterDataTypes()
        {
            log.Debug(Properties.Resources.DeltaShellApplication_RegisterDataTypes_Registering_persistent_data_types____);

            foreach (var dataName in Plugins.SelectMany(plugin => plugin.GetDataItemInfos().Select(dii => dii.Name)))
            {
                log.DebugFormat(Properties.Resources.DeltaShellApplication_RegisterDataTypes_Registering_data_type__0_, dataName);
            }
        }

        private void RegisterImporters()
        {
            log.Debug(Properties.Resources.DeltaShellApplication_RegisterImporters_Registering_importers____);

            foreach (var fileImporter in Plugins.SelectMany(plugin => plugin.GetFileImporters()))
            {
                var projectImporter = fileImporter as IProjectImporter;
                if (projectImporter != null)
                {
                    // TODO: implement
                    // projectImporter.ProjectService = ProjectService;
                }

                fileImporters.Add(fileImporter);

                log.DebugFormat(Properties.Resources.DeltaShellApplication_RegisterImporters_Registering_importer__0_, fileImporter.Name);
            }
        }

        private void RegisterExporters()
        {
            log.Debug(Properties.Resources.DeltaShellApplication_RegisterExporters_Registering_exporters____);

            foreach (var fileExporter in Plugins.SelectMany(plugin => plugin.GetFileExporters()))
            {
                var projectExporter = fileExporter as IProjectItemExporter;
                if (projectExporter != null)
                {
                    // TODO: implement
                    // projectExporter.ProjectService = ProjectService;
                }

                fileExporters.Add(fileExporter);

                log.DebugFormat(Properties.Resources.DeltaShellApplication_RegisterExporters_Registering_exporter__0_, fileExporter.Name);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    CloseProject();

                    projectBeingCreated = null;

                    foreach (var plugin in Plugins)
                    {
                        plugin.Deactivate();
                    }

                    foreach (var disposable in Plugins.OfType<IDisposable>())
                    {
                        disposable.Dispose();
                    }

                    Plugins.Clear();

                    Plugins = null;

                    if (RunningActivityLogAppender.Instance != null)
                    {
                        RunningActivityLogAppender.Instance.ActivityRunner = null;
                        RunningActivityLogAppender.Instance = null;
                    }
                }
            }
            disposed = true;
        }

        ~RingtoetsApplication()
        {
            Dispose(false);
        }

        #region Nested type: RingtoetsTraceListener

        internal class RingtoetsTraceListener : TraceListener
        {
            private static ILog log;
            private static bool logTraceMessages = true;

            public RingtoetsTraceListener()
            {
                log = LogManager.GetLogger(GetType());
            }

            public static bool LogTraceMessages
            {
                get
                {
                    return logTraceMessages;
                }
                set
                {
                    logTraceMessages = value;
                }
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
    }
}