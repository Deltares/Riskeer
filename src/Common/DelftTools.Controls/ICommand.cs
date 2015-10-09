using System.Drawing;

namespace DelftTools.Controls
{
    /// <summary>
    /// Command pattern implementation.
    /// </summary>
    public interface ICommand
    {
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
        /// HACK: command can't be checked, button associated with command can be checked! Find a better design.
        /// </summary>
        bool Checked { set; get; }

        /// <summary>
        /// Execute a Command with the supplied arguments.
        /// </summary>
        /// <param name="arguments">Arguments used in executing the Command.</param>
        void Execute(params object[] arguments);

        /// <summary>
        /// Unexecute or undo the Command. 
        /// </summary>
        void Unexecute();
    }
}