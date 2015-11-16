using System;
using System.Collections;
using System.Windows.Forms;
using Core.Common.Base.Workflow;
using Core.Common.Controls;

using Ringtoets.Common.Forms.Extensions;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service;

using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// This class presents the data on <see cref="PipingData"/> as a node in a <see cref="ITreeView"/> and
    /// implements the way the user can interact with the node.
    /// </summary>
    public class PipingCalculationInputsNodePresenter : PipingNodePresenterBase<PipingCalculationInputs>
    {
        /// <summary>
        /// Injection points for a method to cause an <see cref="IActivity"/> to be scheduled for execution.
        /// </summary>
        public Action<IActivity> RunActivityAction { private get; set; }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingCalculationInputs pipingCalculationInputs)
        {
            node.Text = pipingCalculationInputs.PipingData.Name;
            node.Image = Resources.PipingIcon;
        }

        protected override IEnumerable GetChildNodeObjects(PipingCalculationInputs pipingCalculationInputs, ITreeNode node)
        {
            var pipingOutput = pipingCalculationInputs.PipingData.Output;
            if (pipingOutput != null)
            {
                yield return pipingOutput;
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

        protected override void OnNodeRenamed(PipingCalculationInputs pipingCalculationInputs, string newName)
        {
            pipingCalculationInputs.PipingData.Name = newName;
            pipingCalculationInputs.PipingData.NotifyObservers();
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, PipingCalculationInputs nodeData)
        {
            PipingData pipingData = nodeData.PipingData;

            var contextMenu = new ContextMenuStrip();
            contextMenu.AddMenuItem(Resources.Validate,
                                    null,
                                    null,
                                    (o, args) =>
                                    {
                                        PipingCalculationService.Validate(pipingData);
                                    });
            contextMenu.AddMenuItem(Resources.Calculate,
                                    null,
                                    Resources.Play,
                                    (o, args) =>
                                    {
                                        RunActivityAction(new PipingCalculationActivity(pipingData));
                                    });

            var clearOutputItem = contextMenu.AddMenuItem(Resources.Clear_output,
                                    null,
                                    Resources.PipingOutputClear,
                                    (o, args) =>
                                    {
                                        pipingData.ClearOutput();
                                        pipingData.NotifyObservers();
                                    });

            if (!pipingData.HasOutput)
            {
                clearOutputItem.Enabled = false;
                clearOutputItem.ToolTipText = Resources.ClearOutput_No_output_to_clear;
            }

            return contextMenu;
        }
    }
}