using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Resources;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Dao;
using DelftTools.Shell.Core.Services;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Utils.Threading;

namespace DeltaShell.Tests.TestObjects
{
    class TestApplication: IApplication
    {
        int disposeCallCount;

        public IList<ApplicationPlugin> Plugins { get; set; }
        public void LoadPluginsFromPath(string path)
        {
            throw new NotImplementedException();
        }

        public IList<string> DisabledPlugins
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ApplicationPlugin GetPluginForType(Type type)
        {
            throw new NotImplementedException();
        }

        public Project Project { get; set; }
        public IProjectRepository ProjectRepository { get; private set; }

        public IProjectRepositoryFactory ProjectRepositoryFactory
        {
            get { throw new NotImplementedException(); }    
            set { }
        }

        public event Action<Project> ProjectOpening;
        public event Action<Project> ProjectOpened;
        public event Action<Project> ProjectClosing;
        public event Action<Project> ProjectSaving;
        public event Action<Project> ProjectSaveFailed;
        public event Action<Project> ProjectSaved;
        public NotifyingThreadQueue<IActivity> CurrentActivities { get; private set; }
        public IActivityRunner ActivityRunner
        {
            get { return new ActivityRunner();}
        }

        public bool IsActivityRunning()
        {
            throw new NotImplementedException();
        }

        public NameValueCollection Settings { get; set; }
        public ApplicationSettingsBase UserSettings { get; set; }
        public string GetUserSettingsDirectoryPath()
        {
            throw new NotImplementedException();
        }

        public ResourceManager Resources { get; set; }

        public bool IsProjectCreatedInTemporaryDirectory { get; set; }

        public void Run()
        {
            throw new NotImplementedException();
        }

        public void Run(string projectPath)
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public IProjectService ProjectService { get; set; }

        public IEnumerable<IFileImporter> FileImporters
        {
            get { yield break; }
        }

        public IEnumerable<IFileExporter> FileExporters
        {
            get { yield break; }
        }

        public void RunActivity(IActivity activity)
        {
            throw new NotImplementedException();
        }

        public void RunActivityInBackground(IActivity activity)
        {
            throw new NotImplementedException();
        }

        public bool IsActivityRunningOrWaiting(IActivity activity)
        {
            throw new NotImplementedException();
        }

        public void StopActivity(IActivity activity)
        {
            throw new NotImplementedException();
        }

        public void CreateNewProject()
        {
            Project = new Project();
        }

        public bool OpenProject(string path)
        {
            throw new NotImplementedException();
        }

        public void CloseProject()
        {
            Project = null;
        }

        public void SaveProjectAs(string path)
        {
            throw new NotImplementedException();
        }

        public void SaveProject()
        {
            throw new NotImplementedException();
        }

        public void InitializeProjectRepositoryFactory(IProjectRepositoryFactory factory)
        {
            throw new NotImplementedException();
        }

        public string ProjectDataDirectory
        {
            get { throw new NotImplementedException(); }
        }

        public string ProjectFilePath
        {
            get { throw new NotImplementedException(); }
        }

        public void ExportProjectItem(IProjectItem projectItem, string targetProjectRepositoryPath, bool includeLinkedFiles)
        {
            throw new NotImplementedException();
        }

        public string GetNewFileBasedItemPath(string fileNamePrefix, string fileNameSuffix)
        {
            throw new NotImplementedException();
        }

        public string Version
        {
            get { return ""; }
        }

        public string PluginVersions { get; private set; }

        public string ApplicationNameAndVersion
        {
            get {return "DeltaShell"; }
        }

        public void Dispose()
        {
            
            disposeCallCount++;
        }

        public int DisposeCallCount
        {
            get { return disposeCallCount; }
        }

        public event Action AfterRun;
        public bool IsDataAccessSynchronizationDisabled { get; set; }
    }
}