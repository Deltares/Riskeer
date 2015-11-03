using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Resources;
using Core.Common.Base;
using Core.Common.Base.Workflow;

namespace Core.Common.Tests.TestObjects
{
    internal class TestApplication : IApplication
    {
        // Required by interface, but not used (yet)
#pragma warning disable 67
        public event Action<Project> ProjectOpening;

        public event Action<Project> ProjectOpened;
        public event Action<Project> ProjectClosing;
        public event Action<Project> ProjectSaving;
        public event Action<Project> ProjectSaveFailed;
        public event Action<Project> ProjectSaved;

        public event Action AfterRun;
#pragma warning restore 67

        public int DisposeCallCount { get; private set; }

        public IList<ApplicationPlugin> Plugins { get; set; }

        public Project Project { get; set; }

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

        public string PluginVersions { get; private set; }

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