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
    /// Class representing the geometry of a 2D soil layer.
    /// </summary>
    internal class SoilLayer2DGeometry
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer2DGeometry"/>.
        /// </summary>
        /// <param name="outerLoop">The outer loop of the geometry.</param>
        /// <param name="innerLoops">The inner loops of the geometry.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public SoilLayer2DGeometry(SoilLayer2DLoop outerLoop, IEnumerable<SoilLayer2DLoop> innerLoops)
        {
            if (outerLoop == null)
            {
                throw new ArgumentNullException(nameof(outerLoop));
            }

            if (innerLoops == null)
            {
                throw new ArgumentNullException(nameof(innerLoops));
            }

            OuterLoop = outerLoop;
            InnerLoops = innerLoops;
        }

        /// <summary>
        /// Gets the outer loop of the geometry.
        /// </summary>
        public SoilLayer2DLoop OuterLoop { get; }

        /// <summary>
        /// Gets the inner loops of the geometry.
        /// </summary>
        public IEnumerable<SoilLayer2DLoop> InnerLoops { get; }
    }
}