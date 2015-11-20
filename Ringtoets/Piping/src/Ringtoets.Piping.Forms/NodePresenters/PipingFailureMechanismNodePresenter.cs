using System;
using System.Collections;
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

        protected override IEnumerable GetChildNodeObjects(PipingFailureMechanism failureMechanism, ITreeNode node)
        {
            yield return new CategoryTreeFolder("Invoer", GetInputs(failureMechanism), TreeFolderCategory.Input);
            yield return new CategoryTreeFolder("Berekeningen", GetCalculations(failureMechanism));
            yield return new CategoryTreeFolder("Uitvoer", GetOutputs(failureMechanism), TreeFolderCategory.Output);
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, PipingFailureMechanism failureMechanism)
        {
            var rootMenu = new ContextMenuStrip();

            rootMenu.AddMenuItem(Resources.PipingFailureMechanism_Add_Piping_Calculation,
                                 Resources.PipingFailureMechanism_Add_Piping_Calculation_Tooltip,
                                 Resources.PipingIcon, (o, args) =>
                                 {
                                     var pipingData = new PipingData
                                     {
                                         Name = NamingHelper.GetUniqueName(failureMechanism.Calculations, "Piping", pd => pd.Name)
                                     };
                                     failureMechanism.Calculations.Add(pipingData);
                                     failureMechanism.NotifyObservers();
                                 });
            rootMenu.AddMenuItem(Resources.CalculateAll,
                                 Resources.PipingFailureMechanism_Calculate_Tooltip,
                                 Resources.PlayAll, (o, args) =>
                                 {
                                     foreach (var calc in failureMechanism.Calculations)
                                     {
                                         RunActivityAction(new PipingCalculationActivity(calc));
                                     }
                                 });

            var clearOutputNode = rootMenu.AddMenuItem(Resources.Clear_all_output,
                                                       Resources.PipingFailureMechanism_Clear_all_output_Tooltip,
                                                       Resources.PipingOutputClear, (o, args) =>
                                                       {
                                                           foreach (var calc in failureMechanism.Calculations)
                                                           {
                                                               calc.ClearOutput();
                                                               calc.NotifyObservers();
                                                           }
                                                       });

            if (!failureMechanism.Calculations.Any(c => c.HasOutput))
            {
                clearOutputNode.Enabled = false;
                clearOutputNode.ToolTipText = Resources.ClearOutput_No_calculation_with_output_to_clear;
            }

            return rootMenu;
        }

        private static IEnumerable GetInputs(PipingFailureMechanism failureMechanism)
        {
            yield return failureMechanism.SectionDivisions;
            yield return failureMechanism.SoilProfiles;
            yield return failureMechanism.SurfaceLines;
            yield return failureMechanism.BoundaryConditions;
        }

        private IEnumerable GetCalculations(PipingFailureMechanism failureMechanism)
        {
            return failureMechanism.Calculations.Select(calculation => new PipingCalculationInputs
            {
                PipingData = calculation,
                AvailablePipingSurfaceLines = failureMechanism.SurfaceLines,
                AvailablePipingSoilProfiles = failureMechanism.SoilProfiles
            });
        }

        private IEnumerable GetOutputs(PipingFailureMechanism failureMechanism)
        {
            yield return failureMechanism.AssessmentResult;
        }
    }
}