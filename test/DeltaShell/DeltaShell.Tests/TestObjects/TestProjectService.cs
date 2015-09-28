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
    class TestProjectService : IProjectService
    {
        private int closeCallCount;
        public IProjectRepositoryFactory ProjectRepositoryFactory { get; set; }

        public string ProjectDataDirectory
        {
            get { throw new NotImplementedException(); }
        }

        public string CreateAndGetExternalDataDirectory(string basePath)
        {
            throw new NotImplementedException();
        }

        public IProjectRepository ProjectRepository
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler ProjectSaved;
        public int CloseCallCount
        {
            get { return closeCallCount; }
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
            closeCallCount++;
        }

        public void Export(IProjectItem projectItem, string targetProjectRepositoryPath, bool includeLinkedFiles, bool ClearModelOutputsOnExport = false)
        {
            throw new NotImplementedException();
        }

        public IProjectService Clone()
        {
            throw new NotImplementedException();
        }

        public void SaveProjectInTemporaryFolder(Project project)
        {
            throw new NotImplementedException();
        }

        #region IProjectService Members


        public event EventHandler<CancelEventArgs> ProjectSaving;
        public event EventHandler ProjectSaveFailed;

        public Project CreateNewProjectInTemporaryFolder()
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Dispose()
        {
            
        }

        public event EventHandler<CancelEventArgs> ProjectOpening;

        public event EventHandler ProjectOpened;
    }
}