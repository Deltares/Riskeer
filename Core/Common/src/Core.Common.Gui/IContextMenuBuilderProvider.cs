using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface which describes classes that are able to provide a <see cref="ContextMenuBuilder"/>.
    /// </summary>
    public interface IContextMenuBuilderProvider
    {
        /// <summary>
        /// Returns a new <see cref="ContextMenuBuilder"/> for creating a <see cref="ContextMenuStrip"/>
        /// for the given <paramref name="treeNode"/>.
        /// </summary>
        /// <param name="treeNode">The <see cref="ITreeNode"/> to have the <see cref="ContextMenuBuilder"/>
        /// create a <see cref="ContextMenuStrip"/> for.</param>
        /// <returns>The <see cref="ContextMenuBuilder"/> which can be used to create a <see cref="ContextMenuStrip"/>
        /// for <paramref name="treeNode"/>.</returns>
        /// <exception cref="ContextMenuBuilderException">Thrown when the <see cref="IContextMenuBuilder"/> instance could
        /// not be created.</exception>
        IContextMenuBuilder Get(ITreeNode treeNode);
    }
}