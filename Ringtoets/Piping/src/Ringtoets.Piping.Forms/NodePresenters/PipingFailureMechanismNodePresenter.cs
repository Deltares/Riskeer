using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base.Workflow;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Helpers;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Service;

using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// This class presents the data on <see cref="PipingFailureMechanism"/> as a node in a 
    /// <see cref="ITreeView"/> and implements the way the user can interact with the node.
    /// </summary>
    public class PipingFailureMechanismNodePresenter : RingtoetsNodePresenterBase<PipingFailureMechanism>
    {
        public IContextMenuBuilderProvider ContextMenuBuilderProvider { private get; set; }

        /// <summary>
        /// Injection points for a method to cause a collection of <see cref="IActivity"/> to be scheduled for execution.
        /// </summary>
        public Action<IEnumerable<IActivity>> RunActivitiesAction { private get; set; }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingFailureMechanism nodeData)
        {
            node.Text = Resources.PipingFailureMechanism_DisplayName;
            node.Image = Resources.PipingIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override IEnumerable GetChildNodeObjects(PipingFailureMechanism failureMechanism)
        {
            yield return new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(failureMechanism), TreeFolderCategory.Input);
            yield return new PipingCalculationsTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Calculations_DisplayName, failureMechanism);
            yield return new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(failureMechanism), TreeFolderCategory.Output);
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, PipingFailureMechanism failureMechanism)
        {
            if (ContextMenuBuilderProvider == null)
            {
                return null;
            }

            var addCalculationItem = new StrictContextMenuItem(
                Resources.PipingFailureMechanism_Add_PipingCalculation,
                Resources.PipingFailureMechanism_Add_PipingCalculation_Tooltip,
                Resources.PipingIcon,
                (s, e) => AddCalculation(failureMechanism)
                );

            var addCalculationGroupItem = new StrictContextMenuItem(
                Resources.PipingFailureMechanism_Add_PipingCalculationGroup,
                Resources.PipingFailureMechanism_Add_PipingCalculationGroup_Tooltip,
                Resources.AddFolderIcon,
                (o, args) => AddCalculationGroup(failureMechanism)
                );

            var calculateAllItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                Resources.PipingFailureMechanism_Calculate_Tooltip,
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
                clearAllItem.ToolTipText = Resources.ClearOutput_No_calculation_with_output_to_clear;
            }

            return ContextMenuBuilderProvider.Get(sender)
                                             .AddCustomItem(addCalculationItem)
                                             .AddCustomItem(addCalculationGroupItem)
                                             .AddSeparator()
                                             .AddCustomItem(calculateAllItem)
                                             .AddCustomItem(clearAllItem)
                                             .AddSeparator()
                                             .AddExpandAllItem()
                                             .AddCollapseAllItem()
                                             .AddSeparator()
                                             .AddImportItem()
                                             .AddExportItem()
                                             .Build();
        }

        private static void ClearAll(PipingFailureMechanism failureMechanism)
        {
            foreach (PipingCalculation calc in GetAllPipingCalculationsResursively(failureMechanism))
            {
                calc.ClearOutput();
                calc.NotifyObservers();
            }
        }

        private void CalculateAll(PipingFailureMechanism failureMechanism)
        {
            RunActivitiesAction(GetAllPipingCalculationsResursively(failureMechanism).Select(calc => new PipingCalculationActivity(calc)));
        }

        private static void AddCalculationGroup(PipingFailureMechanism failureMechanism)
        {
            var calculation = new PipingCalculationGroup
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.Calculations, PipingDataResources.PipingCalculationGroup_DefaultName, c => c.Name)
            };
            failureMechanism.Calculations.Add(calculation);
            failureMechanism.NotifyObservers();
        }

        private static void AddCalculation(PipingFailureMechanism failureMechanism)
        {
            var calculation = new PipingCalculation
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.Calculations, PipingDataResources.PipingCalculation_DefaultName, c => c.Name)
            };
            failureMechanism.Calculations.Add(calculation);
            failureMechanism.NotifyObservers();
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
            return failureMechanism.Calculations.GetPipingCalculations().ToArray();
        }
    }
}