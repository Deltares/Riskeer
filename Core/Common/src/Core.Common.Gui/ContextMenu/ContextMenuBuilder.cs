using System;
using System.Windows.Forms;
using Core.Common.Gui.Properties;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// This class represents a builder which can be used to create a context menu, which's items require information
    /// on the <see cref="IGui"/> in order to render or perform actions.
    /// </summary>
    public class ContextMenuBuilder : IContextMenuBuilder
    {
        private readonly GuiContextMenuItemFactory guiItemsFactory;
        private readonly TreeViewContextMenuItemFactory treeViewItemsFactory;
        private readonly ContextMenuStrip contextMenu;

        /// <summary>
        /// Creates a new instance of <see cref="ContextMenuBuilder"/>, which uses the given <paramref name="commandHandler"/> to 
        /// create a <see cref="ContextMenuStrip"/> for the given <paramref name="treeNode"/>.
        /// </summary>
        /// <param name="commandHandler">The <see cref="IGuiCommandHandler"/> from which to obtain information to render and bind actions
        /// to the items of the <see cref="ContextMenu"/>. If <c>null</c>, this builder will not render items which
        /// require this type of information.</param>
        /// <param name="importExportHandler">The <see cref="IExportImportCommandHandler"/> 
        /// from which to obtain information to render and bind actions to the items of the 
        /// <see cref="ContextMenu"/>. If <c>null</c>, this builder will not render items
        /// which require this type of information.</param>
        /// <param name="treeNode">The <see cref="Controls.TreeView.TreeNode"/> for which to create a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ContextMenuBuilderException">Thrown when the required object instances could not be created based on
        /// the <paramref name="commandHandler"/> or <paramref name="treeNode"/>.</exception>
        public ContextMenuBuilder(IGuiCommandHandler commandHandler, IExportImportCommandHandler importExportHandler, TreeNode treeNode)
        {
            try
            {
                guiItemsFactory = new GuiContextMenuItemFactory(commandHandler, importExportHandler, treeNode);
                treeViewItemsFactory = new TreeViewContextMenuItemFactory(treeNode);
            }
            catch (ArgumentNullException e)
            {
                throw new ContextMenuBuilderException(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, e);
            }
            contextMenu = new ContextMenuStrip();
        }

        public IContextMenuBuilder AddRenameItem()
        {
            AddItem(treeViewItemsFactory.CreateRenameItem());
            return this;
        }

        public IContextMenuBuilder AddDeleteItem()
        {
            AddItem(treeViewItemsFactory.CreateDeleteItem());
            return this;
        }

        public IContextMenuBuilder AddExpandAllItem()
        {
            AddItem(treeViewItemsFactory.CreateExpandAllItem());
            return this;
        }

        public IContextMenuBuilder AddCollapseAllItem()
        {
            AddItem(treeViewItemsFactory.CreateCollapseAllItem());
            return this;
        }

        public IContextMenuBuilder AddOpenItem()
        {
            AddItem(guiItemsFactory.CreateOpenItem());
            return this;
        }

        public IContextMenuBuilder AddExportItem()
        {
            AddItem(guiItemsFactory.CreateExportItem());
            return this;
        }

        public IContextMenuBuilder AddImportItem()
        {
            AddItem(guiItemsFactory.CreateImportItem());
            return this;
        }

        public IContextMenuBuilder AddPropertiesItem()
        {
            AddItem(guiItemsFactory.CreatePropertiesItem());
            return this;
        }

        public IContextMenuBuilder AddSeparator()
        {
            if (MayAddSeparator())
            {
                AddItem(new ToolStripSeparator());
            }
            return this;
        }

        public IContextMenuBuilder AddCustomItem(StrictContextMenuItem item)
        {
            AddItem(item);
            return this;
        }

        public ContextMenuStrip Build()
        {
            if (contextMenu.Items.Count > 0)
            {
                var lastIndex = contextMenu.Items.Count - 1;
                if (contextMenu.Items[lastIndex] is ToolStripSeparator)
                {
                    contextMenu.Items.RemoveAt(lastIndex);
                }
            }
            return contextMenu;
        }

        private bool MayAddSeparator()
        {
            if (contextMenu.Items.Count == 0)
            {
                return false;
            }
            return !(contextMenu.Items[contextMenu.Items.Count - 1] is ToolStripSeparator);
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