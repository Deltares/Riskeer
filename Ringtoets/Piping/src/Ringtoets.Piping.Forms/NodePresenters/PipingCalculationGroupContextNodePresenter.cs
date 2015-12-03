using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Service;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;

using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Properties;
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
        /// Creates a new instance of <see cref="EmptyPipingCalculationReportNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public PipingCalculationGroupContextNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

        /// <summary>
        /// Injection points for a method to cause an <see cref="Activity"/> to be scheduled for execution.
        /// </summary>
        public Action<IEnumerable<Activity>> RunActivitiesAction { private get; set; }

        public override bool CanRenameNode(ITreeNode node)
        {
            return node.Parent == null || !(node.Parent.Tag is PipingFailureMechanism);
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
            var group = parentNodeData as PipingCalculationGroupContext;
            if (group != null)
            {
                return group.WrappedData.Children.Contains(nodeData.WrappedData);
            }

            return base.CanRemove(parentNodeData, nodeData);
        }

        protected override bool RemoveNodeData(object parentNodeData, PipingCalculationGroupContext nodeData)
        {
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
            var group = nodeData.WrappedData;
            var addCalculationGroupItem = new StrictContextMenuItem(PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup,
                                                                    PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup_ToolTip,
                                                                    PipingFormsResources.AddFolderIcon, (o, args) =>
                                                                    {
                                                                        var newGroup = new PipingCalculationGroup
                                                                        {
                                                                            Name = NamingHelper.GetUniqueName(group.Children, Resources.PipingCalculationGroup_DefaultName, c => c.Name)
                                                                        };
                                                                        group.Children.Add(newGroup);
                                                                        nodeData.NotifyObservers();
                                                                    });
            var addCalculationItem = new StrictContextMenuItem(PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation,
                                                               PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation_ToolTip,
                                                               PipingFormsResources.PipingIcon, (o, args) =>
                                                               {
                                                                   var calculation = new PipingCalculation
                                                                   {
                                                                       Name = NamingHelper.GetUniqueName(group.Children, Resources.PipingCalculation_DefaultName, c => c.Name)
                                                                   };
                                                                   group.Children.Add(calculation);
                                                                   nodeData.NotifyObservers();
                                                               });
            var validateAllItem = new StrictContextMenuItem(PipingFormsResources.PipingCalculationItem_Validate,
                                                            PipingFormsResources.PipingCalculationGroup_Validate_ToolTip,
                                                            PipingFormsResources.ValidationIcon, (o, args) =>
                                                            {
                                                                foreach (PipingCalculation calculation in group.Children.GetPipingCalculations())
                                                                {
                                                                    PipingCalculationService.Validate(calculation);
                                                                }
                                                            });
            var calculateAllItem = new StrictContextMenuItem(RingtoetsFormsResources.Calculate_all,
                                                             PipingFormsResources.PipingCalculationGroup_CalculateAll_ToolTip,
                                                             RingtoetsFormsResources.CalculateAllIcon, (o, args) =>
                                                             {
                                                                 RunActivitiesAction(group.GetPipingCalculations().Select(pc => new PipingCalculationActivity(pc)));
                                                             });
            var clearAllItem = new StrictContextMenuItem(RingtoetsFormsResources.Clear_all_output,
                                                         PipingFormsResources.PipingCalculationGroup_ClearOutput_ToolTip,
                                                         RingtoetsFormsResources.ClearIcon, (o, args) =>
                                                         {
                                                             foreach (PipingCalculation calc in group.GetPipingCalculations().Where(c => c.HasOutput))
                                                             {
                                                                 calc.ClearOutput();
                                                                 calc.NotifyObservers();
                                                             }
                                                         });

            if (!nodeData.WrappedData.GetPipingCalculations().Any(c => c.HasOutput))
            {
                clearAllItem.Enabled = false;
                clearAllItem.ToolTipText = PipingFormsResources.PipingCalculationGroup_ClearOutput_No_calculation_with_output_to_clear;
            }

            return contextMenuBuilderProvider.Get(sender)
                                             .AddCustomItem(addCalculationGroupItem)
                                             .AddCustomItem(addCalculationItem)
                                             .AddSeparator()
                                             .AddCustomItem(validateAllItem)
                                             .AddCustomItem(calculateAllItem)
                                             .AddCustomItem(clearAllItem)
                                             .AddSeparator()
                                             .AddExpandAllItem()
                                             .AddCollapseAllItem()
                                             .AddSeparator()
                                             .AddImportItem()
                                             .AddExportItem()
                                             .AddSeparator()
                                             .AddPropertiesItem()
                                             .Build();
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