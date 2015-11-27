using System.Windows.Forms;
using Core.Common.Controls;

namespace Core.Common.Gui.ContextMenu
{
    public class ContextMenuBuilder
    {
        private readonly ContextMenuItemFactory itemsFactory;
        private readonly ContextMenuStrip contextMenu;
        private readonly ITreeNode treeNode;

        public ContextMenuBuilder(IGui gui, ITreeNode treeNode)
        {
            contextMenu = new ContextMenuStrip();
            itemsFactory = new ContextMenuItemFactory(gui);
            this.treeNode = treeNode;
        }

        public ContextMenuBuilder AddExpandAllItem()
        {
            AddItem(itemsFactory.CreateExpandAllItem(treeNode));
            return this;
        }

        public ContextMenuBuilder AddCollapseAllItem()
        {
            AddItem(itemsFactory.CreateCollapseAllItem(treeNode));
            return this;
        }

        public ContextMenuBuilder AddExportItem()
        {
            AddItem(itemsFactory.CreateExportItem(treeNode));
            return this;
        }

        public ContextMenuBuilder AddImportItem()
        {
            AddItem(itemsFactory.CreateImportItem(treeNode));
            return this;
        }

        public ContextMenuBuilder AddPropertiesItem()
        {
            AddItem(itemsFactory.CreatePropertiesItem(treeNode));
            return this;
        }

        public ContextMenuBuilder AddSeparator()
        {
            if (MayAddSeparator())
            {
                AddItem(new ToolStripSeparator());
            }
            return this;
        }

        private bool MayAddSeparator()
        {
            if (contextMenu.Items.Count == 0)
            {
                return false;
            }
            return !(contextMenu.Items[contextMenu.Items.Count - 1] is ToolStripSeparator);
        }

        public ContextMenuBuilder AddCustomItem(ToolStripMenuItem item)
        {
            AddItem(item);
            return this;
        }

        public ContextMenuStrip Build()
        {
            return contextMenu;
        }

        private void AddItem(ToolStripItem item)
        {
            if (item != null)
            {
                contextMenu.Items.Add(item);
            }
        }
    }
}