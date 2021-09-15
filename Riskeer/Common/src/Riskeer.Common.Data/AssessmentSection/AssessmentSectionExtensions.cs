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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.AssessmentSection
{
    /// <summary>
    /// Extension methods for <see cref="IAssessmentSection"/>.
    /// </summary>
    public static class AssessmentSectionExtensions
    {
        /// <summary>
        /// Gets the normative assessment level for a <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to get the normative assessment level from.</param>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to get the normative assessment level for.</param>
        /// <returns>The normative assessment level or <see cref="RoundedDouble.NaN"/> when:
        /// <list type="bullet">
        /// <item><paramref name="hydraulicBoundaryLocation"/> is <c>null</c>;</item>
        /// <item><paramref name="hydraulicBoundaryLocation"/> is not part of <paramref name="assessmentSection"/>;</item>
        /// <item><paramref name="hydraulicBoundaryLocation"/> contains no corresponding calculation output.</item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assessmentSection"/>
        /// contains an invalid value of <see cref="NormType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assessmentSection"/>
        /// contains a valid value of <see cref="NormType"/>, but unsupported.</exception>
        public static RoundedDouble GetNormativeAssessmentLevel(this IAssessmentSection assessmentSection,
                                                                HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            HydraulicBoundaryLocationCalculation calculation = GetNormativeHydraulicBoundaryLocationCalculation(assessmentSection,
                                                                                                                hydraulicBoundaryLocation);

            return calculation?.Output?.Result ?? RoundedDouble.NaN;
        }

        /// <summary>
        /// Gets the normative <see cref="HydraulicBoundaryLocationCalculation"/> for a <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to get the <see cref="HydraulicBoundaryLocationCalculation"/> from.</param>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to get the normative
        /// <see cref="HydraulicBoundaryLocationCalculation"/> for.</param>
        /// <returns>The normative <see cref="HydraulicBoundaryLocationCalculation"/> or <c>null</c> when:
        /// <list type="bullet">
        /// <item><paramref name="hydraulicBoundaryLocation"/> is <c>null</c>;</item>
        /// <item><paramref name="hydraulicBoundaryLocation"/> is not part of <paramref name="assessmentSection"/>.</item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assessmentSection"/>
        /// contains an invalid value of <see cref="NormType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assessmentSection"/>
        /// contains a valid value of <see cref="NormType"/>, but unsupported.</exception>
        public static HydraulicBoundaryLocationCalculation GetNormativeHydraulicBoundaryLocationCalculation(this IAssessmentSection assessmentSection,
                                                                                                            HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            IEnumerable<HydraulicBoundaryLocationCalculation> calculations = GetHydraulicBoundaryLocationCalculations(assessmentSection);
            return GetHydraulicBoundaryLocationCalculationFromCalculations(hydraulicBoundaryLocation, calculations);
        }

        /// <summary>
        /// Gets the relevant collection of <see cref="HydraulicBoundaryLocationCalculation"/> based on the <see cref="NormType"/> of the
        /// assessment section.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to get the collections of
        /// <see cref="HydraulicBoundaryLocationCalculation"/> from.</param>
        /// <returns>A collection of <see cref="HydraulicBoundaryLocationCalculation"/> from the <see cref="IAssessmentSection"/>
        /// based on the <see cref="NormType"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assessmentSection"/>
        /// contains an invalid value of <see cref="NormType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assessmentSection"/>
        /// contains a valid value of <see cref="NormType"/>, but unsupported.</exception>
        private static IEnumerable<HydraulicBoundaryLocationCalculation> GetHydraulicBoundaryLocationCalculations(IAssessmentSection assessmentSection)
        {
            NormType normType = assessmentSection.FailureMechanismContribution.NormativeNorm;

            if (!Enum.IsDefined(typeof(NormType), normType))
            {
                throw new InvalidEnumArgumentException(nameof(normType),
                                                       (int) normType,
                                                       typeof(NormType));
            }

            IEnumerable<HydraulicBoundaryLocationCalculation> calculations;

            switch (normType)
            {
                case NormType.Signaling:
                    calculations = assessmentSection.WaterLevelCalculationsForSignalingNorm;
                    break;
                case NormType.LowerLimit:
                    calculations = assessmentSection.WaterLevelCalculationsForLowerLimitNorm;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return calculations;
        }

        private static HydraulicBoundaryLocationCalculation GetHydraulicBoundaryLocationCalculationFromCalculations(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                                                                    IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            return calculations.FirstOrDefault(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation));
        }
    }
}