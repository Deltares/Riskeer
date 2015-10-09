using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Dao;
using DelftTools.Shell.Core.Services;
using DelftTools.Utils.IO;
using DeltaShell.Core.Properties;
using log4net;

namespace DeltaShell.Core.Services
{
    /// <summary>
    /// Implemens all common operations with the Project in the IApplication
    /// 
    /// TODO: this is a stateful class, rename to HybridProjectRepository
    /// </summary>
    public class ProjectService : IProjectService
    {
        public event EventHandler<CancelEventArgs> ProjectSaving;
        public event EventHandler ProjectSaved;
        public event EventHandler ProjectSaveFailed;

        public event EventHandler<CancelEventArgs> ProjectOpening;
        public event EventHandler ProjectOpened;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProjectService));

        private IProjectRepositoryFactory projectRepositoryFactory;

        private IProjectRepository projectRepository;

        public ProjectService()
            : this(new ProjectRepositoryFactory<InMemoryProjectRepository>()) {}

        public ProjectService(IProjectRepositoryFactory projectRepositoryFactory)
        {
            this.projectRepositoryFactory = projectRepositoryFactory;
        }

        /// <summary>
        /// Gets the external data directory. 
        /// Note that if the directory is not found then it will be created...
        /// Maybe this need to be refactored because of the single responsibility principle
        /// </summary>
        public string ProjectExternalDataDirectory
        {
            get
            {
                return CreateAndGetExternalDataDirectory(ProjectDataDirectory);
            }
        }

        public IProjectRepositoryFactory ProjectRepositoryFactory
        {
            get
            {
                return projectRepositoryFactory;
            }
            set
            {
                projectRepositoryFactory = value;
                projectRepository = null;
            }
        }

        public IProjectRepository ProjectRepository
        {
            get
            {
                if (ProjectRepositoryFactory != null && projectRepository == null)
                {
                    // HACK: new repository should not be created here!!
                    ProjectRepository = projectRepositoryFactory.CreateNew();
                }

                return projectRepository;
            }
            private set
            {
                projectRepository = value;
            }
        }

        public string ProjectDataDirectory
        {
            get
            {
                return GetProjectDataDirectory(ProjectRepository.Path);
            }
        }

        /// <summary>
        /// Gets the external data directory. The directory will be created if not there because it should :)
        /// </summary>
        /// <param name="basePath">The base path to create the full path from</param>
        /// <returns>A full path to the external project data directory</returns>
        public string CreateAndGetExternalDataDirectory(string basePath)
        {
            var externalDataDirectory = Path.Combine(basePath, "external_data");
            FileUtils.CreateDirectoryIfNotExists(externalDataDirectory);
            return externalDataDirectory;
        }

        /// <summary>
        /// Creates a new project and saves it in a temporary folder
        /// </summary>
        /// <returns></returns>
        public Project CreateNewProjectInTemporaryFolder()
        {
            log.InfoFormat(Resources.ProjectService_CreateNewProjectInTemporaryFolder_Creating_new_empty_project_in_temporary_folder____);

            var path = Path.GetTempFileName();

            ProjectRepository.Create(path);

            log.InfoFormat(Resources.ProjectService_CreateNewProjectInTemporaryFolder_New_empty_project_created);

            var project = ProjectRepository.GetProject();

            FileUtils.CreateDirectoryIfNotExists(ProjectDataDirectory);

            project.IsTemporary = true;

            return project;
        }

        public void SaveProjectInTemporaryFolder(Project project)
        {
            var path = Path.GetTempFileName();

            SaveProjectAs(project, path, true);

            project.IsTemporary = true;
        }

        /// <summary>
        /// Creates a new project repository using provided the path
        /// </summary>
        /// <param name="path">Path to project repository, usually file with .dsproj extension.</param>
        /// <returns>A new project instance</returns>
        public Project Create(string path)
        {
            ProjectRepository.Create(path);

            FileUtils.DeleteIfExists(ProjectDataDirectory);

            var project = ProjectRepository.GetProject();

            CollectMemory();

            return project;
        }

        /// <summary>
        /// Opens an existing repository using provided path.
        /// </summary>
        /// <param name="path">Path to project repository, usually file with .dsproj extension.</param>
        public Project Open(string path)
        {
            log.InfoFormat(Resources.ProjectService_Open_Loading_project__0_____, path);

            if (ProjectOpening != null)
            {
                var args = new CancelEventArgs();
                ProjectOpening(path, args);
                if (args.Cancel)
                {
                    log.InfoFormat(Resources.ProjectService_Open_Stopped_loading_project__0_, path);
                    return null;
                }
            }

            if (FileUtils.PathIsRelative(path))
            {
                var currentDir = Directory.GetCurrentDirectory();
                path = Path.Combine(currentDir, path);
            }
            var project = ProjectRepository.Open(path);

            if (project != null)
            {
                FileUtils.CreateDirectoryIfNotExists(ProjectDataDirectory);
                CreateWorkingDirectories(project, ProjectDataDirectory);
            }

            CollectMemory();

            if (ProjectOpened != null)
            {
                ProjectOpened(project, null);
            }

            return project;
        }

        /// <summary>
        /// Save a project with all its task's and call savestate on the models
        /// </summary>
        /// <param name="project">the project to be saved</param>
        /// <returns></returns>
        public void Save(Project project)
        {
            var saveAction = new Action<Project>(p =>
            {
                // create model working directory
                var projectDataDirectory = GetProjectDataDirectory(ProjectRepository.Path);
                FileUtils.CreateDirectoryIfNotExists(projectDataDirectory);
                CreateWorkingDirectories(project, projectDataDirectory);

                ProjectRepository.SaveOrUpdate(project);
            });

            DoSaving(saveAction, project, ProjectRepository.Path);
        }

        public void Close(Project project)
        {
            if (ProjectRepository.IsOpen)
            {
                var path = ProjectRepository.Path;
                var dir = ProjectDataDirectory;

                if (project != null)
                {
                    foreach (var disposableItem in project.Items.OfType<IDisposable>())
                    {
                        disposableItem.Dispose();
                    }
                }

                ProjectRepository.Close();

                if (project != null && project.IsTemporary)
                {
                    DeleteProjectFiles(path, dir);
                }
            }

            CollectMemory();
        }

        /// <summary>
        /// Save a project with all its task's and call savestate on the models
        /// </summary>
        /// <param name="path">the path where the model is saved</param>
        /// <param name="project">the project to be saved</param>
        /// <returns></returns>
        public void SaveProjectAs(Project project, string path)
        {
            SaveProjectAs(project, path, false);
        }

        public void Dispose()
        {
            if (ProjectRepository != null)
            {
                if (ProjectRepository.IsOpen)
                {
                    Close(ProjectRepository.GetProject());
                }

                ProjectRepository.Dispose();
                ProjectRepository = null;
            }

            projectRepositoryFactory = null;
        }

        private static string GetProjectDataDirectory(string projectRepositoryPath)
        {
            return projectRepositoryPath != null ? projectRepositoryPath + "_data" : null;
        }

        /// <summary>
        /// Collect all possible memory after Save/Load operation (improve fragmentation)
        /// </summary>
        private void CollectMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers(); // wait until all collect operations are finished
        }

        /// <summary>
        /// Delete all projectfiles: the projectfile and the dir and files of the data directory( for example if project is temp)
        /// </summary>
        /// <param name="pathProject"></param>
        /// <param name="pathProjectData"></param>
        private static void DeleteProjectFiles(string pathProject, string pathProjectData)
        {
            FileUtils.DeleteIfExists(pathProjectData);

            if (File.Exists(pathProject))
            {
                File.Delete(pathProject);
            }
        }

        private void SaveProjectAs(Project project, string path, bool tempProject)
        {
            var oldProjectPath = ProjectRepository.Path;
            var oldProjectDataDirectory = ProjectDataDirectory;
            var oldProjectWasTemporary = project.IsTemporary;

            if (!string.IsNullOrEmpty(oldProjectPath) && !string.IsNullOrEmpty(path)
                && Path.GetFullPath(oldProjectPath) == Path.GetFullPath(path))
            {
                Save(project); // path did not change, just save
                return;
            }

            var saveAction = new Action<Project>(p =>
            {
                path = Path.GetFullPath(path);
                var newProjectDataDirectory = GetProjectDataDirectory(path);
                FileUtils.DeleteIfExists(newProjectDataDirectory);
                FileUtils.CreateDirectoryIfNotExists(newProjectDataDirectory);

                //create working directories for all models contained in the project
                CreateWorkingDirectories(p, newProjectDataDirectory);

                if ((ProjectRepository.IsOpen) && (p != ProjectRepository.GetProject()))
                {
                    ProjectRepository.Close();
                }

                ProjectRepository.SaveAs(p, path);

                if (oldProjectWasTemporary && !tempProject)
                {
                    try
                    {
                        DeleteProjectFiles(oldProjectPath, oldProjectDataDirectory);
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat(Resources.ProjectService_SaveProjectAs_Error_during_delete_of_old_project_file_and_directory__skipping____, e);
                    }
                }

                p.IsTemporary = false;
            });

            DoSaving(saveAction, project, path);
        }

        private void DoSaving(Action<Project> saveAction, Project project, string path)
        {
            if (!FireProjectSaving(project, path))
            {
                return;
            }

            try
            {
                saveAction(project);
            }
            catch (Exception)
            {
                FireProjectSaveFailed(project);
                throw;
            }

            FireProjectSaved(project);

            CollectMemory();
        }

        private void CreateWorkingDirectories(Project project, string projectDataDirectory)
        {
            if (FileUtils.PathIsRelative(projectDataDirectory))
            {
                throw new InvalidDataException(Resources.ProjectService_CreateWorkingDirectories_Parameter_projectDataDirectory_should_be_absolute_path);
            }
        }

        #region Event firing

        private bool FireProjectSaving(Project project, string path)
        {
            log.InfoFormat(Resources.ProjectService_FireProjectSaving_Saving_project__0__as__1_, project.Name, path);

            if (ProjectSaving != null)
            {
                var cancelEventArgs = new CancelEventArgs();
                ProjectSaving(project, cancelEventArgs);
                if (cancelEventArgs.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        private void FireProjectSaved(Project project)
        {
            if (ProjectSaved != null)
            {
                ProjectSaved(project, null);
            }

            log.InfoFormat(Resources.ProjectService_FireProjectSaved_Project__0__saved, project.Name);
        }

        private void FireProjectSaveFailed(Project project)
        {
            if (ProjectSaveFailed != null)
            {
                ProjectSaveFailed(project, null);
            }
        }

        #endregion
    }
}