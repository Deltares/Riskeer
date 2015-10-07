using System;
using System.ComponentModel;
using DelftTools.Shell.Core.Dao;

namespace DelftTools.Shell.Core.Services
{
    /// <summary>
    /// All common project manipulations in the Application
    /// </summary>
    public interface IProjectService:IDisposable
    {
        string ProjectDataDirectory { get; }

        string CreateAndGetExternalDataDirectory(string basePath); // TODO: remove this hack, what is external data directory?!?

        IProjectRepository ProjectRepository { get; }

        IProjectRepositoryFactory ProjectRepositoryFactory { get; set; }

        event EventHandler ProjectSaved;
        event EventHandler<CancelEventArgs> ProjectSaving;
        event EventHandler ProjectSaveFailed; 
        event EventHandler<CancelEventArgs> ProjectOpening;
        event EventHandler ProjectOpened;
        
        /// <summary>
        /// Creates new project.
        /// </summary>
        Project CreateNewProjectInTemporaryFolder();

        /// <summary>
        /// Saves project under new location.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="path"></param>
        void SaveProjectAs(Project project, string path);

        /// <summary>
        /// Create Project.
        /// </summary>
        /// <param name="path"></param>
        Project Create(string path);

        /// <summary>
        /// Opens project. Returns null if project has invalid version.
        /// </summary>
        /// <param name="path"></param>
        Project Open(string path);

        /// <summary>
        /// Saves project.
        /// </summary>
        /// <param name="project"></param>
        void Save(Project project);

        /// <summary>
        /// Close project (and release files).
        /// </summary>
        /// <param name="project"></param>
        void Close(Project project);

        void SaveProjectInTemporaryFolder(Project project);
    }
}   