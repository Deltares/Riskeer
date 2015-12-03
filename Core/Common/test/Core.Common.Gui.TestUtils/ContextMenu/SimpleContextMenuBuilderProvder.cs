using System;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui.ContextMenu;

namespace Core.Common.Gui.TestUtils.ContextMenu
{
    /// <summary>
    /// Proves a simple implementation of <see cref="IContextMenuBuilderProvider"/> to be
    /// used in tests.
    /// </summary>
    public class SimpleContextMenuBuilderProvder : IContextMenuBuilderProvider
    {
        private readonly IContextMenuBuilder contextMenuBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleContextMenuBuilderProvder"/> class.
        /// </summary>
        /// <param name="contextMenuBuilder">The context menu builder.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="contextMenuBuilder"/> is null.</exception>
        public SimpleContextMenuBuilderProvder(IContextMenuBuilder contextMenuBuilder)
        {
            if (contextMenuBuilder == null)
            {
                throw new ArgumentNullException("contextMenuBuilder");
            }
            this.contextMenuBuilder = contextMenuBuilder;
        }

        public IContextMenuBuilder Get(ITreeNode treeNode)
        {
            return contextMenuBuilder;
        }
    }
}