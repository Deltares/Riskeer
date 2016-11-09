// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Gui.Properties;
using log4net;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Class responsible for persistency of <see cref="IProject"/>.
    /// </summary>
    public class StorageCommandHandler : IStorageCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StorageCommandHandler));

        private readonly IWin32Window dialogParent;
        private readonly IProjectOwner projectOwner;
        private readonly IStoreProject projectPersistor;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCommandHandler"/> class.
        /// </summary>
        /// <param name="projectStorage">Class responsible to storing and loading the application project.</param>
        /// <param name="projectOwner">The class owning the application project.</param>
        /// <param name="dialogParent">Controller for UI.</param>
        public StorageCommandHandler(IStoreProject projectStorage, IProjectOwner projectOwner, IWin32Window dialogParent)
        {
            this.dialogParent = dialogParent;
            this.projectOwner = projectOwner;
            projectPersistor = projectStorage;
        }

        /// <summary>
        /// Checks if an action may continue when changes are detected.
        /// </summary>
        /// <returns><c>True</c> if the action should continue, <c>false</c> otherwise.</returns>
        public bool ContinueIfHasChanges()
        {
            var project = projectOwner.Project;
            if (project == null || projectOwner.IsCurrentNew())
            {
                return true;
            }
            projectPersistor.StageProject(project);
            try
            {
                if (!projectPersistor.HasStagedProjectChanges())
                {
                    projectPersistor.UnstageProject();
                    return true;
                }
            }
            catch (StorageException e)
            {
                log.Error(e.Message, e);
                projectPersistor.UnstageProject();
                return false;
            }

            var openSaveOrDiscardProjectDialog = OpenSaveOrDiscardProjectDialog();
            if (projectPersistor.HasStagedProject)
            {
                projectPersistor.UnstageProject();
            }
            return openSaveOrDiscardProjectDialog;
        }

        public void CreateNewProject()
        {
            if (!ContinueIfHasChanges())
            {
                log.Info(Resources.StorageCommandHandler_NewProject_Creating_new_project_cancelled);
                return;
            }
            CloseProject();

            log.Info(Resources.StorageCommandHandler_NewProject_Creating_new_project);
            projectOwner.CreateNewProject();
            projectOwner.ProjectFilePath = "";
            log.Info(Resources.StorageCommandHandler_NewProject_Created_new_project_succesful);
        }

        public bool OpenExistingProject()
        {
            using (var openFileDialog = new OpenFileDialog
            {
                Filter = projectPersistor.FileFilter,
                Title = Resources.OpenFileDialog_Title
            })
            {
                if (openFileDialog.ShowDialog(dialogParent) != DialogResult.Cancel && ContinueIfHasChanges())
                {
                    return OpenExistingProject(openFileDialog.FileName);
                }
            }

            log.Warn(Resources.StorageCommandHandler_OpenExistingProject_Opening_existing_project_cancelled);
            return false;
        }

        public bool OpenExistingProject(string filePath)
        {
            log.Info(Resources.StorageCommandHandler_OpenExistingProject_Opening_existing_project);

            CloseProject();

            var newProject = LoadProjectFromStorage(filePath);
            var isOpenProjectSuccessful = newProject != null;
            if (!isOpenProjectSuccessful)
            {
                log.Error(Resources.StorageCommandHandler_OpeningExistingProject_Opening_existing_project_failed);
            }
            else
            {
                log.Info(Resources.StorageCommandHandler_OpeningExistingProject_Opening_existing_project_successful);
                newProject.Name = Path.GetFileNameWithoutExtension(filePath);
                projectOwner.ProjectFilePath = filePath;
                projectOwner.Project = newProject;
            }

            return isOpenProjectSuccessful;
        }

        public bool SaveProjectAs()
        {
            var project = projectOwner.Project;
            if (project == null)
            {
                return false;
            }

            var filePath = OpenProjectSaveFileDialog(project.Name);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            if (!TrySaveProjectAs(filePath))
            {
                return false;
            }

            // Save was successful, store location
            projectOwner.ProjectFilePath = filePath;
            project.Name = Path.GetFileNameWithoutExtension(filePath);
            project.NotifyObservers();
            log.Info(string.Format(CultureInfo.CurrentCulture,
                                   Resources.StorageCommandHandler_SaveProject_Successfully_saved_project_0_,
                                   project.Name));
            return true;
        }

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

            if (!TrySaveProjectAs(filePath))
            {
                return false;
            }

            log.Info(string.Format(CultureInfo.CurrentCulture,
                                   Resources.StorageCommandHandler_SaveProject_Successfully_saved_project_0_,
                                   project.Name));
            return true;
        }

        private void CloseProject()
        {
            if (projectOwner.Project == null)
            {
                return;
            }
            projectOwner.CloseProject();
            projectOwner.ProjectFilePath = "";
            projectPersistor.CloseProject();
        }

        private bool OpenSaveOrDiscardProjectDialog()
        {
            var confirmation = MessageBox.Show(
                string.Format(CultureInfo.CurrentCulture,
                              Resources.StorageCommandHandler_OpenSaveOrDiscardProjectDialog_SaveChangesToProject_0,
                              projectOwner.Project.Name),
                Resources.StorageCommandHandler_ClosingProject_Title,
                MessageBoxButtons.YesNoCancel);

            switch (confirmation)
            {
                case DialogResult.Cancel:
                    return false;
                case DialogResult.Yes:
                    ReleaseDatabaseFileHandle();
                    return SaveProject();
                case DialogResult.No:
                    break;
            }
            return true;
        }

        private static void ReleaseDatabaseFileHandle()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Loads the project from the <see cref="IStoreProject"/>.
        /// </summary>
        /// <param name="filePath">The path to load a <see cref="IProject"/> from.</param>
        /// <returns>The loaded <see cref="IProject"/> from <paramref name="filePath"/> or <c>null</c> if the project
        /// could not be loaded from <paramref name="filePath"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> is invalid.</exception>
        private IProject LoadProjectFromStorage(string filePath)
        {
            IProject loadedProject = null;

            try
            {
                loadedProject = projectPersistor.LoadProject(filePath);
            }
            catch (StorageException e)
            {
                log.Error(e.Message, e.InnerException);
            }

            return loadedProject;
        }

        /// <summary>
        /// Prompts a new <see cref="SaveFileDialog"/> to select a location for saving the project file.
        /// </summary>
        /// <param name="projectName">A string containing the file name selected in the file dialog box.</param>
        /// <returns>The selected project file, or <c>null</c> otherwise.</returns>
        private string OpenProjectSaveFileDialog(string projectName)
        {
            using (var saveFileDialog = new SaveFileDialog
            {
                Title = Resources.SaveFileDialog_Title,
                Filter = projectPersistor.FileFilter,
                FileName = projectName
            })
            {
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    log.Warn(Resources.StorageCommandHandler_SaveProject_Saving_project_cancelled);
                    return null;
                }
                return saveFileDialog.FileName;
            }
        }

        private bool TrySaveProjectAs(string filePath)
        {
            try
            {
                if (!projectPersistor.HasStagedProject)
                {
                    projectPersistor.StageProject(projectOwner.Project);
                }
                projectPersistor.SaveProjectAs(filePath);
                return true;
            }
            catch (StorageException e)
            {
                log.Error(e.Message, e.InnerException);
                log.Error(Resources.StorageCommandHandler_Saving_project_failed);
                return false;
            }
        }
    }
}