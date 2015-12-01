using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;
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
        /// <summary>
        /// Sets the <see cref="IContextMenuBuilderProvider"/> to be used for creating the <see cref="ContextMenuStrip"/>.
        /// </summary>
        public IContextMenuBuilderProvider ContextMenuBuilderProvider { private get; set; }

        public override bool CanRenameNode(ITreeNode node)
        {
            return true;
        }

        public override bool CanRenameNodeTo(ITreeNode node, string newName)
        {
            return true;
        }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, AssessmentSectionBase nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = RingtoetsFormsResources.AssessmentSectionFolderIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override IEnumerable GetChildNodeObjects(AssessmentSectionBase nodeData)
        {
            yield return nodeData.ReferenceLine;
            yield return nodeData.FailureMechanismContribution;
            yield return nodeData.HydraulicBoundaryDatabase;
            foreach (var failureMechanism in nodeData.GetFailureMechanisms())
            {
                yield return failureMechanism;
            }
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

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, AssessmentSectionBase nodeData)
        {
            if (ContextMenuBuilderProvider == null)
            {
                return null;
            }
            return ContextMenuBuilderProvider
                .Get(sender)
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .AddSeparator()
                .AddImportItem()
                .AddExportItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }
    }
}