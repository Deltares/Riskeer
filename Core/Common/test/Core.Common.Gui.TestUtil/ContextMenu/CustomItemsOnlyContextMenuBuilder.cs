using System.Windows.Forms;
using Core.Common.Gui.ContextMenu;

namespace Core.Common.Gui.TestUtil.ContextMenu
{
    /// <summary>
    /// This class can be used for easily testing the custom items which are added to a context menu.
    /// </summary>
    public class CustomItemsOnlyContextMenuBuilder : IContextMenuBuilder
    {
        /// <summary>
        /// The context menu which is build.
        /// </summary>
        private readonly ContextMenuStrip contextMenu = new ContextMenuStrip();

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddRenameItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddDeleteItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddExpandAllItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddCollapseAllItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddOpenItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddExportItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddImportItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddPropertiesItem()
        {
            return this;
        }

        /// <summary>
        /// Adds a toolstrip separator.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddSeparator()
        {
            contextMenu.Items.Add(new ToolStripSeparator());
            return this;
        }

        /// <summary>
        /// Adds a custom item to the <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="item">The custom <see cref="StrictContextMenuItem"/> to add to the <see cref="ContextMenuStrip"/>.</param>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddCustomItem(StrictContextMenuItem item)
        {
            contextMenu.Items.Add(item);
            return this;
        }

        /// <summary>
        /// Obtain the <see cref="ContextMenuStrip"/>, which has been constructed by using <see cref="AddCustomItem"/>.
        /// </summary>
        /// <returns>The constructed <see cref="ContextMenuStrip"/>.</returns>
        public ContextMenuStrip Build()
        {
            return contextMenu;
        }
    }
}