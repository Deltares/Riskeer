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
    /// The GUI plug-in for the <see cref="Ringtoets.GrassCoverErosionInwards.Data.GrassCoverErosionInwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionInwardsGuiPlugin : GuiPlugin
    {
        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new DefaultFailureMechanismTreeNodeInfo<GrassCoverErosionInwardsFailureMechanismContext,
                GrassCoverErosionInwardsFailureMechanism>(FailureMechanismChildNodeObjects,
                                                          FailureMechanismContextMenuStrip, Gui);

            yield return new TreeNodeInfo<GrassCoverErosionInwardsCalculationGroupContext>
            {
                Text = grassCoverErosionInwardsFailureMechanismContext => grassCoverErosionInwardsFailureMechanismContext.WrappedData.Name,
                Image = grassCoverErosionInwardsCalculationGroupContext => RingtoetsCommonFormsResources.GeneralFolderIcon,
                EnsureVisibleOnCreate = grassCoverErosionInwardsCalculationGroupContext => true
            };
        }

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
                                                inwardsInput.DikeHeight, inwardsInput.CriticalFlowRate.StandardDeviation, inwardsInput.CriticalFlowRate.Mean,
                                                ParseProfilePoints(inwardsInput.DikeGeometry), ParseForeshore(inwardsInput), ParseBreakWater(inwardsInput)
                    ),
                output => { ParseHydraRingOutput(inwardsOutput, output); });
        }

        private static IEnumerable<HydraRingBreakWater> ParseBreakWater(GrassCoverErosionInwardsInput input)
        {
            return input.BreakWaterPresent ?
                       input.BreakWater.Select(water => new HydraRingBreakWater((int) water.Type, water.Height)) :
                       Enumerable.Empty<HydraRingBreakWater>();
        }

        private static IEnumerable<HydraRingForelandPoint> ParseForeshore(GrassCoverErosionInwardsInput input)
        {
            if (!input.ForeshorePresent)
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

        private ContextMenuStrip FailureMechanismContextMenuStrip(GrassCoverErosionInwardsFailureMechanismContext grassCoverErosionInwardsFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var addCalculationGroupItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup,
                RingtoetsCommonFormsResources.FailureMechanism_Add_CalculationGroup_Tooltip,
                RingtoetsCommonFormsResources.AddFolderIcon,
                (o, args) => AddCalculationGroup(grassCoverErosionInwardsFailureMechanismContext.WrappedData)
                );

            var addCalculationItem = new StrictContextMenuItem(
                GrassCoverErosionInwardsFormsResources.CalculationGroup_Add_GrassCoverErosionInwardsCalculation,
                GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsFailureMechanism_Add_GrassCoverErosionInwardsCalculation_Tooltip,
                GrassCoverErosionInwardsFormsResources.GrassCoverErosionInwardsIcon,
                (s, e) => AddCalculation(grassCoverErosionInwardsFailureMechanismContext.WrappedData)
                );

            return Gui.Get(grassCoverErosionInwardsFailureMechanismContext, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(addCalculationGroupItem)
                      .AddCustomItem(addCalculationItem)
                      .AddSeparator()
                      .AddSeparator()
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .Build();
        }

        private static void AddCalculationGroup(IFailureMechanism failureMechanism)
        {
            var calculation = new CalculationGroup
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, RingtoetsCommonDataResources.CalculationGroup_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.NotifyObservers();
        }

        private static void AddCalculation(IFailureMechanism failureMechanism)
        {
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsCalculation_DefaultName, c => c.Name)
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.NotifyObservers();
        }

        #endregion
    }
}