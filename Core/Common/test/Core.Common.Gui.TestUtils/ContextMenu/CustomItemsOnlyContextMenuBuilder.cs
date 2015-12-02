using System.Windows.Forms;
using Core.Common.Gui.ContextMenu;

namespace Core.Common.Gui.TestUtils.ContextMenu
{
    public class CustomItemsOnlyContextMenuBuilder : IContextMenuBuilder
    {
        private readonly ContextMenuStrip contextMenu = new ContextMenuStrip();

        public IContextMenuBuilder AddDeleteItem()
        {
            return this;
        }

        public IContextMenuBuilder AddExpandAllItem()
        {
            return this;
        }

        public IContextMenuBuilder AddCollapseAllItem()
        {
            return this;
        }

        public IContextMenuBuilder AddOpenItem()
        {
            return this;
        }

        public IContextMenuBuilder AddExportItem()
        {
            return this;
        }

        public IContextMenuBuilder AddImportItem()
        {
            return this;
        }

        public IContextMenuBuilder AddPropertiesItem()
        {
            return this;
        }

        public IContextMenuBuilder AddSeparator()
        {
            return this;
        }

        public IContextMenuBuilder AddCustomItem(StrictContextMenuItem item)
        {
            contextMenu.Items.Add(item);
            return this;
        }

        public ContextMenuStrip Build()
        {
            return contextMenu;
        }
    }
}