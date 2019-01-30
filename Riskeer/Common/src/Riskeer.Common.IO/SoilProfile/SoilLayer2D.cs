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

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// A two-dimensional soil layer.
    /// </summary>
    public class SoilLayer2D : SoilLayerBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer2D"/>.
        /// </summary>
        /// <param name="outerLoop">The outer loop of the soil layer.</param>
        /// <param name="nestedLayers">The nested layers of the soil layer.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public SoilLayer2D(SoilLayer2DLoop outerLoop, IEnumerable<SoilLayer2D> nestedLayers)
        {
            if (outerLoop == null)
            {
                throw new ArgumentNullException(nameof(outerLoop));
            }

            if (nestedLayers == null)
            {
                throw new ArgumentNullException(nameof(nestedLayers));
            }

            OuterLoop = outerLoop;
            NestedLayers = nestedLayers;
        }

        /// <summary>
        /// Gets the outer loop of the soil layer.
        /// </summary>
        public SoilLayer2DLoop OuterLoop { get; }

        /// <summary>
        /// Gets the nested layers of the soil layer.
        /// </summary>
        public IEnumerable<SoilLayer2D> NestedLayers { get; }
    }
}