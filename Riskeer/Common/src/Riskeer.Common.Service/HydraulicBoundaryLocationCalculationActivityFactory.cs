// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// This class defines factory methods that can be used to create instances of <see cref="CalculatableActivity"/> for
    /// hydraulic boundary location calculations.
    /// </summary>
    public static class HydraulicBoundaryLocationCalculationActivityFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for wave height calculations
        /// based on the given parameters.
        /// </summary>
        /// <param name="calculations">The collection of <see cref="HydraulicBoundaryLocationCalculation"/> to create
        /// the activities for.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculations"/> belong to.</param>
        /// <param name="targetProbability">The target probability to use during the calculations.</param>
        /// <param name="calculationIdentifier">The calculation identifier to use in all messages.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="calculationIdentifier"/> is <c>null</c> or empty.</exception>
        public static IEnumerable<CalculatableActivity> CreateWaveHeightCalculationActivities(
            IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
            IAssessmentSection assessmentSection,
            double targetProbability,
            string calculationIdentifier)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase);
            return calculations.Select(calculation => new WaveHeightCalculationActivity(calculation,
                                                                                        settings,
                                                                                        targetProbability,
                                                                                        calculationIdentifier)).ToArray();
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for design water level calculations
        /// based on the given parameters.
        /// </summary>
        /// <param name="calculations">The collection of <see cref="HydraulicBoundaryLocationCalculation"/> to create
        /// the activities for.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculations"/> belong to.</param>
        /// <param name="targetProbability">The target probability to use during the calculations.</param>
        /// <param name="calculationIdentifier">The calculation identifier to use in all messages.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="calculationIdentifier"/> is <c>null</c> or empty.</exception>
        public static IEnumerable<CalculatableActivity> CreateDesignWaterLevelCalculationActivities(
            IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
            IAssessmentSection assessmentSection,
            double targetProbability,
            string calculationIdentifier)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase);
            return calculations.Select(calculation => new DesignWaterLevelCalculationActivity(calculation,
                                                                                              settings,
                                                                                              targetProbability,
                                                                                              calculationIdentifier)).ToArray();
        }
    }
}