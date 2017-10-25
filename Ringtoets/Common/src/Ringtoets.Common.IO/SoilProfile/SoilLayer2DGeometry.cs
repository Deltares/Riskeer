// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Class representing the full geometry of a 2D soil layer, taking into account
    /// layer properties, an outer loop and zero to many inner loops.
    /// </summary>
    internal class SoilLayer2DGeometry
    {
        /// <summary>
        /// Gets or sets the layer properties going with the geometry.
        /// </summary>
        public LayerProperties LayerProperties { get; set; }

        /// <summary>
        /// Gets or sets the outer loop of the geometry.
        /// </summary>
        public SoilLayer2DLoop OuterLoop { get; set; }

        /// <summary>
        /// Gets or sets the inner loops of the geometry.
        /// </summary>
        public IEnumerable<SoilLayer2DLoop> InnerLoops { get; set; }
    }
}