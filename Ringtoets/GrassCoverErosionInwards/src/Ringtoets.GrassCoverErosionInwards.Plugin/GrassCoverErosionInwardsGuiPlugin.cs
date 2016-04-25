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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin.Properties;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;
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
            yield return new DefaultFailureMechanismTreeNodeInfo<GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanism>(null, null, Gui);
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
    }
}