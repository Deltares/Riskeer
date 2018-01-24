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
using System.ComponentModel;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.AssessmentSection
{
    /// <summary>
    /// Extension methods for <see cref="IAssessmentSection"/>.
    /// </summary>
    public static class AssessmentSectionExtensions
    {
        /// <summary>
        /// Gets the normative assessment level from a <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to get the normative
        /// assessment level from.</param>
        /// <returns>The normative assessment level or <see cref="RoundedDouble.NaN"/> when:
        /// <list type="bullet">
        /// <item><paramref name="hydraulicBoundaryLocation"/> is <c>null</c>;</item>
        /// <item><paramref name="hydraulicBoundaryLocation"/> contains no corresponding calculation output.</item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assessmentSection"/>
        /// contains an invalid value of <see cref="NormType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assessmentSection"/>
        /// contains a valid value of <see cref="NormType"/>, but unsupported.</exception>
        public static RoundedDouble GetNormativeAssessmentLevel(this IAssessmentSection assessmentSection, HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            NormType normType = assessmentSection.FailureMechanismContribution.NormativeNorm;

            if (!Enum.IsDefined(typeof(NormType), normType))
            {
                throw new InvalidEnumArgumentException(nameof(normType),
                                                       (int) normType,
                                                       typeof(NormType));
            }

            switch (normType)
            {
                case NormType.Signaling:
                    return hydraulicBoundaryLocation?.DesignWaterLevelCalculation2.Output?.Result ?? RoundedDouble.NaN;
                case NormType.LowerLimit:
                    return hydraulicBoundaryLocation?.DesignWaterLevelCalculation3.Output?.Result ?? RoundedDouble.NaN;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}