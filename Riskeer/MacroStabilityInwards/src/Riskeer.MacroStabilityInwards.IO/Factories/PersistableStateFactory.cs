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
using Components.Persistence.Stability.Data;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableState"/>.
    /// </summary>
    internal static class PersistableStateFactory
    {
        /// <summary>
        /// Creates a new collection of <see cref="PersistableState"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to use.</param>
        /// <param name="idFactory">The factory fo IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>A collection of <see cref="PersistableState"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<PersistableState> Create(IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile,
                                                           IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            if (idFactory == null)
            {
                throw new ArgumentNullException(nameof(idFactory));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            return null;
        }
    }
}