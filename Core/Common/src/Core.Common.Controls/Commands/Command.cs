namespace Core.Common.Controls.Commands
{
    /// <summary>
    /// Abstract class that can be derivied for defining the behaviour of (Ribbon) buttons and/or menu items.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Gets whether or not the <see cref="Command"/> is enabled.
        /// </summary>
        public abstract bool Enabled { get; }

        /// <summary>
        /// Gets whether or not the <see cref="Command"/> is checked.
        /// </summary>
        public abstract bool Checked { get; }

        /// <summary>
        /// This method implements the logic that should be performed after clicking the (Ribbon) button and/or menu item.
        /// </summary>
        public abstract void Execute(params object[] arguments);
    }
}