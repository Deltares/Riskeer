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

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// This class represents a 1D layer that was imported from D-Soil Model.
    /// </summary>
    public class MacroStabilityInwardsSoilLayer1D
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayer1D"/>, where the top is set to <paramref name="top"/>.
        /// </summary>
        /// <param name="top">The top level of the layer.</param>
        public MacroStabilityInwardsSoilLayer1D(double top)
        {
            Top = top;
            Properties = new MacroStabilityInwardsSoilLayerProperties();
        }

        /// <summary>
        /// Gets the properties of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        public MacroStabilityInwardsSoilLayerProperties Properties { get; }

        /// <summary>
        /// Gets the top level of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        public double Top { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((MacroStabilityInwardsSoilLayer1D) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Top.GetHashCode();
                hashCode = (hashCode * 397) ^ Properties.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsSoilLayer1D other)
        {
            return Top.Equals(other.Top)
                   && Properties.Equals(other.Properties);
        }
    }
}