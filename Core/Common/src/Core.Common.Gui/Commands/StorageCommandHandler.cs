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
        private readonly IProjectFactory projectFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCommandHandler"/> class.
        /// </summary>
        /// <param name="projectStorage">Class responsible to storing and loading the application project.</param>
        /// <param name="projectFactory">The factory to use when creating new projects.</param>
        /// <param name="projectOwner">The class owning the application project.</param>
        /// <param name="dialogParent">Controller for UI.</param>
        public StorageCommandHandler(IStoreProject projectStorage, IProjectFactory projectFactory, IProjectOwner projectOwner, IWin32Window dialogParent)
        {
            this.dialogParent = dialogParent;
            this.projectOwner = projectOwner;
            projectPersistor = projectStorage;
            this.projectFactory = projectFactory;
        }

        public bool HandleUnsavedChanges()
        {
            if (IsCurrentNew())
            {
                return true;
            }
            var project = projectOwner.Project;
            projectPersistor.StageProject(project);
            try
            {
                if (!projectPersistor.HasStagedProjectChanges(projectOwner.ProjectFilePath))
                {
                    projectPersistor.UnstageProject();
                    return true;
                }
            }
            catch (StorageException e)
            {
                log.Error(e.Message, e);
                projectPersistor.UnstageProject();
            }

            var unsavedChangesHandled = ShowSaveUnsavedChangesDialog();

            if (projectPersistor.HasStagedProject)
            {
                projectPersistor.UnstageProject();
            }

            return unsavedChangesHandled;
        }

        public void CreateNewProject()
        {
            if (!HandleUnsavedChanges())
            {
                log.Info(Resources.StorageCommandHandler_NewProject_Creating_new_project_cancelled);
                return;
            }
            log.Info(Resources.Creating_new_project);
            projectOwner.SetProject(projectFactory.CreateNewProject(), null);
            log.Info(Resources.Created_new_project_successful);
        }

        public bool OpenExistingProject()
        {
            using (var openFileDialog = new OpenFileDialog
            {
                Filter = projectPersistor.FileFilter,
                Title = Resources.OpenFileDialog_Title
            })
            {
                if (openFileDialog.ShowDialog(dialogParent) != DialogResult.Cancel && HandleUnsavedChanges())
                {
                    return OpenExistingProject(openFileDialog.FileName);
                }
            }

            log.Info(Resources.StorageCommandHandler_OpenExistingProject_Opening_existing_project_cancelled);
            return false;
        }

        public bool OpenExistingProject(string filePath)
        {
            log.Info(Resources.StorageCommandHandler_OpenExistingProject_Opening_existing_project);

            var newProject = LoadProjectFromStorage(filePath);
            var isOpenProjectSuccessful = newProject != null;

            if (isOpenProjectSuccessful)
            {
                log.Info(Resources.StorageCommandHandler_OpeningExistingProject_Opening_existing_project_successful);
                projectOwner.SetProject(newProject, filePath);
                newProject.Name = Path.GetFileNameWithoutExtension(filePath);
                newProject.NotifyObservers();
            }
            else
            {
                log.Error(Resources.StorageCommandHandler_OpeningExistingProject_Opening_existing_project_failed);
                projectOwner.SetProject(projectFactory.CreateNewProject(), null);
            }

            return isOpenProjectSuccessful;
        }

        public bool SaveProjectAs()
        {
            var project = projectOwner.Project;
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
            projectOwner.SetProject(project, filePath);
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
            var filePath = projectOwner.ProjectFilePath;

            // If filepath is not set, go to SaveAs
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
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

        private bool IsCurrentNew()
        {
            return projectOwner.Project.Equals(projectFactory.CreateNewProject());
        }

        private bool ShowSaveUnsavedChangesDialog()
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
                    log.Info(Resources.StorageCommandHandler_SaveProject_Saving_project_cancelled);
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