using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
using TreeView = Core.Common.Controls.TreeView.TreeView;

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
        /// <exception cref="ArgumentNullException">Thrown when <see cref="IContextMenuBuilderProvider"/> is <c>null</c>.</exception>
        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new PipingFailureMechanismNodePresenter(Gui)
            {
                RunActivitiesAction = activities => ActivityProgressDialogRunner.Run(Gui.MainWindow, activities)
            };
            yield return new PipingCalculationContextNodePresenter(Gui)
            {
                RunActivityAction = activity => ActivityProgressDialogRunner.Run(Gui.MainWindow, activity)
            };
            yield return new PipingCalculationGroupContextNodePresenter(Gui)
            {
                RunActivitiesAction = activities => ActivityProgressDialogRunner.Run(Gui.MainWindow, activities)
            };
            yield return new PipingInputContextNodePresenter(Gui);
            yield return new PipingSurfaceLineCollectionNodePresenter(Gui);
            yield return new PipingSurfaceLineNodePresenter(Gui);
            yield return new PipingSoilProfileCollectionNodePresenter(Gui);
            yield return new PipingSoilProfileNodePresenter(Gui);
            yield return new PipingOutputNodePresenter(Gui);
            yield return new EmptyPipingOutputNodePresenter(Gui);
            yield return new EmptyPipingCalculationReportNodePresenter(Gui);
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
                ContextMenu = PipingCalculationContextContextMenu,
                ChildNodeObjects = PipingCalculationContextChildNodeObjects,
                CanRename = pipingCalculationContext => true,
                OnNodeRenamed = PipingCalculationContextOnNodeRenamed,
                CanRemove = PipingCalculationContextCanRemove,
                OnNodeRemoved = PipingCalculationContextOnNodeRemoved,
                CanDrag = (pipingCalculationContext, sourceNode) => DragOperations.Move
            };

            yield return new TreeNodeInfo<PipingCalculationGroupContext>
            {
                Text = pipingCalculationGroupContext => pipingCalculationGroupContext.WrappedData.Name,
                Image = pipingCalculationGroupContext => PipingFormsResources.FolderIcon,
                ChildNodeObjects = PipingCalculationGroupContextChildNodeObjects,
                ContextMenu = PipingCalculationGroupContextContextMenu,
                CanRename = PipingCalculationGroupContextCanRenameNode,
                OnNodeRenamed = PipingCalculationGroupContextOnNodeRenamed,
                CanRemove = PipingCalculationGroupContextCanRemove,
                OnNodeRemoved = PipingCalculationGroupContextOnNodeRemoved,
                CanDrag = PipingCalculationGroupContextCanDrag,
                CanDrop = PipingCalculationGroupContextCanDrop,
                CanInsert = PipingCalculationGroupContextCanInsert,
                OnDrop = PipingCalculationGroupContextOnDrop
            };

            yield return new TreeNodeInfo<PipingInputContext>
            {
                Text = pipingInputContext => PipingFormsResources.PipingInputContextNodePresenter_NodeDisplayName,
                Image = pipingInputContext => PipingFormsResources.PipingInputIcon,
                ContextMenu = (nodeData, node) => Gui
                                                     .Get(node)
                                                     .AddImportItem()
                                                     .AddExportItem()
                                                     .AddSeparator()
                                                     .AddPropertiesItem()
                                                     .Build()
            };

            yield return new TreeNodeInfo<IEnumerable<RingtoetsPipingSurfaceLine>>
            {
                Text = ringtoetsPipingSurfaceLine => PipingFormsResources.PipingSurfaceLinesCollection_DisplayName,
                Image = ringtoetsPipingSurfaceLine => PipingFormsResources.FolderIcon,
                ForegroundColor = ringtoetsPipingSurfaceLine => ringtoetsPipingSurfaceLine.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = ringtoetsPipingSurfaceLine => ringtoetsPipingSurfaceLine.Cast<object>().ToArray(),
                ContextMenu = (nodeData, node) => Gui
                                                     .Get(node)
                                                     .AddImportItem()
                                                     .AddExportItem()
                                                     .AddSeparator()
                                                     .AddExpandAllItem()
                                                     .AddCollapseAllItem()
                                                     .Build()
            };

            yield return new TreeNodeInfo<RingtoetsPipingSurfaceLine>
            {
                Text = pipingSurfaceLine => pipingSurfaceLine.Name,
                Image = pipingSurfaceLine => PipingFormsResources.PipingSurfaceLineIcon,

                ContextMenu = (nodeData, node) => Gui
                                                     .Get(node)
                                                     .AddPropertiesItem()
                                                     .Build()
            };

            yield return new TreeNodeInfo<IEnumerable<PipingSoilProfile>>
            {
                Text = pipingSoilProfiles => PipingFormsResources.PipingSoilProfilesCollection_DisplayName,
                Image = pipingSoilProfiles => PipingFormsResources.FolderIcon,
                ForegroundColor = pipingSoilProfiles => pipingSoilProfiles.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = pipingSoilProfiles => pipingSoilProfiles.Cast<object>().ToArray(),
                ContextMenu = (nodeData, node) => Gui
                                                     .Get(node)
                                                     .AddImportItem()
                                                     .AddExportItem()
                                                     .AddSeparator()
                                                     .AddExpandAllItem()
                                                     .AddCollapseAllItem()
                                                     .Build()
            };

            yield return new TreeNodeInfo<PipingSoilProfile>
            {
                Text = pipingSoilProfile => pipingSoilProfile.Name,
                Image = pipingSoilProfile => PipingFormsResources.PipingSoilProfileIcon,
                ContextMenu = (nodeData, node) => Gui
                                                     .Get(node)
                                                     .AddPropertiesItem()
                                                     .Build()
            };

            yield return new TreeNodeInfo<PipingOutput>
            {
                Text = pipingOutput => PipingFormsResources.PipingOutput_DisplayName,
                Image = pipingOutput => PipingFormsResources.PipingOutputIcon,
                ContextMenu = (nodeData, node) => Gui
                                                     .Get(node)
                                                     .AddExportItem()
                                                     .AddSeparator()
                                                     .AddPropertiesItem()
                                                     .Build()
            };

            yield return new TreeNodeInfo<EmptyPipingOutput>
            {
                Text = emptyPipingOutput => PipingFormsResources.PipingOutput_DisplayName,
                Image = emptyPipingOutput => PipingFormsResources.PipingOutputIcon,
                ForegroundColor = emptyPipingOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenu = (nodeData, node) => Gui
                                                     .Get(node)
                                                     .AddExportItem()
                                                     .AddSeparator()
                                                     .AddPropertiesItem()
                                                     .Build()
            };

            yield return new TreeNodeInfo<EmptyPipingCalculationReport>
            {
                Text = emptyPipingCalculationReport => PipingDataResources.CalculationReport_DisplayName,
                Image = emptyPipingCalculationReport => PipingFormsResources.PipingCalculationReportIcon,
                ForegroundColor = emptyPipingCalculationReport => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenu = (nodeData, node) => Gui
                                                     .Get(node)
                                                     .AddOpenItem()
                                                     .AddSeparator()
                                                     .AddExportItem()
                                                     .AddSeparator()
                                                     .AddPropertiesItem()
                                                     .Build()
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

            return Gui.Get(node)
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

            SelectNewlyAddedPipingFailureMechanismItemInTreeView(failureMechanismNode);
        }

        private void AddCalculation(PipingFailureMechanism failureMechanism, TreeNode failureMechanismNode)
        {
            var calculation = new PipingCalculation
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, PipingDataResources.PipingCalculation_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.NotifyObservers();

            SelectNewlyAddedPipingFailureMechanismItemInTreeView(failureMechanismNode);
        }

        private void SelectNewlyAddedPipingFailureMechanismItemInTreeView(TreeNode failureMechanismNode)
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

        # region PipingCalculationContext TreeNodeInfo

        private ContextMenuStrip PipingCalculationContextContextMenu(PipingCalculationContext nodeData, TreeNode node)
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

            return Gui.Get(node)
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

        # region PipingCalculationGroupContextNodePresenter TreeNodeInfo

        private object[] PipingCalculationGroupContextChildNodeObjects(PipingCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (IPipingCalculationItem item in nodeData.WrappedData.Children)
            {
                var calculation = item as PipingCalculation;
                var group = item as PipingCalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new PipingCalculationContext(calculation,
                                                              nodeData.AvailablePipingSurfaceLines,
                                                              nodeData.AvailablePipingSoilProfiles));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new PipingCalculationGroupContext(group,
                                                                   nodeData.AvailablePipingSurfaceLines,
                                                                   nodeData.AvailablePipingSoilProfiles));
                }
                else
                {
                    childNodeObjects.Add(item);
                }
            }

            return childNodeObjects.ToArray();
        }

        private ContextMenuStrip PipingCalculationGroupContextContextMenu(PipingCalculationGroupContext nodeData, TreeNode node)
        {
            var group = nodeData.WrappedData;
            var addCalculationGroupItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup,
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup_ToolTip,
                PipingFormsResources.AddFolderIcon, (o, args) =>
                {
                    var newGroup = new PipingCalculationGroup
                    {
                        Name = NamingHelper.GetUniqueName(group.Children, PipingDataResources.PipingCalculationGroup_DefaultName, c => c.Name)
                    };

                    group.Children.Add(newGroup);
                    nodeData.NotifyObservers();

                    SelectNewlyAddedPipingCalculationGroupContextItemInTreeView(node);
                });

            var addCalculationItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation,
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation_ToolTip,
                PipingFormsResources.PipingIcon, (o, args) =>
                {
                    var calculation = new PipingCalculation
                    {
                        Name = NamingHelper.GetUniqueName(group.Children, PipingDataResources.PipingCalculation_DefaultName, c => c.Name)
                    };

                    group.Children.Add(calculation);
                    nodeData.NotifyObservers();

                    SelectNewlyAddedPipingCalculationGroupContextItemInTreeView(node);
                });

            var validateAllItem = CreateValidateAllItem(group);

            var calculateAllItem = CreateCalculateAllItem(group);

            var clearAllItem = new StrictContextMenuItem(
                RingtoetsFormsResources.Clear_all_output,
                PipingFormsResources.PipingCalculationGroup_ClearOutput_ToolTip,
                RingtoetsFormsResources.ClearIcon, (o, args) =>
                {
                    if (MessageBox.Show(PipingFormsResources.PipingCalculationGroupContextNodePresenter_GetContextMenu_Are_you_sure_clear_all_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
                    {
                        return;
                    }

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

            var builder = Gui.Get(node)
                .AddCustomItem(addCalculationGroupItem)
                .AddCustomItem(addCalculationItem)
                .AddSeparator()
                .AddCustomItem(validateAllItem)
                .AddCustomItem(calculateAllItem)
                .AddCustomItem(clearAllItem)
                .AddSeparator();

            var isRenamable = PipingCalculationGroupContextCanRenameNode(node);
            var isRemovable = PipingCalculationGroupContextCanRemove(nodeData, node.Parent.Tag);

            if (isRenamable)
            {
                builder.AddRenameItem();
            }
            if (isRemovable)
            {
                builder.AddDeleteItem();
            }

            if (isRemovable || isRenamable)
            {
                builder.AddSeparator();
            }

            return builder
                .AddImportItem()
                .AddExportItem()
                .AddSeparator()
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }

        private StrictContextMenuItem CreateCalculateAllItem(PipingCalculationGroup group)
        {
            var menuItem = new StrictContextMenuItem(
                RingtoetsFormsResources.Calculate_all,
                PipingFormsResources.PipingCalculationGroup_CalculateAll_ToolTip,
                RingtoetsFormsResources.CalculateAllIcon, (o, args) => { CalculateAll(group); });


            if (!group.GetPipingCalculations().Any())
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = PipingFormsResources.PipingFailureMechanismNodePresenter_CreateCalculateAllItem_No_calculations_to_run;
            }

            return menuItem;
        }

        private static StrictContextMenuItem CreateValidateAllItem(PipingCalculationGroup group)
        {
            var menuItem = new StrictContextMenuItem(
                RingtoetsFormsResources.Validate_all,
                PipingFormsResources.PipingCalculationGroup_Validate_All_ToolTip,
                RingtoetsFormsResources.ValidateAllIcon, (o, args) =>
                {
                    ValidateAll(group);
                });

            if (!group.GetPipingCalculations().Any())
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = PipingFormsResources.PipingFailureMechanismNodePresenter_CreateValidateAllItem_No_calculations_to_validate;
            }

            return menuItem;
        }

        private void CalculateAll(PipingCalculationGroup group)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, group.GetPipingCalculations().Select(pc => new PipingCalculationActivity(pc)));
        }

        private static void ValidateAll(PipingCalculationGroup group)
        {
            foreach (PipingCalculation calculation in group.Children.GetPipingCalculations())
            {
                PipingCalculationService.Validate(calculation);
            }
        }

        private bool PipingCalculationGroupContextCanRenameNode(TreeNode node)
        {
            var parentNode = node.Parent;
            return parentNode == null || !(parentNode.Tag is PipingFailureMechanism);
        }

        private void PipingCalculationGroupContextOnNodeRenamed(PipingCalculationGroupContext nodeData, string newName)
        {
            nodeData.WrappedData.Name = newName;
            nodeData.NotifyObservers();
        }

        private bool PipingCalculationGroupContextCanRemove(PipingCalculationGroupContext nodeData, object parentNodeData)
        {
            var group = parentNodeData as PipingCalculationGroupContext;
            if (group != null)
            {
                return group.WrappedData.Children.Contains(nodeData.WrappedData);
            }

            return false;
        }

        private void PipingCalculationGroupContextOnNodeRemoved(PipingCalculationGroupContext nodeData, object parentNodeData)
        {
            var group = parentNodeData as PipingCalculationGroupContext;
            if (group != null)
            {
                group.WrappedData.Children.Remove(nodeData.WrappedData);
                group.NotifyObservers();
            }
        }

        private DragOperations PipingCalculationGroupContextCanDrag(PipingCalculationGroupContext nodeData, TreeNode sourceNode)
        {
            if (sourceNode.Parent.Tag is PipingFailureMechanism)
            {
                return DragOperations.None;
            }
            return DragOperations.Move;
        }

        private DragOperations PipingCalculationGroupContextCanDrop(object item, TreeNode sourceNode, TreeNode targetNode, DragOperations validOperations)
        {
            if (GetAsIPipingCalculationItem(item) != null && NodesHaveSameParentFailureMechanism(sourceNode, targetNode))
            {
                return validOperations;
            }

            return DragOperations.None;
        }

        private static IPipingCalculationItem GetAsIPipingCalculationItem(object item)
        {
            var calculationContext = item as PipingCalculationContext;
            if (calculationContext != null)
            {
                return calculationContext.WrappedData;
            }

            var groupContext = item as PipingCalculationGroupContext;
            if (groupContext != null)
            {
                return groupContext.WrappedData;
            }

            return null;
        }

        private bool NodesHaveSameParentFailureMechanism(TreeNode sourceNode, TreeNode targetNode)
        {
            var sourceFailureMechanism = GetParentFailureMechanism(sourceNode);
            var targetFailureMechanism = GetParentFailureMechanism(targetNode);

            return ReferenceEquals(sourceFailureMechanism, targetFailureMechanism);
        }

        private static PipingFailureMechanism GetParentFailureMechanism(TreeNode sourceNode)
        {
            PipingFailureMechanism sourceFailureMechanism;
            var node = sourceNode;
            while ((sourceFailureMechanism = node.Tag as PipingFailureMechanism) == null)
            {
                // No parent found, go search higher up hierarchy!
                node = node.Parent;
                if (node == null)
                {
                    break;
                }
            }
            return sourceFailureMechanism;
        }

        private bool PipingCalculationGroupContextCanInsert(object item, TreeNode sourceNode, TreeNode targetNode)
        {
            return GetAsIPipingCalculationItem(item) != null && NodesHaveSameParentFailureMechanism(sourceNode, targetNode);
        }

        private void PipingCalculationGroupContextOnDrop(object item, TreeNode sourceNode, TreeNode targetNode, DragOperations operation, int position)
        {
            IPipingCalculationItem pipingCalculationItem = GetAsIPipingCalculationItem(item);
            var originalOwnerContext = sourceNode.Parent.Tag as PipingCalculationGroupContext;
            var target = targetNode.Tag as PipingCalculationGroupContext;

            if (pipingCalculationItem != null && originalOwnerContext != null && target != null)
            {
                var isMoveWithinSameContainer = ReferenceEquals(targetNode.Tag, originalOwnerContext);

                DroppingPipingCalculationInContainerStrategy dropHandler = GetDragDropStrategy(isMoveWithinSameContainer, originalOwnerContext, target, targetNode.TreeView);
                dropHandler.Execute(item, pipingCalculationItem, position);
            }
        }

        private DroppingPipingCalculationInContainerStrategy GetDragDropStrategy(bool isMoveWithinSameContainer, PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target, TreeView treeView)
        {
            if (isMoveWithinSameContainer)
            {
                return new DroppingPipingCalculationWithinSameContainer(treeView, originalOwnerContext, target);
            }
            return new DroppingPipingCalculationToNewContainer(treeView, originalOwnerContext, target);
        }

        private void SelectNewlyAddedPipingCalculationGroupContextItemInTreeView(TreeNode node)
        {
            // Expand parent of 'newItem' to ensure its selected state is visible.
            if (!node.IsExpanded)
            {
                node.Expand();
            }
            TreeNode newlyAppendedNodeForNewItem = node.Nodes.Last();
            node.TreeView.SelectedNode = newlyAppendedNodeForNewItem;
        }

        #region Nested Types: DroppingPipingCalculationInContainerStrategy and implementations

        /// <summary>
        /// Strategy pattern implementation for dealing with drag & dropping a <see cref="IPipingCalculationItem"/>
        /// onto <see cref="PipingCalculationGroup"/> data.
        /// </summary>
        private abstract class DroppingPipingCalculationInContainerStrategy
        {
            protected readonly TreeView treeView;
            protected readonly PipingCalculationGroupContext target;
            private readonly PipingCalculationGroupContext originalOwnerContext;

            protected DroppingPipingCalculationInContainerStrategy(TreeView treeView, PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target)
            {
                this.treeView = treeView;
                this.originalOwnerContext = originalOwnerContext;
                this.target = target;
            }

            /// <summary>
            /// Perform the drag & drop operation.
            /// </summary>
            /// <param name="draggedDataObject">The actual dragged data object.</param>
            /// <param name="pipingCalculationItem">The piping calculation item corresponding with <see cref="draggedDataObject"/>.</param>
            /// <param name="newPosition">The index of the new position within the new owner's collection.</param>
            public virtual void Execute(object draggedDataObject, IPipingCalculationItem pipingCalculationItem, int newPosition)
            {
                var targetRecordedNodeState = new TreeNodeExpandCollapseState(treeView.GetNodeByTag(target));

                MoveCalculationItemToNewOwner(pipingCalculationItem, newPosition);

                NotifyObservers();

                TreeNode draggedNode = treeView.GetNodeByTag(draggedDataObject);
                UpdateTreeView(draggedNode, targetRecordedNodeState);
            }

            /// <summary>
            /// Moves the <see cref="IPipingCalculationItem"/> instance to its new location.
            /// </summary>
            /// <param name="pipingCalculationItem">The instance to be relocated.</param>
            /// <param name="position">The index in the new <see cref="PipingCalculationGroup"/>
            /// owner within its <see cref="PipingCalculationGroup.Children"/>.</param>
            protected void MoveCalculationItemToNewOwner(IPipingCalculationItem pipingCalculationItem, int position)
            {
                originalOwnerContext.WrappedData.Children.Remove(pipingCalculationItem);
                target.WrappedData.Children.Insert(position, pipingCalculationItem);
            }

            /// <summary>
            /// Notifies observers of the change in state.
            /// </summary>
            protected virtual void NotifyObservers()
            {
                originalOwnerContext.NotifyObservers();
            }

            /// <summary>
            /// Updates the <see cref="System.Windows.Forms.TreeView"/> where the drag & drop
            /// operation took place.
            /// </summary>
            /// <param name="draggedNode">The dragged node.</param>
            /// <param name="targetRecordedNodeState">Recorded state of the target node
            /// before the drag & drop operation.</param>
            protected virtual void UpdateTreeView(TreeNode draggedNode, TreeNodeExpandCollapseState targetRecordedNodeState)
            {
                HandlePostDragExpandCollapseOfNewOwner(draggedNode.Parent, targetRecordedNodeState);
                treeView.SelectedNode = draggedNode;
            }

            private static void HandlePostDragExpandCollapseOfNewOwner(TreeNode parentOfDraggedNode, TreeNodeExpandCollapseState newOwnerRecordedNodeState)
            {
                newOwnerRecordedNodeState.Restore(parentOfDraggedNode);

                // Expand parent of 'draggedNode' to ensure 'draggedNode' is visible.
                if (!parentOfDraggedNode.IsExpanded)
                {
                    parentOfDraggedNode.Expand();
                }
            }
        }

        /// <summary>
        /// Strategy implementation for rearranging the order of an <see cref="IPipingCalculationItem"/>
        /// within a <see cref="PipingCalculationGroup"/> through a drag & drop action.
        /// </summary>
        private class DroppingPipingCalculationWithinSameContainer : DroppingPipingCalculationInContainerStrategy
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingPipingCalculationWithinSameContainer"/> class.
            /// </summary>
            /// <param name="treeView">The tree view where the drag & drop operation occurs.</param>
            /// <param name="originalOwnerContext">The calculation group context that is 
            /// the original owner of the dragged item.</param>
            /// <param name="target">The calculation group context that is the target
            /// of the drag & drop operation.</param>
            public DroppingPipingCalculationWithinSameContainer(TreeView treeView, PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target) :
                base(treeView, originalOwnerContext, target) { }
        }

        /// <summary>
        /// Strategy implementation for moving an <see cref="IPipingCalculationItem"/> from
        /// one <see cref="PipingCalculationGroup"/> to another using a drag & drop action.
        /// </summary>
        private class DroppingPipingCalculationToNewContainer : DroppingPipingCalculationInContainerStrategy
        {
            private TreeNodeExpandCollapseState recordedNodeState;

            private bool renamed;

            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingPipingCalculationToNewContainer"/> class.
            /// </summary>
            /// <param name="treeView">The tree view where the drag & drop operation occurs.</param>
            /// <param name="originalOwnerContext">The calculation group context that is 
            /// the original owner of the dragged item.</param>
            /// <param name="target">The calculation group context that is the target
            /// of the drag & drop operation.</param>
            public DroppingPipingCalculationToNewContainer(TreeView treeView, PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target) :
                base(treeView, originalOwnerContext, target) { }

            public override void Execute(object draggedDataObject, IPipingCalculationItem pipingCalculationItem, int newPosition)
            {
                var targetRecordedNodeState = new TreeNodeExpandCollapseState(treeView.GetNodeByTag(target));

                recordedNodeState = new TreeNodeExpandCollapseState(treeView.GetNodeByTag(draggedDataObject));
                EnsureDraggedNodeHasUniqueNameInNewOwner(pipingCalculationItem, target);

                MoveCalculationItemToNewOwner(pipingCalculationItem, newPosition);

                NotifyObservers();

                TreeNode draggedNode = treeView.GetNodeByTag(draggedDataObject);
                UpdateTreeView(draggedNode, targetRecordedNodeState);
            }

            protected override void UpdateTreeView(TreeNode draggedNode, TreeNodeExpandCollapseState targetRecordedNodeState)
            {
                base.UpdateTreeView(draggedNode, targetRecordedNodeState);
                recordedNodeState.Restore(draggedNode);
                if (renamed)
                {
                    treeView.StartLabelEdit(draggedNode);
                }
            }

            protected override void NotifyObservers()
            {
                base.NotifyObservers();
                target.NotifyObservers();
            }

            private void EnsureDraggedNodeHasUniqueNameInNewOwner(IPipingCalculationItem pipingCalculationItem, PipingCalculationGroupContext newOwner)
            {
                renamed = false;
                string uniqueName = NamingHelper.GetUniqueName(newOwner.WrappedData.Children, pipingCalculationItem.Name, pci => pci.Name);
                if (!pipingCalculationItem.Name.Equals(uniqueName))
                {
                    renamed = TryRenameTo(pipingCalculationItem, uniqueName);
                }
            }

            private static bool TryRenameTo(IPipingCalculationItem pipingCalculationItem, string newName)
            {
                var calculation = pipingCalculationItem as PipingCalculation;
                if (calculation != null)
                {
                    calculation.Name = newName;
                    return true;
                }

                var group = pipingCalculationItem as PipingCalculationGroup;
                if (group != null && group.IsNameEditable)
                {
                    group.Name = newName;
                    return true;
                }

                return false;
            }
        }

        # endregion

        # endregion
    }
}