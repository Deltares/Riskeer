using System.Collections.Generic;
using Core.Common.Controls;
using Core.Common.Controls.Commands;

namespace Core.Common.Gui.Forms
{
    /// <summary>
    /// Implemented in the gui plugin, used to extend ribbon control.
    /// </summary>
    public interface IRibbonCommandHandler
    {
        /// <summary>
        /// Call this action in the implementation when command needs to be handled in the graphical user interface.
        /// </summary>
        IEnumerable<Command> Commands { get; }

        /// <summary>
        /// Gets Ribbon control implementation in the gui plugin. Gui will merge it with the existing ribbon.
        /// </summary>
        object GetRibbonControl();

        /// <summary>
        /// Called by the gui when ribbon items need to be validated (e.g. enable/disable).
        /// </summary>
        void ValidateItems();

        /// <summary>
        /// Called when context changes (like selection, active window.).
        /// </summary>
        /// <param name="tabGroupName"></param>
        /// <param name="tabName"></param>
        /// <returns>Return false when contextual tab is not used.</returns>
        bool IsContextualTabVisible(string tabGroupName, string tabName);
    }
}