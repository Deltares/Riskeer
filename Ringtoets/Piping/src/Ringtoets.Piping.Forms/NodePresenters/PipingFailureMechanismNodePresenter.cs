﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base.Workflow;
using Core.Common.Controls;

using Ringtoets.Common.Forms.Extensions;
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
        /// <summary>
        /// Injection points for a method to cause an <see cref="IActivity"/> to be scheduled for execution.
        /// </summary>
        public Action<IActivity> RunActivityAction { private get; set; }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingFailureMechanism nodeData)
        {
            node.Text = Resources.PipingFailureMechanism_DisplayName;
            node.Image = Resources.PipingIcon;
        }

        protected override IEnumerable GetChildNodeObjects(PipingFailureMechanism failureMechanism)
        {
            yield return new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(failureMechanism), TreeFolderCategory.Input);
            yield return new PipingCalculationsTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Calculations_DisplayName, failureMechanism);
            yield return new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(failureMechanism), TreeFolderCategory.Output);
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, PipingFailureMechanism failureMechanism)
        {
            var rootMenu = new ContextMenuStrip();

            rootMenu.AddMenuItem(Resources.PipingFailureMechanism_Add_PipingCalculation,
                                 Resources.PipingFailureMechanism_Add_PipingCalculation_Tooltip,
                                 Resources.PipingIcon, (o, args) =>
                                 {
                                     var calculation = new PipingCalculation
                                     {
                                         Name = NamingHelper.GetUniqueName(failureMechanism.Calculations, PipingDataResources.PipingCalculation_DefaultName, c => c.Name)
                                     };
                                     failureMechanism.Calculations.Add(calculation);
                                     failureMechanism.NotifyObservers();
                                 });
            rootMenu.AddMenuItem(Resources.PipingFailureMechanism_Add_PipingCalculationGroup,
                                 Resources.PipingFailureMechanism_Add_PipingCalculationGroup_Tooltip,
                                 Resources.AddFolderIcon, (o, args) =>
                                 {
                                     var calculation = new PipingCalculationGroup
                                     {
                                         Name = NamingHelper.GetUniqueName(failureMechanism.Calculations, PipingDataResources.PipingCalculationGroup_DefaultName, c => c.Name)
                                     };
                                     failureMechanism.Calculations.Add(calculation);
                                     failureMechanism.NotifyObservers();
                                 });
            rootMenu.AddMenuItem(RingtoetsCommonFormsResources.Calculate_all,
                                 Resources.PipingFailureMechanism_Calculate_Tooltip,
                                 RingtoetsCommonFormsResources.CalculateAllIcon, (o, args) =>
                                 {
                                     foreach (PipingCalculation calc in GetAllPipingCalculationsResursively(failureMechanism))
                                     {
                                         RunActivityAction(new PipingCalculationActivity(calc));
                                     }
                                 });

            var clearOutputNode = rootMenu.AddMenuItem(RingtoetsCommonFormsResources.Clear_all_output,
                                                       RingtoetsCommonFormsResources.Clear_all_output_ToolTip,
                                                       RingtoetsCommonFormsResources.ClearIcon, (o, args) =>
                                                       {
                                                           foreach (PipingCalculation calc in GetAllPipingCalculationsResursively(failureMechanism))
                                                           {
                                                               calc.ClearOutput();
                                                               calc.NotifyObservers();
                                                           }
                                                       });

            if (!GetAllPipingCalculationsResursively(failureMechanism).Any(c => c.HasOutput))
            {
                clearOutputNode.Enabled = false;
                clearOutputNode.ToolTipText = Resources.ClearOutput_No_calculation_with_output_to_clear;
            }

            return rootMenu;
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