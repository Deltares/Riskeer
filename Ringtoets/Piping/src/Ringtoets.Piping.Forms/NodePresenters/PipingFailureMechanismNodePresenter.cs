using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Service;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Service;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// This class presents the data on <see cref="PipingFailureMechanism"/> as a node in a 
    /// <see cref="ITreeView"/> and implements the way the user can interact with the node.
    /// </summary>
    public class PipingFailureMechanismNodePresenter : RingtoetsNodePresenterBase<PipingFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="EmptyPipingCalculationReportNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public PipingFailureMechanismNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

        /// <summary>
        /// Injection points for a method to cause a collection of <see cref="Activity"/> to be scheduled for execution.
        /// </summary>
        public Action<IEnumerable<Activity>> RunActivitiesAction { private get; set; }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingFailureMechanism nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = PipingFormsResources.PipingIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override IEnumerable GetChildNodeObjects(PipingFailureMechanism failureMechanism)
        {
            yield return new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(failureMechanism), TreeFolderCategory.Input);
            yield return new PipingCalculationGroupContext(failureMechanism.CalculationsGroup, failureMechanism.SurfaceLines, failureMechanism.SoilProfiles);
            yield return new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(failureMechanism), TreeFolderCategory.Output);
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode node, PipingFailureMechanism failureMechanism)
        {
            var addCalculationGroupItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup,
                PipingFormsResources.PipingFailureMechanism_Add_PipingCalculationGroup_Tooltip,
                PipingFormsResources.AddFolderIcon,
                (o, args) => AddCalculationGroup(failureMechanism, node)
                );

            var addCalculationItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation,
                PipingFormsResources.PipingFailureMechanism_Add_PipingCalculation_Tooltip,
                PipingFormsResources.PipingIcon,
                (s, e) => AddCalculation(failureMechanism, node)
                );

            var validateAllItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationItem_Validate,
                PipingFormsResources.PipingFailureMechanism_ValidateAll_Tooltip,
                PipingFormsResources.ValidationIcon,
                (o, args) => ValidateAll(failureMechanism)
                );

            var calculateAllItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                RingtoetsCommonFormsResources.Calculate_all_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (o, args) => CalculateAll(failureMechanism)
                );

            var clearAllItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Clear_all_output,
                RingtoetsCommonFormsResources.Clear_all_output_ToolTip,
                RingtoetsCommonFormsResources.ClearIcon,
                (o, args) => ClearAll(failureMechanism)
                );

            if (!GetAllPipingCalculationsResursively(failureMechanism).Any(c => c.HasOutput))
            {
                clearAllItem.Enabled = false;
                clearAllItem.ToolTipText = PipingFormsResources.PipingCalculationGroup_ClearOutput_No_calculation_with_output_to_clear;
            }

            return contextMenuBuilderProvider.Get(node)
                                             .AddCustomItem(addCalculationGroupItem)
                                             .AddCustomItem(addCalculationItem)
                                             .AddSeparator()
                                             .AddCustomItem(validateAllItem)
                                             .AddCustomItem(calculateAllItem)
                                             .AddCustomItem(clearAllItem)
                                             .AddSeparator()
                                             .AddImportItem()
                                             .AddExportItem()
                                             .AddSeparator()
                                             .AddExpandAllItem()
                                             .AddCollapseAllItem()
                                             .Build();
        }

        private static void ClearAll(PipingFailureMechanism failureMechanism)
        {
            if (MessageBox.Show(PipingFormsResources.PipingCalculationGroupContextNodePresenter_GetContextMenu_Are_you_sure_clear_all_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }
            foreach (PipingCalculation calc in GetAllPipingCalculationsResursively(failureMechanism))
            {
                calc.ClearOutput();
                calc.NotifyObservers();
            }
        }

        private void ValidateAll(PipingFailureMechanism failureMechanism)
        {
            foreach (PipingCalculation calculation in GetAllPipingCalculationsResursively(failureMechanism))
            {
                PipingCalculationService.Validate(calculation);
            }
        }

        private void CalculateAll(PipingFailureMechanism failureMechanism)
        {
            RunActivitiesAction(GetAllPipingCalculationsResursively(failureMechanism).Select(calc => new PipingCalculationActivity(calc)));
        }

        private void AddCalculationGroup(PipingFailureMechanism failureMechanism, ITreeNode failureMechanismNode)
        {
            var calculation = new PipingCalculationGroup
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, PipingDataResources.PipingCalculationGroup_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.NotifyObservers();

            SelectNewlyAddedItemInTreeView(failureMechanismNode);
        }

        private void AddCalculation(PipingFailureMechanism failureMechanism, ITreeNode failureMechanismNode)
        {
            var calculation = new PipingCalculation
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, PipingDataResources.PipingCalculation_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.NotifyObservers();

            SelectNewlyAddedItemInTreeView(failureMechanismNode);
        }

        private void SelectNewlyAddedItemInTreeView(ITreeNode failureMechanismNode)
        {
            if (!failureMechanismNode.IsExpanded)
            {
                failureMechanismNode.Expand();
            }

            // Childnode at index 1 is the PipingCalculationGroup where the new item has been added:
            ITreeNode failureMechanismsCalculationsNode = failureMechanismNode.Nodes[1];

            // New childnode is appended at the end of PipingCalculationGroup:
            ITreeNode newlyAddedGroupNode = failureMechanismsCalculationsNode.Nodes.Last();
            if (!failureMechanismsCalculationsNode.IsExpanded)
            {
                failureMechanismsCalculationsNode.Expand();
            }
            TreeView.SelectedNode = newlyAddedGroupNode;
        }

        private static IEnumerable GetInputs(PipingFailureMechanism failureMechanism)
        {
            yield return failureMechanism.SectionDivisions;
            yield return failureMechanism.SurfaceLines;
            yield return failureMechanism.SoilProfiles;
            yield return failureMechanism.BoundaryConditions;
        }

        private IEnumerable GetOutputs(PipingFailureMechanism failureMechanism)
        {
            yield return failureMechanism.AssessmentResult;
        }

        private static IEnumerable<PipingCalculation> GetAllPipingCalculationsResursively(PipingFailureMechanism failureMechanism)
        {
            return failureMechanism.CalculationsGroup.GetPipingCalculations().ToArray();
        }
    }
}