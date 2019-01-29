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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Service;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

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
            activities.AddRange(CreateDesignWaterLevelCalculationActivities(assessmentSection));
            activities.AddRange(CreateWaveHeightCalculationActivities(assessmentSection));
            return activities;
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for wave height calculations
        /// based on the given <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create the activities for.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateWaveHeightCalculationActivities(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var activities = new List<CalculatableActivity>();

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                                    assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm,
                                    assessmentSection,
                                    assessmentSection.GetNorm(AssessmentSectionCategoryType.FactorizedSignalingNorm),
                                    RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedSignalingNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                                    assessmentSection.WaveHeightCalculationsForSignalingNorm,
                                    assessmentSection,
                                    assessmentSection.GetNorm(AssessmentSectionCategoryType.SignalingNorm),
                                    RingtoetsCommonDataResources.AssessmentSectionCategoryType_SignalingNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                                    assessmentSection.WaveHeightCalculationsForLowerLimitNorm,
                                    assessmentSection,
                                    assessmentSection.GetNorm(AssessmentSectionCategoryType.LowerLimitNorm),
                                    RingtoetsCommonDataResources.AssessmentSectionCategoryType_LowerLimitNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                                    assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm,
                                    assessmentSection,
                                    assessmentSection.GetNorm(AssessmentSectionCategoryType.FactorizedLowerLimitNorm),
                                    RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedLowerLimitNorm_DisplayName));

            return activities;
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for design water level calculations
        /// based on the given <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create the activities for.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateDesignWaterLevelCalculationActivities(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var activities = new List<CalculatableActivity>();

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                    assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm,
                                    assessmentSection,
                                    assessmentSection.GetNorm(AssessmentSectionCategoryType.FactorizedSignalingNorm),
                                    RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedSignalingNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                    assessmentSection.WaterLevelCalculationsForSignalingNorm,
                                    assessmentSection,
                                    assessmentSection.GetNorm(AssessmentSectionCategoryType.SignalingNorm),
                                    RingtoetsCommonDataResources.AssessmentSectionCategoryType_SignalingNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                    assessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                                    assessmentSection,
                                    assessmentSection.GetNorm(AssessmentSectionCategoryType.LowerLimitNorm),
                                    RingtoetsCommonDataResources.AssessmentSectionCategoryType_LowerLimitNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                    assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm,
                                    assessmentSection,
                                    assessmentSection.GetNorm(AssessmentSectionCategoryType.FactorizedLowerLimitNorm),
                                    RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedLowerLimitNorm_DisplayName));

            return activities;
        }
    }
}