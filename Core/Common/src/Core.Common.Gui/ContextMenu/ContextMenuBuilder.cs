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
    public class ContextMenuBuilder
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
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="treeNode"/> is <c>null</c>.</exception>
        public ContextMenuBuilder(IGuiCommandHandler commandHandler, ITreeNode treeNode)
        {
            if (commandHandler != null)
            {
                guiItemsFactory = new GuiContextMenuItemFactory(commandHandler);
            }
            treeViewItemsFactory = new TreeViewContextMenuItemFactory(treeNode);
            contextMenu = new ContextMenuStrip();
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which expands the <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself, so that operations can be easily chained.</returns>
        public ContextMenuBuilder AddExpandAllItem()
        {
            AddItem(treeViewItemsFactory.CreateExpandAllItem());
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which collapses the <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself, so that operations can be easily chained.</returns>
        public ContextMenuBuilder AddCollapseAllItem()
        {
            AddItem(treeViewItemsFactory.CreateCollapseAllItem());
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which opens a view for the data of the <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself, so that operations can be easily chained.</returns>
        /// <remarks>If the <see cref="IGui"/> was not passed on construction, this method will not add an item.</remarks>
        public ContextMenuBuilder AddOpenItem()
        {
            if (guiItemsFactory != null)
            {
                AddItem(guiItemsFactory.CreateOpenItem());
            }
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which exports the data of the <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself, so that operations can be easily chained.</returns>
        /// <remarks>If the <see cref="IGui"/> was not passed on construction, this method will not add an item.</remarks>
        public ContextMenuBuilder AddExportItem()
        {
            if (guiItemsFactory != null)
            {
                AddItem(guiItemsFactory.CreateExportItem());
            }
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which imports the data of the <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself, so that operations can be easily chained.</returns>
        /// <remarks>If the <see cref="IGui"/> was not passed on construction, this method will not add an item.</remarks>
        public ContextMenuBuilder AddImportItem()
        {
            if (guiItemsFactory != null)
            {
                AddItem(guiItemsFactory.CreateImportItem());
            }
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which shows properties of the data of the <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself, so that operations can be easily chained.</returns>
        /// <remarks>If the <see cref="IGui"/> was not passed on construction, this method will not add an item.</remarks>
        public ContextMenuBuilder AddPropertiesItem()
        {
            if (guiItemsFactory != null)
            {
                AddItem(guiItemsFactory.CreatePropertiesItem());
            }
            return this;
        }

        /// <summary>
        /// Adds a <see cref="ToolStripSeparator"/> to the <see cref="ContextMenuStrip"/>. A <see cref="ToolStripSeparator"/>
        /// is only added if the last item that was added to the <see cref="ContextMenuStrip"/> exists and is not a 
        /// <see cref="ToolStripSeparator"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself, so that operations can be easily chained.</returns>
        public ContextMenuBuilder AddSeparator()
        {
            if (MayAddSeparator())
            {
                AddItem(new ToolStripSeparator());
            }
            return this;
        }

        /// <summary>
        /// Adds a custom item to the <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="item">The custom <see cref="StrictContextMenuItem"/> to add to the <see cref="ContextMenuStrip"/>.</param>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself, so that operations can be easily chained.</returns>
        public ContextMenuBuilder AddCustomItem(StrictContextMenuItem item)
        {
            AddItem(item);
            return this;
        }

        /// <summary>
        /// Obtain the <see cref="ContextMenuStrip"/>, which has been constructed by using the other methods of
        /// <see cref="ContextMenuBuilder"/>.
        /// </summary>
        /// <returns>The constructed <see cref="ContextMenuStrip"/>.</returns>
        public ContextMenuStrip Build()
        {
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