using System;
using System.IO;

using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Gui.Properties;

using log4net;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Opens an existing project file from hard disk and loads that into the application.
    /// </summary>
    public class OpenProjectCommand : IGuiCommand
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GuiCommandHandler));

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenProjectCommand"/> class.
        /// </summary>
        public OpenProjectCommand()
        {
            Enabled = true;
            Checked = false;
        }

        /// <summary>
        /// Gets a value indicating whether a <see cref="Project"/> has successfully been loaded.
        /// </summary>
        public bool LoadWasSuccesful { get; private set; }

        public bool Enabled { get; private set; }
        public bool Checked { get; private set; }

        public IGui Gui { get; set; }

        /// <summary>
        /// Start the operation of opening a specified project file.
        /// </summary>
        /// <param name="filePath">The file path of the project to open.</param>
        public void Execute(string filePath)
        {
            Execute(new object[]
            {
                filePath
            });
        }

        public void Execute(params object[] arguments)
        {
            LoadWasSuccesful = false;
            Log.Info(Resources.Project_existing_opening_project);

            var filePath = (string)arguments[0];
            Project loadedProject = TryReadProjectFromPath(filePath);
            if (loadedProject == null)
            {
                Log.Warn(Resources.Project_existing_project_opening_failed);
                return;
            }

            // Project loaded successfully, close current project
            if (Gui.Project != null)
            {
                // remove views before closing project. 
                Gui.CommandHandler.RemoveAllViewsForItem(Gui.Project);
            }

            Gui.ProjectFilePath = filePath;
            Gui.Project = loadedProject;
            Gui.Project.Name = Path.GetFileNameWithoutExtension(filePath);

            // Set the gui selection to the current project
            Gui.Selection = Gui.Project;

            // Update the window title
            Gui.UpdateTitle();

            LoadWasSuccesful = true;
            Log.Info(Resources.Project_existing_successfully_opened);
        }

        private Project TryReadProjectFromPath(string filePath)
        {
            IStoreProject storage = Gui.Storage;
            try
            {
                return storage.LoadProject(filePath);
            }
            catch (ArgumentException e)
            {
                Log.Warn(e.Message);
            }
            catch (CouldNotConnectException e)
            {
                Log.Warn(e.Message);
            }
            catch (StorageValidationException e)
            {
                Log.Warn(e.Message);
            }
            return null;
        }
    }
}