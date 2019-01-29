// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.Base.Storage;
using Core.Common.Gui.Properties;
using log4net;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Core.Common.Gui
{
    /// <summary>
    /// Activity to save an <see cref="IProject"/>.
    /// </summary>
    public class SaveProjectActivity : Activity
    {
        private readonly ILog log = LogManager.GetLogger(typeof(SaveProjectActivity));
        private readonly bool savingExistingProject;
        private readonly IProject project;
        private readonly string filePath;
        private readonly IStoreProject storeProject;
        private readonly IProjectOwner projectOwner;
        private int totalNumberOfSteps;

        private bool cancel;

        /// <summary>
        /// Creates a new instance of <see cref="SaveProjectActivity"/>.
        /// </summary>
        /// <param name="project">The project to be saved.</param>
        /// <param name="filePath">The location to save the project to.</param>
        /// <param name="savingExistingProject">When <c>true</c> it indicates that <paramref name="project"/>
        /// is already located at <paramref name="filePath"/>. When <c>false</c> then <paramref name="project"/>
        /// is not already located at <paramref name="filePath"/>.</param>
        /// <param name="storeProject">The object responsible for saving <paramref name="project"/>.</param>
        /// <param name="projectOwner">The object responsible for hosting <paramref name="project"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public SaveProjectActivity(IProject project, string filePath, bool savingExistingProject, IStoreProject storeProject, IProjectOwner projectOwner)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (storeProject == null)
            {
                throw new ArgumentNullException(nameof(storeProject));
            }

            if (projectOwner == null)
            {
                throw new ArgumentNullException(nameof(projectOwner));
            }

            this.savingExistingProject = savingExistingProject;
            this.project = project;
            this.filePath = filePath;
            this.storeProject = storeProject;
            this.projectOwner = projectOwner;

            Description = savingExistingProject
                              ? Resources.SaveProjectActivity_Save_existing_project
                              : Resources.SaveProjectActivity_Save_project;
        }

        protected override void OnRun()
        {
            cancel = false;
            totalNumberOfSteps = savingExistingProject ? 1 : 2;
            var currentStep = 1;

            if (!storeProject.HasStagedProject)
            {
                totalNumberOfSteps++;
                UpdateProgressText(Resources.SaveProjectActivity_ProgressTextStepName_StagingProject,
                                   currentStep++,
                                   totalNumberOfSteps);

                storeProject.StageProject(project);
            }

            if (cancel)
            {
                return;
            }

            SaveProjectUncancellable(currentStep);
        }

        protected override void OnCancel()
        {
            cancel = true;
        }

        protected override void OnFinish()
        {
            if (State == ActivityState.Executed && !savingExistingProject)
            {
                InitializeProjectForNewLocation();
            }
        }

        private void SaveProjectUncancellable(int currentStep)
        {
            try
            {
                UpdateProgressText(Resources.SaveProjectActivity_ProgressTextStepName_SavingProject,
                                   currentStep,
                                   totalNumberOfSteps);

                storeProject.SaveProjectAs(filePath);
            }
            catch (StorageException e)
            {
                log.Error(e.Message, e.InnerException);
                State = ActivityState.Failed;
                return;
            }
            catch (ArgumentException e)
            {
                log.Error(e.Message, e);
                State = ActivityState.Failed;
                return;
            }

            // Override State (might be Cancelled) due to cancelling not possible
            State = ActivityState.Executed;
        }

        private void InitializeProjectForNewLocation()
        {
            UpdateProgressText(Resources.SaveProjectActivity_ProgressTextStepName_InitializeSavedProject,
                               totalNumberOfSteps,
                               totalNumberOfSteps);

            projectOwner.SetProject(project, filePath);
            project.Name = Path.GetFileNameWithoutExtension(filePath);
            project.NotifyObservers();
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
    }
}