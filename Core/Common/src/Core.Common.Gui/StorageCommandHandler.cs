using System;
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
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiCommandHandler));

        private readonly IGuiCommandHandler guiCommandHandler;
        private readonly IGui gui;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCommandHandler"/> class.
        /// </summary>
        /// <param name="guiCommandHandler">The GUI command handler.</param>
        /// <param name="gui">The GUI.</param>
        public StorageCommandHandler(IGuiCommandHandler guiCommandHandler, IGui gui)
        {
            this.guiCommandHandler = guiCommandHandler;
            this.gui = gui;

            this.gui.ProjectOpened += ApplicationProjectOpened;
            this.gui.ProjectClosing += ApplicationProjectClosing;
        }

        public void CreateNewProject()
        {
            CloseProject();

            log.Info(Resources.Project_new_opening);
            gui.Project = new Project();
            log.Info(Resources.Project_new_successfully_opened);

            gui.RefreshGui();
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

            if (openFileDialog.ShowDialog(gui.MainWindow) != DialogResult.Cancel)
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

            var storage = gui.Storage;
            Project loadedProject;
            try
            {
                loadedProject = storage.LoadProject(filePath);
            }
            catch (ArgumentException e)
            {
                log.Warn(e.Message);
                log.Warn(Resources.Project_existing_project_opening_failed);
                return false;
            }
            catch (CouldNotConnectException e)
            {
                log.Warn(e.Message);
                log.Warn(Resources.Project_existing_project_opening_failed);
                return false;
            }
            catch (StorageValidationException e)
            {
                log.Warn(e.Message);
                log.Warn(Resources.Project_saving_project_failed);
                return false;
            }

            if (loadedProject == null)
            {
                log.Warn(Resources.Project_existing_project_opening_failed);
                return false;
            }

            // Project loaded successfully, close current project
            CloseProject();

            gui.ProjectFilePath = filePath;
            gui.Project = loadedProject;
            gui.Project.Name = Path.GetFileNameWithoutExtension(filePath);

            gui.RefreshGui();
            log.Info(Resources.Project_existing_successfully_opened);
            return true;
        }

        /// <summary>
        /// Close current project (if any) and related views.
        /// </summary>
        public void CloseProject()
        {
            if (gui.Project == null)
            {
                return;
            }

            // remove views before closing project. 
            guiCommandHandler.RemoveAllViewsForItem(gui.Project);

            gui.Project = null;

            gui.RefreshGui();
        }

        /// <summary>
        /// Saves the current <see cref="Project"/> to the selected storage file.
        /// </summary>
        /// <returns>Returns if the save was successful.</returns>
        public bool SaveProjectAs()
        {
            var project = gui.Project;
            if (project == null)
            {
                return false;
            }

            log.Info(Resources.Project_saving_project);
            // show file open dialog and select project file
            var saveFileDialog = new SaveFileDialog
            {
                Filter = string.Format(Resources.Ringtoets_project_file_filter),
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = string.Format("{0}", project.Name)
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                log.Warn(Resources.Project_saving_project_cancelled);
                return false;
            }

            var filePath = saveFileDialog.FileName;
            var storage = gui.Storage;
            try
            {
                storage.SaveProjectAs(filePath, gui.Project);
            }
            catch (ArgumentException e)
            {
                log.Warn(e.Message);
                log.Warn(Resources.Project_existing_project_opening_failed);
                return false;
            }
            catch (CouldNotConnectException e)
            {
                log.Warn(e.Message);
                log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
            catch (StorageValidationException e)
            {
                log.Warn(e.Message);
                log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
            catch (UpdateStorageException e)
            {
                log.Warn(e.Message);
                log.Warn(Resources.Project_saving_project_failed);
                return false;
            }

            // Save was successful, store location
            gui.ProjectFilePath = filePath;
            project.Name = Path.GetFileNameWithoutExtension(filePath);
            log.Info(Resources.Project_saving_project_saved);
            return true;
        }

        /// <summary>
        /// Saves the current <see cref="Project"/> to the defined storage file.
        /// </summary>
        /// <returns>Returns if the save was succesful.</returns>
        public bool SaveProject()
        {
            var project = gui.Project;
            if (project == null)
            {
                return false;
            }
            var filePath = gui.ProjectFilePath;

            // If filepath is not set, go to SaveAs
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return SaveProjectAs();
            }

            log.Info(Resources.Project_saving_project);
            var storage = gui.Storage;
            try
            {
                storage.SaveProject(filePath, gui.Project);
            }
            catch (ArgumentException e)
            {
                log.Warn(e.Message);
                log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
            catch (CouldNotConnectException e)
            {
                log.Warn(e.Message);
                log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
            catch (StorageValidationException e)
            {
                log.Warn(e.Message);
                log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
            catch (UpdateStorageException e)
            {
                log.Warn(e.Message);
                log.Warn(Resources.Project_saving_project_failed);
                return false;
            }

            log.Info(Resources.Project_saving_project_saved);
            return true;
        }

        public void UpdateObserver()
        {
            gui.RefreshGui();
        }

        public void Dispose()
        {
            gui.ProjectOpened -= ApplicationProjectOpened;
            gui.ProjectClosing -= ApplicationProjectClosing;
        }

        private void ApplicationProjectClosing(Project project)
        {
            // clean all views
            if (gui.DocumentViews != null)
            {
                guiCommandHandler.RemoveAllViewsForItem(project);
            }

            if (gui.ToolWindowViews != null)
            {
                foreach (IView view in gui.ToolWindowViews)
                {
                    view.Data = null;
                }
            }

            project.Detach(this);
        }

        private void ApplicationProjectOpened(Project project)
        {
            gui.Selection = project;

            project.Attach(this);
        }
    }
}