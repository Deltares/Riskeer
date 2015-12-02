using System;
using System.Drawing;
using System.Windows.Forms;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// Class used by the <see cref="ContextMenuBuilder"/> to enforce instantiation the following properties.
    /// <list type="bullet">
    /// <item><see cref="ToolStripMenuItem.Text"/></item>
    /// <item><see cref="ToolStripItem.ToolTipText"/></item>
    /// <item><see cref="ToolStripMenuItem.Image"/></item>
    /// <item><see cref="ToolStripMenuItem.Click"/></item>
    /// </list>
    /// </summary>
    public sealed class StrictContextMenuItem : ToolStripMenuItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="StrictContextMenuItem"/>.
        /// </summary>
        /// <param name="text">The text of the <see cref="StrictContextMenuItem"/>.</param>
        /// <param name="toolTip">The tooltip of the <see cref="StrictContextMenuItem"/>.</param>
        /// <param name="image">The icon used for the <see cref="StrictContextMenuItem"/>.</param>
        /// <param name="clickHandler">The handler for a mouse click on the created
        /// <see cref="StrictContextMenuItem"/>.</param>
        public StrictContextMenuItem(string text, string toolTip, Image image, EventHandler clickHandler)
        {
            Text = text;
            ToolTipText = toolTip;
            Image = image;
            Click += clickHandler;
        }
    }
}