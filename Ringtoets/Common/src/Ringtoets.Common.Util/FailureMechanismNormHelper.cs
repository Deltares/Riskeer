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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Util
{
    /// <summary>
    /// Class that contains helper methods to retrieve failure mechanism specific norms.
    /// </summary>
    public static class FailureMechanismNormHelper
    {
        /// <summary>
        /// Gets the norm based on <see cref="FailureMechanismCategoryType"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to get the norm from.</param>
        /// <param name="categoryType">The category type to use while obtaining the norm.</param>
        /// <param name="failureMechanismContribution">The failure mechanism contribution.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>The norm corresponding to the provided category type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="categoryType"/>
        /// is an invalid <see cref="FailureMechanismCategoryType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="categoryType"/>
        /// is a valid but unsupported <see cref="FailureMechanismCategoryType"/>.</exception>
        public static double GetNorm(IAssessmentSection assessmentSection,
                                     FailureMechanismCategoryType categoryType,
                                     double failureMechanismContribution,
                                     double n)
        {
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
                failureMechanismContribution,
                n);

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
    }
}