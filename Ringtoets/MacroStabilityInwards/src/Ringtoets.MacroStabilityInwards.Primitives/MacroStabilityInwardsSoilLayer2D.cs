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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// This class represents a 2D layer that was imported from D-Soil Model.
    /// </summary>
    public class MacroStabilityInwardsSoilLayer2D : IMacroStabilityInwardsSoilLayer
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        /// <param name="outerRing">The ring describing the outer boundaries of the layer.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilLayer2D(Ring outerRing)
            : this(outerRing, new MacroStabilityInwardsSoilLayerData(), Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>()) {}

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        /// <param name="outerRing">The ring describing the outer boundaries of the layer.</param>
        /// <param name="data">The data of the soil layer.</param>
        /// <param name="nestedLayers">The nested <see cref="MacroStabilityInwardsSoilLayer2D"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilLayer2D(Ring outerRing, MacroStabilityInwardsSoilLayerData data, IEnumerable<MacroStabilityInwardsSoilLayer2D> nestedLayers)
        {
            if (outerRing == null)
            {
                throw new ArgumentNullException(nameof(outerRing));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (nestedLayers == null)
            {
                throw new ArgumentNullException(nameof(nestedLayers));
            }

            OuterRing = outerRing;
            Data = data;
            NestedLayers = nestedLayers;
        }

        /// <summary>
        /// Gets the outer ring of the soil layer.
        /// </summary>
        public Ring OuterRing { get; }

        /// <summary>
        /// Gets the nested layers of the soil layer.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsSoilLayer2D> NestedLayers { get; }

        public MacroStabilityInwardsSoilLayerData Data { get; }

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

            return Equals((MacroStabilityInwardsSoilLayer2D) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Data.GetHashCode();

                hashCode = (hashCode * 397) ^ OuterRing.GetHashCode();

                foreach (MacroStabilityInwardsSoilLayer2D nestedLayer in NestedLayers)
                {
                    hashCode = (hashCode * 397) ^ nestedLayer.GetHashCode();
                }

                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsSoilLayer2D other)
        {
            return Data.Equals(other.Data)
                   && OuterRing.Equals(other.OuterRing)
                   && NestedLayers.SequenceEqual(other.NestedLayers);
        }
    }
}