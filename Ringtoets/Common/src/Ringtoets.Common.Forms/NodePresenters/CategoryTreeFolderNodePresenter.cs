using System;
using System.Collections;
using System.Drawing;

using Core.Common.Controls;

using Ringtoets.Common.Forms.PresentationObjects;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="CategoryTreeFolder"/>.
    /// </summary>
    public class CategoryTreeFolderNodePresenter : RingtoetsNodePresenterBase<CategoryTreeFolder>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, CategoryTreeFolder nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = GetFolderIcon(nodeData.Category);
        }

        protected override IEnumerable GetChildNodeObjects(CategoryTreeFolder nodeData, ITreeNode node)
        {
            return nodeData.Contents;
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