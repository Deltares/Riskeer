﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

            yield return new ViewInfo<
                FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>, 
                IEnumerable<PipingFailureMechanismSectionResult>, 
                PipingFailureMechanismResultView>
            {
                GetViewName = (v, o) => RingtoetsCommonDataResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.SectionResults,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
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
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<PipingFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<PipingCalculationScenarioContext>(
                PipingFormsResources.PipingIcon,
                PipingCalculationContextChildNodeObjects,
                PipingCalculationContextContextMenuStrip,
                PipingCalculationContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<PipingCalculationGroupContext>(
                PipingCalculationGroupContextChildNodeObjects,
                PipingCalculationGroupContextContextMenuStrip,
                PipingCalculationGroupContextOnNodeRemoved);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonDataResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
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
                Image = pipingOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyPipingOutput>
            {
                Text = emptyPipingOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyPipingOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
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

        # region PipingFailureMechanismView ViewInfo

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

        # region FailureMechanismResultsView ViewInfo

        private static bool CloseFailureMechanismResultViewForData(PipingFailureMechanismResultView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as PipingFailureMechanism;
            var failureMechanismContext = o as IFailureMechanismContext<PipingFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                    .GetFailureMechanisms()
                    .OfType<PipingFailureMechanism>()
                    .Any(fm => ReferenceEquals(view.Data, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
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

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(PipingFailureMechanismContext pipingFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var validateAllItem = CreateValidateAllItem(pipingFailureMechanismContext.WrappedData);

            var builder = new RingtoetsContextMenuBuilder(Gui.Get(pipingFailureMechanismContext, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(pipingFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCustomItem(validateAllItem)
                          .AddPerformAllCalculationsInFailureMechanismItem(pipingFailureMechanismContext, CalculateAll, context => null)
                          .AddClearAllCalculationOutputInFailureMechanismItem(pipingFailureMechanismContext.WrappedData)
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

        private void RemoveAllViewsForItem(PipingFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(PipingFailureMechanismContext pipingFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(pipingFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(pipingFailureMechanismContext, null)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .Build();
        }

        private static IEnumerable<PipingCalculation> GetAllPipingCalculations(PipingFailureMechanism failureMechanism)
        {
            return failureMechanism.Calculations.OfType<PipingCalculation>();
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
                menuItem.ToolTipText = RingtoetsCommonFormsResources.FailureMechanism_CreateValidateAllItem_No_calculations_to_validate;
            }

            return menuItem;
        }

        private void ValidateAll(PipingFailureMechanism failureMechanism)
        {
            foreach (PipingCalculation calculation in GetAllPipingCalculations(failureMechanism))
            {
                PipingCalculationService.Validate(calculation);
            }
        }

        private void CalculateAll(PipingFailureMechanismContext failureMechanismContext)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, GetAllPipingCalculations(failureMechanismContext.WrappedData).Select(calc => new PipingCalculationActivity(calc)).ToList());
        }

        private object[] FailureMechanismEnabledChildNodeObjects(PipingFailureMechanismContext pipingFailureMechanismContext)
        {
            PipingFailureMechanism wrappedData = pipingFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, pipingFailureMechanismContext.Parent), TreeFolderCategory.Input),
                new PipingCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData.SurfaceLines, wrappedData.StochasticSoilModels, wrappedData, pipingFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private object[] FailureMechanismDisabledChildNodeObjects(PipingFailureMechanismContext pipingFailureMechanismContext)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(pipingFailureMechanismContext.WrappedData)
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
                new FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        # endregion

        # region PipingCalculationScenarioContext TreeNodeInfo

        private ContextMenuStrip PipingCalculationContextContextMenuStrip(PipingCalculationScenarioContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            PipingCalculation calculation = nodeData.WrappedData;

            var validateItem = new StrictContextMenuItem(RingtoetsCommonFormsResources.Validate,
                                                         RingtoetsCommonFormsResources.Validate_ToolTip,
                                                         RingtoetsCommonFormsResources.ValidateIcon,
                                                         (o, args) => { PipingCalculationService.Validate(calculation); });

            return builder.AddCustomItem(validateItem)
                          .AddPerformCalculationItem(calculation, nodeData, PerformCalculation, context => null)
                          .AddClearCalculationOutputItem(calculation)
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

        private void PerformCalculation(PipingCalculation calculation, PipingCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, new PipingCalculationActivity(calculation));
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
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var isNestedGroup = parentData is PipingCalculationGroupContext;

            var generateCalculationsItem = CreateGeneratePipingCalculationsItem(nodeData);
            var validateAllItem = CreateValidateAllItem(group);

            if (!isNestedGroup)
            {
                builder.AddOpenItem()
                       .AddSeparator()
                       .AddCustomItem(generateCalculationsItem)
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(nodeData, AddCalculationScenario)
                   .AddSeparator()
                   .AddCustomItem(validateAllItem)
                   .AddPerformAllCalculationsInGroupItem(group, nodeData, CalculateAll, context => null)
                   .AddClearAllCalculationOutputInGroupItem(group)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
                builder.AddDeleteItem();
                builder.AddSeparator();
            }

            return builder.AddImportItem()
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
            var calculation = new PipingCalculationScenario(nodeData.FailureMechanism.GeneralInput, nodeData.FailureMechanism.PipingProbabilityAssessmentInput)
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

            GeneratePipingCalculations(nodeData.WrappedData, view.SelectedSurfaceLines, nodeData.AvailableStochasticSoilModels, nodeData.FailureMechanism.GeneralInput, nodeData.FailureMechanism.PipingProbabilityAssessmentInput);

            nodeData.NotifyObservers();

            nodeData.WrappedData.AddCalculationScenariosToFailureMechanismSectionResult(nodeData.FailureMechanism);
            nodeData.FailureMechanism.NotifyObservers();
        }

        private void GeneratePipingCalculations(CalculationGroup target, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<StochasticSoilModel> soilModels, GeneralPipingInput generalInput, PipingProbabilityAssessmentInput pipingProbabilityAssessmentInput)
        {
            foreach (var group in PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(surfaceLines, soilModels, generalInput, pipingProbabilityAssessmentInput))
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
                menuItem.ToolTipText = RingtoetsCommonFormsResources.FailureMechanism_CreateValidateAllItem_No_calculations_to_validate;
            }

            return menuItem;
        }

        private void CalculateAll(CalculationGroup group, PipingCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, group.GetCalculations().OfType<PipingCalculationScenario>().Select(pc => new PipingCalculationActivity(pc)).ToList());
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