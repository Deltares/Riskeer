// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.Base.Storage;
using Core.Common.Gui.Properties;
using log4net;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Core.Common.Gui
{
    /// <summary>
    /// Activity that handles opening an <see cref="IProject"/>.
    /// </summary>
    public class OpenProjectActivity : Activity
    {
        private readonly string filePath;
        private readonly IProjectOwner projectOwner;
        private readonly IProjectFactory projectFactory;
        private readonly IStoreProject storage;
        private readonly ILog log = LogManager.GetLogger(typeof(OpenProjectActivity));
        private readonly string migratedProjectFilePath;
        private readonly IMigrateProject migrator;
        private readonly int totalNumberOfSteps = 2;
        private IProject openedProject;
        private bool cancel;

        /// <summary>
        /// Creates a new instance of <see cref="OpenProjectActivity"/>.
        /// </summary>
        /// <param name="requiredOpenProjectProperties">All mandatory properties for being
        /// able to open a project.</param>
        /// <param name="optionalProjectMigrationProperties">Optional: Properties for migrating
        /// the project to the current version.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="requiredOpenProjectProperties"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when any input argument has invalid values.</exception>
        public OpenProjectActivity(OpenProjectConstructionProperties requiredOpenProjectProperties,
                                   ProjectMigrationConstructionProperties optionalProjectMigrationProperties = null)
        {
            if (requiredOpenProjectProperties == null)
            {
                throw new ArgumentNullException(nameof(requiredOpenProjectProperties));
            }

            ValidateOpenProjectProperties(requiredOpenProjectProperties);
            if (optionalProjectMigrationProperties != null)
            {
                ValidateProjectMigrationProperties(optionalProjectMigrationProperties);

                migratedProjectFilePath = optionalProjectMigrationProperties.MigrationFilePath;
                migrator = optionalProjectMigrationProperties.Migrator;
                totalNumberOfSteps = 3;
            }

            filePath = requiredOpenProjectProperties.FilePath;
            projectOwner = requiredOpenProjectProperties.ProjectOwner;
            projectFactory = requiredOpenProjectProperties.ProjectFactory;
            storage = requiredOpenProjectProperties.ProjectStorage;

            Description = Resources.OpenProjectActivity_Open_project;
        }

        protected override void OnRun()
        {
            cancel = false;
            var currentStepNumber = 1;
            ClearOpenedProject();

            if (migrator != null && !MigrateProject(currentStepNumber++))
            {
                return;
            }

            LoadProject(currentStepNumber);
        }

        protected override void OnCancel()
        {
            cancel = true;
        }

        protected override void OnFinish()
        {
            switch (State)
            {
                case ActivityState.Executed:
                    InitializeSuccssfulOpenedProject();
                    break;
                case ActivityState.Failed:
                    InitializeEmptyProject();
                    break;
                case ActivityState.Canceled:
                    ClearOpenedProject();
                    break;
            }
        }

        private string FilePathTakingMigrationIntoAccount
        {
            get
            {
                return migratedProjectFilePath ?? filePath;
            }
        }

        /// <summary>
        /// Migrates the project.
        /// </summary>
        /// <param name="currentStepNumber">The current step number.</param>
        /// <returns><c>true</c> if migration was successful, <c>false</c> otherwise.</returns>
        private bool MigrateProject(int currentStepNumber)
        {
            UpdateProgressText(Resources.OpenProjectActivity_ProgressTextStepName_MigrateProject,
                               currentStepNumber,
                               totalNumberOfSteps);

            try
            {
                bool migrationSuccessful = migrator.Migrate(filePath, migratedProjectFilePath);

                if (cancel)
                {
                    return false;
                }

                if (!migrationSuccessful)
                {
                    State = ActivityState.Failed;
                    return false;
                }
            }
            catch (ArgumentException e)
            {
                if (cancel)
                {
                    return false;
                }

                log.Error(e.Message, e);
                State = ActivityState.Failed;
                return false;
            }

            return true;
        }

        private void LoadProject(int currentStepNumber)
        {
            UpdateProgressText(Resources.OpenProjectActivity_ProgressTextStepName_ReadProject,
                               currentStepNumber,
                               totalNumberOfSteps);
            try
            {
                openedProject = storage.LoadProject(FilePathTakingMigrationIntoAccount);
            }
            catch (StorageException e)
            {
                if (cancel)
                {
                    return;
                }

                log.Error(e.Message, e.InnerException);
            }

            if (cancel)
            {
                return;
            }

            if (openedProject == null)
            {
                State = ActivityState.Failed;
            }
        }

        private void ClearOpenedProject()
        {
            openedProject = null;
        }

        private void InitializeEmptyProject()
        {
            UpdateProgressText(Resources.OpenProjectActivity_ProgressTextStepName_InitializeEmptyProject,
                               totalNumberOfSteps,
                               totalNumberOfSteps);

            projectOwner.SetProject(projectFactory.CreateNewProject(), null);
        }

        private void InitializeSuccssfulOpenedProject()
        {
            UpdateProgressText(Resources.OpenProjectActivity_ProgressTextStepName_InitializeProject,
                               totalNumberOfSteps,
                               totalNumberOfSteps);

            projectOwner.SetProject(openedProject, FilePathTakingMigrationIntoAccount);
            openedProject.Name = Path.GetFileNameWithoutExtension(FilePathTakingMigrationIntoAccount);
            openedProject.NotifyObservers();
        }

        /// <summary>
        /// Updates the progress text.
        /// </summary>
        /// <param name="currentStepName">A short description of the current step.</param>
        /// <param name="currentStep">The number of the current step.</param>
        /// <param name="totalSteps">The total numbers of steps.</param>
        private void UpdateProgressText(string currentStepName, int currentStep, int totalSteps)
        {
            ProgressText = string.Format(CultureInfo.CurrentCulture,
                                         CoreCommonBaseResources.Activity_UpdateProgressText_CurrentStepNumber_0_of_TotalStepsNumber_1_StepDescription_2_,
                                         currentStep, totalSteps, currentStepName);
        }

        /// <summary>
        /// Validates the construction arguments required for opening a project file.
        /// </summary>
        /// <param name="requiredOpenProjectProperties">The construction arguments to be validated.</param>
        /// <exception cref="ArgumentException">Thrown when any construction property of
        /// <paramref name="requiredOpenProjectProperties"/> is invalid.</exception>
        private static void ValidateOpenProjectProperties(OpenProjectConstructionProperties requiredOpenProjectProperties)
        {
            if (requiredOpenProjectProperties.FilePath == null)
            {
                throw new ArgumentException(@"Filepath should be set.", nameof(requiredOpenProjectProperties));
            }

            if (requiredOpenProjectProperties.ProjectOwner == null)
            {
                throw new ArgumentException(@"Project owner should be set.", nameof(requiredOpenProjectProperties));
            }

            if (requiredOpenProjectProperties.ProjectFactory == null)
            {
                throw new ArgumentException(@"Project factory should be set.", nameof(requiredOpenProjectProperties));
            }

            if (requiredOpenProjectProperties.ProjectStorage == null)
            {
                throw new ArgumentException(@"Project storage should be set.", nameof(requiredOpenProjectProperties));
            }
        }

        /// <summary>
        /// Validates the construction arguments required for migrating a project file.
        /// </summary>
        /// <param name="optionalProjectMigrationProperties">The construction arguments to be validated.</param>
        /// <exception cref="ArgumentException">Thrown when any construction property of
        /// <paramref name="optionalProjectMigrationProperties"/> is invalid.</exception>
        private static void ValidateProjectMigrationProperties(ProjectMigrationConstructionProperties optionalProjectMigrationProperties)
        {
            if (optionalProjectMigrationProperties.Migrator == null)
            {
                throw new ArgumentException(@"Project migrator should be set.", nameof(optionalProjectMigrationProperties));
            }

            if (optionalProjectMigrationProperties.MigrationFilePath == null)
            {
                throw new ArgumentException(@"Migration target file path should be set.", nameof(optionalProjectMigrationProperties));
            }
        }

        /// <summary>
        /// All construction properties related to opening a project file.
        /// </summary>
        public class OpenProjectConstructionProperties
        {
            /// <summary>
            /// Filepath to the project file that should be opened.
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// The <see cref="IProjectOwner"/> to be updated with the newly opened project.
            /// </summary>
            public IProjectOwner ProjectOwner { get; set; }

            /// <summary>
            /// The <see cref="IProjectFactory"/> to create a default project in case of
            /// an error scenario.
            /// </summary>
            public IProjectFactory ProjectFactory { get; set; }

            /// <summary>
            /// The object used to open the file at <see cref="FilePath"/>.
            /// </summary>
            public IStoreProject ProjectStorage { get; set; }
        }

        /// <summary>
        /// All construction properties related to migrating a project file.
        /// </summary>
        public class ProjectMigrationConstructionProperties
        {
            /// <summary>
            /// The <see cref="IMigrateProject"/> responsible for performing the migration.
            /// </summary>
            public IMigrateProject Migrator { get; set; }

            /// <summary>
            /// The file path to where the migrate project should be written to. This path
            /// will override <see cref="OpenProjectConstructionProperties.FilePath"/> with
            /// regards of opening the project.
            /// </summary>
            public string MigrationFilePath { get; set; }
        }
    }
}