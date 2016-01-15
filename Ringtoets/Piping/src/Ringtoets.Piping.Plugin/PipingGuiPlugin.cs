using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Service;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

namespace Ringtoets.Piping.Plugin
{
    public class PipingGuiPlugin : GuiPlugin
    {
        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new PipingRibbon();
            }
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<PipingCalculationContext, PipingCalculationContextProperties>();
            yield return new PropertyInfo<PipingCalculationGroupContext, PipingCalculationGroupContextProperties>();
            yield return new PropertyInfo<PipingInputContext, PipingInputContextProperties>();
            yield return new PropertyInfo<PipingOutput, PipingOutputProperties>();
            yield return new PropertyInfo<RingtoetsPipingSurfaceLine, RingtoetsPipingSurfaceLineProperties>();
            yield return new PropertyInfo<PipingSoilProfile, PipingSoilProfileProperties>();
        }

        /// <summary>
        /// Get the <see cref="ITreeNodePresenter"/> defined for the <see cref="PipingGuiPlugin"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ITreeNodePresenter"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="IGui.ContextMenuProvider"/> is <c>null</c>.</exception>
        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new PipingFailureMechanismNodePresenter(Gui.ContextMenuProvider)
            {
                RunActivitiesAction = activities => ActivityProgressDialogRunner.Run(Gui.MainWindow, activities)
            };
            yield return new PipingCalculationContextNodePresenter(Gui.ContextMenuProvider)
            {
                RunActivityAction = activity => ActivityProgressDialogRunner.Run(Gui.MainWindow, activity)
            };
            yield return new PipingCalculationGroupContextNodePresenter(Gui.ContextMenuProvider)
            {
                RunActivitiesAction = activities => ActivityProgressDialogRunner.Run(Gui.MainWindow, activities)
            };
            yield return new PipingInputContextNodePresenter(Gui.ContextMenuProvider);
            yield return new PipingSurfaceLineCollectionNodePresenter(Gui.ContextMenuProvider);
            yield return new PipingSurfaceLineNodePresenter(Gui.ContextMenuProvider);
            yield return new PipingSoilProfileCollectionNodePresenter(Gui.ContextMenuProvider);
            yield return new PipingSoilProfileNodePresenter(Gui.ContextMenuProvider);
            yield return new PipingOutputNodePresenter(Gui.ContextMenuProvider);
            yield return new EmptyPipingOutputNodePresenter(Gui.ContextMenuProvider);
            yield return new EmptyPipingCalculationReportNodePresenter(Gui.ContextMenuProvider);
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new TreeNodeInfo<PipingFailureMechanism>
            {
                Text = pipingFailureMechanism => pipingFailureMechanism.Name,
                Image = pipingFailureMechanism => PipingFormsResources.PipingIcon,
                ContextMenu = FailureMechanismContextMenu,
                ChildNodeObjects = FailureMechanismChildNodeObjects
            };

            yield return new TreeNodeInfo<PipingCalculationContext>
            {
                Text = pipingCalculationContext => pipingCalculationContext.WrappedData.Name,
                Image = pipingCalculationContext => PipingFormsResources.PipingIcon,
                ContextMenu = GetPipingCalculationContextContextMenu,
                ChildNodeObjects = PipingCalculationContextChildNodeObjects,
                CanRename = pipingCalculationContext => true,
                OnNodeRenamed = PipingCalculationContextOnNodeRenamed,
                CanRemove = PipingCalculationContextCanRemove,
                OnNodeRemoved = PipingCalculationContextOnNodeRemoved,
                CanDrag = pipingCalculationContext => DragOperations.Move
            };
        }

        # region PipingFailureMechanism TreeNodeInfo

        private ContextMenuStrip FailureMechanismContextMenu(PipingFailureMechanism failureMechanism, TreeNode node)
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

            var validateAllItem = CreateValidateAllItem(failureMechanism);

            var calculateAllItem = CreateCalculateAllItem(failureMechanism);

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

            return Gui.ContextMenuProvider.Get(node)
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

        private StrictContextMenuItem CreateCalculateAllItem(PipingFailureMechanism failureMechanism)
        {
            var menuItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                RingtoetsCommonFormsResources.Calculate_all_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (o, args) => CalculateAll(failureMechanism)
                );

            if (!GetAllPipingCalculationsResursively(failureMechanism).Any())
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = PipingFormsResources.PipingFailureMechanismNodePresenter_CreateCalculateAllItem_No_calculations_to_run;
            }

            return menuItem;
        }

        private StrictContextMenuItem CreateValidateAllItem(PipingFailureMechanism failureMechanism)
        {
            var menuItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Validate_all,
                RingtoetsCommonFormsResources.Validate_all_ToolTip,
                RingtoetsCommonFormsResources.ValidateAllIcon,
                (o, args) => ValidateAll(failureMechanism)
                );

            if (!GetAllPipingCalculationsResursively(failureMechanism).Any())
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = PipingFormsResources.PipingFailureMechanismNodePresenter_CreateValidateAllItem_No_calculations_to_validate;
            }

            return menuItem;
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
            ActivityProgressDialogRunner.Run(Gui.MainWindow, GetAllPipingCalculationsResursively(failureMechanism).Select(calc => new PipingCalculationActivity(calc)));
        }

        private void AddCalculationGroup(PipingFailureMechanism failureMechanism, TreeNode failureMechanismNode)
        {
            var calculation = new PipingCalculationGroup
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, PipingDataResources.PipingCalculationGroup_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.NotifyObservers();

            SelectNewlyAddedItemInTreeView(failureMechanismNode);
        }

        private void AddCalculation(PipingFailureMechanism failureMechanism, TreeNode failureMechanismNode)
        {
            var calculation = new PipingCalculation
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, PipingDataResources.PipingCalculation_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.NotifyObservers();

            SelectNewlyAddedItemInTreeView(failureMechanismNode);
        }

        private void SelectNewlyAddedItemInTreeView(TreeNode failureMechanismNode)
        {
            if (!failureMechanismNode.IsExpanded)
            {
                failureMechanismNode.Expand();
            }

            // Childnode at index 1 is the PipingCalculationGroup where the new item has been added:
            TreeNode failureMechanismsCalculationsNode = failureMechanismNode.Nodes[1];

            // New childnode is appended at the end of PipingCalculationGroup:
            TreeNode newlyAddedGroupNode = failureMechanismsCalculationsNode.Nodes.Last();
            if (!failureMechanismsCalculationsNode.IsExpanded)
            {
                failureMechanismsCalculationsNode.Expand();
            }

            failureMechanismNode.TreeView.SelectedNode = newlyAddedGroupNode;
        }

        private static IEnumerable<PipingCalculation> GetAllPipingCalculationsResursively(PipingFailureMechanism failureMechanism)
        {
            return failureMechanism.CalculationsGroup.GetPipingCalculations().ToArray();
        }

        private object[] FailureMechanismChildNodeObjects(PipingFailureMechanism pipingFailureMechanism)
        {
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(pipingFailureMechanism), TreeFolderCategory.Input),
                new PipingCalculationGroupContext(pipingFailureMechanism.CalculationsGroup, pipingFailureMechanism.SurfaceLines, pipingFailureMechanism.SoilProfiles),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(pipingFailureMechanism), TreeFolderCategory.Output)
            };
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

        # endregion

        # region PipingFailureMechanism PipingCalculationContext

        private ContextMenuStrip GetPipingCalculationContextContextMenu(PipingCalculationContext nodeData, TreeNode node)
        {
            PipingCalculation calculation = nodeData.WrappedData;
            var validateItem = new StrictContextMenuItem(RingtoetsFormsResources.Validate,
                                                         RingtoetsFormsResources.Validate_ToolTip,
                                                         RingtoetsFormsResources.ValidateIcon,
                                                         (o, args) => { PipingCalculationService.Validate(calculation); });
            var calculateItem = new StrictContextMenuItem(RingtoetsFormsResources.Calculate,
                                                          RingtoetsFormsResources.Calculate_ToolTip,
                                                          RingtoetsFormsResources.CalculateIcon,
                                                          (o, args) => { ActivityProgressDialogRunner.Run(Gui.MainWindow, new PipingCalculationActivity(calculation)); });

            var clearOutputItem = new StrictContextMenuItem(PipingFormsResources.Clear_output,
                                                            PipingFormsResources.Clear_output_ToolTip,
                                                            RingtoetsFormsResources.ClearIcon,
                                                            (o, args) => ClearOutput(calculation));

            if (!calculation.HasOutput)
            {
                clearOutputItem.Enabled = false;
                clearOutputItem.ToolTipText = PipingFormsResources.ClearOutput_No_output_to_clear;
            }

            return Gui.ContextMenuProvider.Get(node)
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

        private static object[] PipingCalculationContextChildNodeObjects(PipingCalculationContext pipingCalculationContext)
        {
            var childNodes = new List<object>
            {
                pipingCalculationContext.WrappedData.Comments,
                new PipingInputContext(pipingCalculationContext.WrappedData.InputParameters,
                                       pipingCalculationContext.AvailablePipingSurfaceLines,
                                       pipingCalculationContext.AvailablePipingSoilProfiles)
            };

            if (pipingCalculationContext.WrappedData.HasOutput)
            {
                childNodes.Add(pipingCalculationContext.WrappedData.Output);
                childNodes.Add(new EmptyPipingCalculationReport());
            }
            else
            {
                childNodes.Add(new EmptyPipingOutput());
                childNodes.Add(new EmptyPipingCalculationReport());
            }

            return childNodes.ToArray();
        }

        private static void PipingCalculationContextOnNodeRenamed(PipingCalculationContext pipingCalculationContext, string newName)
        {
            pipingCalculationContext.WrappedData.Name = newName;
            pipingCalculationContext.WrappedData.NotifyObservers();
        }

        private bool PipingCalculationContextCanRemove(PipingCalculationContext pipingCalculationContext, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as PipingCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                return calculationGroupContext.WrappedData.Children.Contains(pipingCalculationContext.WrappedData);
            }

            return false;
        }

        private static void PipingCalculationContextOnNodeRemoved(PipingCalculationContext pipingCalculationContext, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as PipingCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                var succesfullyRemovedData = calculationGroupContext.WrappedData.Children.Remove(pipingCalculationContext.WrappedData);
                if (succesfullyRemovedData)
                {
                    calculationGroupContext.NotifyObservers();
                }
            }
        }

        private static void ClearOutput(PipingCalculation calculation)
        {
            if (MessageBox.Show(PipingFormsResources.PipingCalculationContextNodePresenter_GetContextMenu_Are_you_sure_clear_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            calculation.ClearOutput();
            calculation.NotifyObservers();
        }

        # endregion
    }
}