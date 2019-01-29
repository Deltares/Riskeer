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

namespace Riskeer.Common.Util
{
    /// <summary>
    /// Factory for creating <see cref="AggregatedHydraulicBoundaryLocation"/> instances.
    /// </summary>
    public static class AggregatedHydraulicBoundaryLocationFactory
    {
        /// <summary>
        /// Creates the aggregated hydraulic boundary locations based on the locations and calculations
        /// from an assessment section.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to get the locations and calculations from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="AggregatedHydraulicBoundaryLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<AggregatedHydraulicBoundaryLocation> CreateAggregatedHydraulicBoundaryLocations(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waterLevelLookupForFactorizedSignalingNorm =
                assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waterLevelLookupForSignalingNorm =
                assessmentSection.WaterLevelCalculationsForSignalingNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                      c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waterLevelLookupForLowerLimitNorm =
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                       c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waterLevelLookupForFactorizedLowerLimitNorm =
                assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                 c => c);

            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waveHeightLookupForFactorizedSignalingNorm =
                assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waveHeightLookupForSignalingNorm =
                assessmentSection.WaveHeightCalculationsForSignalingNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                      c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waveHeightLookupForLowerLimitNorm =
                assessmentSection.WaveHeightCalculationsForLowerLimitNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                       c => c);
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> waveHeightLookupForFactorizedLowerLimitNorm =
                assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ToDictionary(c => c.HydraulicBoundaryLocation,
                                                                                                 c => c);

            return assessmentSection.HydraulicBoundaryDatabase.Locations
                                    .Select(location => new AggregatedHydraulicBoundaryLocation(
                                                location.Id, location.Name, location.Location,
                                                GetCalculationResult(waterLevelLookupForFactorizedSignalingNorm[location].Output),
                                                GetCalculationResult(waterLevelLookupForSignalingNorm[location].Output),
                                                GetCalculationResult(waterLevelLookupForLowerLimitNorm[location].Output),
                                                GetCalculationResult(waterLevelLookupForFactorizedLowerLimitNorm[location].Output),
                                                GetCalculationResult(waveHeightLookupForFactorizedSignalingNorm[location].Output),
                                                GetCalculationResult(waveHeightLookupForSignalingNorm[location].Output),
                                                GetCalculationResult(waveHeightLookupForLowerLimitNorm[location].Output),
                                                GetCalculationResult(waveHeightLookupForFactorizedLowerLimitNorm[location].Output))).ToArray();
        }

        private static RoundedDouble GetCalculationResult(HydraulicBoundaryLocationCalculationOutput output)
        {
            return output?.Result ?? RoundedDouble.NaN;
        }
    }
}