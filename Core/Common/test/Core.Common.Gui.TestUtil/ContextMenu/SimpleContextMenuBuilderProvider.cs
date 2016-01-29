using System;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;

namespace Core.Common.Gui.TestUtil.ContextMenu
{
    /// <summary>
    /// Proves a simple implementation of <see cref="IContextMenuBuilderProvider"/> to be
    /// used in tests.
    /// </summary>
    public class SimpleContextMenuBuilderProvider : IContextMenuBuilderProvider
    {
        private readonly IContextMenuBuilder contextMenuBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleContextMenuBuilderProvider"/> class.
        /// </summary>
        /// <param name="contextMenuBuilder">The context menu builder.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="contextMenuBuilder"/> is null.</exception>
        public SimpleContextMenuBuilderProvider(IContextMenuBuilder contextMenuBuilder)
        {
            if (contextMenuBuilder == null)
            {
                throw new ArgumentNullException("contextMenuBuilder");
            }
            this.contextMenuBuilder = contextMenuBuilder;
        }

        public IContextMenuBuilder Get(TreeNode treeNode, TreeNodeInfo treeNodeInfo)
        {
            return contextMenuBuilder;
        }
    }
}