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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Piping.Plugin
{
    /// <summary>
    /// The GUI plug-in for the <see cref="PipingFailureMechanism"/>.
    /// </summary>
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
            yield return new PropertyInfo<PipingFailureMechanismContext, PipingFailureMechanismContextProperties>();
            yield return new PropertyInfo<PipingCalculationContext, PipingCalculationContextProperties>();
            yield return new PropertyInfo<PipingCalculationGroupContext, PipingCalculationGroupContextProperties>();
            yield return new PropertyInfo<PipingInputContext, PipingInputContextProperties>();
            yield return new PropertyInfo<PipingSemiProbabilisticOutput, PipingSemiProbabilisticOutputProperties>();
            yield return new PropertyInfo<RingtoetsPipingSurfaceLine, RingtoetsPipingSurfaceLineProperties>();
            yield return new PropertyInfo<StochasticSoilModel, StochasticSoilModelProperties>();
            yield return new PropertyInfo<StochasticSoilProfile, StochasticSoilProfileProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<PipingFailureMechanismContext, PipingFailureMechanismView>
            {
                GetViewName = (view, mechanism) => PipingDataResources.PipingFailureMechanism_DisplayName,
                Image = PipingFormsResources.PipingIcon,
                CloseForData = ClosePipingFailureMechanismViewForData
            };

            yield return new ViewInfo<PipingCalculationGroupContext, PipingCalculationGroup, PipingCalculationsView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, calculationGroup) => calculationGroup.Name,
                Image = PipingFormsResources.FolderIcon,
                AdditionalDataCheck = context => context.WrappedData == context.PipingFailureMechanism.CalculationsGroup,
                CloseForData = ClosePipingCalculationsViewForData,
                AfterCreate = (view, context) =>
                {
                    view.ApplicationSelection = Gui;
                    view.AssessmentSection = context.AssessmentSection;
                    view.PipingFailureMechanism = context.PipingFailureMechanism;
                }
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new DefaultFailureMechanismTreeNodeInfo<PipingFailureMechanismContext, PipingFailureMechanism>(
                FailureMechanismChildNodeObjects,
                FailureMechanismContextMenuStrip,
                Gui);

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
                CanDrop = PipingCalculationGroupContextCanDropOrCanInsert,
                CanInsert = PipingCalculationGroupContextCanDropOrCanInsert,
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

            yield return new TreeNodeInfo<RingtoetsPipingSurfaceLinesContext>
            {
                Text = ringtoetsPipingSurfaceLine => PipingFormsResources.PipingSurfaceLinesCollection_DisplayName,
                Image = ringtoetsPipingSurfaceLine => PipingFormsResources.FolderIcon,
                ForeColor = ringtoetsPipingSurfaceLine => ringtoetsPipingSurfaceLine.FailureMechanism.SurfaceLines.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = ringtoetsPipingSurfaceLine => ringtoetsPipingSurfaceLine.FailureMechanism.SurfaceLines.Cast<object>().ToArray(),
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

            yield return new TreeNodeInfo<StochasticSoilModelContext>
            {
                Text = stochasticSoilModelContext => PipingFormsResources.StochasticSoilProfileCollection_DisplayName,
                Image = stochasticSoilModelContext => PipingFormsResources.FolderIcon,
                ForeColor = stochasticSoilModelContext => stochasticSoilModelContext.FailureMechanism.StochasticSoilModels.Any() ?
                                                              Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = stochasticSoilModelContext => stochasticSoilModelContext.FailureMechanism.StochasticSoilModels.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddImportItem()
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddExpandAllItem()
                                                                                 .AddCollapseAllItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StochasticSoilModel>
            {
                Text = stochasticSoilModel => stochasticSoilModel.Name,
                Image = stochasticSoilModel => PipingFormsResources.StochasticSoilModelIcon,
                ChildNodeObjects = stochasticSoilModel => stochasticSoilModel.StochasticSoilProfiles.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StochasticSoilProfile>
            {
                Text = pipingSoilProfile => (pipingSoilProfile.SoilProfile != null) ? pipingSoilProfile.SoilProfile.Name : "Profile",
                Image = pipingSoilProfile => PipingFormsResources.PipingSoilProfileIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<PipingSemiProbabilisticOutput>
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

        # region PipingCalculationsView ViewInfo

        private bool ClosePipingFailureMechanismViewForData(PipingFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            return assessmentSection != null && ReferenceEquals(((PipingFailureMechanismContext) view.Data).Parent, assessmentSection);
        }

        # endregion

        # region PipingCalculationsView ViewInfo

        private static bool ClosePipingCalculationsViewForData(PipingCalculationsView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            if (assessmentSection != null)
            {
                var pipingFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                              .OfType<PipingFailureMechanism>()
                                                              .FirstOrDefault();

                if (pipingFailureMechanism != null)
                {
                    return view.Data == pipingFailureMechanism.CalculationsGroup;
                }
            }

            return false;
        }

        #endregion endregion

        # region Piping TreeNodeInfo

        private ContextMenuStrip FailureMechanismContextMenuStrip(PipingFailureMechanismContext pipingFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var addCalculationGroupItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup,
                PipingFormsResources.PipingFailureMechanism_Add_PipingCalculationGroup_Tooltip,
                PipingFormsResources.AddFolderIcon,
                (o, args) => AddCalculationGroup(pipingFailureMechanismContext.WrappedData)
                );

            var addCalculationItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation,
                PipingFormsResources.PipingFailureMechanism_Add_PipingCalculation_Tooltip,
                PipingFormsResources.PipingIcon,
                (s, e) => AddCalculation(pipingFailureMechanismContext.WrappedData)
                );

            var validateAllItem = CreateValidateAllItem(pipingFailureMechanismContext.WrappedData);

            var calculateAllItem = CreateCalculateAllItem(pipingFailureMechanismContext.WrappedData);

            var clearAllItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Clear_all_output,
                RingtoetsCommonFormsResources.Clear_all_output_ToolTip,
                RingtoetsCommonFormsResources.ClearIcon,
                (o, args) => ClearAll(pipingFailureMechanismContext.WrappedData)
                );

            if (!GetAllPipingCalculations(pipingFailureMechanismContext.WrappedData).Any(c => c.HasOutput))
            {
                clearAllItem.Enabled = false;
                clearAllItem.ToolTipText = PipingFormsResources.PipingCalculationGroup_ClearOutput_No_calculation_with_output_to_clear;
            }

            return Gui.Get(pipingFailureMechanismContext, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
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

        private static IEnumerable<PipingCalculation> GetAllPipingCalculations(PipingFailureMechanism failureMechanism)
        {
            return failureMechanism.Calculations.OfType<PipingCalculation>();
        }

        private StrictContextMenuItem CreateCalculateAllItem(PipingFailureMechanism failureMechanism)
        {
            var menuItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                RingtoetsCommonFormsResources.Calculate_all_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (o, args) => CalculateAll(failureMechanism)
                );

            if (!GetAllPipingCalculations(failureMechanism).Any())
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

            if (!GetAllPipingCalculations(failureMechanism).Any())
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = PipingFormsResources.PipingFailureMechanism_CreateValidateAllItem_No_calculations_to_validate;
            }

            return menuItem;
        }

        private static void ClearAll(PipingFailureMechanism failureMechanism)
        {
            if (MessageBox.Show(PipingFormsResources.PipingCalculationGroupContext_ContextMenuStrip_Are_you_sure_clear_all_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            foreach (ICalculation calc in failureMechanism.Calculations)
            {
                calc.ClearOutput();
                calc.NotifyObservers();
            }
        }

        private void ValidateAll(PipingFailureMechanism failureMechanism)
        {
            foreach (PipingCalculation calculation in GetAllPipingCalculations(failureMechanism))
            {
                PipingCalculationService.Validate(calculation);
            }
        }

        private void CalculateAll(PipingFailureMechanism failureMechanism)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, GetAllPipingCalculations(failureMechanism).Select(calc => new PipingCalculationActivity(calc)));
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
            var calculation = new PipingCalculationScenario(failureMechanism.GeneralInput, failureMechanism.SemiProbabilisticInput)
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, PipingDataResources.PipingCalculation_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.NotifyObservers();
        }

        private object[] FailureMechanismChildNodeObjects(PipingFailureMechanismContext pipingFailureMechanismContext)
        {
            PipingFailureMechanism wrappedData = pipingFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, pipingFailureMechanismContext.Parent), TreeFolderCategory.Input),
                new PipingCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData.SurfaceLines, wrappedData.StochasticSoilModels, wrappedData, pipingFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static IList GetInputs(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection),
                new StochasticSoilModelContext(failureMechanism, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private IList GetOutputs(PipingFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext(failureMechanism.SectionResults, failureMechanism)
            };
        }

        # endregion

        # region PipingCalculationContext TreeNodeInfo

        private ContextMenuStrip PipingCalculationContextContextMenuStrip(PipingCalculationContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            PipingCalculation calculation = nodeData.WrappedData;
            var validateItem = new StrictContextMenuItem(RingtoetsCommonFormsResources.Validate,
                                                         RingtoetsCommonFormsResources.Validate_ToolTip,
                                                         RingtoetsCommonFormsResources.ValidateIcon,
                                                         (o, args) => { PipingCalculationService.Validate(calculation); });
            var calculateItem = new StrictContextMenuItem(RingtoetsCommonFormsResources.Calculate,
                                                          RingtoetsCommonFormsResources.Calculate_ToolTip,
                                                          RingtoetsCommonFormsResources.CalculateIcon,
                                                          (o, args) => { ActivityProgressDialogRunner.Run(Gui.MainWindow, new PipingCalculationActivity(calculation)); });

            var clearOutputItem = new StrictContextMenuItem(PipingFormsResources.Clear_output,
                                                            PipingFormsResources.Clear_output_ToolTip,
                                                            RingtoetsCommonFormsResources.ClearIcon,
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
                new CommentContext<ICommentable>(pipingCalculationContext.WrappedData),
                new PipingInputContext(pipingCalculationContext.WrappedData.InputParameters,
                                       pipingCalculationContext.WrappedData,
                                       pipingCalculationContext.AvailablePipingSurfaceLines,
                                       pipingCalculationContext.AvailableStochasticSoilModels,
                                       pipingCalculationContext.PipingFailureMechanism,
                                       pipingCalculationContext.AssessmentSection)
            };

            if (pipingCalculationContext.WrappedData.HasOutput)
            {
                childNodes.Add(pipingCalculationContext.WrappedData.SemiProbabilisticOutput);
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
            return calculationGroupContext != null && calculationGroupContext.WrappedData.Children.Contains(pipingCalculationContext.WrappedData);
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
            if (MessageBox.Show(PipingFormsResources.PipingCalculationContext_ContextMenuStrip_Are_you_sure_clear_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
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

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                var calculation = item as PipingCalculation;
                var group = item as PipingCalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new PipingCalculationContext(calculation,
                                                                      nodeData.AvailablePipingSurfaceLines,
                                                                      nodeData.AvailableStochasticSoilModels,
                                                                      nodeData.PipingFailureMechanism,
                                                                      nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new PipingCalculationGroupContext(group,
                                                                           nodeData.AvailablePipingSurfaceLines,
                                                                           nodeData.AvailableStochasticSoilModels,
                                                                           nodeData.PipingFailureMechanism,
                                                                           nodeData.AssessmentSection));
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
                    var calculation = new PipingCalculationScenario(nodeData.PipingFailureMechanism.GeneralInput, nodeData.PipingFailureMechanism.SemiProbabilisticInput)
                    {
                        Name = NamingHelper.GetUniqueName(group.Children, PipingDataResources.PipingCalculation_DefaultName, c => c.Name)
                    };

                    group.Children.Add(calculation);
                    nodeData.NotifyObservers();
                });

            var generateCalculationsItem = CreateGeneratePipingCalculationsItem(nodeData);

            var validateAllItem = CreateValidateAllItem(group);

            var calculateAllItem = CreateCalculateAllItem(group);

            var clearAllItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Clear_all_output,
                PipingFormsResources.PipingCalculationGroup_ClearOutput_ToolTip,
                RingtoetsCommonFormsResources.ClearIcon, (o, args) =>
                {
                    if (MessageBox.Show(PipingFormsResources.PipingCalculationGroupContext_ContextMenuStrip_Are_you_sure_clear_all_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
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

            var builder = Gui.Get(nodeData, treeViewControl);

            if (parentData is PipingFailureMechanismContext)
            {
                builder
                    .AddOpenItem()
                    .AddSeparator()
                    .AddCustomItem(generateCalculationsItem)
                    .AddSeparator();
            }

            builder
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

        private StrictContextMenuItem CreateGeneratePipingCalculationsItem(PipingCalculationGroupContext nodeData)
        {
            var surfaceLineAvailable = nodeData.AvailablePipingSurfaceLines.Any() && nodeData.AvailableStochasticSoilModels.Any();

            var pipingCalculationGroupGeneratePipingCalculationsToolTip =
                surfaceLineAvailable ?
                    PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_ToolTip :
                    PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_NoSurfaceLinesOrSoilModels_ToolTip;

            var generateCalculationsItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations,
                pipingCalculationGroupGeneratePipingCalculationsToolTip,
                PipingFormsResources.GeneratePipingCalculationsIcon, (o, args) => { ShowSurfaceLineSelectionDialog(nodeData); })
            {
                Enabled = surfaceLineAvailable
            };
            return generateCalculationsItem;
        }

        private void ShowSurfaceLineSelectionDialog(PipingCalculationGroupContext nodeData)
        {
            var view = new PipingSurfaceLineSelectionDialog(Gui.MainWindow, nodeData.AvailablePipingSurfaceLines);
            view.ShowDialog();

            GeneratePipingCalculations(nodeData.WrappedData, view.SelectedSurfaceLines, nodeData.AvailableStochasticSoilModels, nodeData.PipingFailureMechanism.GeneralInput, nodeData.PipingFailureMechanism.SemiProbabilisticInput);

            nodeData.NotifyObservers();

            nodeData.WrappedData.AddCalculationScenariosToFailureMechanismSectionResult(nodeData.PipingFailureMechanism);
        }

        private void GeneratePipingCalculations(PipingCalculationGroup target, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<StochasticSoilModel> soilModels, GeneralPipingInput generalInput, SemiProbabilisticPipingInput semiProbabilisticInput)
        {
            foreach (var group in PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(surfaceLines, soilModels, generalInput, semiProbabilisticInput))
            {
                target.Children.Add(group);
            }
        }

        private StrictContextMenuItem CreateCalculateAllItem(PipingCalculationGroup group)
        {
            var menuItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                PipingFormsResources.PipingCalculationGroup_CalculateAll_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon, (o, args) => { CalculateAll(group); });

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
                RingtoetsCommonFormsResources.Validate_all,
                PipingFormsResources.PipingCalculationGroup_Validate_All_ToolTip,
                RingtoetsCommonFormsResources.ValidateAllIcon, (o, args) => { ValidateAll(group); });

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
            return !(parentData is PipingFailureMechanismContext);
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
            if (parentData is PipingFailureMechanismContext)
            {
                return false;
            }

            return true;
        }

        private bool PipingCalculationGroupContextCanDropOrCanInsert(object draggedData, object targetData)
        {
            return GetAsICalculationItem(draggedData) != null && NodesHaveSameParentFailureMechanism(draggedData, targetData);
        }

        private static ICalculationBase GetAsICalculationItem(object item)
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

        private void PipingCalculationGroupContextOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl treeViewControl)
        {
            ICalculationBase calculationItem = GetAsICalculationItem(droppedData);
            var originalOwnerContext = oldParentData as PipingCalculationGroupContext;
            var target = newParentData as PipingCalculationGroupContext;

            if (calculationItem != null && originalOwnerContext != null && target != null)
            {
                var isMoveWithinSameContainer = ReferenceEquals(originalOwnerContext, target);

                DroppingPipingCalculationInContainerStrategy dropHandler = GetDragDropStrategy(isMoveWithinSameContainer, originalOwnerContext, target);
                dropHandler.Execute(droppedData, calculationItem, position, treeViewControl);
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
        /// Strategy pattern implementation for dealing with drag & dropping a <see cref="ICalculationItem"/>
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
            /// <param name="calculationItem">The calculation item wrapped by <see cref="draggedData"/>.</param>
            /// <param name="newPosition">The index of the new position within the new owner's collection.</param>
            /// <param name="treeViewControl">The tree view control which is at stake.</param>
            public virtual void Execute(object draggedData, ICalculationBase calculationItem, int newPosition, TreeViewControl treeViewControl)
            {
                MoveCalculationItemToNewOwner(calculationItem, newPosition);

                NotifyObservers();
            }

            /// <summary>
            /// Moves the <see cref="ICalculationBase"/> instance to its new location.
            /// </summary>
            /// <param name="calculationItem">The instance to be relocated.</param>
            /// <param name="position">The index in the new <see cref="PipingCalculationGroup"/>
            /// owner within its <see cref="PipingCalculationGroup.Children"/>.</param>
            protected void MoveCalculationItemToNewOwner(ICalculationBase calculationItem, int position)
            {
                originalOwnerContext.WrappedData.Children.Remove(calculationItem);
                target.WrappedData.Children.Insert(position, calculationItem);
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
        /// Strategy implementation for rearranging the order of an <see cref="ICalculationItem"/>
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
                base(originalOwnerContext, target) {}
        }

        /// <summary>
        /// Strategy implementation for moving an <see cref="ICalculationItem"/> from
        /// one <see cref="PipingCalculationGroup"/> to another using a drag & drop action.
        /// </summary>
        private class DroppingPipingCalculationToNewContainer : DroppingPipingCalculationInContainerStrategy
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingPipingCalculationToNewContainer"/> class.
            /// </summary>
            /// <param name="originalOwnerContext">The calculation group context that is 
            /// the original owner of the dragged item.</param>
            /// <param name="target">The calculation group context that is the target
            /// of the drag & drop operation.</param>
            public DroppingPipingCalculationToNewContainer(PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target) :
                base(originalOwnerContext, target) {}

            public override void Execute(object draggedData, ICalculationBase calculationItem, int newPosition, TreeViewControl treeViewControl)
            {
                MoveCalculationItemToNewOwner(calculationItem, newPosition);

                NotifyObservers();

                // Try to start a name edit action when an item with the same name was already present
                if (target.WrappedData.Children.Except(new[]
                {
                    calculationItem
                }).Any(c => c.Name.Equals(calculationItem.Name)))
                {
                    treeViewControl.TryRenameNodeForData(draggedData);
                }
            }

            protected override void NotifyObservers()
            {
                base.NotifyObservers();
                target.NotifyObservers();
            }
        }

        # endregion

        # endregion
    }
}