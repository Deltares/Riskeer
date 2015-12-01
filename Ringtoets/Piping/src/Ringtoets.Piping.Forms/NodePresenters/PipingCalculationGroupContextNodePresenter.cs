using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base.Workflow;
using Core.Common.Controls;

using Ringtoets.Common.Forms.Extensions;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Properties;
using Ringtoets.Piping.Forms.Helpers;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Service;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter class for <see cref="PipingCalculationGroup"/> instances.
    /// </summary>
    public class PipingCalculationGroupContextNodePresenter : RingtoetsNodePresenterBase<PipingCalculationGroupContext>
    {
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

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingCalculationGroupContext nodeData)
        {
            node.Text = nodeData.WrappedData.Name;
            node.Image = PipingFormsResources.FolderIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override void OnNodeRenamed(PipingCalculationGroupContext nodeData, string newName)
        {
            nodeData.WrappedData.Name = newName;
            nodeData.NotifyObservers();
        }

        protected override bool CanRemove(object parentNodeData, PipingCalculationGroupContext nodeData)
        {
            var failureMechanism = parentNodeData as PipingFailureMechanism;
            if (failureMechanism != null)
            {
                return failureMechanism.Calculations.Contains(nodeData.WrappedData);
            }

            var group = parentNodeData as PipingCalculationGroupContext;
            if (group != null)
            {
                return group.WrappedData.Children.Contains(nodeData.WrappedData);
            }

            return base.CanRemove(parentNodeData, nodeData);
        }

        protected override bool RemoveNodeData(object parentNodeData, PipingCalculationGroupContext nodeData)
        {
            var failureMechanism = parentNodeData as PipingFailureMechanism;
            if (failureMechanism != null)
            {
                var removeNodeData = failureMechanism.Calculations.Remove(nodeData.WrappedData);
                failureMechanism.NotifyObservers();
                return removeNodeData;
            }

            var group = parentNodeData as PipingCalculationGroupContext;
            if (group != null)
            {
                var removeNodeData = group.WrappedData.Children.Remove(nodeData.WrappedData);
                group.NotifyObservers();
                return removeNodeData;
            }

            return base.RemoveNodeData(parentNodeData, nodeData);
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, PipingCalculationGroupContext nodeData)
        {
            var rootMenu = new ContextMenuStrip();

            var group = nodeData.WrappedData;
            rootMenu.AddMenuItem("Map toevoegen",
                                 "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                 PipingFormsResources.AddFolderIcon, (o, args) =>
                                 {
                                     var newGroup = new PipingCalculationGroup
                                     {
                                         Name = NamingHelper.GetUniqueName(group.Children, Resources.PipingCalculationGroup_DefaultName, c => c.Name)
                                     };
                                     group.Children.Add(newGroup);
                                     nodeData.NotifyObservers();
                                 });
            rootMenu.AddMenuItem("Berekening toevoegen",
                                 "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                 PipingFormsResources.PipingIcon, (o, args) =>
                                 {
                                     var calculation = new PipingCalculation
                                     {
                                         Name = NamingHelper.GetUniqueName(group.Children, Resources.PipingCalculation_DefaultName, c => c.Name)
                                     };
                                     group.Children.Add(calculation);
                                     nodeData.NotifyObservers();
                                 });
            rootMenu.AddMenuItem("Valideren",
                                 "Valideer alle berekeningen binnen deze berekeningsmap.",
                                 PipingFormsResources.ValidationIcon, (o, args) =>
                                 {
                                     foreach (PipingCalculation calculation in group.Children.GetPipingCalculations())
                                     {
                                         PipingCalculationService.Validate(calculation);
                                     }
                                 });
            rootMenu.AddMenuItem("Alles be&rekenen",
                                 "Valideer en voer alle berekeningen binnen deze berekeningsmap uit.",
                                 RingtoetsFormsResources.CalculateAllIcon, (o, args) =>
                                 {
                                     foreach (PipingCalculation calc in group.GetPipingCalculations())
                                     {
                                         RunActivityAction(new PipingCalculationActivity(calc));
                                     }
                                 });
            rootMenu.AddMenuItem("&Wis alle uitvoer",
                                 "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                 RingtoetsFormsResources.ClearIcon, (o, args) =>
                                 {
                                     foreach (PipingCalculation calc in group.GetPipingCalculations().Where(c => c.HasOutput))
                                     {
                                         calc.ClearOutput();
                                         calc.NotifyObservers();
                                     }
                                 });

            return rootMenu;
        }

        protected override IEnumerable GetChildNodeObjects(PipingCalculationGroupContext nodeData)
        {
            foreach (IPipingCalculationItem item in nodeData.WrappedData.Children)
            {
                var calculation = item as PipingCalculation;
                var group = item as PipingCalculationGroup;

                if (calculation != null)
                {
                    yield return new PipingCalculationContext(calculation,
                                                              nodeData.AvailablePipingSurfaceLines,
                                                              nodeData.AvailablePipingSoilProfiles);
                }
                else if (group != null)
                {
                    yield return new PipingCalculationGroupContext(group,
                                                                   nodeData.AvailablePipingSurfaceLines,
                                                                   nodeData.AvailablePipingSoilProfiles);
                }
                else
                {
                    yield return item;
                }
            }
        }
    }
}