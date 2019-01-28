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
using Deltares.WTIStability.Data.Geo;
using Deltares.WTIStability.Data.Standard;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="SoilModel"/> instances which are required in a calculation.
    /// </summary>
    internal static class SoilModelCreator
    {
        /// <summary>
        /// Creates a <see cref="SoilModel"/> with the given <paramref name="soils"/>
        /// which can be used in a calculation.
        /// </summary>
        /// <param name="soils">The array of <see cref="Soil"/> to use in the <see cref="SoilModel"/>.</param>
        /// <returns>A new <see cref="SoilModel"/> with the <paramref name="soils"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soils"/> is <c>null</c>.</exception>
        public static SoilModel Create(Soil[] soils)
        {
            if (soils == null)
            {
                throw new ArgumentNullException(nameof(soils));
            }

            var soilModel = new SoilModel();
            soilModel.Soils.AddRange(soils);
            return soilModel;
        }
    }
}