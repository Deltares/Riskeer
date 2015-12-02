using System;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui.Properties;

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
        /// <param name="treeNode">The <see cref="ITreeNode"/> for which to create a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="treeNode"/> is <c>null</c></item>
        /// <item><paramref name="commandHandler"/> is <c>null</c></item>
        /// </list></exception>
        public ContextMenuBuilder(IGuiCommandHandler commandHandler, ITreeNode treeNode)
        {
            if (commandHandler != null)
            {
                guiItemsFactory = new GuiContextMenuItemFactory(commandHandler, treeNode);
            }
            treeViewItemsFactory = new TreeViewContextMenuItemFactory(treeNode);
            contextMenu = new ContextMenuStrip();
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
            CheckGuiItemsFactory();
            AddItem(guiItemsFactory.CreateOpenItem());
            return this;
        }

        public IContextMenuBuilder AddExportItem()
        {
            CheckGuiItemsFactory();
            AddItem(guiItemsFactory.CreateExportItem());
            return this;
        }

        public IContextMenuBuilder AddImportItem()
        {
            CheckGuiItemsFactory();
            AddItem(guiItemsFactory.CreateImportItem());
            return this;
        }

        public IContextMenuBuilder AddPropertiesItem()
        {
            CheckGuiItemsFactory();
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

        private void CheckGuiItemsFactory()
        {
            if (guiItemsFactory == null)
            {
                throw new InvalidOperationException(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui);
            }
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