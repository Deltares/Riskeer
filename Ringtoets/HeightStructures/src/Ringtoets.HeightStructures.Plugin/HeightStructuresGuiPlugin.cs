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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Service;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.PropertyClasses;
using Ringtoets.HeightStructures.Forms.Views;
using Ringtoets.HeightStructures.Plugin.Properties;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.IO;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using HeightStructuresDataResources = Ringtoets.HeightStructures.Data.Properties.Resources;
using HeightStructuresFormsResources = Ringtoets.HeightStructures.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Plugin
{
    /// <summary>
    /// The GUI plug-in for the <see cref="HeightStructuresFailureMechanism"/>.
    /// </summary>
    public class HeightStructuresGuiPlugin : GuiPlugin
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<HeightStructuresFailureMechanismContext, HeightStructuresFailureMechanismContextProperties>();
            yield return new PropertyInfo<HeightStructuresInputContext, HeightStructuresInputContextProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<
                FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>,
                IEnumerable<HeightStructuresFailureMechanismSectionResult>,
                HeightStructuresFailureMechanismResultView>
            {
                GetViewName = (v, o) => RingtoetsCommonDataResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.SectionResults,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<HeightStructuresFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<HeightStructuresCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<HeightStructuresCalculationContext>(
                HeightStructuresFormsResources.CalculationIcon,
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<HeightStructuresInputContext>
            {
                Text = inputContext => HeightStructuresFormsResources.HeightStructuresInputContext_NodeDisplayName,
                Image = inputContext => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddImportItem()
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ProbabilityAssessmentOutput>
            {
                Text = output => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = output => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return RingtoetsTreeNodeInfoFactory.CreateEmptyProbabilityAssessmentOutputTreeNodeInfo(
                EmptyProbabilityAssessmentOutputContextMenuStrip);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonDataResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
        }

        private static ExceedanceProbabilityCalculationActivity CreateHydraRingExceedenceProbabilityCalculationActivity(
            string hlcdDirectory,
            HeightStructuresCalculation calculation,
            HeightStructuresFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            var hydraulicBoundaryLocationId = (int) calculation.InputParameters.HydraulicBoundaryLocation.Id;
            var failureMechanismSection = failureMechanism.Sections.First(); // TODO: Obtain dike section based on cross section of structure with reference line
            var sectionLength = failureMechanismSection.GetSectionLength();
            var generalInputParameters = failureMechanism.GeneralInput;
            var inputParameters = calculation.InputParameters;

            return HydraRingActivityFactory.Create(
                calculation.Name,
                hlcdDirectory,
                failureMechanismSection.Name, // TODO: Provide name of reference line instead
                HydraRingTimeIntegrationSchemeType.FBC,
                HydraRingUncertaintiesType.All,
                new StructuresOvertoppingCalculationInput(hydraulicBoundaryLocationId,
                                                          new HydraRingSection(1, failureMechanismSection.Name, sectionLength, inputParameters.OrientationOfTheNormalOfTheStructure),
                                                          generalInputParameters.GravitationalAcceleration,
                                                          generalInputParameters.ModelFactorOvertoppingFlow.Mean, generalInputParameters.ModelFactorOvertoppingFlow.StandardDeviation,
                                                          inputParameters.LevelOfCrestOfStructure.Mean, inputParameters.LevelOfCrestOfStructure.StandardDeviation,
                                                          inputParameters.OrientationOfTheNormalOfTheStructure,
                                                          inputParameters.ModelFactorOvertoppingSuperCriticalFlow.Mean, inputParameters.ModelFactorOvertoppingSuperCriticalFlow.StandardDeviation,
                                                          inputParameters.AllowableIncreaseOfLevelForStorage.Mean, inputParameters.AllowableIncreaseOfLevelForStorage.StandardDeviation,
                                                          generalInputParameters.ModelFactorForStorageVolume.Mean, generalInputParameters.ModelFactorForStorageVolume.StandardDeviation,
                                                          inputParameters.StorageStructureArea.Mean, inputParameters.StorageStructureArea.GetVariationCoefficient(),
                                                          generalInputParameters.ModelFactorForIncomingFlowVolume,
                                                          inputParameters.FlowWidthAtBottomProtection.Mean, inputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                                                          inputParameters.CriticalOvertoppingDischarge.Mean, inputParameters.CriticalOvertoppingDischarge.GetVariationCoefficient(),
                                                          inputParameters.FailureProbabilityOfStructureGivenErosion,
                                                          inputParameters.WidthOfFlowApertures.Mean, inputParameters.WidthOfFlowApertures.GetVariationCoefficient(),
                                                          inputParameters.DeviationOfTheWaveDirection,
                                                          inputParameters.StormDuration.Mean, inputParameters.StormDuration.GetVariationCoefficient()),
                calculation.ClearOutput,
                output => { ParseHydraRingOutput(calculation, failureMechanism, assessmentSection, output); });
        }

        private void CalculateAll(HeightStructuresFailureMechanism failureMechanism, IEnumerable<HeightStructuresCalculation> calculations, IAssessmentSection assessmentSection)
        {
            // TODO: Remove "Where" filter when validation is implemented
            ActivityProgressDialogRunner.Run(Gui.MainWindow, calculations.Where(calc => calc.InputParameters.HydraulicBoundaryLocation != null)
                                                                         .Select(calc => CreateHydraRingExceedenceProbabilityCalculationActivity(
                                                                             Path.GetDirectoryName(assessmentSection.HydraulicBoundaryDatabase.FilePath),
                                                                             calc,
                                                                             failureMechanism,
                                                                             assessmentSection
                                                                             )).ToList());
        }

        private static string AllDataAvailable(IAssessmentSection assessmentSection, HeightStructuresFailureMechanism failureMechanism)
        {
            if (!failureMechanism.Sections.Any())
            {
                return Resources.HeightStructuresGuiPlugin_AllDataAvailable_No_failure_mechanism_sections_imported;
            }

            if (assessmentSection.HydraulicBoundaryDatabase == null)
            {
                return Resources.HeightStructuresGuiPlugin_AllDataAvailable_No_hydraulic_boundary_database_imported;
            }

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(RingtoetsCommonFormsResources.GuiPlugin_VerifyHydraulicBoundaryDatabasePath_Hydraulic_boundary_database_connection_failed_0_, validationProblem);
            }

            return null;
        }

        private static void ParseHydraRingOutput(HeightStructuresCalculation calculation, HeightStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection, ExceedanceProbabilityCalculationOutput output)
        {
            if (output != null)
            {
                calculation.Output = ProbabilityAssessmentService.Calculate(assessmentSection.FailureMechanismContribution.Norm, failureMechanism.Contribution, failureMechanism.ProbabilityAssessmentInput.N, output.Beta);
                calculation.NotifyObservers();
            }
            else
            {
                throw new InvalidOperationException(Resources.HeightStructuresGuiPlugin_Error_during_overtopping_calculation);
            }
        }

        #region EmptyProbabilityAssessmentOutput TreeNodeInfo

        private ContextMenuStrip EmptyProbabilityAssessmentOutputContextMenuStrip(EmptyProbabilityAssessmentOutput output, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(output, treeViewControl));
            return builder.AddExportItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        #endregion

        #region HeightStructuresFailureMechanismResultView ViewInfo

        private static bool CloseFailureMechanismResultViewForData(HeightStructuresFailureMechanismResultView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as HeightStructuresFailureMechanism;
            var failureMechanismContext = o as IFailureMechanismContext<HeightStructuresFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                    .GetFailureMechanisms()
                    .OfType<HeightStructuresFailureMechanism>()
                    .Any(fm => ReferenceEquals(view.Data, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
        }

        #endregion

        #region HeightStructuresFailureMechanismContext TreeNodeInfo

        private object[] FailureMechanismEnabledChildNodeObjects(HeightStructuresFailureMechanismContext context)
        {
            HeightStructuresFailureMechanism wrappedData = context.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, context.Parent), TreeFolderCategory.Input),
                new HeightStructuresCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData, context.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static IList GetInputs(HeightStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private static IList GetOutputs(HeightStructuresFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private object[] FailureMechanismDisabledChildNodeObjects(HeightStructuresFailureMechanismContext context)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(context.WrappedData)
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(HeightStructuresFailureMechanismContext context, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(context, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddPerformAllCalculationsInFailureMechanismItem(context, CalculateAll, EnablePerformAllCalculationsInFailureMechanism)
                          .AddClearAllCalculationOutputInFailureMechanismItem(context.WrappedData)
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

        private void RemoveAllViewsForItem(HeightStructuresFailureMechanismContext context)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(context);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(HeightStructuresFailureMechanismContext context, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(context, null)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .Build();
        }

        private static string EnablePerformAllCalculationsInFailureMechanism(HeightStructuresFailureMechanismContext context)
        {
            return AllDataAvailable(context.Parent, context.WrappedData);
        }

        private void CalculateAll(HeightStructuresFailureMechanismContext context)
        {
            CalculateAll(context.WrappedData, context.WrappedData.Calculations.OfType<HeightStructuresCalculation>(), context.Parent);
        }

        #endregion

        #region HeightStructuresCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(HeightStructuresCalculationGroupContext context)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in context.WrappedData.Children)
            {
                var calculation = calculationItem as HeightStructuresCalculation;
                var group = calculationItem as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new HeightStructuresCalculationContext(calculation,
                                                                                context.FailureMechanism,
                                                                                context.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new HeightStructuresCalculationGroupContext(group,
                                                                                     context.FailureMechanism,
                                                                                     context.AssessmentSection));
                }
                else
                {
                    childNodeObjects.Add(calculationItem);
                }
            }

            return childNodeObjects.ToArray();
        }

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(HeightStructuresCalculationGroupContext context, object parentData, TreeViewControl treeViewControl)
        {
            var group = context.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            var isNestedGroup = parentData is HeightStructuresCalculationGroupContext;

            if (!isNestedGroup)
            {
                builder.AddOpenItem()
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation)
                   .AddSeparator()
                   .AddPerformAllCalculationsInGroupItem(group, context, CalculateAll, EnablePerformAllCalculationsInGroup)
                   .AddClearAllCalculationOutputInGroupItem(group)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem()
                       .AddDeleteItem()
                       .AddSeparator();
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

        private static void CalculationGroupContextOnNodeRemoved(HeightStructuresCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (HeightStructuresCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);
            parentGroupContext.NotifyObservers();
        }

        private static void AddCalculation(HeightStructuresCalculationGroupContext context)
        {
            var calculation = new HeightStructuresCalculation
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, HeightStructuresDataResources.HeightStructuresCalculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static string EnablePerformAllCalculationsInGroup(HeightStructuresCalculationGroupContext context)
        {
            return AllDataAvailable(context.AssessmentSection, context.FailureMechanism);
        }

        private void CalculateAll(CalculationGroup group, HeightStructuresCalculationGroupContext context)
        {
            CalculateAll(context.FailureMechanism, group.GetCalculations().OfType<HeightStructuresCalculation>(), context.AssessmentSection);
        }

        #endregion

        #region HeightStructuresCalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(HeightStructuresCalculationContext context)
        {
            var childNodes = new List<object>
            {
                new CommentContext<ICommentable>(context.WrappedData),
                new HeightStructuresInputContext(context.WrappedData.InputParameters,
                                                 context.WrappedData,
                                                 context.FailureMechanism,
                                                 context.AssessmentSection)
            };

            if (context.WrappedData.HasOutput)
            {
                childNodes.Add(context.WrappedData.Output);
            }
            else
            {
                childNodes.Add(new EmptyProbabilityAssessmentOutput());
            }

            return childNodes.ToArray();
        }

        private ContextMenuStrip CalculationContextContextMenuStrip(HeightStructuresCalculationContext context, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            HeightStructuresCalculation calculation = context.WrappedData;

            return builder.AddPerformCalculationItem(calculation, context, Calculate, EnablePerformCalculation)
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

        private static string EnablePerformCalculation(HeightStructuresCalculationContext context)
        {
            return AllDataAvailable(context.AssessmentSection, context.FailureMechanism);
        }

        private void Calculate(HeightStructuresCalculation calculation, HeightStructuresCalculationContext context)
        {
            // TODO: Remove null-check when validation is implemented
            if (calculation.InputParameters.HydraulicBoundaryLocation == null)
            {
                return;
            }
            var activity = CreateHydraRingExceedenceProbabilityCalculationActivity(
                Path.GetDirectoryName(context.AssessmentSection.HydraulicBoundaryDatabase.FilePath),
                calculation,
                context.FailureMechanism,
                context.AssessmentSection);

            ActivityProgressDialogRunner.Run(Gui.MainWindow, activity);
        }

        private void CalculationContextOnNodeRemoved(HeightStructuresCalculationContext context, object parentData)
        {
            var calculationGroupContext = parentData as HeightStructuresCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                calculationGroupContext.NotifyObservers();
            }
        }

        #endregion
    }
}