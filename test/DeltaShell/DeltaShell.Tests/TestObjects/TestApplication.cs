using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Resources;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Dao;
using DelftTools.Shell.Core.Services;
using DelftTools.Shell.Core.Workflow;

namespace DeltaShell.Tests.TestObjects
{
    internal class TestApplication : IApplication
    {
        public event Action<Project> ProjectOpening;
        public event Action<Project> ProjectOpened;
        public event Action<Project> ProjectClosing;
        public event Action<Project> ProjectSaving;
        public event Action<Project> ProjectSaveFailed;
        public event Action<Project> ProjectSaved;

        public event Action AfterRun;

        public int DisposeCallCount { get; private set; }

        public IList<ApplicationPlugin> Plugins { get; set; }

        public Project Project { get; set; }

        public IProjectRepositoryFactory ProjectRepositoryFactory
        {
            get
            {
                throw new NotImplementedException();
            }
            set {}
        }

        public IActivityRunner ActivityRunner
        {
            get
            {
                return new ActivityRunner();
            }
        }

        public NameValueCollection Settings { get; set; }
        public ApplicationSettingsBase UserSettings { get; set; }

        public ResourceManager Resources { get; set; }

        public IProjectService ProjectService { get; set; }

        public IEnumerable<IFileImporter> FileImporters
        {
            get
            {
                yield break;
            }
        }

        public IEnumerable<IFileExporter> FileExporters
        {
            get
            {
                yield break;
            }
        }

        public string ProjectDataDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ProjectFilePath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Version
        {
            get
            {
                return "";
            }
        }

        public string PluginVersions { get; private set; }
        public bool IsDataAccessSynchronizationDisabled { get; set; }

        public ApplicationPlugin GetPluginForType(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsActivityRunning()
        {
            throw new NotImplementedException();
        }

        public string GetUserSettingsDirectoryPath()
        {
            throw new NotImplementedException();
        }

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

        public void Dispose()
        {
            DisposeCallCount++;
        }
    }
}