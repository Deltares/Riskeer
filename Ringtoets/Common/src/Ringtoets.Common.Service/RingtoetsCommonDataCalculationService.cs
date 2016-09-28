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

using System;
using Core.Common.Utils;
using Ringtoets.HydraRing.Calculation.Data.Output;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service for providing common data calculation services 
    /// </summary>
    public static class RingtoetsCommonDataCalculationService
    {
        /// <summary>
        /// Determines whether the calculated output is converged,
        /// based on the <paramref name="output"/> and the <paramref name="norm"/>
        /// </summary>
        /// <param name="output">The resultant <see cref="ReliabilityIndexCalculationOutput"/> 
        /// object after a calculation.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <returns>True if the solution converged, false if otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> 
        /// is <c>null</c></exception>
        public static bool CalculationConverged(ReliabilityIndexCalculationOutput output, double norm)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }
            return Math.Abs(output.CalculatedReliabilityIndex - StatisticsConverter.NormToBeta(norm)) <= 1.0e-3;
        }
    }
}