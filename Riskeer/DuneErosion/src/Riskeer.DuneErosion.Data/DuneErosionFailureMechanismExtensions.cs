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
using System.ComponentModel;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Util;

namespace Ringtoets.DuneErosion.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="DuneErosionFailureMechanism"/> instances.
    /// </summary>
    public static class DuneErosionFailureMechanismExtensions
    {
        private const double failureMechanismSpecificNormFactor = 2.15;

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
        public static double GetNorm(this DuneErosionFailureMechanism failureMechanism,
                                     IAssessmentSection assessmentSection,
                                     FailureMechanismCategoryType categoryType)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanismSpecificNormFactor * FailureMechanismNormHelper.GetNorm(assessmentSection,
                                                                                           categoryType,
                                                                                           failureMechanism.Contribution,
                                                                                           failureMechanism.GeneralInput.N);
        }
    }
}