﻿using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Workflow;
using Core.Common.Controls;

using Ringtoets.Common.Forms.Extensions;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service;

using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// This class presents the data on <see cref="PipingCalculation"/> as a node in a <see cref="ITreeView"/> and
    /// implements the way the user can interact with the node.
    /// </summary>
    public class PipingCalculationContextNodePresenter : RingtoetsNodePresenterBase<PipingCalculationContext>
    {
        /// <summary>
        /// Injection points for a method to cause an <see cref="IActivity"/> to be scheduled for execution.
        /// </summary>
        public Action<IActivity> RunActivityAction { private get; set; }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingCalculationContext pipingCalculationContext)
        {
            node.Text = pipingCalculationContext.WrappedData.Name;
            node.Image = Resources.PipingIcon;
        }

        protected override IEnumerable GetChildNodeObjects(PipingCalculationContext pipingCalculationContext)
        {
            yield return pipingCalculationContext.WrappedData.Comments;
            yield return new PipingInputContext
            {
                WrappedPipingInput = pipingCalculationContext.WrappedData.InputParameters,
                AvailablePipingSurfaceLines = pipingCalculationContext.AvailablePipingSurfaceLines,
                AvailablePipingSoilProfiles = pipingCalculationContext.AvailablePipingSoilProfiles
            };
            if (pipingCalculationContext.WrappedData.HasOutput)
            {
                yield return pipingCalculationContext.WrappedData.Output;
                yield return pipingCalculationContext.WrappedData.CalculationReport;
            }
            else
            {
                yield return new EmptyPipingOutput();
                yield return new EmptyPipingCalculationReport();
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

        protected override bool CanRemove(object parentNodeData, PipingCalculationContext nodeData)
        {
            var calculationsFolder = parentNodeData as PipingCalculationsTreeFolder;
            if (calculationsFolder != null)
            {
                return calculationsFolder.Contents.OfType<PipingCalculationContext>().Contains(nodeData);
            }
            return base.CanRemove(parentNodeData, nodeData);
        }

        protected override bool RemoveNodeData(object parentNodeData, PipingCalculationContext nodeData)
        {
            var calculationsFolder = parentNodeData as PipingCalculationsTreeFolder;
            if (calculationsFolder != null)
            {
                var succesfullyRemovedData = calculationsFolder.ParentFailureMechanism.Calculations.Remove(nodeData.WrappedData);
                calculationsFolder.ParentFailureMechanism.NotifyObservers();
                return succesfullyRemovedData;
            }
            return base.RemoveNodeData(parentNodeData, nodeData);
        }

        protected override void OnNodeRenamed(PipingCalculationContext pipingCalculationContext, string newName)
        {
            pipingCalculationContext.WrappedData.Name = newName;
            pipingCalculationContext.WrappedData.NotifyObservers();
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, PipingCalculationContext nodeData)
        {
            PipingCalculation calculation = nodeData.WrappedData;

            var contextMenu = new ContextMenuStrip();
            contextMenu.AddMenuItem(Resources.Validate,
                                    null,
                                    Resources.ValidationIcon,
                                    (o, args) =>
                                    {
                                        PipingCalculationService.Validate(calculation);
                                    });
            contextMenu.AddMenuItem(Resources.Calculate,
                                    null,
                                    Resources.Play,
                                    (o, args) =>
                                    {
                                        RunActivityAction(new PipingCalculationActivity(calculation));
                                    });

            var clearOutputItem = contextMenu.AddMenuItem(Resources.Clear_output,
                                    null,
                                    RingtoetsFormsResources.ClearIcon,
                                    (o, args) =>
                                    {
                                        calculation.ClearOutput();
                                        calculation.NotifyObservers();
                                    });

            if (!calculation.HasOutput)
            {
                clearOutputItem.Enabled = false;
                clearOutputItem.ToolTipText = Resources.ClearOutput_No_output_to_clear;
            }

            return contextMenu;
        }
    }
}