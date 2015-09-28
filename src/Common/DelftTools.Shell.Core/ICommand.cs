using System.Drawing;

namespace DelftTools.Core
{
    /// <summary>
    /// Command pattern implementation.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Execute a Command with the supplied arguments.
        /// </summary>
        /// <param name="arguments">Arguments used in executing the Command.</param>
        void Execute(params object[] arguments);

        /// <summary>
        /// Unexecute or undo the Command. 
        /// </summary>
        void Unexecute();

        /// <summary>
        /// Name of the Command. This might show up in the toolbar button.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Commands can be disabled if they cannot be executed.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Image displayed commandtext in interface
        /// </summary>
        Image Image { set; get; }

        /// <summary>
        /// Commands can checked if they represent a (boolean) state.
        /// HACK: command can't be checked, button associated with command can be checked! Find a better design. Command in general an action (non-gui as well)
        /// </summary>
        bool Checked { set; get; }
    }
}