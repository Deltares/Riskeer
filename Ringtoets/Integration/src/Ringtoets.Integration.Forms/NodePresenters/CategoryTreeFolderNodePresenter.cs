using System;
using System.Collections;
using System.Drawing;

using Core.Common.Controls;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Integration.Forms.PresentationObjects;

using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.NodePresenters
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
                    return RingtoetsFormsResources.GeneralFolderIcon;
                case TreeFolderCategory.Input:
                    return RingtoetsFormsResources.InputFolderIcon;
                case TreeFolderCategory.Output:
                    return RingtoetsFormsResources.OutputFolderIcon;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}