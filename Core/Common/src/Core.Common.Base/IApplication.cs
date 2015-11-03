using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Resources;
using Core.Common.Base.Workflow;

namespace Core.Common.Base
{
    /// <summary>
    /// Defines core functionality of any application which uses modelling framework.
    /// Application can be implemented as a gui, web, console application.
    /// </summary>
    public interface IApplication : IDisposable
    {
        event Action<Project> ProjectOpening;

        event Action<Project> ProjectOpened;

        event Action<Project> ProjectClosing;

        event Action<Project> ProjectSaving;

        event Action<Project> ProjectSaved;

        event Action<Project> ProjectSaveFailed;

        /// <summary>
        /// Fired after application has been started.
        /// </summary>
        event Action AfterRun;

        /// <summary>
        /// Gets or sets collection of plugins.
        /// </summary>
        IList<ApplicationPlugin> Plugins { get; }

        /// <summary>
        /// The current application working project.
        /// </summary>
        Project Project { get; }

        IActivityRunner ActivityRunner { get; }

        // HACK: make queue check this

        /// <summary>
        /// Gets Application settings.
        /// Application settings are usually located in the app.config file of the main executable.
        /// </summary>
        /// <seealso href="http://www.codeproject.com/useritems/SystemConfiguration.asp"/>
        NameValueCollection Settings { get; set; }

        /// <summary>
        /// Gets User settings.
        /// User settings will be automatically saved in the user home folder.
        /// </summary>
        ApplicationSettingsBase UserSettings { get; set; }

        /// <summary>
        /// TODO: should be custom providing access to text string, icons from Application.
        /// </summary>
        ResourceManager Resources { get; set; }

        IEnumerable<IFileImporter> FileImporters { get; }

        IEnumerable<IFileExporter> FileExporters { get; }

        // TODO: hide it?
        string ProjectFilePath { get; }

        string PluginVersions { get; }

        ApplicationPlugin GetPluginForType(Type type);

        /// <summary>
        /// Check if any activity is running.
        /// </summary>
        /// <returns>true if one or more runnables are running</returns>
        bool IsActivityRunning();

        /// <summary>
        /// Returns path to the user settings directory for the current application.
        /// </summary>
        /// <returns></returns>
        string GetUserSettingsDirectoryPath();

        /// <summary>
        /// Runs application.
        /// </summary>
        void Run();

        /// <summary>
        /// Runs application and open project.
        /// </summary>
        void Run(string projectPath);

        /// <summary>
        /// Exits application.
        /// </summary>
        void Exit();

        /// <summary>
        /// Runs given activity synchronously.
        /// </summary>
        /// <param name="activity"></param>
        void RunActivity(IActivity activity);

        ///<summary>
        /// Adds the activity to the CurrentActivities, runs it 
        ///</summary>
        ///<param name="activity"> </param>
        void RunActivityInBackground(IActivity activity);

        /// <summary>
        /// Is activity running or queued in the CurrentActivities
        /// </summary>
        /// <param name="activity"> </param>
        /// <returns></returns>
        bool IsActivityRunningOrWaiting(IActivity activity);

        /// <summary>
        /// Cancels the running activity (stop or remove from CurrentActivities).
        /// </summary>
        /// <param name="activity"></param>
        void StopActivity(IActivity activity);

        void CreateNewProject();

        bool OpenProject(string path);

        void CloseProject();

        void SaveProjectAs(string path);

        void SaveProject();
    }
}