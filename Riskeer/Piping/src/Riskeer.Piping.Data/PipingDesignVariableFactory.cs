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
using Riskeer.Common.Data.Probabilistics;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Factory for creating design variables based on distributions for both semi-probabilistic and probabilistic
    /// piping calculations.
    /// </summary>
    public static class PipingDesignVariableFactory
    {
        /// <summary>
        /// Creates the design variable for the phreatic level at the exit point.
        /// </summary>
        /// <param name="pipingInput">The piping input.</param>
        /// <returns>The created <see cref="DesignVariable{NormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingInput"/> is <c>null</c>.</exception>
        public static DesignVariable<NormalDistribution> GetPhreaticLevelExit(PipingInput pipingInput)
        {
            if (pipingInput == null)
            {
                throw new ArgumentNullException(nameof(pipingInput));
            }

            return new NormalDistributionDesignVariable(pipingInput.PhreaticLevelExit)
            {
                Percentile = 0.05
            };
        }
    }
}