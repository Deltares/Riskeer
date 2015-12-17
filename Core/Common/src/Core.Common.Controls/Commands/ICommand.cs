namespace Core.Common.Controls.Commands
{
    /// <summary>
    /// Abstract class that can be derived for defining the behaviour of (Ribbon) buttons and/or menu items.
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
        void Execute(params object[] arguments);
    }
}