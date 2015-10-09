using System;
using System.ComponentModel;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Dao;
using DelftTools.Shell.Core.Services;

namespace DeltaShell.Tests.TestObjects
{
    /// <summary>
    /// 'Hand rolled' ;) mock for projectservice. Replace by mocks if easy.
    /// </summary>
    internal class TestProjectService : IProjectService
    {
        public event EventHandler ProjectSaved;

        public event EventHandler<CancelEventArgs> ProjectOpening;

        public event EventHandler ProjectOpened;

        public int CloseCallCount { get; private set; }

        public IProjectRepositoryFactory ProjectRepositoryFactory { get; set; }

        public string ProjectDataDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IProjectRepository ProjectRepository
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Export(IProjectItem projectItem, string targetProjectRepositoryPath, bool includeLinkedFiles, bool ClearModelOutputsOnExport = false)
        {
            throw new NotImplementedException();
        }

        public IProjectService Clone()
        {
            throw new NotImplementedException();
        }

        public string CreateAndGetExternalDataDirectory(string basePath)
        {
            throw new NotImplementedException();
        }

        public void SaveProjectAs(Project project, string path)
        {
            throw new NotImplementedException();
        }

        public Project Create(string path)
        {
            throw new NotImplementedException();
        }

        public Project Open(string path)
        {
            throw new NotImplementedException();
        }

        public void Save(Project project)
        {
            throw new NotImplementedException();
        }

        public void Close(Project project)
        {
            CloseCallCount++;
        }

        public void SaveProjectInTemporaryFolder(Project project)
        {
            throw new NotImplementedException();
        }

        public void Dispose() {}

        #region IProjectService Members

        public event EventHandler<CancelEventArgs> ProjectSaving;
        public event EventHandler ProjectSaveFailed;

        public Project CreateNewProjectInTemporaryFolder()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}