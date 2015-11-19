using System.Collections;

using Core.Common.Base;
using Core.Common.Controls;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Integration.Data;

using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="Ringtoets.Integration.Data.AssessmentSectionBase"/> items in the tree view.
    /// </summary>
    public class AssessmentSectionBaseNodePresenter : RingtoetsNodePresenterBase<AssessmentSectionBase>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, AssessmentSectionBase nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = RingtoetsFormsResources.AssessmentSectionFolderIcon;
        }

        protected override IEnumerable GetChildNodeObjects(AssessmentSectionBase nodeData, ITreeNode node)
        {
            yield return nodeData.ReferenceLine;
            yield return nodeData.FailureMechanismContribution;
            yield return nodeData.HydraulicBoundaryDatabase;
            foreach (var failureMechanism in nodeData.GetFailureMechanisms())
            {
                yield return failureMechanism;
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

        protected override void OnNodeRenamed(AssessmentSectionBase nodeData, string newName)
        {
            nodeData.Name = newName;
            nodeData.NotifyObservers();
        }

        protected override bool CanRemove(object parentNodeData, AssessmentSectionBase nodeData)
        {
            return true;
        }

        protected override bool RemoveNodeData(object parentNodeData, AssessmentSectionBase nodeData)
        {
            var parentProject = (Project) parentNodeData;

            parentProject.Items.Remove(nodeData);
            parentProject.NotifyObservers();

            return true;
        }
    }
}