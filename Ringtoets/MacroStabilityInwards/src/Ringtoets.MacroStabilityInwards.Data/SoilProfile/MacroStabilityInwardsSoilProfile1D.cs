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
using Ringtoets.MacroStabilityInwards.Primitives.Properties;

namespace Ringtoets.MacroStabilityInwards.Data.SoilProfile
{
    /// <summary>
    /// This class represents a soil profile, which was imported for use in a macro stability inwards calculation.
    /// </summary>
    public class MacroStabilityInwardsSoilProfile1D : IMacroStabilityInwardsSoilProfile<MacroStabilityInwardsSoilLayer1D>
    {
        private MacroStabilityInwardsSoilLayer1D[] layers;
        private string name;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilProfile1D"/>, with the given <paramref name="name"/>, 
        /// <paramref name="bottom"/> and <paramref name="layers"/>.
        /// A new collection is created for <paramref name="layers"/> and used in the <see cref="MacroStabilityInwardsSoilProfile1D"/>.
        /// </summary>
        /// <param name="name">The name of the profile.</param>
        /// <param name="bottom">The bottom level of the profile.</param>
        /// <param name="layers">The collection of layers that should be part of the profile.</param>
        /// <exception cref="ArgumentException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="layers"/> contains no layers</item>
        /// <item><paramref name="layers"/> contains a layer with the <see cref="MacroStabilityInwardsSoilLayer1D.Top"/> less than
        /// <see cref="Bottom"/></item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="layers"/> 
        /// is <c>null</c>.</exception>
        /// <remarks>The <see cref="Layers"/> in this soil profile are ordered by
        /// <see cref="MacroStabilityInwardsSoilLayer1D.Top"/> in descending order.</remarks>
        public MacroStabilityInwardsSoilProfile1D(string name, double bottom, IEnumerable<MacroStabilityInwardsSoilLayer1D> layers)
        {
            Name = name;
            Bottom = bottom;
            ValidateLayersCollection(layers);
            Layers = layers.OrderByDescending(l => l.Top);
        }

        /// <summary>
        /// Gets the bottom level of the <see cref="MacroStabilityInwardsSoilProfile1D"/>.
        /// </summary>
        public double Bottom { get; }

        public IEnumerable<MacroStabilityInwardsSoilLayer1D> Layers
        {
            get
            {
                return layers;
            }
            private set
            {
                layers = value.ToArray();
            }
        }

        /// <summary>
        /// Gets the name of <see cref="MacroStabilityInwardsSoilProfile1D"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the value is <c>null</c>.</exception>
        public string Name
        {
            get
            {
                return name;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                name = value;
            }
        }

        /// <summary>
        /// Gets the thickness of the given layer in the <see cref="MacroStabilityInwardsSoilProfile1D"/>.
        /// Thickness of a layer is determined by its top and the top of the layer below it.
        /// </summary>
        /// <param name="layer">The <see cref="MacroStabilityInwardsSoilLayer1D"/> to determine the thickness of.</param>
        /// <returns>The thickness of the <paramref name="layer"/>.</returns>
        /// <exception cref="ArgumentException"><see cref="Layers"/> does not contain <paramref name="layer"/>.</exception>
        public double GetLayerThickness(MacroStabilityInwardsSoilLayer1D layer)
        {
            IEnumerable<MacroStabilityInwardsSoilLayer1D> layersOrderedByTopAscending = layers.Reverse();
            double previousLevel = Bottom;
            foreach (MacroStabilityInwardsSoilLayer1D oLayer in layersOrderedByTopAscending)
            {
                if (ReferenceEquals(layer, oLayer))
                {
                    return layer.Top - previousLevel;
                }
                previousLevel = oLayer.Top;
            }
            throw new ArgumentException("Layer not found in profile.");
        }

        public override string ToString()
        {
            return Name;
        }

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
            return Equals((MacroStabilityInwardsSoilProfile1D) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Bottom.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsSoilProfile1D other)
        {
            return AreLayersEqual(other.layers)
                   && Bottom.Equals(other.Bottom)
                   && string.Equals(Name, other.Name);
        }

        private bool AreLayersEqual(MacroStabilityInwardsSoilLayer1D[] otherLayers)
        {
            int layerCount = layers.Length;
            if (layerCount != otherLayers.Length)
            {
                return false;
            }

            for (var i = 0; i < layerCount; i++)
            {
                if (!layers[i].Equals(otherLayers[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Validates the given <paramref name="collection"/>. A valid <paramref name="collection"/> has layers which 
        /// all have values for <see cref="MacroStabilityInwardsSoilLayer1D.Top"/> which are greater than or equal to <see cref="Bottom"/>.
        /// </summary>
        /// <param name="collection">The collection of <see cref="MacroStabilityInwardsSoilLayer1D"/> to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="collection"/> contains no layers</item>
        /// <item><paramref name="collection"/> contains a layer with the <see cref="MacroStabilityInwardsSoilLayer1D.Top"/> less than
        /// <see cref="Bottom"/></item>
        /// </list></exception>
        private void ValidateLayersCollection(IEnumerable<MacroStabilityInwardsSoilLayer1D> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection), string.Format(Resources.Error_Cannot_Construct_MacroStabilityInwardsSoilProfile_Without_Layers));
            }
            if (!collection.Any())
            {
                throw new ArgumentException(Resources.Error_Cannot_Construct_MacroStabilityInwardsSoilProfile_Without_Layers);
            }
            if (collection.Any(l => l.Top < Bottom))
            {
                throw new ArgumentException(Resources.MacroStabilityInwardsSoilProfile_Layers_Layer_top_below_profile_bottom);
            }
        }
    }
}