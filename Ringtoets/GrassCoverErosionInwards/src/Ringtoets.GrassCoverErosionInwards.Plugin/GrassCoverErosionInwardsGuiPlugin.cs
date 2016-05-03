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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
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
using Ringtoets.HydraRing.Data;
using GrassCoverErosionInwardsDataResources = Ringtoets.GrassCoverErosionInwards.Data.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

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
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new DefaultFailureMechanismTreeNodeInfo<GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanism>(
                FailureMechanismChildNodeObjects,
                FailureMechanismContextMenuStrip,
                Gui);

            yield return TreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<GrassCoverErosionInwardsCalculationGroupContext>(CalculationGroupContextChildNodeObjects, context => AddCalculation(context.FailureMechanism, context.WrappedData), Gui);

            yield return new TreeNodeInfo<GrassCoverErosionInwardsCalculationContext>
            {
                Text = context => context.WrappedData.Name,
                Image = context => GrassCoverErosionInwardsFormsResources.CalculationIcon,
                EnsureVisibleOnCreate = context => true,
                ChildNodeObjects = CalculationContextChildNodeObjects
            };

            yield return new TreeNodeInfo<GrassCoverErosionInwardsInputContext>
            {
                Text = pipingInputContext => GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsInputContext_NodeDisplayName,
                Image = pipingInputContext => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddImportItem()
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyGrassCoverErosionInwardsOutput>
            {
                Text = emptyPipingOutput => GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsOutput_DisplayName,
                Image = emptyPipingOutput => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ForeColor = emptyPipingOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };
        }

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

        private static ExceedanceProbabilityCalculationActivity CreateHydraRingTargetProbabilityCalculationActivity(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                                                                    string hlcdDirectory,
                                                                                                                    GrassCoverErosionInwardsInput inwardsInput,
                                                                                                                    GrassCoverErosionInwardsOutput inwardsOutput
            )
        {
            var hydraulicBoundaryLocationId = (int) hydraulicBoundaryLocation.Id;

            return HydraRingActivityFactory.Create(
                string.Format(Resources.GrassCoverErosionInwardsGuiPlugin_Calculate_overtopping_for_location_0_, hydraulicBoundaryLocationId),
                hlcdDirectory,
                hydraulicBoundaryLocationId.ToString(),
                HydraRingTimeIntegrationSchemeType.FBC,
                HydraRingUncertaintiesType.All,
                new OvertoppingCalculationInput(hydraulicBoundaryLocationId, new HydraRingSection(hydraulicBoundaryLocationId, hydraulicBoundaryLocationId.ToString(), double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN),
                                                inwardsInput.DikeHeight,
                                                inwardsInput.Mqc,
                                                inwardsInput.Fb.Mean, inwardsInput.Fb.StandardDeviation,
                                                inwardsInput.Fn.Mean, inwardsInput.Fn.StandardDeviation,
                                                inwardsInput.Mqo,
                                                inwardsInput.CriticalFlowRate.StandardDeviation, inwardsInput.CriticalFlowRate.Mean,
                                                inwardsInput.Mz2.Mean, inwardsInput.Mz2.StandardDeviation,
                                                inwardsInput.Fshallow.Mean, inwardsInput.Fshallow.StandardDeviation,
                                                ParseProfilePoints(inwardsInput.DikeGeometry),
                                                ParseForeshore(inwardsInput),
                                                ParseBreakWater(inwardsInput)
                    ),
                output => { ParseHydraRingOutput(inwardsOutput, output); });
        }

        private static HydraRingBreakWater ParseBreakWater(GrassCoverErosionInwardsInput input)
        {
            return input.UseBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null;
        }

        private static IEnumerable<HydraRingForelandPoint> ParseForeshore(GrassCoverErosionInwardsInput input)
        {
            if (!input.UseForeshore)
            {
                yield break;
            }
            if (input.ForeshoreGeometry.Any())
            {
                var first = input.ForeshoreGeometry.First();
                yield return new HydraRingForelandPoint(first.StartingPoint.X, first.StartingPoint.Y);
            }

            foreach (var foreshore in input.ForeshoreGeometry)
            {
                yield return new HydraRingForelandPoint(foreshore.EndingPoint.X, foreshore.EndingPoint.Y);
            }
        }

        private static IEnumerable<HydraRingRoughnessProfilePoint> ParseProfilePoints(IEnumerable<RoughnessProfileSection> profileSections)
        {
            if (profileSections.Any())
            {
                var first = profileSections.First();
                yield return new HydraRingRoughnessProfilePoint(first.StartingPoint.X, first.StartingPoint.Y, 0);
            }

            foreach (var profileSection in profileSections)
            {
                yield return new HydraRingRoughnessProfilePoint(profileSection.EndingPoint.X, profileSection.EndingPoint.Y, profileSection.Roughness);
            }
        }

        private static void ParseHydraRingOutput(GrassCoverErosionInwardsOutput grassCoverErosionInwardsOutput, ExceedanceProbabilityCalculationOutput output)
        {
            if (output != null)
            {
                grassCoverErosionInwardsOutput.Probability = (RoundedDouble) output.Beta;
            }
            else
            {
                throw new InvalidOperationException(Resources.GrassCoverErosionInwardsGuiPlugin_Error_during_overtopping_calculation);
            }
        }

        #region GrassCoverErosionInwards TreeNodeInfo

        private object[] FailureMechanismChildNodeObjects(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext)
        {
            GrassCoverErosionInwardsFailureMechanism wrappedData = grassCoverErosionInwardsFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, grassCoverErosionInwardsFailureMechanismContext.Parent), TreeFolderCategory.Input),
                new GrassCoverErosionInwardsCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData, grassCoverErosionInwardsFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
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
                new FailureMechanismSectionResultContext(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private ContextMenuStrip FailureMechanismContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var changeRelevancyItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant,
                RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                RingtoetsCommonFormsResources.Checkbox_ticked,
                (sender, args) =>
                {
                    Gui.ViewCommands.RemoveAllViewsForItem(grassCoverErosionInwardsFailureMechanismContext);
                    grassCoverErosionInwardsFailureMechanismContext.WrappedData.IsRelevant = false;
                    grassCoverErosionInwardsFailureMechanismContext.WrappedData.NotifyObservers();
                }
                );

            var addCalculationGroupItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup,
                RingtoetsCommonFormsResources.FailureMechanism_Add_CalculationGroup_Tooltip,
                RingtoetsCommonFormsResources.AddFolderIcon,
                (o, args) => AddCalculationGroup(grassCoverErosionInwardsFailureMechanismContext.WrappedData)
                )
            {
                Enabled = false
            };

            var addCalculationItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Add_Calculation,
                GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsFailureMechanism_Add_GrassCoverErosionInwardsCalculation_Tooltip,
                GrassCoverErosionInwardsFormsResources.CalculationIcon,
                (s, e) => AddCalculation(grassCoverErosionInwardsFailureMechanismContext.WrappedData, grassCoverErosionInwardsFailureMechanismContext.WrappedData.CalculationsGroup)
                )
            {
                Enabled = false
            };

            return Gui.Get(grassCoverErosionInwardsFailureMechanismContext, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(changeRelevancyItem)
                      .AddSeparator()
                      .AddCustomItem(addCalculationGroupItem)
                      .AddCustomItem(addCalculationItem)
                      .AddSeparator()
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .Build();
        }

        private static void AddCalculationGroup(ICalculatableFailureMechanism failureMechanism)
        {
            var calculation = new CalculationGroup
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, RingtoetsCommonDataResources.CalculationGroup_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.NotifyObservers();
        }

        private static void AddCalculation(GrassCoverErosionInwardsFailureMechanism failureMechanism, CalculationGroup calculationGroup)
        {
            var calculation = new GrassCoverErosionInwardsCalculation(failureMechanism.GeneralInput)
            {
                Name = NamingHelper.GetUniqueName(calculationGroup.Children, GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsCalculation_DefaultName, c => c.Name)
            };
            calculationGroup.Children.Add(calculation);
            calculationGroup.NotifyObservers();
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

        #endregion
    }
}