// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.GrassCoverErosionOutwards.Data;

namespace Riskeer.GrassCoverErosionOutwards.Util
{
    /// <summary>
    /// Factory for creating <see cref="GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation"/> instances.
    /// </summary>
    public static class GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationFactory
    {
        /// <summary>
        /// Creates the grass cover erosion outwards aggregated hydraulic boundary locations based on the locations and calculations
        /// from an assessment section and failure mechanism.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to get the locations and calculations from.</param>
        /// <param name="failureMechanism">The failure mechanism to get the calculations from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation> CreateAggregatedHydraulicBoundaryLocations(
            IAssessmentSection assessmentSection, GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waterLevelLookupForMechanismSpecificFactorizedSignalingNorm =
                failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                                c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waterLevelLookupForMechanismSpecificSignalingNorm =
                failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                      c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waterLevelLookupForMechanismSpecificLowerLimitNorm =
                failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                       c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waterLevelLookupForLowerLimitNorm =
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                       c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waterLevelLookupForFactorizedLowerLimitNorm =
                assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                 c => c);

            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waveHeightLookupForMechanismSpecificFactorizedSignalingNorm =
                failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                                c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waveHeightLookupForMechanismSpecificSignalingNorm =
                failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                      c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waveHeightLookupForMechanismSpecificLowerLimitNorm =
                failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                       c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waveHeightLookupForLowerLimitNorm =
                assessmentSection.WaveHeightCalculationsForLowerLimitNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                       c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waveHeightLookupForFactorizedLowerLimitNorm =
                assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                 c => c);

            return assessmentSection.HydraulicBoundaryDatabase.Locations
                                    .Select(location => new GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation(
                                                location.Id, location.Name, location.Location,
                                                GetCalculationResult(waterLevelLookupForMechanismSpecificFactorizedSignalingNorm[location].Output),
                                                GetCalculationResult(waterLevelLookupForMechanismSpecificSignalingNorm[location].Output),
                                                GetCalculationResult(waterLevelLookupForMechanismSpecificLowerLimitNorm[location].Output),
                                                GetCalculationResult(waterLevelLookupForLowerLimitNorm[location].Output),
                                                GetCalculationResult(waterLevelLookupForFactorizedLowerLimitNorm[location].Output),
                                                GetCalculationResult(waveHeightLookupForMechanismSpecificFactorizedSignalingNorm[location].Output),
                                                GetCalculationResult(waveHeightLookupForMechanismSpecificSignalingNorm[location].Output),
                                                GetCalculationResult(waveHeightLookupForMechanismSpecificLowerLimitNorm[location].Output),
                                                GetCalculationResult(waveHeightLookupForLowerLimitNorm[location].Output),
                                                GetCalculationResult(waveHeightLookupForFactorizedLowerLimitNorm[location].Output))).ToArray();
        }

        private static RoundedDouble GetCalculationResult(HydraulicBoundaryLocationCalculationOutput output)
        {
            return output?.Result ?? RoundedDouble.NaN;
        }
    }
}