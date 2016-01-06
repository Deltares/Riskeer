namespace Core.Common.Controls.Commands
{
    /// <summary>
    /// Interface for defining the behavior of (Ribbon) buttons and/or menu items.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets whether or not the <see cref="ICommand"/> is enabled.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Gets whether or not the <see cref="ICommand"/> is checked.
        /// </summary>
        bool Checked { get; }

        /// <summary>
        /// This method implements the logic that should be performed after clicking the (Ribbon) button and/or menu item.
        /// </summary>
        /// <param name="arguments">The arguments to use during execution.</param>
        void Execute(params object[] arguments);
    }
}