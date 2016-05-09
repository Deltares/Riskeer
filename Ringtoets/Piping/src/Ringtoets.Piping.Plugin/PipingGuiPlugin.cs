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
            yield return new PropertyInfo<PipingCalculationScenarioContext, PipingCalculationContextProperties>();
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
                CloseForData = ClosePipingFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant
            };

            yield return new ViewInfo<PipingCalculationGroupContext, CalculationGroup, PipingCalculationsView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, calculationGroup) => calculationGroup.Name,
                Image = RingtoetsCommonFormsResources.GeneralFolderIcon,
                AdditionalDataCheck = context => context.WrappedData == context.FailureMechanism.CalculationsGroup,
                CloseForData = ClosePipingCalculationsViewForData,
                AfterCreate = (view, context) =>
                {
                    view.ApplicationSelection = Gui;
                    view.AssessmentSection = context.AssessmentSection;
                    view.PipingFailureMechanism = context.FailureMechanism;
                }
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new DefaultFailureMechanismTreeNodeInfo<PipingFailureMechanismContext, PipingFailureMechanism>(
                FailureMechanismChildNodeObjects,
                FailureMechanismContextMenuStrip,
                Gui);

//            yield return new TreeNodeInfo<PipingCalculationScenarioContext>
//            {
//                ContextMenuStrip = PipingCalculationContextContextMenuStrip,
//                ChildNodeObjects = PipingCalculationContextChildNodeObjects,
//                CanRename = (pipingCalculationContext, parentData) => true,
//                OnNodeRenamed = PipingCalculationContextOnNodeRenamed,
//                CanRemove = PipingCalculationContextCanRemove,
//                OnNodeRemoved = PipingCalculationContextOnNodeRemoved,
//                CanDrag = (pipingCalculationContext, parentData) => true
//            };

            yield return CalculationTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<PipingCalculationScenarioContext>(
                PipingFormsResources.PipingIcon,
                PipingCalculationContextChildNodeObjects);

            yield return CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<PipingCalculationGroupContext>(
                PipingCalculationGroupContextChildNodeObjects,
                PipingCalculationGroupContextContextMenuStrip,
                PipingCalculationGroupContextOnNodeRemoved);

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
                Image = ringtoetsPipingSurfaceLine => RingtoetsCommonFormsResources.GeneralFolderIcon,
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
                Image = stochasticSoilModelContext => RingtoetsCommonFormsResources.GeneralFolderIcon,
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
                Text = pipingOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = pipingOutput => PipingFormsResources.PipingOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyPipingOutput>
            {
                Text = emptyPipingOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
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
            var pipingFailureMechanism = o as PipingFailureMechanism;

            var viewPipingFailureMechanismContext = (PipingFailureMechanismContext) view.Data;
            var viewPipingFailureMechanism = viewPipingFailureMechanismContext.WrappedData;

            return assessmentSection != null
                       ? ReferenceEquals(viewPipingFailureMechanismContext.Parent, assessmentSection)
                       : ReferenceEquals(viewPipingFailureMechanism, pipingFailureMechanism);
        }

        # endregion

        # region PipingCalculationsView ViewInfo

        private static bool ClosePipingCalculationsViewForData(PipingCalculationsView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var pipingFailureMechanism = o as PipingFailureMechanism;
            var pipingFailureMechanismContext = o as PipingFailureMechanismContext;

            if (pipingFailureMechanismContext != null)
            {
                pipingFailureMechanism = pipingFailureMechanismContext.WrappedData;
            }
            if (assessmentSection != null)
            {
                pipingFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                          .OfType<PipingFailureMechanism>()
                                                          .FirstOrDefault();
            }

            return pipingFailureMechanism != null && ReferenceEquals(view.Data, pipingFailureMechanism.CalculationsGroup);
        }

        #endregion endregion

        # region Piping TreeNodeInfo

        private ContextMenuStrip FailureMechanismContextMenuStrip(PipingFailureMechanismContext pipingFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var changeRelevancyItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant,
                RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                RingtoetsCommonFormsResources.Checkbox_ticked,
                (sender, args) =>
                {
                    Gui.ViewCommands.RemoveAllViewsForItem(pipingFailureMechanismContext);
                    pipingFailureMechanismContext.WrappedData.IsRelevant = false;
                    pipingFailureMechanismContext.WrappedData.NotifyObservers();
                }
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
                      .AddCustomItem(changeRelevancyItem)
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

        # region PipingCalculationScenarioContext TreeNodeInfo

        private ContextMenuStrip PipingCalculationContextContextMenuStrip(PipingCalculationScenarioContext nodeData, object parentData, TreeViewControl treeViewControl)
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

        private static object[] PipingCalculationContextChildNodeObjects(PipingCalculationScenarioContext pipingCalculationScenarioContext)
        {
            var childNodes = new List<object>
            {
                new CommentContext<ICommentable>(pipingCalculationScenarioContext.WrappedData),
                new PipingInputContext(pipingCalculationScenarioContext.WrappedData.InputParameters,
                                       pipingCalculationScenarioContext.WrappedData,
                                       pipingCalculationScenarioContext.AvailablePipingSurfaceLines,
                                       pipingCalculationScenarioContext.AvailableStochasticSoilModels,
                                       pipingCalculationScenarioContext.FailureMechanism,
                                       pipingCalculationScenarioContext.AssessmentSection)
            };

            if (pipingCalculationScenarioContext.WrappedData.HasOutput)
            {
                childNodes.Add(pipingCalculationScenarioContext.WrappedData.SemiProbabilisticOutput);
                childNodes.Add(new EmptyPipingCalculationReport());
            }
            else
            {
                childNodes.Add(new EmptyPipingOutput());
                childNodes.Add(new EmptyPipingCalculationReport());
            }

            return childNodes.ToArray();
        }

        private static void PipingCalculationContextOnNodeRenamed(PipingCalculationScenarioContext pipingCalculationScenarioContext, string newName)
        {
            pipingCalculationScenarioContext.WrappedData.Name = newName;
            pipingCalculationScenarioContext.WrappedData.NotifyObservers();
        }

        private bool PipingCalculationContextCanRemove(PipingCalculationScenarioContext pipingCalculationScenarioContext, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as PipingCalculationGroupContext;
            return calculationGroupContext != null && calculationGroupContext.WrappedData.Children.Contains(pipingCalculationScenarioContext.WrappedData);
        }

        private void PipingCalculationContextOnNodeRemoved(PipingCalculationScenarioContext pipingCalculationScenarioContext, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as PipingCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                var succesfullyRemovedData = calculationGroupContext.WrappedData.Children.Remove(pipingCalculationScenarioContext.WrappedData);
                if (succesfullyRemovedData)
                {
                    PipingCalculationScenarioService.RemoveCalculationScenarioFromSectionResult(pipingCalculationScenarioContext.WrappedData, pipingCalculationScenarioContext.FailureMechanism);
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
                var calculation = item as PipingCalculationScenario;
                var group = item as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new PipingCalculationScenarioContext(calculation,
                                                                              nodeData.AvailablePipingSurfaceLines,
                                                                              nodeData.AvailableStochasticSoilModels,
                                                                              nodeData.FailureMechanism,
                                                                              nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new PipingCalculationGroupContext(group,
                                                                           nodeData.AvailablePipingSurfaceLines,
                                                                           nodeData.AvailableStochasticSoilModels,
                                                                           nodeData.FailureMechanism,
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
            var builder = Gui.Get(nodeData, treeViewControl);
            var isNestedGroup = parentData is PipingCalculationGroupContext;

            var generateCalculationsItem = CreateGeneratePipingCalculationsItem(nodeData);
            var validateAllItem = CreateValidateAllItem(group);

            if (!isNestedGroup)
            {
                builder
                    .AddOpenItem()
                    .AddSeparator()
                    .AddCustomItem(generateCalculationsItem)
                    .AddSeparator();
            }

            CalculationTreeNodeInfoFactory.AddCreateCalculationGroupItem(builder, group);
            CalculationTreeNodeInfoFactory.AddCreateCalculationItem(builder, nodeData, AddCalculationScenario);
            builder.AddSeparator();

            builder
                .AddCustomItem(validateAllItem);
            CalculationTreeNodeInfoFactory.AddPerformAllCalculationsInGroupItem(builder, group, CalculateAll);
            CalculationTreeNodeInfoFactory.AddClearAllCalculationOutputInGroupItem(builder, group);
            builder.AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
                builder.AddDeleteItem();
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

        private static void AddCalculationScenario(PipingCalculationGroupContext nodeData)
        {
            var calculation = new PipingCalculationScenario(nodeData.FailureMechanism.GeneralInput, nodeData.FailureMechanism.NormProbabilityInput)
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children, PipingDataResources.PipingCalculation_DefaultName, c => c.Name)
            };

            nodeData.WrappedData.Children.Add(calculation);
            nodeData.WrappedData.NotifyObservers();
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

            GeneratePipingCalculations(nodeData.WrappedData, view.SelectedSurfaceLines, nodeData.AvailableStochasticSoilModels, nodeData.FailureMechanism.GeneralInput, nodeData.FailureMechanism.NormProbabilityInput);

            nodeData.NotifyObservers();

            nodeData.WrappedData.AddCalculationScenariosToFailureMechanismSectionResult(nodeData.FailureMechanism);
            nodeData.FailureMechanism.NotifyObservers();
        }

        private void GeneratePipingCalculations(CalculationGroup target, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<StochasticSoilModel> soilModels, GeneralPipingInput generalInput, NormProbabilityPipingInput normProbabilityInput)
        {
            foreach (var group in PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(surfaceLines, soilModels, generalInput, normProbabilityInput))
            {
                target.Children.Add(group);
            }
        }

        private static StrictContextMenuItem CreateValidateAllItem(CalculationGroup group)
        {
            var menuItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Validate_all,
                PipingFormsResources.PipingCalculationGroup_Validate_All_ToolTip,
                RingtoetsCommonFormsResources.ValidateAllIcon, (o, args) => { ValidateAll(group); });

            if (!group.GetCalculations().Any())
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = PipingFormsResources.PipingFailureMechanism_CreateValidateAllItem_No_calculations_to_validate;
            }

            return menuItem;
        }

        private void CalculateAll(CalculationGroup group)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, group.GetCalculations().OfType<PipingCalculationScenario>().Select(pc => new PipingCalculationActivity(pc)));
        }

        private static void ValidateAll(CalculationGroup group)
        {
            foreach (PipingCalculation calculation in group.GetCalculations().OfType<PipingCalculation>())
            {
                PipingCalculationService.Validate(calculation);
            }
        }

        private void PipingCalculationGroupContextOnNodeRemoved(PipingCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (PipingCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            foreach (var calculation in nodeData.WrappedData.GetCalculations().Cast<PipingCalculationScenario>())
            {
                PipingCalculationScenarioService.RemoveCalculationScenarioFromSectionResult(calculation, nodeData.FailureMechanism);
            }

            parentGroupContext.NotifyObservers();
        }

        #endregion
    }
}