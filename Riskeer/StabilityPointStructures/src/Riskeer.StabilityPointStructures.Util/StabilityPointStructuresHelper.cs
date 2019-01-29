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
using Core.Common.Base;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Util;
using Ringtoets.StabilityPointStructures.Data;

namespace Riskeer.StabilityPointStructures.Util
{
    /// <summary>
    /// Class holds helper methods to match <see cref="StabilityPointStructuresFailureMechanismSectionResult"/> objects 
    /// with <see cref="StructuresCalculation{T}"/> objects. 
    /// </summary>
    public static class StabilityPointStructuresHelper
    {
        /// <summary>
        /// Updates the <see cref="StabilityPointStructuresFailureMechanismSectionResult.Calculation"/> for each element
        /// of <see cref="StabilityPointStructuresFailureMechanism.SectionResults"/> if required due to a change.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism which contains the <see cref="StabilityPointStructuresFailureMechanismSectionResult"/>
        /// and calculations to update with.</param>
        /// <returns>All affected objects by the update.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<StabilityPointStructuresFailureMechanismSectionResult> UpdateCalculationToSectionResultAssignments(StabilityPointStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IObservableEnumerable<StabilityPointStructuresFailureMechanismSectionResult> sectionResults = failureMechanism.SectionResults;
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations = failureMechanism.Calculations
                                                                                                             .Cast<StructuresCalculation<StabilityPointStructuresInput>>();
            return AssignUnassignCalculations.Update(sectionResults.Select(AsCalculationAssignment),
                                                     AsCalculationsWithLocations(calculations))
                                             .Cast<StabilityPointStructuresFailureMechanismSectionResult>()
                                             .ToArray();
        }

        private static IEnumerable<CalculationWithLocation> AsCalculationsWithLocations(
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations)
        {
            var calculationsWithLocation = new List<CalculationWithLocation>();
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculations)
            {
                if (calculation.InputParameters.Structure != null)
                {
                    calculationsWithLocation.Add(new CalculationWithLocation(calculation,
                                                                             calculation.InputParameters.Structure.Location));
                }
            }

            return calculationsWithLocation;
        }

        private static SectionResultWithCalculationAssignment AsCalculationAssignment(
            StabilityPointStructuresFailureMechanismSectionResult failureMechanismSectionResult)
        {
            return new SectionResultWithCalculationAssignment(
                failureMechanismSectionResult,
                result => ((StabilityPointStructuresFailureMechanismSectionResult) result).Calculation,
                (result, calculation) => ((StabilityPointStructuresFailureMechanismSectionResult) result).Calculation = (StructuresCalculation<StabilityPointStructuresInput>) calculation);
        }
    }
}