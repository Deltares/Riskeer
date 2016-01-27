using System.Collections.Generic;

namespace Core.Common.Gui
{
    /// <summary>
    /// Object that holds the commands available within the application.
    /// </summary>
    public interface ICommandsOwner
    {
        /// <summary>
        /// Gets or sets CommandHandler.
        /// </summary>
        IApplicationFeatureCommands ApplicationCommands { get; }

        IStorageCommands StorageCommands { get; }

        IProjectCommands ProjectCommands { get; }

        IViewCommands ViewCommands { get; }

        /// <summary>
        /// Gets commands.
        /// </summary>
        IList<IGuiCommand> Commands { get; }
    }
}