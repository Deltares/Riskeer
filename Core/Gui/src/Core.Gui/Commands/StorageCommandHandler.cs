// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.IO;
using Core.Common.Base.Service;
using Core.Common.Base.Storage;
using Core.Gui.Forms.ProgressDialog;
using Core.Gui.Helpers;
using Core.Gui.Properties;
using log4net;

namespace Core.Gui.Commands
{
    /// <summary>
    /// Class responsible for persistency of <see cref="IProject"/>.
    /// </summary>
    public class StorageCommandHandler : IStorageCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StorageCommandHandler));

        private readonly IMainWindowController mainWindowController;
        private readonly IProjectOwner projectOwner;
        private readonly IStoreProject projectPersister;
        private readonly IProjectFactory projectFactory;
        private readonly IMigrateProject projectMigrator;
        private readonly IInquiryHelper inquiryHelper;

        /// <summary>
        /// Creates a new instance of <see cref="StorageCommandHandler"/>.
        /// </summary>
        /// <param name="projectStorage">Class responsible to storing and loading the application project.</param>
        /// <param name="projectMigrator">Class responsible for the migration of the application projects.</param>
        /// <param name="projectFactory">The factory to use when creating new projects.</param>
        /// <param name="projectOwner">The class owning the application project.</param>
        /// <param name="inquiryHelper">The object facilitating user interaction.</param>
        /// <param name="mainWindowController">The object owning the parent controller for UI.</param>
        public StorageCommandHandler(IStoreProject projectStorage, IMigrateProject projectMigrator,
                                     IProjectFactory projectFactory, IProjectOwner projectOwner,
                                     IInquiryHelper inquiryHelper, IMainWindowController mainWindowController)
        {
            projectPersister = projectStorage;
            this.projectMigrator = projectMigrator;
            this.projectFactory = projectFactory;
            this.projectOwner = projectOwner;
            this.inquiryHelper = inquiryHelper;
            this.mainWindowController = mainWindowController;
        }

        public bool HandleUnsavedChanges()
        {
            if (projectOwner.Project == null)
            {
                return true;
            }

            IProject project = projectOwner.Project;
            projectPersister.StageProject(project);
            try
            {
                if (!projectPersister.HasStagedProjectChanges(projectOwner.ProjectFilePath))
                {
                    projectPersister.UnstageProject();
                    return true;
                }
            }
            catch (StorageException e)
            {
                log.Error(e.Message, e);
                projectPersister.UnstageProject();
            }

            bool unsavedChangesHandled = ShowSaveUnsavedChangesDialog();

            if (projectPersister.HasStagedProject)
            {
                projectPersister.UnstageProject();
            }

            return unsavedChangesHandled;
        }

        public void CreateNewProject()
        {
            if (!HandleUnsavedChanges())
            {
                log.Info(Resources.StorageCommandHandler_NewProject_Creating_new_project_canceled);
                return;
            }

            log.Info(Resources.Creating_new_project_started);
            var errorOccurred = false;
            IProject newProject = null;

            try
            {
                newProject = projectFactory.CreateNewProject();
            }
            catch (ProjectFactoryException e)
            {
                log.Error(e.Message);
                log.Info(Resources.StorageCommandHandler_NewProject_Creating_new_project_failed);
                errorOccurred = true;
            }

            if (newProject == null && !errorOccurred)
            {
                log.Info(Resources.StorageCommandHandler_NewProject_Creating_new_project_canceled);
            }

            projectOwner.SetProject(newProject, null);

            if (newProject != null)
            {
                log.Info(Resources.Creating_new_project_successful);
            }
        }

        public string GetExistingProjectFilePath()
        {
            using (var openFileDialog = new OpenFileDialog
            {
                Filter = projectPersister.OpenProjectFileFilter,
                Title = Resources.OpenFileDialog_Title
            })
            {
                if (openFileDialog.ShowDialog(mainWindowController.MainWindow) != DialogResult.Cancel && HandleUnsavedChanges())
                {
                    return openFileDialog.FileName;
                }
            }

            return null;
        }

        public bool OpenExistingProject(string filePath)
        {
            try
            {
                return OpenExistingProjectCore(filePath);
            }
            catch (Exception e) when (e is ArgumentException || e is CriticalFileReadException || e is StorageValidationException)
            {
                log.Error(e.Message, e);
                projectOwner.SetProject(null, null);
                return false;
            }
        }

        public bool SaveProjectAs()
        {
            IProject project = projectOwner.Project;
            string filePath = OpenProjectSaveFileDialog(project.Name);

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            var activity = new SaveProjectActivity(project, filePath, false, projectPersister, projectOwner);
            ActivityProgressDialogRunner.Run(mainWindowController.MainWindow, activity);
            return activity.State == ActivityState.Finished;
        }

        public bool SaveProject()
        {
            IProject project = projectOwner.Project;
            string filePath = projectOwner.ProjectFilePath;

            // If file path is not set, go to SaveAs
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return SaveProjectAs();
            }

            var activity = new SaveProjectActivity(project, filePath, true, projectPersister, projectOwner);
            ActivityProgressDialogRunner.Run(mainWindowController.MainWindow, activity);
            return activity.State == ActivityState.Finished;
        }

        /// <summary>
        /// Opens the given project file and determines if migration to the current version is
        /// required and performs migration if needed.
        /// </summary>
        /// <param name="filePath">The project file to open.</param>
        /// <returns>Returns <c>true</c> if the project was successfully opened; Returns
        /// <c>false</c> if an error occurred or when opening the project has been cancelled.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is
        /// not a valid file path.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when the file at <paramref name="filePath"/>
        /// couldn't be read.</exception>
        /// <exception cref="StorageValidationException">Thrown when the file at <paramref name="filePath"/>
        /// is not a valid project file.</exception>
        private bool OpenExistingProjectCore(string filePath)
        {
            OpenProjectActivity.ProjectMigrationConstructionProperties migrationProperties;
            if (PrepareProjectMigration(filePath, out migrationProperties) == MigrationRequired.Aborted)
            {
                return false;
            }

            return MigrateAndOpenProject(filePath, migrationProperties);
        }

        /// <summary>
        /// Check if migration is required and if so the migration settings are initialized.
        /// </summary>
        /// <param name="filePath">The project file to open.</param>
        /// <param name="migrationConstructionProperties">Output: Will be null if this method
        /// returns <see cref="MigrationRequired.No"/> or <see cref="MigrationRequired.Aborted"/>.
        /// Will be a concrete instance if this method returns <see cref="MigrationRequired.Yes"/>.</param>
        /// <returns>Indicates if migration is required or not, or that the operation has
        /// been cancelled.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is
        /// not a valid file path.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when the file at <paramref name="filePath"/>
        /// couldn't be read.</exception>
        /// <exception cref="StorageValidationException">Thrown when the file at <paramref name="filePath"/>
        /// is not a valid project file.</exception>
        private MigrationRequired PrepareProjectMigration(string filePath,
                                                          out OpenProjectActivity.ProjectMigrationConstructionProperties migrationConstructionProperties)
        {
            migrationConstructionProperties = null;
            MigrationRequired migrationNeeded = projectMigrator.ShouldMigrate(filePath);
            if (migrationNeeded == MigrationRequired.Yes)
            {
                string projectFilePathTakingIntoAccountMigration = projectMigrator.DetermineMigrationLocation(filePath);
                if (string.IsNullOrWhiteSpace(projectFilePathTakingIntoAccountMigration))
                {
                    migrationNeeded = MigrationRequired.Aborted;
                }
                else
                {
                    migrationConstructionProperties = new OpenProjectActivity.ProjectMigrationConstructionProperties
                    {
                        Migrator = projectMigrator,
                        MigrationFilePath = projectFilePathTakingIntoAccountMigration
                    };
                }
            }

            return migrationNeeded;
        }

        /// <summary>
        /// Starts an activity that can migrate the project if required, then opens the project.
        /// </summary>
        /// <param name="filePath">The project file to open.</param>
        /// <param name="migrationProperties">The construction properties related to migrating
        /// a project. Can be <c>null</c>.</param>
        /// <returns>Returns <c>true</c> if the operation completed successfully; Returns
        /// <c>false</c> otherwise (for example when the operation failed or was cancelled).</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is
        /// not a valid file path.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/>
        /// is <c>null</c>.</exception>
        private bool MigrateAndOpenProject(string filePath, OpenProjectActivity.ProjectMigrationConstructionProperties migrationProperties)
        {
            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = filePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectPersister
            };
            var activity = new OpenProjectActivity(openProjectProperties, migrationProperties);
            ActivityProgressDialogRunner.Run(mainWindowController.MainWindow, activity);

            return activity.State == ActivityState.Finished;
        }

        private bool ShowSaveUnsavedChangesDialog()
        {
            string inquiry = string.Format(CultureInfo.CurrentCulture,
                                           Resources.StorageCommandHandler_OpenSaveOrDiscardProjectDialog_SaveChangesToProject_0,
                                           projectOwner.Project.Name);
            OptionalStepResult confirmation = inquiryHelper.InquirePerformOptionalStep(Resources.StorageCommandHandler_ClosingProject_Title,
                                                                                       inquiry);

            switch (confirmation)
            {
                case OptionalStepResult.Cancel:
                    return false;
                case OptionalStepResult.PerformOptionalStep:
                    ReleaseDatabaseFileHandle();
                    return SaveProject();
            }

            return true;
        }

        private static void ReleaseDatabaseFileHandle()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Prompts a new <see cref="SaveFileDialog"/> to select a location for saving the project file.
        /// </summary>
        /// <param name="projectName">A string containing the file name selected in the file dialog box.</param>
        /// <returns>The selected project file, or <c>null</c> otherwise.</returns>
        private string OpenProjectSaveFileDialog(string projectName)
        {
            return inquiryHelper.GetTargetFileLocation(projectPersister.SaveProjectFileFilter, projectName);
        }
    }
}