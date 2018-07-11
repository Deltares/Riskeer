// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// This class defines factory methods that can be used to create instances of <see cref="CalculatableActivity"/> for
    /// hydraulic boundary location calculations on grass cover erosion outwards level.
    /// </summary>
    public static class GrassCoverErosionOutwardsHydraulicBoundaryLocationCalculationActivityFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for all hydraulic boundary location calculations
        /// on grass cover erosion outwards level.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to create the activities for.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateHydraulicBoundaryLocationCalculationActivities(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var activities = new List<CalculatableActivity>();
            activities.AddRange(CreateDesignWaterLevelCalculationActivities(failureMechanism, assessmentSection));
            activities.AddRange(CreateWaveHeightCalculationActivities(failureMechanism, assessmentSection));
            return activities;
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for wave height calculations
        /// on grass cover erosion outwards level.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to create the activities for.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateWaveHeightCalculationActivities(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var activities = new List<CalculatableActivity>();

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                                    failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_MechanismSpecificFactorizedSignalingNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                                    failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificSignalingNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_MechanismSpecificSignalingNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                                    failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_MechanismSpecificLowerLimitNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                                    assessmentSection.WaveHeightCalculationsForLowerLimitNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.LowerLimitNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_LowerLimitNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                                    assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.FactorizedLowerLimitNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_FactorizedLowerLimitNorm_DisplayName));

            return activities;
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for design water level calculations
        /// on grass cover erosion outwards level.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to create the activities for.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateDesignWaterLevelCalculationActivities(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var activities = new List<CalculatableActivity>();

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                    failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_MechanismSpecificFactorizedSignalingNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                    failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificSignalingNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_MechanismSpecificSignalingNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                    failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_MechanismSpecificLowerLimitNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                    assessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.LowerLimitNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_LowerLimitNorm_DisplayName));

            activities.AddRange(HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                    assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.FactorizedLowerLimitNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_FactorizedLowerLimitNorm_DisplayName));

            return activities;
        }
    }
}