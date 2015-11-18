using System.Collections;

using Core.Common.Base;
using Core.Common.Controls;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Integration.Data;

using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="Ringtoets.Integration.Data.DikeAssessmentSection"/> items in the tree view.
    /// </summary>
    public class DikeAssessmentSectionNodePresenter : RingtoetsNodePresenterBase<DikeAssessmentSection>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, DikeAssessmentSection nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = RingtoetsFormsResources.AssessmentSectionFolderIcon;
        }

        protected override IEnumerable GetChildNodeObjects(DikeAssessmentSection nodeData, ITreeNode node)
        {
            if (nodeData.PipingFailureMechanism != null)
            {
                yield return nodeData.PipingFailureMechanism;
            }
        }

        public override bool CanRenameNode(ITreeNode node)
        {
            return true;
        }

        public override bool CanRenameNodeTo(ITreeNode node, string newName)
        {
            return true;
        }

        protected override void OnNodeRenamed(DikeAssessmentSection nodeData, string newName)
        {
            nodeData.Name = newName;
            nodeData.NotifyObservers();
        }

        protected override bool CanRemove(object parentNodeData, DikeAssessmentSection nodeData)
        {
            return true;
        }

        protected override bool RemoveNodeData(object parentNodeData, DikeAssessmentSection nodeData)
        {
            var parentProject = (Project) parentNodeData;

            parentProject.Items.Remove(nodeData);
            parentProject.NotifyObservers();

            return true;
        }
    }
}