﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Service;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.Service
{
    /// <summary>
    /// This class defines factory methods that can be used to create instances of <see cref="CalculatableActivity"/> for
    /// macro stability inwards calculations.
    /// </summary>
    public static class MacroStabilityInwardsCalculationActivityFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> based on the calculations in
        /// <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing the calculations to create
        /// activities for.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="failureMechanism"/> belongs to.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateCalculationActivities(MacroStabilityInwardsFailureMechanism failureMechanism,
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

            return CreateCalculationActivities(failureMechanism.CalculationsGroup, failureMechanism.GeneralInput, assessmentSection);
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> based on the calculations in
        /// <paramref name="calculationGroup"/>.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to create activities for.</param>
        /// <param name="generalInput">General calculation parameters that are the same across all calculations.</param>
        /// <param name="assessmentSection">The assessment section the calculations in <paramref name="calculationGroup"/>
        /// belong to.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateCalculationActivities(CalculationGroup calculationGroup,
                                                                                    GeneralMacroStabilityInwardsInput generalInput,
                                                                                    IAssessmentSection assessmentSection)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return calculationGroup.GetCalculations()
                                   .Cast<MacroStabilityInwardsCalculation>()
                                   .Select(calc => CreateCalculationActivity(calc, generalInput, assessmentSection))
                                   .ToArray();
        }

        /// <summary>
        /// Creates a <see cref="CalculatableActivity"/> based on the given <paramref name="calculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create an activity for.</param>
        /// <param name="generalInput">General calculation parameters that are the same across all calculations.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculation"/>
        /// belongs to.</param>
        /// <returns>A <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static CalculatableActivity CreateCalculationActivity(MacroStabilityInwardsCalculation calculation,
                                                                     GeneralMacroStabilityInwardsInput generalInput,
                                                                     IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return new MacroStabilityInwardsCalculationActivity(calculation,
                                                                generalInput,
                                                                assessmentSection.GetNormativeAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation));
        }
    }
}