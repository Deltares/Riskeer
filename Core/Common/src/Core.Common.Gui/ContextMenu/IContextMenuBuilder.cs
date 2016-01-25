using System;
using System.Windows.Forms;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// Specifies the interface for objects that build context menus.
    /// </summary>
    public interface IContextMenuBuilder 
    {

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which starts edit mode for the name of <see cref="Controls.TreeView.TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddRenameItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which deletes the <see cref="Controls.TreeView.TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddDeleteItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which expands the <see cref="Controls.TreeView.TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddExpandAllItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which collapses the <see cref="Controls.TreeView.TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddCollapseAllItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which opens a view for the data of the <see cref="Controls.TreeView.TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="IApplicationFeatureCommands"/> was not passed on construction.</exception>
        IContextMenuBuilder AddOpenItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which exports the data of the <see cref="Controls.TreeView.TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="IApplicationFeatureCommands"/> was not passed on construction.</exception>
        IContextMenuBuilder AddExportItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which imports the data of the <see cref="Controls.TreeView.TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="IApplicationFeatureCommands"/> was not passed on construction.</exception>
        IContextMenuBuilder AddImportItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which shows properties of the data of the <see cref="Controls.TreeView.TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="IApplicationFeatureCommands"/> was not passed on construction.</exception>
        IContextMenuBuilder AddPropertiesItem();

        /// <summary>
        /// Adds a <see cref="ToolStripSeparator"/> to the <see cref="ContextMenuStrip"/>. A <see cref="ToolStripSeparator"/>
        /// is only added if the last item that was added to the <see cref="ContextMenuStrip"/> exists and is not a 
        /// <see cref="ToolStripSeparator"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddSeparator();

        /// <summary>
        /// Adds a custom item to the <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="item">The custom <see cref="StrictContextMenuItem"/> to add to the <see cref="ContextMenuStrip"/>.</param>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddCustomItem(StrictContextMenuItem item);

        /// <summary>
        /// Obtain the <see cref="ContextMenuStrip"/>, which has been constructed by using the other methods of
        /// <see cref="IContextMenuBuilder"/>.
        /// </summary>
        /// <returns>The constructed <see cref="ContextMenuStrip"/>.</returns>
        ContextMenuStrip Build();
    }
}