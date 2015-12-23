using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
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
        /// Creates a new instance of <see cref="AssessmentSectionBaseNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public AssessmentSectionBaseNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

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

        protected override ContextMenuStrip GetContextMenu(ITreeNode node, AssessmentSectionBase nodeData)
        {
            return contextMenuBuilderProvider
                .Get(node)
                .AddRenameItem()
                .AddDeleteItem()
                .AddSeparator()
                .AddImportItem()
                .AddExportItem()
                .AddSeparator()
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }
    }
}