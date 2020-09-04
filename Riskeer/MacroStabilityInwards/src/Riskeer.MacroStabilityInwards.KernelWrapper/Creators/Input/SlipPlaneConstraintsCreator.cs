// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Deltares.MacroStability.CSharpWrapper.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="SlipPlaneConstraints"/> instances which are required by <see cref="IUpliftVanKernel"/>.
    /// </summary>
    public static class SlipPlaneConstraintsCreator
    {
        /// <summary>
        /// Creates a <see cref="SlipPlaneConstraints"/> based on the given <paramref name="input"/>
        /// which can be used by <see cref="IUpliftVanKernel"/>.
        /// </summary>
        /// <param name="input">The <see cref="UpliftVanSlipPlaneConstraints"/> to get the information from.</param>
        /// <returns>A new <see cref="SlipPlaneConstraints"/> with the given information from <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        public static SlipPlaneConstraints Create(UpliftVanSlipPlaneConstraints input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return new SlipPlaneConstraints
            {
                SlipPlaneMinDepth = input.SlipPlaneMinimumDepth,
                SlipPlaneMinLength = input.SlipPlaneMinimumLength,
                XEntryMin = input.ZoneBoundaryLeft,
                XEntryMax = input.ZoneBoundaryRight
            };
        }
    }
}