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

namespace Ringtoets.MacroStabilityInwards.IO.Builders
{
    /// <summary>
    /// Class containing parameters which are defined for both 1D and 2D soil layers.
    /// </summary>
    internal abstract class GenericSoilLayerParameters
    {
        /// <summary>
        /// Gets or sets a <see cref="double"/> value representing whether the layer is an aquifer.
        /// </summary>
        public double? IsAquifer { get; set; }

        /// <summary>
        /// Gets or sets the name of the material that was assigned to the layer.
        /// </summary>
        internal string MaterialName { get; set; }

        /// <summary>
        /// Gets or sets the value representing the color that was used to represent the layer.
        /// </summary>
        internal double? Color { get; set; }
    }
}