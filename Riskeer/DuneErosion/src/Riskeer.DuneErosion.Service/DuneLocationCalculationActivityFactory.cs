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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service;
using Riskeer.DuneErosion.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Riskeer.DuneErosion.Service
{
    /// <summary>
    /// This class defines factory methods that can be used to create instances of <see cref="CalculatableActivity"/> for
    /// dune location calculations.
    /// </summary>
    public static class DuneLocationCalculationActivityFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> based on <paramref name="calculations"/>.
        /// </summary>
        /// <param name="calculations">The calculations to create activities for.</param>
        /// <param name="assessmentSection">The assessment section the calculations belong to.</param>
        /// <param name="norm">The norm to use during the calculations.</param>
        /// <param name="categoryBoundaryName">The name of the category boundary.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> or
        /// <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        public static IEnumerable<CalculatableActivity> CreateCalculationActivities(IEnumerable<DuneLocationCalculation> calculations,
                                                                                    IAssessmentSection assessmentSection,
                                                                                    double norm,
                                                                                    string categoryBoundaryName)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase);
            return calculations.Select(calculation => new DuneLocationCalculationActivity(calculation,
                                                                                          settings,
                                                                                          norm,
                                                                                          categoryBoundaryName)).ToArray();
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> based on <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to create activities for.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> or
        /// <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateCalculationActivities(DuneErosionFailureMechanism failureMechanism,
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

            activities.AddRange(CreateCalculationActivities(
                                    failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_MechanismSpecificFactorizedSignalingNorm_DisplayName));

            activities.AddRange(CreateCalculationActivities(
                                    failureMechanism.CalculationsForMechanismSpecificSignalingNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificSignalingNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_MechanismSpecificSignalingNorm_DisplayName));

            activities.AddRange(CreateCalculationActivities(
                                    failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_MechanismSpecificLowerLimitNorm_DisplayName));

            activities.AddRange(CreateCalculationActivities(
                                    failureMechanism.CalculationsForLowerLimitNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.LowerLimitNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_LowerLimitNorm_DisplayName));

            activities.AddRange(CreateCalculationActivities(
                                    failureMechanism.CalculationsForFactorizedLowerLimitNorm,
                                    assessmentSection,
                                    failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.FactorizedLowerLimitNorm),
                                    RingtoetsCommonDataResources.FailureMechanismCategoryType_FactorizedLowerLimitNorm_DisplayName));

            return activities;
        }
    }
}