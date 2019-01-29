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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;

namespace Riskeer.Common.Util
{
    /// <summary>
    /// Class holds helper methods to match <see cref="FailureMechanismSection"/> objects 
    /// with <see cref="StructuresCalculation{T}"/> objects. 
    /// </summary>
    public static class StructuresHelper
    {
        /// <summary>
        /// Determine which <see cref="StructuresCalculation{T}"/> objects are available for a
        /// <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> objects.</param>
        /// <param name="calculations">The <see cref="CalculationWithLocation"/> objects.</param>
        /// <returns>A <see cref="IDictionary{K, V}"/> containing a <see cref="List{T}"/> 
        /// of <see cref="FailureMechanismSectionResult"/> objects 
        /// for each section which has calculations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>
        /// or when an element in <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when an element in <paramref name="sections"/> is 
        /// <c>null</c>.</exception>
        public static IDictionary<string, List<ICalculation>> CollectCalculationsPerSection<T>(IEnumerable<FailureMechanismSection> sections,
                                                                                               IEnumerable<StructuresCalculation<T>> calculations)
            where T : IStructuresCalculationInput<StructureBase>, new()
        {
            return AssignUnassignCalculations.CollectCalculationsPerSection(sections, AsCalculationsWithLocations(calculations));
        }

        /// <summary>
        /// Transforms the <paramref name="calculations"/> into <see cref="CalculationWithLocation"/> and filter out the calculations
        /// for which a <see cref="CalculationWithLocation"/> could not be made.
        /// </summary>
        /// <param name="calculations">The <see cref="StructuresCalculation{T}"/> collection to transform.</param>
        /// <returns>A collection of <see cref="CalculationWithLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c> or when
        /// an element in <paramref name="calculations"/> is <c>null</c>.</exception>
        private static IEnumerable<CalculationWithLocation> AsCalculationsWithLocations<T>(IEnumerable<StructuresCalculation<T>> calculations)
            where T : IStructuresCalculationInput<StructureBase>, new()
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            return calculations.Select(AsCalculationWithLocation).Where(c => c != null);
        }

        private static CalculationWithLocation AsCalculationWithLocation<T>(StructuresCalculation<T> calculation)
            where T : IStructuresCalculationInput<StructureBase>, new()
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (calculation.InputParameters.Structure == null)
            {
                return null;
            }

            return new CalculationWithLocation(calculation, calculation.InputParameters.Structure.Location);
        }
    }
}