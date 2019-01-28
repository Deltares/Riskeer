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
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service for synchronizing common data 
    /// </summary>
    public static class RingtoetsCommonDataSynchronizationService
    {
        /// <summary>
        /// Clears the output of the provided hydraulic boundary location calculations.
        /// </summary>
        /// <param name="calculations">The calculations for which the output needs to be cleared.</param>
        /// <returns>All objects changed during the clear.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearHydraulicBoundaryLocationCalculationOutput(IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            IEnumerable<HydraulicBoundaryLocationCalculation> affectedCalculations = calculations.Where(c => c.HasOutput).ToArray();

            affectedCalculations.ForEachElementDo(c => c.Output = null);

            return affectedCalculations;
        }

        /// <summary>
        /// Clears the output of the given <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> to clear the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        /// <returns>All objects that have been changed.</returns>
        public static IEnumerable<IObservable> ClearCalculationOutput(ICalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (calculation.HasOutput)
            {
                calculation.ClearOutput();
                return new[]
                {
                    calculation
                };
            }

            return Enumerable.Empty<IObservable>();
        }

        /// <summary>
        /// Clears the given foreshore profile from a <see cref="StructuresCalculation{T}"/> collection.
        /// </summary>
        /// <typeparam name="TStructureInput">Object type of the structure calculation input.</typeparam>
        /// <typeparam name="TStructure">Object type of the structure property of <typeparamref name="TStructureInput"/>.</typeparam>
        /// <param name="calculations">The calculations.</param>
        /// <param name="profile">The profile to be cleared.</param>
        /// <returns>All affected objects by the clear.</returns>
        public static IEnumerable<IObservable> ClearForeshoreProfile<TStructureInput, TStructure>(IEnumerable<StructuresCalculation<TStructureInput>> calculations, ForeshoreProfile profile)
            where TStructureInput : StructuresInputBase<TStructure>, new()
            where TStructure : StructureBase
        {
            var affectedObjects = new List<IObservable>();
            foreach (StructuresCalculation<TStructureInput> calculation in calculations.Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile)))
            {
                affectedObjects.AddRange(ClearCalculationOutput(calculation));

                calculation.InputParameters.ForeshoreProfile = null;
                affectedObjects.Add(calculation.InputParameters);
            }

            return affectedObjects;
        }
    }
}