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
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.Views;
using Core.Common.Gui.Properties;
using Core.Common.Gui.Selection;
using log4net;

namespace Core.Common.Gui.Commands
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

        /// <summary>
        /// This method performs an update of the <seealso cref="IObserver"/>, triggered by a notification of an <seealso cref="IObservable"/>.
        /// </summary>
        public void UpdateObserver()
        {
            mainWindowController.RefreshGui();
        }

        public void CreateNewProject()
        {
            CloseProject();

            log.Info(Resources.Project_new_opening);
            projectOwner.Project = new Project();
            projectOwner.ProjectFilePath = "";
            log.Info(Resources.Project_new_successfully_opened);

            mainWindowController.RefreshGui();
        }

        public bool OpenExistingProject()
        {
            using (var openFileDialog = new OpenFileDialog
            {
                Filter = Resources.Ringtoets_project_file_filter,
                FilterIndex = 1,
                RestoreDirectory = true
            })
            {
                if (openFileDialog.ShowDialog(mainWindowController.MainWindow) != DialogResult.Cancel)
                {
                    return OpenExistingProject(openFileDialog.FileName);
                }
            }

            log.Warn(Resources.Project_existing_project_opening_cancelled);
            return false;
        }

        public bool OpenExistingProject(string filePath)
        {
            log.Info(Resources.Project_existing_opening_project);

            var loadedProject = LoadProjectFromStorage(filePath);

            if (loadedProject == null)
            {
                log.Error(Resources.Project_existing_project_opening_failed);
                return false;
            }

            // Project loaded successfully, close current project
            CloseProject();

            projectOwner.ProjectFilePath = filePath;
            projectOwner.Project = loadedProject;
            projectOwner.Project.Name = Path.GetFileNameWithoutExtension(filePath);
            projectOwner.Project.NotifyObservers();
            mainWindowController.RefreshGui();
            log.Info(Resources.Project_existing_successfully_opened);
            return true;
        }

        /// <summary>
        /// Loads the project from the <see cref="IStoreProject"/>.
        /// </summary>
        /// <param name="filePath">The path to load a <see cref="Project"/> from.</param>
        /// <returns>The loaded <see cref="Project"/> from <paramref name="filePath"/> or <c>null</c> if the project
        /// could not be loaded from <paramref name="filePath"/>.</returns>
        private Project LoadProjectFromStorage(string filePath)
        {
            Project loadedProject = null;

            try
            {
                loadedProject = projectPersistor.LoadProject(filePath);
            }
            catch (StorageException e)
            {
                log.Error(e.Message, e.InnerException);
            }
            catch (ArgumentException e)
            {
                log.Error(e.Message, e.InnerException);
            }

            return loadedProject;
        }

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
            using (var saveFileDialog = new SaveFileDialog
            {
                Filter = string.Format(Resources.Ringtoets_project_file_filter),
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = projectName
            })
            {
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    log.Warn(Resources.Project_saving_project_cancelled);
                    return null;
                }
                return saveFileDialog.FileName;
            }
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
                log.Error(e.Message, e.InnerException);
                log.Error(Resources.Project_saving_project_failed);
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
                log.Error(e.Message, e.InnerException);
                log.Error(Resources.Project_saving_project_failed);
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
            var mruList = (StringCollection) Properties.Settings.Default["mruList"];
            if (mruList.Contains(projectOwner.ProjectFilePath))
            {
                mruList.Remove(projectOwner.ProjectFilePath);
            }

            mruList.Insert(0, projectOwner.ProjectFilePath);
        }
    }
}