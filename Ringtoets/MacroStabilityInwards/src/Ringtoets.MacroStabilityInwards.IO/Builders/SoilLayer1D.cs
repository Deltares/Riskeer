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

using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Builders
{
    /// <summary>
    /// This class represents objects which were imported from a D-Soil Model database. Instances of this class are transient and are not to be used
    /// once the D-Soil Model database has been imported.
    /// </summary>
    internal class SoilLayer1D : GenericSoilLayerParameters
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer1D"/>.
        /// </summary>
        public SoilLayer1D(double top)
        {
            Top = top;
        }

        /// <summary>
        /// Gets the top level of the <see cref="SoilLayer1D"/>.
        /// </summary>
        public double Top { get; }

        /// <summary>
        /// Constructs a (1D) <see cref="MacroStabilityInwardsSoilLayer1D"/> based on the properties set for the <see cref="SoilLayer1D"/>.
        /// </summary>
        /// <returns>The <see cref="MacroStabilityInwardsSoilLayer1D"/> with properties corresponding to those set on the <see cref="SoilLayer1D"/>.</returns>
        internal MacroStabilityInwardsSoilLayer1D AsMacroStabilityInwardsSoilLayer()
        {
            return new MacroStabilityInwardsSoilLayer1D(Top)
            {
                Properties =
                {
                    IsAquifer = IsAquifer.HasValue && IsAquifer.Value.Equals(1.0),
                    MaterialName = MaterialName ?? string.Empty,
                    Color = SoilLayerColorConversionHelper.ColorFromNullableDouble(Color)
                }
            };
        }
    }
}