using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Core.Common.Base.Workflow;
using Core.Common.Utils;
using Core.Common.Utils.Aop;
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

        private readonly List<ApplicationPlugin> plugins;

        public Action WaitMethod;

        private Project project;
        private ApplicationCoreSettings userSettings;

        private bool isRunning;

        private bool disposed;

        public ApplicationCore()
        {
            plugins = new List<ApplicationPlugin>();

            Settings = ConfigurationManager.AppSettings;
            UserSettings = Properties.Settings.Default;

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

        public IEnumerable<ApplicationPlugin> Plugins 
        {
            get
            {
                return plugins;
            }
        }

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

        public void AddPlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Add(applicationPlugin);

            applicationPlugin.ApplicationCore = this;

            applicationPlugin.Activate();
        }

        public void RemovePlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Remove(applicationPlugin);

            applicationPlugin.ApplicationCore = null;

            applicationPlugin.Deactivate();
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

            isRunning = true;

            log.Info(Properties.Resources.ApplicationCore_Run_starting);

            // load all assemblies from current assembly directory
            AssemblyUtils.LoadAllAssembliesFromDirectory(Path.GetFullPath(Path.GetDirectoryName(GetType().Assembly.Location))).ToList();

            log.Info(Properties.Resources.ApplicationCore_Run_Creating_new_project);
            Project = new Project();
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

        private static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            //HACK : this is needed because of issue 4382...the black boxes in PG. It seem like the assembly for 
            //enum types like AggregationOptions cannot be found without this 
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.FullName == args.Name);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    CloseProject();

                    foreach (var plugin in Plugins.ToList())
                    {
                        RemovePlugin(plugin);
                    }

                    foreach (var disposable in Plugins.OfType<IDisposable>())
                    {
                        disposable.Dispose();
                    }

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