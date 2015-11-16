using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ringtoets.Common.Forms.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="ContextMenuStrip"/>.
    /// </summary>
    public static class ContextMenuStripExtensions
    {
        /// <summary>
        /// Adds a new <see cref="ToolStripMenuItem"/> with all standard parameters.
        /// </summary>
        /// <param name="parentMenu">The parent menu to which the new item is added.</param>
        /// <param name="text">Value for <see cref="ToolStripItem.Text"/>.</param>
        /// <param name="tooltip">Value for <see cref="ToolStripItem.ToolTipText"/>.</param>
        /// <param name="icon">Value for <see cref="ToolStripItem.Image"/>.</param>
        /// <param name="clickHandler">Method to handle the user clicking on the item..</param>
        /// <returns>The newly created menu item.</returns>
        public static ToolStripItem AddMenuItem(this ContextMenuStrip parentMenu, string text, string tooltip, Image icon, EventHandler clickHandler)
        {
            var newItem = new ToolStripMenuItem(text)
            {
                ToolTipText = tooltip,
                Image = icon
            };
            newItem.Click += clickHandler;

            parentMenu.Items.Add(newItem);

            return newItem;
        }
    }
}