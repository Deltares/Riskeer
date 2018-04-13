﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Util.TestUtil
{
    /// <summary>
    /// Test helper for dealing with hydraulic boundary locations and calculations in
    /// <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
    /// </summary>
    public static class GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper
    {
        private static readonly Random random = new Random(39);

        /// <summary>
        /// Sets the given <paramref name="hydraulicBoundaryLocations"/> to
        /// the <paramref name="assessmentSection"/> and <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to set the locations to.</param>
        /// <param name="assessmentSection">The assessment section to set the locations to.</param>
        /// <param name="hydraulicBoundaryLocations">The locations to set.</param>
        /// <param name="setCalculationOutput">Whether to set dummy output for the automatically generated
        /// hydraulic boundary location calculations.</param>
        public static void SetHydraulicBoundaryLocations(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                         AssessmentSectionStub assessmentSection,
                                                         IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                         bool setCalculationOutput = false)
        {
            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations, setCalculationOutput);
            failureMechanism.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

            if (setCalculationOutput)
            {
                CreateHydraulicBoundaryLocationCalculationsOutput(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
                CreateHydraulicBoundaryLocationCalculationsOutput(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
                CreateHydraulicBoundaryLocationCalculationsOutput(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
                CreateHydraulicBoundaryLocationCalculationsOutput(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
                CreateHydraulicBoundaryLocationCalculationsOutput(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
                CreateHydraulicBoundaryLocationCalculationsOutput(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
            }
        }

        /// <summary>
        /// Gets all the <see cref="HydraulicBoundaryLocationCalculation"/> that have an output within the <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to retrieve the calculations from.</param>
        /// <returns>A collection of all the hydraulic boundary location calculations that contain an output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<HydraulicBoundaryLocationCalculation> GetAllHydraulicBoundaryLocationsCalculationsWithOutput(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Where(HasHydraulicBoundaryLocationCalculationOutput)
                                   .Concat(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.Where(HasHydraulicBoundaryLocationCalculationOutput))
                                   .Concat(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.Where(HasHydraulicBoundaryLocationCalculationOutput))
                                   .Concat(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Where(HasHydraulicBoundaryLocationCalculationOutput))
                                   .Concat(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.Where(HasHydraulicBoundaryLocationCalculationOutput))
                                   .Concat(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.Where(HasHydraulicBoundaryLocationCalculationOutput));
        }

        private static void CreateHydraulicBoundaryLocationCalculationsOutput(
            IEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations)
        {
            foreach (HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation in hydraulicBoundaryLocationCalculations)
            {
                hydraulicBoundaryLocationCalculation.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            }
        }

        private static bool HasHydraulicBoundaryLocationCalculationOutput(HydraulicBoundaryLocationCalculation calculation)
        {
            return calculation.HasOutput;
        }
    }
}