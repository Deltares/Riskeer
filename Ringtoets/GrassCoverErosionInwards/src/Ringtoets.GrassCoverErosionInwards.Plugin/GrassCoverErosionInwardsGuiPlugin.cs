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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Plugin.Properties;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Data.Output;
using GrassCoverErosionInwardsDataResources = Ringtoets.GrassCoverErosionInwards.Data.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin
{
    /// <summary>
    /// The GUI plug-in for the <see cref="GrassCoverErosionInwards.Data.GrassCoverErosionInwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionInwardsGuiPlugin : GuiPlugin
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanismContextProperties>();
            yield return new PropertyInfo<GrassCoverErosionInwardsCalculationContext, GrassCoverErosionInwardsCalculationContextProperties>();
            yield return new PropertyInfo<GrassCoverErosionInwardsInputContext, GrassCoverErosionInwardsInputContextProperties>();
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<GrassCoverErosionInwardsFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<GrassCoverErosionInwardsCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<GrassCoverErosionInwardsCalculationContext>(
                GrassCoverErosionInwardsFormsResources.CalculationIcon,
                CalculationContextChildNodeObjects,
                CalculationContextContextmenuStrip,
                CalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<GrassCoverErosionInwardsInputContext>
            {
                Text = inputContext => GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsInputContext_NodeDisplayName,
                Image = inputContext => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddImportItem()
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyGrassCoverErosionInwardsOutput>
            {
                Text = emptyOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyOutput => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ForeColor = emptyOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };
        }

        private static ExceedanceProbabilityCalculationActivity CreateHydraRingTargetProbabilityCalculationActivity(FailureMechanismSection failureMechanismSection,
                                                                                                                    string hlcdDirectory,
                                                                                                                    GrassCoverErosionInwardsCalculation calculation)
        {
            var hydraulicBoundaryLocationId = (int) calculation.InputParameters.HydraulicBoundaryLocation.Id;
            var sectionLength = failureMechanismSection.GetSectionLength();
            var inwardsInput = calculation.InputParameters;

            return HydraRingActivityFactory.Create(
                calculation.Name,
                hlcdDirectory,
                failureMechanismSection.Name, // TODO: Provide name of reference line instead
                HydraRingTimeIntegrationSchemeType.FBC,
                HydraRingUncertaintiesType.All,
                new OvertoppingCalculationInput(hydraulicBoundaryLocationId, new HydraRingSection(1, failureMechanismSection.Name, sectionLength, inwardsInput.Orientation),
                                                inwardsInput.DikeHeight,
                                                inwardsInput.CriticalOvertoppingModelFactor,
                                                inwardsInput.FbFactor.Mean, inwardsInput.FbFactor.StandardDeviation,
                                                inwardsInput.FnFactor.Mean, inwardsInput.FnFactor.StandardDeviation,
                                                inwardsInput.OvertoppingModelFactor,
                                                inwardsInput.CriticalFlowRate.Mean, inwardsInput.CriticalFlowRate.StandardDeviation,
                                                inwardsInput.FrunupModelFactor.Mean, inwardsInput.FrunupModelFactor.StandardDeviation,
                                                inwardsInput.FshallowModelFactor.Mean, inwardsInput.FshallowModelFactor.StandardDeviation,
                                                ParseProfilePoints(inwardsInput.DikeGeometry),
                                                ParseForeshore(inwardsInput),
                                                ParseBreakWater(inwardsInput)
                    ),
                calculation.ClearOutput,
                output => { ParseHydraRingOutput(calculation, output); });
        }

        private static HydraRingBreakWater ParseBreakWater(GrassCoverErosionInwardsInput input)
        {
            return input.UseBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null;
        }

        private static IEnumerable<HydraRingForelandPoint> ParseForeshore(GrassCoverErosionInwardsInput input)
        {
            var firstProfileSection = input.ForeshoreGeometry.FirstOrDefault();
            if (!input.UseForeshore || firstProfileSection == null)
            {
                yield break;
            }

            yield return new HydraRingForelandPoint(firstProfileSection.StartingPoint.X, firstProfileSection.StartingPoint.Y);

            foreach (var foreshore in input.ForeshoreGeometry)
            {
                yield return new HydraRingForelandPoint(foreshore.EndingPoint.X, foreshore.EndingPoint.Y);
            }
        }

        private static IEnumerable<HydraRingRoughnessProfilePoint> ParseProfilePoints(IEnumerable<RoughnessProfileSection> profileSections)
        {
            var firstProfileSection = profileSections.FirstOrDefault();
            if (firstProfileSection == null)
            {
                yield break;
            }

            // By default, the roughness is 1.0 (no reduction due to bed friction).
            yield return new HydraRingRoughnessProfilePoint(firstProfileSection.StartingPoint.X, firstProfileSection.StartingPoint.Y, 1);

            foreach (var profileSection in profileSections)
            {
                yield return new HydraRingRoughnessProfilePoint(profileSection.EndingPoint.X, profileSection.EndingPoint.Y, profileSection.Roughness);
            }
        }

        private static void ParseHydraRingOutput(GrassCoverErosionInwardsCalculation calculation, ExceedanceProbabilityCalculationOutput output)
        {
            if (output != null)
            {
                calculation.Output = new GrassCoverErosionInwardsOutput(double.NaN, double.NaN, (RoundedDouble)output.Beta, double.NaN, double.NaN);
            }
            else
            {
                throw new InvalidOperationException(Resources.GrassCoverErosionInwardsGuiPlugin_Error_during_overtopping_calculation);
            }
        }

        private void CalculateAll(IFailureMechanism failureMechanism, IEnumerable<GrassCoverErosionInwardsCalculation> calculations, IAssessmentSection assessmentSection)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, calculations.Select(calc =>
                                                                                 CreateHydraRingTargetProbabilityCalculationActivity(
                                                                                     failureMechanism.Sections.First(), // TODO: Pass dike section based on cross section of calculation with reference line
                                                                                     Path.GetDirectoryName(assessmentSection.HydraulicBoundaryDatabase.FilePath),
                                                                                     calc)).ToList());
        }

        private static bool AllDataAvailable(IAssessmentSection assessmentSection, GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            return assessmentSection.HydraulicBoundaryDatabase != null && failureMechanism.Sections.Any();
        }

        #region GrassCoverErosionInwards TreeNodeInfo

        private object[] FailureMechanismEnabledChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext)
        {
            GrassCoverErosionInwardsFailureMechanism wrappedData = grassCoverErosionInwardsFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, grassCoverErosionInwardsFailureMechanismContext.Parent), TreeFolderCategory.Input),
                new GrassCoverErosionInwardsCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData, grassCoverErosionInwardsFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private object[] FailureMechanismDisabledChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(grassCoverErosionInwardsFailureMechanismContext.WrappedData)
            };
        }

        private static IList GetInputs(GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private static IList GetOutputs(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext<FailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(grassCoverErosionInwardsFailureMechanismContext, treeViewControl));
            return builder
                .AddOpenItem()
                .AddSeparator()
                .AddChangeRelevancyOfFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext, RemoveAllViewsForItem)
                .AddSeparator()
                .AddPerformAllCalculationsInFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext, CalculateAll, EnablePerformAllCalculationsInFailureMechanism)
                .AddClearAllCalculationOutputInFailureMechanismItem(grassCoverErosionInwardsFailureMechanismContext.WrappedData)
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

        private static bool EnablePerformAllCalculationsInFailureMechanism(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            return AllDataAvailable(context.Parent, context.WrappedData);
        }

        private void RemoveAllViewsForItem(GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(grassCoverErosionInwardsFailureMechanismContext, treeViewControl));

            return builder.AddDisabledChangeRelevancyItem(grassCoverErosionInwardsFailureMechanismContext)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .Build();
        }

        private static void AddCalculation(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            var calculation = new GrassCoverErosionInwardsCalculation(context.FailureMechanism.GeneralInput)
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsCalculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private void CalculateAll(GrassCoverErosionInwardsFailureMechanismContext context)
        {
            CalculateAll(context.WrappedData, context.WrappedData.Calculations.OfType<GrassCoverErosionInwardsCalculation>(), context.Parent);
        }

        #endregion

        #region CalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(GrassCoverErosionInwardsCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in nodeData.WrappedData.Children)
            {
                var calculation = calculationItem as GrassCoverErosionInwardsCalculation;
                var group = calculationItem as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new GrassCoverErosionInwardsCalculationContext(calculation,
                                                                                        nodeData.FailureMechanism,
                                                                                        nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                             nodeData.FailureMechanism,
                                                                                             nodeData.AssessmentSection));
                }
                else
                {
                    childNodeObjects.Add(calculationItem);
                }
            }

            return childNodeObjects.ToArray();
        }

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(GrassCoverErosionInwardsCalculationGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var group = nodeData.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var isNestedGroup = parentData is GrassCoverErosionInwardsCalculationGroupContext;

            if (!isNestedGroup)
            {
                builder.AddOpenItem()
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group);
            builder.AddCreateCalculationItem(nodeData, AddCalculation);
            builder.AddSeparator();
            builder.AddPerformAllCalculationsInGroupItem(group, nodeData, CalculateAll, EnablePerformAllCalculationsInGroup);
            builder.AddClearAllCalculationOutputInGroupItem(group);
            builder.AddSeparator();

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

        private static bool EnablePerformAllCalculationsInGroup(GrassCoverErosionInwardsCalculationGroupContext context)
        {
            return AllDataAvailable(context.AssessmentSection, context.FailureMechanism);
        }

        private static void CalculationGroupContextOnNodeRemoved(GrassCoverErosionInwardsCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (GrassCoverErosionInwardsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);
            parentGroupContext.NotifyObservers();
        }

        private void CalculateAll(CalculationGroup group, GrassCoverErosionInwardsCalculationGroupContext context)
        {
            CalculateAll(context.FailureMechanism, group.GetCalculations().OfType<GrassCoverErosionInwardsCalculation>(), context.AssessmentSection);
        }

        #endregion

        #region CalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(GrassCoverErosionInwardsCalculationContext calculationContext)
        {
            var childNodes = new List<object>
            {
                new CommentContext<ICommentable>(calculationContext.WrappedData),
                new GrassCoverErosionInwardsInputContext(calculationContext.WrappedData.InputParameters,
                                                         calculationContext.WrappedData,
                                                         calculationContext.FailureMechanism,
                                                         calculationContext.AssessmentSection)
            };

            if (!calculationContext.WrappedData.HasOutput)
            {
                childNodes.Add(new EmptyGrassCoverErosionInwardsOutput());
            }

            return childNodes.ToArray();
        }

        private ContextMenuStrip CalculationContextContextmenuStrip(GrassCoverErosionInwardsCalculationContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            GrassCoverErosionInwardsCalculation calculation = nodeData.WrappedData;

            return builder.AddPerformCalculationItem(calculation, nodeData, PerformCalculation, EnablePerformCalculation)
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

        private static bool EnablePerformCalculation(GrassCoverErosionInwardsCalculationContext context)
        {
            return AllDataAvailable(context.AssessmentSection, context.FailureMechanism);
        }

        private void PerformCalculation(GrassCoverErosionInwardsCalculation calculation, GrassCoverErosionInwardsCalculationContext context)
        {
            var activity = CreateHydraRingTargetProbabilityCalculationActivity(
                context.FailureMechanism.Sections.First(), // TODO: Pass dike section based on cross section of calculation with reference line
                Path.GetDirectoryName(context.AssessmentSection.HydraulicBoundaryDatabase.FilePath),
                calculation);

            ActivityProgressDialogRunner.Run(Gui.MainWindow, activity);
        }

        private void CalculationContextOnNodeRemoved(GrassCoverErosionInwardsCalculationContext calculationScenarioContext, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as GrassCoverErosionInwardsCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculationGroupContext.WrappedData.Children.Remove(calculationScenarioContext.WrappedData);
                calculationGroupContext.NotifyObservers();
            }
        }

        #endregion
    }
}