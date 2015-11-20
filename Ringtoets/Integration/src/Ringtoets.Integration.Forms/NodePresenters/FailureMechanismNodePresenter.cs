using System.Collections;
using System.Drawing;

using Core.Common.Controls;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.Properties;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.NodePresenters
{
    public class FailureMechanismNodePresenter : RingtoetsNodePresenterBase<FailureMechanismPlaceholder>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, FailureMechanismPlaceholder nodeData)
        {
            node.Text = nodeData.Name;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.GrayText);
            node.Image = Resources.FailureMechanismIcon;
        }

        protected override IEnumerable GetChildNodeObjects(FailureMechanismPlaceholder nodeData, ITreeNode node)
        {
            yield return new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(nodeData), TreeFolderCategory.Input);
            yield return new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(nodeData), TreeFolderCategory.Output);
        }

        private IEnumerable GetInputs(FailureMechanismPlaceholder nodeData)
        {
            yield return nodeData.SectionDivisions;
            yield return nodeData.Locations;
            yield return nodeData.BoundaryConditions;
        }

        private IEnumerable GetOutputs(FailureMechanismPlaceholder nodeData)
        {
            yield return nodeData.AssessmentResult;
        }
    }
}