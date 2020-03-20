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
using Deltares.MacroStability.Geometry;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates a collection of <see cref="Soil"/> instances which are required in a calculation.
    /// </summary>
    internal static class SoilModelCreator
    {
        /// <summary>
        /// Creates a collection of <see cref="Soil"/> with the given <paramref name="soils"/>
        /// which can be used in a calculation.
        /// </summary>
        /// <param name="soils">The array of <see cref="Soil"/> to use in the collection of <see cref="Soil"/>.</param>
        /// <returns>A new collection of <see cref="Soil"/> with the <paramref name="soils"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soils"/> is <c>null</c>.</exception>
        public static IList<Soil> Create(Soil[] soils)
        {
            if (soils == null)
            {
                throw new ArgumentNullException(nameof(soils));
            }

            var soilModel = new List<Soil>();
            soilModel.AddRange(soils);
            return soilModel;
        }
    }
}