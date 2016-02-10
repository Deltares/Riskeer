// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;

using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Service;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

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

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new TreeNodeInfo<PipingFailureMechanism>
            {
                Text = pipingFailureMechanism => pipingFailureMechanism.Name,
                Image = pipingFailureMechanism => PipingFormsResources.PipingIcon,
                ContextMenuStrip = FailureMechanismContextMenuStrip,
                ChildNodeObjects = FailureMechanismChildNodeObjects,
            };

            yield return new TreeNodeInfo<PipingCalculationContext>
            {
                Text = pipingCalculationContext => pipingCalculationContext.WrappedData.Name,
                Image = pipingCalculationContext => PipingFormsResources.PipingIcon,
                EnsureVisibleOnCreate = pipingCalculationContext => true,
                ContextMenuStrip = PipingCalculationContextContextMenuStrip,
                ChildNodeObjects = PipingCalculationContextChildNodeObjects,
                CanRename = (pipingCalculationContext, parentData) => true,
                OnNodeRenamed = PipingCalculationContextOnNodeRenamed,
                CanRemove = PipingCalculationContextCanRemove,
                OnNodeRemoved = PipingCalculationContextOnNodeRemoved,
                CanDrag = (pipingCalculationContext, parentData) => true
            };

            yield return new TreeNodeInfo<PipingCalculationGroupContext>
            {
                Text = pipingCalculationGroupContext => pipingCalculationGroupContext.WrappedData.Name,
                Image = pipingCalculationGroupContext => PipingFormsResources.FolderIcon,
                EnsureVisibleOnCreate = pipingCalculationGroupContext => true,
                ChildNodeObjects = PipingCalculationGroupContextChildNodeObjects,
                ContextMenuStrip = PipingCalculationGroupContextContextMenuStrip,
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
                Text = pipingInputContext => PipingFormsResources.PipingInputContext_NodeDisplayName,
                Image = pipingInputContext => PipingFormsResources.PipingInputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
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
                ForeColor = ringtoetsPipingSurfaceLine => ringtoetsPipingSurfaceLine.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = ringtoetsPipingSurfaceLine => ringtoetsPipingSurfaceLine.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
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
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<IEnumerable<PipingSoilProfile>>
            {
                Text = pipingSoilProfiles => PipingFormsResources.PipingSoilProfilesCollection_DisplayName,
                Image = pipingSoilProfiles => PipingFormsResources.FolderIcon,
                ForeColor = pipingSoilProfiles => pipingSoilProfiles.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = pipingSoilProfiles => pipingSoilProfiles.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
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
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<PipingOutput>
            {
                Text = pipingOutput => PipingFormsResources.PipingOutput_DisplayName,
                Image = pipingOutput => PipingFormsResources.PipingOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyPipingOutput>
            {
                Text = emptyPipingOutput => PipingFormsResources.PipingOutput_DisplayName,
                Image = emptyPipingOutput => PipingFormsResources.PipingOutputIcon,
                ForeColor = emptyPipingOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyPipingCalculationReport>
            {
                Text = emptyPipingCalculationReport => PipingDataResources.CalculationReport_DisplayName,
                Image = emptyPipingCalculationReport => PipingFormsResources.PipingCalculationReportIcon,
                ForeColor = emptyPipingCalculationReport => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };
        }

        # region PipingFailureMechanism TreeNodeInfo

        private ContextMenuStrip FailureMechanismContextMenuStrip(PipingFailureMechanism failureMechanism, object parentData, TreeViewControl treeViewControl)
        {
            var addCalculationGroupItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup,
                PipingFormsResources.PipingFailureMechanism_Add_PipingCalculationGroup_Tooltip,
                PipingFormsResources.AddFolderIcon,
                (o, args) => AddCalculationGroup(failureMechanism)
                );

            var addCalculationItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation,
                PipingFormsResources.PipingFailureMechanism_Add_PipingCalculation_Tooltip,
                PipingFormsResources.PipingIcon,
                (s, e) => AddCalculation(failureMechanism)
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

            return Gui.Get(failureMechanism, treeViewControl)
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
                menuItem.ToolTipText = PipingFormsResources.PipingFailureMechanism_CreateCalculateAllItem_No_calculations_to_run;
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
                menuItem.ToolTipText = PipingFormsResources.PipingFailureMechanism_CreateValidateAllItem_No_calculations_to_validate;
            }

            return menuItem;
        }

        private static void ClearAll(PipingFailureMechanism failureMechanism)
        {
            if (MessageBox.Show(PipingFormsResources.PipingCalculationGroupContext_GetContextMenu_Are_you_sure_clear_all_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
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

        private void AddCalculationGroup(PipingFailureMechanism failureMechanism)
        {
            var calculation = new PipingCalculationGroup
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, PipingDataResources.PipingCalculationGroup_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.NotifyObservers();
        }

        private void AddCalculation(PipingFailureMechanism failureMechanism)
        {
            var calculation = new PipingCalculation
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, PipingDataResources.PipingCalculation_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.NotifyObservers();
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
                new PipingCalculationGroupContext(pipingFailureMechanism.CalculationsGroup, pipingFailureMechanism.SurfaceLines, pipingFailureMechanism.SoilProfiles, pipingFailureMechanism),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(pipingFailureMechanism), TreeFolderCategory.Output)
            };
        }

        private static IList GetInputs(PipingFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                failureMechanism.SectionDivisions,
                failureMechanism.SurfaceLines,
                failureMechanism.SoilProfiles,
                failureMechanism.BoundaryConditions
            };
        }

        private IList GetOutputs(PipingFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                failureMechanism.AssessmentResult
            };
        }

        # endregion

        # region PipingCalculationContext TreeNodeInfo

        private ContextMenuStrip PipingCalculationContextContextMenuStrip(PipingCalculationContext nodeData, object parentData, TreeViewControl treeViewControl)
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

            return Gui.Get(nodeData, treeViewControl)
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
            if (MessageBox.Show(PipingFormsResources.PipingCalculationContext_GetContextMenu_Are_you_sure_clear_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            calculation.ClearOutput();
            calculation.NotifyObservers();
        }

        # endregion

        # region PipingCalculationGroupContext TreeNodeInfo

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
                                                                      nodeData.AvailablePipingSoilProfiles,
                                                                      nodeData.PipingFailureMechanism));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new PipingCalculationGroupContext(group,
                                                                           nodeData.AvailablePipingSurfaceLines,
                                                                           nodeData.AvailablePipingSoilProfiles,
                                                                           nodeData.PipingFailureMechanism));
                }
                else
                {
                    childNodeObjects.Add(item);
                }
            }

            return childNodeObjects.ToArray();
        }

        private ContextMenuStrip PipingCalculationGroupContextContextMenuStrip(PipingCalculationGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
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
                });

            var validateAllItem = CreateValidateAllItem(group);

            var calculateAllItem = CreateCalculateAllItem(group);

            var clearAllItem = new StrictContextMenuItem(
                RingtoetsFormsResources.Clear_all_output,
                PipingFormsResources.PipingCalculationGroup_ClearOutput_ToolTip,
                RingtoetsFormsResources.ClearIcon, (o, args) =>
                {
                    if (MessageBox.Show(PipingFormsResources.PipingCalculationGroupContext_GetContextMenu_Are_you_sure_clear_all_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
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

            var builder = Gui.Get(nodeData, treeViewControl)
                             .AddCustomItem(addCalculationGroupItem)
                             .AddCustomItem(addCalculationItem)
                             .AddSeparator()
                             .AddCustomItem(validateAllItem)
                             .AddCustomItem(calculateAllItem)
                             .AddCustomItem(clearAllItem)
                             .AddSeparator();

            var isRenamable = PipingCalculationGroupContextCanRenameNode(nodeData, parentData);
            var isRemovable = PipingCalculationGroupContextCanRemove(nodeData, parentData);

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
                menuItem.ToolTipText = PipingFormsResources.PipingFailureMechanism_CreateCalculateAllItem_No_calculations_to_run;
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
                menuItem.ToolTipText = PipingFormsResources.PipingFailureMechanism_CreateValidateAllItem_No_calculations_to_validate;
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

        private bool PipingCalculationGroupContextCanRenameNode(PipingCalculationGroupContext pipingCalculationGroupContext, object parentData)
        {
            return !(parentData is PipingFailureMechanism);
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

        private bool PipingCalculationGroupContextCanDrag(PipingCalculationGroupContext nodeData, object parentData)
        {
            if (parentData is PipingFailureMechanism)
            {
                return false;
            }

            return true;
        }

        private DragOperations PipingCalculationGroupContextCanDrop(object draggedData, object targetData)
        {
            if (GetAsIPipingCalculationItem(draggedData) != null && NodesHaveSameParentFailureMechanism(draggedData, targetData))
            {
                return DragOperations.Move;
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

        private bool NodesHaveSameParentFailureMechanism(object draggedData, object targetData)
        {
            var sourceFailureMechanism = GetParentFailureMechanism(draggedData);
            var targetFailureMechanism = GetParentFailureMechanism(targetData);

            return ReferenceEquals(sourceFailureMechanism, targetFailureMechanism);
        }

        private static PipingFailureMechanism GetParentFailureMechanism(object data)
        {
            var calculationContext = data as PipingCalculationContext;
            if (calculationContext != null)
            {
                return calculationContext.PipingFailureMechanism;
            }

            var groupContext = data as PipingCalculationGroupContext;
            if (groupContext != null)
            {
                return groupContext.PipingFailureMechanism;
            }

            return null;
        }

        private bool PipingCalculationGroupContextCanInsert(object draggedData, object targetData)
        {
            return GetAsIPipingCalculationItem(draggedData) != null && NodesHaveSameParentFailureMechanism(draggedData, targetData);
        }

        private void PipingCalculationGroupContextOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl treeViewControl)
        {
            IPipingCalculationItem pipingCalculationItem = GetAsIPipingCalculationItem(droppedData);
            var originalOwnerContext = oldParentData as PipingCalculationGroupContext;
            var target = newParentData as PipingCalculationGroupContext;

            if (pipingCalculationItem != null && originalOwnerContext != null && target != null)
            {
                var isMoveWithinSameContainer = ReferenceEquals(originalOwnerContext, target);

                DroppingPipingCalculationInContainerStrategy dropHandler = GetDragDropStrategy(isMoveWithinSameContainer, originalOwnerContext, target);
                dropHandler.Execute(droppedData, pipingCalculationItem, position, treeViewControl);
            }
        }

        private DroppingPipingCalculationInContainerStrategy GetDragDropStrategy(bool isMoveWithinSameContainer, PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target)
        {
            if (isMoveWithinSameContainer)
            {
                return new DroppingPipingCalculationWithinSameContainer(originalOwnerContext, target);
            }
            return new DroppingPipingCalculationToNewContainer(originalOwnerContext, target);
        }

        #region Nested Types: DroppingPipingCalculationInContainerStrategy and implementations

        /// <summary>
        /// Strategy pattern implementation for dealing with drag & dropping a <see cref="IPipingCalculationItem"/>
        /// onto <see cref="PipingCalculationGroup"/> data.
        /// </summary>
        private abstract class DroppingPipingCalculationInContainerStrategy
        {
            protected readonly PipingCalculationGroupContext target;
            private readonly PipingCalculationGroupContext originalOwnerContext;

            protected DroppingPipingCalculationInContainerStrategy(PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target)
            {
                this.originalOwnerContext = originalOwnerContext;
                this.target = target;
            }

            /// <summary>
            /// Perform the drag & drop operation.
            /// </summary>
            /// <param name="draggedData">The dragged data.</param>
            /// <param name="pipingCalculationItem">The piping calculation item wrapped by <see cref="draggedData"/>.</param>
            /// <param name="newPosition">The index of the new position within the new owner's collection.</param>
            /// <param name="treeViewControl">The tree view control which is at stake.</param>
            public virtual void Execute(object draggedData, IPipingCalculationItem pipingCalculationItem, int newPosition, TreeViewControl treeViewControl)
            {
                MoveCalculationItemToNewOwner(pipingCalculationItem, newPosition);

                NotifyObservers();
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
            /// <param name="originalOwnerContext">The calculation group context that is 
            /// the original owner of the dragged item.</param>
            /// <param name="target">The calculation group context that is the target
            /// of the drag & drop operation.</param>
            public DroppingPipingCalculationWithinSameContainer(PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target) :
                base(originalOwnerContext, target) { }
        }

        /// <summary>
        /// Strategy implementation for moving an <see cref="IPipingCalculationItem"/> from
        /// one <see cref="PipingCalculationGroup"/> to another using a drag & drop action.
        /// </summary>
        private class DroppingPipingCalculationToNewContainer : DroppingPipingCalculationInContainerStrategy
        {
            private bool renamed;

            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingPipingCalculationToNewContainer"/> class.
            /// </summary>
            /// <param name="originalOwnerContext">The calculation group context that is 
            /// the original owner of the dragged item.</param>
            /// <param name="target">The calculation group context that is the target
            /// of the drag & drop operation.</param>
            public DroppingPipingCalculationToNewContainer(PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target) :
                base(originalOwnerContext, target) { }

            public override void Execute(object draggedData, IPipingCalculationItem pipingCalculationItem, int newPosition, TreeViewControl treeViewControl)
            {
                EnsureDraggedNodeHasUniqueNameInNewOwner(pipingCalculationItem, target);

                MoveCalculationItemToNewOwner(pipingCalculationItem, newPosition);

                NotifyObservers();

                if (renamed)
                {
                    treeViewControl.StartRenameForData(draggedData);
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