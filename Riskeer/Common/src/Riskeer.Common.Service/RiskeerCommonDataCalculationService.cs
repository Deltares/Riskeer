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

using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// Service for providing common data calculation services.
    /// </summary>
    public static class RiskeerCommonDataCalculationService
    {
        /// <summary>
        /// Determines whether the calculated output is converged.
        /// </summary>
        /// <param name="converged">The value indicating whether convergence has been reached.</param>
        /// <returns><see cref="CalculationConvergence.CalculatedConverged"/> if the calculated output converged,
        /// <see cref="CalculationConvergence.CalculatedNotConverged"/> if the calculated output did not converge,
        /// <see cref="CalculationConvergence.NotCalculated"/> if no convergence was determined.</returns>
        public static CalculationConvergence GetCalculationConvergence(bool? converged)
        {
            if (converged.HasValue)
            {
                return converged.Value
                           ? CalculationConvergence.CalculatedConverged
                           : CalculationConvergence.CalculatedNotConverged;
            }

            return CalculationConvergence.NotCalculated;
        }
    }
}