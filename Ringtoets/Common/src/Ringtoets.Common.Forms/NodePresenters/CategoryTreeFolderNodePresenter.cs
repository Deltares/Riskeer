using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Ringtoets.Common.Forms.PresentationObjects;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="CategoryTreeFolder"/>.
    /// </summary>
    public class CategoryTreeFolderNodePresenter : RingtoetsNodePresenterBase<CategoryTreeFolder>
    {
        /// <summary>
        /// Sets the <see cref="IContextMenuBuilderProvider"/> to be used for creating the <see cref="ContextMenuStrip"/>.
        /// </summary>
        public IContextMenuBuilderProvider ContextMenuBuilderProvider { private get; set; }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, CategoryTreeFolder nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = GetFolderIcon(nodeData.Category);
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override IEnumerable GetChildNodeObjects(CategoryTreeFolder nodeData)
        {
            return nodeData.Contents;
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, CategoryTreeFolder nodeData)
        {
            return ContextMenuBuilderProvider
                .Get(sender)
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