using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Ringtoets.Common.Forms.Extensions;
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

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, FailureMechanismPlaceholder nodeData)
        {
            var contextMenu = new ContextMenuStrip();

            contextMenu.AddMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                RingtoetsCommonFormsResources.Calculate_all_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon, null);
            contextMenu.AddMenuItem(
                RingtoetsCommonFormsResources.Clear_all_output,
                RingtoetsCommonFormsResources.Clear_all_output_ToolTip,
                RingtoetsCommonFormsResources.ClearOutputIcon, null);
            contextMenu.AddSeperator();

            contextMenu.AddMenuItem(
                RingtoetsCommonFormsResources.FailureMechanism_Export,
                RingtoetsCommonFormsResources.FailureMechanism_Export_ToolTip,
                RingtoetsCommonFormsResources.ExportIcon, null);
            contextMenu.AddSeperator();

            contextMenu.AddMenuItem(
                RingtoetsCommonFormsResources.FailureMechanism_Expand_all,
                RingtoetsCommonFormsResources.FailureMechanism_Expand_all_ToolTip,
                RingtoetsCommonFormsResources.ExpandAllIcon, null);
            contextMenu.AddMenuItem(
                RingtoetsCommonFormsResources.FailureMechanism_Collapse_all,
                RingtoetsCommonFormsResources.FailureMechanism_Collapse_all_ToolTip,
                RingtoetsCommonFormsResources.CollapseAllIcon, null);
            contextMenu.AddSeperator();

            contextMenu.AddMenuItem(
                RingtoetsCommonFormsResources.FailureMechanism_Properties,
                RingtoetsCommonFormsResources.FailureMechanism_Properties_ToolTip,
                RingtoetsCommonFormsResources.PropertiesIcon, null);

            return contextMenu;
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