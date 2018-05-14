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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="GrassCoverErosionOutwardsFailureMechanism"/> instances.
    /// </summary>
    public static class GrassCoverErosionOutwardsFailureMechanismExtensions
    {
        /// <summary>
        /// Gets the assessment level for a <see cref="HydraulicBoundaryLocation"/> based on <see cref="FailureMechanismCategoryType"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to get the assessment level from.</param>
        /// <param name="assessmentSection">The assessment section to get the assessment level from.</param>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to get the assessment level for.</param>
        /// <param name="categoryType">The category type to use while obtaining the assessment level.</param>
        /// <returns>The assessment level or <see cref="RoundedDouble.NaN"/> when:
        /// <list type="bullet">
        /// <item><paramref name="hydraulicBoundaryLocation"/> is <c>null</c>;</item>
        /// <item><paramref name="hydraulicBoundaryLocation"/> is not part of <paramref name="failureMechanism"/>;</item>
        /// <item><paramref name="hydraulicBoundaryLocation"/> is not part of <paramref name="assessmentSection"/>;</item>
        /// <item><paramref name="hydraulicBoundaryLocation"/> contains no corresponding calculation output.</item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="categoryType"/>
        /// is an invalid <see cref="FailureMechanismCategoryType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="categoryType"/>
        /// is a valid but unsupported <see cref="FailureMechanismCategoryType"/>.</exception>
        public static RoundedDouble GetAssessmentLevel(this GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                       IAssessmentSection assessmentSection,
                                                       HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                       FailureMechanismCategoryType categoryType)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (!Enum.IsDefined(typeof(FailureMechanismCategoryType), categoryType))
            {
                throw new InvalidEnumArgumentException(nameof(categoryType),
                                                       (int) categoryType,
                                                       typeof(FailureMechanismCategoryType));
            }

            IEnumerable<HydraulicBoundaryLocationCalculation> calculations;

            switch (categoryType)
            {
                case FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm:
                    calculations = failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm;
                    break;
                case FailureMechanismCategoryType.MechanismSpecificSignalingNorm:
                    calculations = failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm;
                    break;
                case FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm:
                    calculations = failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm;
                    break;
                case FailureMechanismCategoryType.LowerLimitNorm:
                    calculations = assessmentSection.WaterLevelCalculationsForLowerLimitNorm;
                    break;
                case FailureMechanismCategoryType.FactorizedLowerLimitNorm:
                    calculations = assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return GetAssessmentLevelFromCalculations(hydraulicBoundaryLocation, calculations);
        }

        /// <summary>
        /// Gets the norm based on <see cref="FailureMechanismCategoryType"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to get the norm from.</param>
        /// <param name="assessmentSection">The assessment section to get the norm from.</param>
        /// <param name="categoryType">The category type to use while obtaining the norm.</param>
        /// <returns>The norm corresponding to the provided category type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="categoryType"/>
        /// is an invalid <see cref="FailureMechanismCategoryType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="categoryType"/>
        /// is a valid but unsupported <see cref="FailureMechanismCategoryType"/>.</exception>
        public static double GetNorm(this GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                     IAssessmentSection assessmentSection,
                                     FailureMechanismCategoryType categoryType)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (!Enum.IsDefined(typeof(FailureMechanismCategoryType), categoryType))
            {
                throw new InvalidEnumArgumentException(nameof(categoryType),
                                                       (int) categoryType,
                                                       typeof(FailureMechanismCategoryType));
            }

            IEnumerable<FailureMechanismSectionAssemblyCategory> categories = AssemblyToolCategoriesFactory.CreateFailureMechanismSectionAssemblyCategories(
                assessmentSection.FailureMechanismContribution.SignalingNorm,
                assessmentSection.FailureMechanismContribution.LowerLimitNorm,
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);

            switch (categoryType)
            {
                case FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm:
                    return categories.First(c => c.Group == FailureMechanismSectionAssemblyCategoryGroup.IIv)
                                     .LowerBoundary;
                case FailureMechanismCategoryType.MechanismSpecificSignalingNorm:
                    return categories.First(c => c.Group == FailureMechanismSectionAssemblyCategoryGroup.IIIv)
                                     .LowerBoundary;
                case FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm:
                    return categories.First(c => c.Group == FailureMechanismSectionAssemblyCategoryGroup.IVv)
                                     .LowerBoundary;
                case FailureMechanismCategoryType.LowerLimitNorm:
                    return categories.First(c => c.Group == FailureMechanismSectionAssemblyCategoryGroup.Vv)
                                     .LowerBoundary;
                case FailureMechanismCategoryType.FactorizedLowerLimitNorm:
                    return categories.First(c => c.Group == FailureMechanismSectionAssemblyCategoryGroup.VIv)
                                     .LowerBoundary;
                default:
                    throw new NotSupportedException();
            }
        }

        private static RoundedDouble GetAssessmentLevelFromCalculations(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                        IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            return calculations.FirstOrDefault(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))?.Output?.Result
                   ?? RoundedDouble.NaN;
        }
    }
}