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
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Service;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Integration.Service
{
    /// <summary>
    /// This class defines factory methods that can be used to create instances of <see cref="CalculatableActivity"/> for
    /// hydraulic boundary location calculations on assessment section level.
    /// </summary>
    public static class AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for all hydraulic boundary location calculations
        /// in the given <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create the activities for.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateHydraulicBoundaryLocationCalculationActivities(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var activities = new List<CalculatableActivity>();
            activities.AddRange(CreateWaterLevelCalculationActivitiesForNormTargetProbabilities(assessmentSection));
            activities.AddRange(CreateWaterLevelCalculationActivitiesForUserDefinedTargetProbabilities(assessmentSection));
            activities.AddRange(CreateWaveHeightCalculationActivitiesForUserDefinedTargetProbabilities(assessmentSection));
            return activities;
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for all norm target probability based water level calculations 
        /// within the given <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create the activities for.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateWaterLevelCalculationActivitiesForNormTargetProbabilities(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var activities = new List<CalculatableActivity>();

            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                    assessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                                    assessmentSection,
                                    lowerLimitNorm,
                                    TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(assessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                                                                                                                                 assessmentSection)));

            double signalingNorm = assessmentSection.FailureMechanismContribution.SignalingNorm;
            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                    assessmentSection.WaterLevelCalculationsForSignalingNorm,
                                    assessmentSection,
                                    signalingNorm,
                                    TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(assessmentSection.WaterLevelCalculationsForSignalingNorm,
                                                                                                                                 assessmentSection)));

            return activities;
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for all user defined target probability based water level calculations 
        /// within the given <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create the activities for.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateWaterLevelCalculationActivitiesForUserDefinedTargetProbabilities(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                    .SelectMany(wlc => HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                                    wlc.HydraulicBoundaryLocationCalculations,
                                                    assessmentSection,
                                                    wlc.TargetProbability,
                                                    TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(wlc.HydraulicBoundaryLocationCalculations,
                                                                                                                                                 assessmentSection))).ToList();
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for all user defined target probability based wave height calculations 
        /// within the given <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create the activities for.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateWaveHeightCalculationActivitiesForUserDefinedTargetProbabilities(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                    .SelectMany(whc => HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                                                    whc.HydraulicBoundaryLocationCalculations,
                                                    assessmentSection,
                                                    whc.TargetProbability,
                                                    TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(whc,
                                                                                                                                       assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                                                                                                                       c => c.TargetProbability)))\.ToList();
        }
    }
}