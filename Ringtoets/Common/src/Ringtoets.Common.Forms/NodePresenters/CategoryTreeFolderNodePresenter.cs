using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Gui;
using Ringtoets.Common.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

namespace Ringtoets.Common.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="CategoryTreeFolder"/>.
    /// </summary>
    public class CategoryTreeFolderNodePresenter : RingtoetsNodePresenterBase<CategoryTreeFolder>
    {
        /// <summary>
        /// Creates a new instance of <see cref="CategoryTreeFolderNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public CategoryTreeFolderNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

        protected override void UpdateNode(TreeNode parentNode, TreeNode node, CategoryTreeFolder nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = GetFolderIcon(nodeData.Category);
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override IEnumerable GetChildNodeObjects(CategoryTreeFolder nodeData)
        {
            return nodeData.Contents;
        }

        protected override ContextMenuStrip GetContextMenu(TreeNode node, CategoryTreeFolder nodeData)
        {
            return contextMenuBuilderProvider
                .Get(node)
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .Build();
        }

        private Image GetFolderIcon(TreeFolderCategory category)
        {
            switch (category)
            {
                case TreeFolderCategory.General:
                    return RingtoetsCommonFormsResources.GeneralFolderIcon;
                case TreeFolderCategory.Input:
                    return RingtoetsCommonFormsResources.InputFolderIcon;
                case TreeFolderCategory.Output:
                    return RingtoetsCommonFormsResources.OutputFolderIcon;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}