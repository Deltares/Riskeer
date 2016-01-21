using Core.Common.Base.Data;
using Core.Common.Gui.Properties;

using log4net;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Command to create a new <see cref="Project"/> instance for the application.
    /// </summary>
    public class CreateNewProjectCommand : IGuiCommand
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CreateNewProjectCommand));

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNewProjectCommand"/> class.
        /// </summary>
        public CreateNewProjectCommand()
        {
            Enabled = true;
            Checked = false;
        }

        public bool Enabled { get; private set; }
        public bool Checked { get; private set; }

        public IGui Gui { get; set; }

        public void Execute(params object[] arguments)
        {
            if (Gui.Project != null)
            {
                // remove views before closing project to prevent leaks or accessing disposed instances.. 
                Gui.CommandHandler.RemoveAllViewsForItem(Gui.Project);
            }

            log.Info(Resources.Project_new_opening);
            Gui.Project = new Project();
            log.Info(Resources.Project_new_successfully_opened);

            // Set the gui selection to the current project
            Gui.Selection = Gui.Project;

            // Update the window title
            Gui.UpdateTitle();
        }
    }
}