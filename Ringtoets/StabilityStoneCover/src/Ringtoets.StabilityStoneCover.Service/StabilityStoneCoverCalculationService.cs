// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Ringtoets.Common.Service;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Service
{
    /// <summary>
    /// Service related to working with <see cref="StabilityStoneCoverWaveConditionsCalculation"/>
    /// for calculation and validation for calculations.
    /// </summary>
    public static class StabilityStoneCoverCalculationService
    {
        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StabilityStoneCoverWaveConditionsCalculation"/>
        /// for which to validate the values.</param>
        /// <returns><c>False</c> if <paramref name="calculation"/> contains validation errors;
        /// <c>True</c> otherwise.</returns>
        public static bool Validate(StabilityStoneCoverWaveConditionsCalculation calculation)
        {
            // TODO: Implement validation with WTI-826
            return CalculationServiceHelper.PerformValidation(calculation.Name,
                                                              () => new string[0]);
        }
    }
}