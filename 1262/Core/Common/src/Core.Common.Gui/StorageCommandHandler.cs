using System;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.Views;
using Core.Common.Gui.Properties;
using log4net;

namespace Core.Common.Gui
{
    /// <summary>
    /// Class responsible for persistency of <see cref="Project"/>.
    /// </summary>
    public class StorageCommandHandler : IStorageCommands, IObserver
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StorageCommandHandler));

        private readonly IViewCommands viewCommands;
        private readonly IMainWindowController mainWindowController;
        private readonly IProjectOwner projectOwner;
        private readonly IStoreProject projectPersistor;
        private readonly IApplicationSelection applicationSelection;
        private readonly IToolViewController toolViewController;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCommandHandler"/> class.
        /// </summary>
        /// <param name="projectStorage">Class responsible to storing and loading the application project.</param>
        /// <param name="projectOwner">The class owning the application project.</param>
        /// <param name="applicationSelection">Class managing the application selection.</param>
        /// <param name="mainWindowController">Controller for UI.</param>
        /// <param name="toolViewController">Controller for Tool Windows.</param>
        /// <param name="viewCommands">The view command handler.</param>
        public StorageCommandHandler(IStoreProject projectStorage, IProjectOwner projectOwner,
                                     IApplicationSelection applicationSelection, IMainWindowController mainWindowController,
                                     IToolViewController toolViewController, IViewCommands viewCommands)
        {
            this.viewCommands = viewCommands;
            this.mainWindowController = mainWindowController;
            this.projectOwner = projectOwner;
            projectPersistor = projectStorage;
            this.applicationSelection = applicationSelection;
            this.toolViewController = toolViewController;

            this.projectOwner.ProjectOpened += ApplicationProjectOpened;
            this.projectOwner.ProjectClosing += ApplicationProjectClosing;
        }

        public void UpdateObserver()
        {
            mainWindowController.RefreshGui();
        }

        /// <summary>
        /// Closes the current <see cref="Project"/> and creates a new (empty) <see cref="Project"/>.
        /// </summary>
        public void CreateNewProject()
        {
            CloseProject();

            log.Info(Resources.Project_new_opening);
            projectOwner.Project = new Project();
            projectOwner.ProjectFilePath = "";
            log.Info(Resources.Project_new_successfully_opened);

            mainWindowController.RefreshGui();
        }

        /// <summary>
        /// Opens a new <see cref="OpenFileDialog"/> where a file can be selected to open.
        /// </summary>
        /// <returns><c>true</c> if an existing <see cref="Project"/> has been loaded, <c>false</c> otherwise.</returns>
        public bool OpenExistingProject()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = Resources.Ringtoets_project_file_filter,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog(mainWindowController.MainWindow) != DialogResult.Cancel)
            {
                return OpenExistingProject(openFileDialog.FileName);
            }
            log.Warn(Resources.Project_existing_project_opening_cancelled);
            return false;
        }

        /// <summary>
        /// Loads a <see cref="Project"/>, based upon <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Location of the storage file.</param>
        /// <returns><c>true</c> if an existing <see cref="Project"/> has been loaded, <c>false</c> otherwise.</returns>
        public bool OpenExistingProject(string filePath)
        {
            log.Info(Resources.Project_existing_opening_project);

            Project loadedProject;
            try
            {
                loadedProject = projectPersistor.LoadProject(filePath);
            }
            catch (StorageException e)
            {
                log.Warn(e.Message, e.InnerException);
                log.Warn(Resources.Project_existing_project_opening_failed);
                return false;
            }

            if (loadedProject == null)
            {
                log.Warn(Resources.Project_existing_project_opening_failed);
                return false;
            }

            // Project loaded successfully, close current project
            CloseProject();

            projectOwner.ProjectFilePath = filePath;
            projectOwner.Project = loadedProject;
            projectOwner.Project.NotifyObservers();
            mainWindowController.RefreshGui();
            log.Info(Resources.Project_existing_successfully_opened);
            return true;
        }

        /// <summary>
        /// Close current project (if any) and related views.
        /// </summary>
        public void CloseProject()
        {
            if (projectOwner.Project == null)
            {
                return;
            }

            // remove views before closing project. 
            viewCommands.RemoveAllViewsForItem(projectOwner.Project);

            projectOwner.Project = null;
            projectOwner.ProjectFilePath = "";
            applicationSelection.Selection = null;

            mainWindowController.RefreshGui();
        }

        /// <summary>
        /// Saves the current <see cref="Project"/> to the selected storage file.
        /// </summary>
        /// <returns>Returns <c>true</c> if the save was successful, <c>false</c> otherwise.</returns>
        public bool SaveProjectAs()
        {
            var project = projectOwner.Project;
            if (project == null)
            {
                return false;
            }

            var filePath = OpenRingtoetsProjectFileSaveDialog(project.Name);
            if (String.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            if (!TrySaveProjectAs(projectPersistor, filePath))
            {
                return false;
            }

            // Save was successful, store location
            projectOwner.ProjectFilePath = filePath;
            project.Name = Path.GetFileNameWithoutExtension(filePath);
            project.NotifyObservers();
            mainWindowController.RefreshGui();
            log.Info(String.Format(Resources.Project_saving_project_saved_0, project.Name));
            return true;
        }

        /// <summary>
        /// Saves the current <see cref="Project"/> to the defined storage file.
        /// </summary>
        /// <returns>Returns if the save was successful.</returns>
        public bool SaveProject()
        {
            var project = projectOwner.Project;
            if (project == null)
            {
                return false;
            }
            var filePath = projectOwner.ProjectFilePath;

            // If filepath is not set, go to SaveAs
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return SaveProjectAs();
            }

            if (!TrySaveProject(projectPersistor, filePath))
            {
                return false;
            }

            log.Info(String.Format(Resources.Project_saving_project_saved_0, project.Name));
            return true;
        }

        public void Dispose()
        {
            projectOwner.ProjectOpened -= ApplicationProjectOpened;
            projectOwner.ProjectClosing -= ApplicationProjectClosing;
        }

        /// <summary>
        /// Prompts a new <see cref="SaveFileDialog"/> to select a location for saving the Ringtoets project file.
        /// </summary>
        /// <param name="projectName">A string containing the file name selected in the file dialog box.</param>
        /// <returns>The selected project file, or <c>null</c> otherwise.</returns>
        private static string OpenRingtoetsProjectFileSaveDialog(string projectName)
        {
            // show file open dialog and select project file
            var saveFileDialog = new SaveFileDialog
            {
                Filter = string.Format(Resources.Ringtoets_project_file_filter),
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = string.Format("{0}", projectName)
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                log.Warn(Resources.Project_saving_project_cancelled);
                return null;
            }
            return saveFileDialog.FileName;
        }

        private bool TrySaveProjectAs(IStoreProject storage, string filePath)
        {
            try
            {
                storage.SaveProjectAs(filePath, projectOwner.Project);
                return true;
            }
            catch (StorageException e)
            {
                log.Warn(e.Message, e.InnerException);
                log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
        }

        private bool TrySaveProject(IStoreProject storage, string filePath)
        {
            try
            {
                storage.SaveProject(filePath, projectOwner.Project);
                return true;
            }
            catch (StorageException e)
            {
                log.Warn(e.Message, e.InnerException);
                log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
        }

        private void ApplicationProjectClosing(Project project)
        {
            // clean all views
            viewCommands.RemoveAllViewsForItem(project);

            if (toolViewController.ToolWindowViews != null)
            {
                foreach (IView view in toolViewController.ToolWindowViews)
                {
                    view.Data = null;
                }
            }

            project.Detach(this);
        }

        private void ApplicationProjectOpened(Project project)
        {
            applicationSelection.Selection = project;

            project.Attach(this);
        }

        private void AddProjectToMruList()
        {
            var mruList = (StringCollection) Settings.Default["mruList"];
            if (mruList.Contains(projectOwner.ProjectFilePath))
            {
                mruList.Remove(projectOwner.ProjectFilePath);
            }

            mruList.Insert(0, projectOwner.ProjectFilePath);
        }
    }
}