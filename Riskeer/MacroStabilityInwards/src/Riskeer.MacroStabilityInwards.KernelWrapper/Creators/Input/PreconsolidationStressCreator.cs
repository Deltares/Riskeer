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
using System.Collections.Generic;
using System.Linq;
using Deltares.MacroStability.CSharpWrapper;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using CSharpWrapperPreconsolidationStress = Deltares.MacroStability.CSharpWrapper.Input.PreconsolidationStress;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="CSharpWrapperPreconsolidationStress"/> instances which are required by <see cref="IUpliftVanKernel"/>.
    /// </summary>
    internal static class PreconsolidationStressCreator
    {
        /// <summary>
        /// Creates <see cref="CSharpWrapperPreconsolidationStress"/> objects based on the given <see cref="PreconsolidationStress"/> objects.
        /// </summary>
        /// <param name="preconsolidationStresses">The preconsolidation stresses to use.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="CSharpWrapperPreconsolidationStress"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="preconsolidationStresses"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<CSharpWrapperPreconsolidationStress> Create(IEnumerable<PreconsolidationStress> preconsolidationStresses)
        {
            if (preconsolidationStresses == null)
            {
                throw new ArgumentNullException(nameof(preconsolidationStresses));
            }

            return preconsolidationStresses.Select(preconsolidationStress => new CSharpWrapperPreconsolidationStress
            {
                StressValue = preconsolidationStress.Stress,
                Point = new Point2D(preconsolidationStress.Coordinate.X, preconsolidationStress.Coordinate.Y)
            }).ToArray();
        }
    }
}