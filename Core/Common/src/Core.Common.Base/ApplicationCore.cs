using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
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
    public class ApplicationCore : IDisposable
    {
        // TODO: migrate into ProjectService
        public event Action<Project> ProjectOpening;
        public event Action<Project> ProjectOpened;
        public event Action<Project> ProjectClosing;
        public event Action<Project> ProjectSaving;
        public event Action<Project> ProjectSaveFailed;
        public event Action<Project> ProjectSaved;

        private readonly ILog log = LogManager.GetLogger(typeof(ApplicationCore));

        public Action WaitMethod;

        private Project project;
        private ApplicationCoreSettings userSettings;

        private bool isRunning;

        private bool running;

        private Project projectBeingCreated;
        private bool initializing;

        private bool disposed;

        public ApplicationCore()
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

        public IList<ApplicationPlugin> Plugins { get; private set; }

        public IActivityRunner ActivityRunner { get; set; }

        public ApplicationSettingsBase UserSettings
        {
            get
            {
                return userSettings;
            }
            set
            {
                userSettings = new ApplicationCoreSettings(value); // small hack, wrap settings so that we will know when they are changed.
            }
        }

        public NameValueCollection Settings { get; set; }

        public string ProjectFilePath{ get; private set; }

        public IEnumerable<IFileImporter> FileImporters
        {
            get
            {
                return Plugins.SelectMany(plugin => plugin.GetFileImporters());
            }
        }

        public IEnumerable<IFileExporter> FileExporters
        {
            get
            {
                return Plugins.SelectMany(plugin => plugin.GetFileExporters());
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
                throw new InvalidOperationException(Properties.Resources.ApplicationCore_Run_Application_is_already_running);
            }

            initializing = true;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            log.Info(Properties.Resources.ApplicationCore_Run_starting);

            if (running)
            {
                throw new InvalidOperationException(Properties.Resources.ApplicationCore_Run_Application_is_already_running);
            }

            running = true;

            // load all assemblies from current assembly directory
            AssemblyUtils.LoadAllAssembliesFromDirectory(Path.GetFullPath(Path.GetDirectoryName(GetType().Assembly.Location))).ToList();

            LogSystemInfo();

            Plugins.ForEach(p => p.ApplicationCore = this);

            isRunning = true;

            log.Info(Properties.Resources.ApplicationCore_Run_Creating_new_project);
            CreateNewProject();

            log.Info(Properties.Resources.ApplicationCore_Run_Activating_plugins);
            ActivatePlugins();

            log.Info(Properties.Resources.ApplicationCore_Run_Waiting_until_all_plugins_are_activated);

            Project = projectBeingCreated; // opens project in application

            initializing = false;

            stopwatch.Stop();

            log.InfoFormat(Properties.Resources.ApplicationCore_Run_Ringtoets_is_ready_started_in_0_F3_seconds, stopwatch.ElapsedMilliseconds/1000.0);
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
                throw new InvalidOperationException(Properties.Resources.ApplicationCore_CreateNewProject_Run_must_be_called_first_before_project_can_be_opened);
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
                throw new InvalidOperationException(Properties.Resources.ApplicationCore_CreateNewProject_Run_must_be_called_first_before_project_can_be_opened);
            }

            // TODO: implement and remove Project = new Project();
            Project = new Project();
            return false;
        }

        public void Exit()
        {
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
            log.Debug(Properties.Resources.ApplicationCore_Run_Activating_plugins);

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

            log.Debug(Properties.Resources.ApplicationCore_ActivatePlugins_All_plugins_were_activated);
        }

        private static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            //HACK : this is needed because of issue 4382...the black boxes in PG. It seem like the assembly for 
            //enum types like AggregationOptions cannot be found without this 
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.FullName == args.Name);
        }

        private void LogSystemInfo()
        {
            log.DebugFormat(Properties.Resources.ApplicationCore_LogSystemInfo_Environmental_variables_);

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

        ~ApplicationCore()
        {
            Dispose(false);
        }
    }
}