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
        /// <returns>The newly created <see cref="ToolStripItem"/>.</returns>
        public static ToolStripItem AddMenuItem(this ContextMenuStrip parentMenu, string text, string tooltip, Image icon, EventHandler clickHandler)
        {
            return parentMenu.InsertMenuItem(parentMenu.Items.Count, text, tooltip, icon, clickHandler);
        }

        /// <summary>
        /// Inserts a new <see cref="ToolStripMenuItem"/> with all standard parameters at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The position in the <see cref="ContextMenuStrip"/> to add the item to.</param>
        /// <param name="parentMenu">The parent menu to which the new item is added.</param>
        /// <param name="text">Value for <see cref="ToolStripItem.Text"/>.</param>
        /// <param name="tooltip">Value for <see cref="ToolStripItem.ToolTipText"/>.</param>
        /// <param name="icon">Value for <see cref="ToolStripItem.Image"/>.</param>
        /// <param name="clickHandler">Method to handle the user clicking on the item..</param>
        /// <returns>The newly created <see cref="ToolStripItem"/>.</returns>
        public static ToolStripItem InsertMenuItem(this ContextMenuStrip parentMenu, int position, string text, string tooltip, Image icon, EventHandler clickHandler)
        {
            var newItem = new ToolStripMenuItem(text)
            {
                ToolTipText = tooltip,
                Image = icon
            };
            newItem.Click += clickHandler;

            parentMenu.Items.Insert(position,newItem);

            return newItem;
        }

        /// <summary>
        /// Adds a new <see cref="ToolStripSeparator"/>.
        /// </summary>
        /// <param name="parentMenu">The parent menu to which the new item is added.</param>
        public static void AddSeperator(this ContextMenuStrip parentMenu)
        {
            parentMenu.InsertSeperator(parentMenu.Items.Count);
        }

        /// <summary>
        /// Insert a new <see cref="ToolStripSeparator"/> at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The position in the <see cref="ContextMenuStrip"/> to add the item to.</param>
        /// <param name="parentMenu">The parent menu to which the new item is added.</param>
        public static void InsertSeperator(this ContextMenuStrip parentMenu, int position)
        {
            var newItem = new ToolStripSeparator();

            parentMenu.Items.Insert(position, newItem);
        }
    }
}