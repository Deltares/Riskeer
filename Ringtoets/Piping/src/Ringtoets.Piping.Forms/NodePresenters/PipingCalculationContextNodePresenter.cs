using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base.Service;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Service;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// This class presents the data on <see cref="PipingCalculation"/> as a node in a <see cref="ITreeView"/> and
    /// implements the way the user can interact with the node.
    /// </summary>
    public class PipingCalculationContextNodePresenter : RingtoetsNodePresenterBase<PipingCalculationContext>
    {
        /// <summary>
        /// Creates a new instance of <see cref="EmptyPipingCalculationReportNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public PipingCalculationContextNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

        /// <summary>
        /// Injection points for a method to cause an <see cref="Activity"/> to be scheduled for execution.
        /// </summary>
        public Action<Activity> RunActivityAction { private get; set; }

        public override bool CanRenameNode(ITreeNode node)
        {
            return true;
        }

        public override bool CanRenameNodeTo(ITreeNode node, string newName)
        {
            return true;
        }

        protected override DragOperations CanDrag(PipingCalculationContext nodeData)
        {
            return DragOperations.Move;
        }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingCalculationContext pipingCalculationContext)
        {
            node.Text = pipingCalculationContext.WrappedData.Name;
            node.Image = Resources.PipingIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override IEnumerable GetChildNodeObjects(PipingCalculationContext pipingCalculationContext)
        {
            yield return pipingCalculationContext.WrappedData.Comments;
            yield return new PipingInputContext(pipingCalculationContext.WrappedData.InputParameters,
                                                pipingCalculationContext.AvailablePipingSurfaceLines,
                                                pipingCalculationContext.AvailablePipingSoilProfiles
                );
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

        protected override bool CanRemove(object parentNodeData, PipingCalculationContext nodeData)
        {
            var calculationGroupContext = parentNodeData as PipingCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                return calculationGroupContext.WrappedData.Children.Contains(nodeData.WrappedData);
            }
            return base.CanRemove(parentNodeData, nodeData);
        }

        protected override bool RemoveNodeData(object parentNodeData, PipingCalculationContext nodeData)
        {
            var calculationGroupContext = parentNodeData as PipingCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                var succesfullyRemovedData = calculationGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);
                if (succesfullyRemovedData)
                {
                    calculationGroupContext.NotifyObservers();
                }
                return succesfullyRemovedData;
            }
            return base.RemoveNodeData(parentNodeData, nodeData);
        }

        protected override void OnNodeRenamed(PipingCalculationContext pipingCalculationContext, string newName)
        {
            pipingCalculationContext.WrappedData.Name = newName;
            pipingCalculationContext.WrappedData.NotifyObservers();
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode node, PipingCalculationContext nodeData)
        {
            PipingCalculation calculation = nodeData.WrappedData;
            var validateItem = new StrictContextMenuItem(RingtoetsFormsResources.Validate,
                                                         RingtoetsFormsResources.Validate_ToolTip,
                                                         RingtoetsFormsResources.ValidateIcon,
                                                         (o, args) => { PipingCalculationService.Validate(calculation); });
            var calculateItem = new StrictContextMenuItem(RingtoetsFormsResources.Calculate,
                                                          RingtoetsFormsResources.Calculate_ToolTip,
                                                          RingtoetsFormsResources.CalculateIcon,
                                                          (o, args) => { RunActivityAction(new PipingCalculationActivity(calculation)); });

            var clearOutputItem = new StrictContextMenuItem(Resources.Clear_output,
                                                            Resources.Clear_output_ToolTip,
                                                            RingtoetsFormsResources.ClearIcon,
                                                            (o, args) => ClearOutput(calculation));

            if (!calculation.HasOutput)
            {
                clearOutputItem.Enabled = false;
                clearOutputItem.ToolTipText = Resources.ClearOutput_No_output_to_clear;
            }

            return contextMenuBuilderProvider
                .Get(node)
                .AddCustomItem(validateItem)
                .AddCustomItem(calculateItem)
                .AddCustomItem(clearOutputItem)
                .AddSeparator()
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
            
        private static void ClearOutput(PipingCalculation calculation)
        {
            if (MessageBox.Show(Resources.PipingCalculationContextNodePresenter_GetContextMenu_Are_you_sure_clear_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }
            calculation.ClearOutput();
            calculation.NotifyObservers();
        }
    }
}