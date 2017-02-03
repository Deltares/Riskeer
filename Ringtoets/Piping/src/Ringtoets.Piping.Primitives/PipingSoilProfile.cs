// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Primitives.Properties;

namespace Ringtoets.Piping.Primitives
{
    /// <summary>
    /// This class represents a soil profile, which was imported for use in a piping calculation.
    /// </summary>
    public class PipingSoilProfile
    {
        private PipingSoilLayer[] layers;

        /// <summary>
        /// Creates a new instance ofL <see cref="PipingSoilProfile"/>, with the given <paramref name="name"/>, <paramref name="bottom"/> and <paramref name="layers"/>.
        /// A new collection is created for <paramref name="layers"/> and used in the <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <param name="name">The name of the profile.</param>
        /// <param name="bottom">The bottom level of the profile.</param>
        /// <param name="layers">The collection of layers that should be part of the profile.</param>
        /// <param name="sourceProfileType">The type of soil profile used as data source
        /// to build this instance.</param>
        /// <param name="pipingSoilProfileId">Identifier of the profile.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="layers"/> contains no layers.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="layers"/> is <c>null</c>.</exception>
        public PipingSoilProfile(string name, double bottom, IEnumerable<PipingSoilLayer> layers, SoilProfileType sourceProfileType, long pipingSoilProfileId)
        {
            Name = name;
            Bottom = bottom;
            Layers = layers;
            SoilProfileType = sourceProfileType;
            PipingSoilProfileId = pipingSoilProfileId;
        }

        /// <summary>
        /// Gets the database identifier of the <see cref="PipingSoilProfile"/>.
        /// </summary>
        public long PipingSoilProfileId { get; }

        /// <summary>
        /// Gets the bottom level of the <see cref="PipingSoilProfile"/>.
        /// </summary>
        public double Bottom { get; }

        /// <summary>
        /// Gets the name of <see cref="PipingSoilProfile"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets an ordered (by <see cref="PipingSoilLayer.Top"/>, descending) <see cref="IEnumerable{T}"/> of 
        /// <see cref="PipingSoilLayer"/> for the <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the value is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the value contains no layers.</exception>
        public IEnumerable<PipingSoilLayer> Layers
        {
            get
            {
                return layers;
            }
            private set
            {
                ValidateLayersCollection(value);
                layers = value.OrderByDescending(l => l.Top).ToArray();
            }
        }

        /// <summary>
        /// Gets the type of soil profile used as data source to build this instance.
        /// </summary>
        public SoilProfileType SoilProfileType { get; }

        /// <summary>
        /// Gets the thickness of the given layer in the <see cref="PipingSoilProfile"/>.
        /// Thickness of a layer is determined by its top and the top of the layer below it.
        /// </summary>
        /// <param name="layer">The <see cref="PipingSoilLayer"/> to determine the thickness of.</param>
        /// <returns>The thickness of the <paramref name="layer"/>.</returns>
        /// <exception cref="ArgumentException"><see cref="Layers"/> does not contain <paramref name="layer"/>.</exception>
        public double GetLayerThickness(PipingSoilLayer layer)
        {
            IEnumerable<PipingSoilLayer> layersOrderedByTopAscending = layers.Reverse();
            double previousLevel = Bottom;
            foreach (PipingSoilLayer oLayer in layersOrderedByTopAscending)
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
            return Name ?? string.Empty;
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
            return Equals((PipingSoilProfile) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = layers?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ PipingSoilProfileId.GetHashCode();
                hashCode = (hashCode * 397) ^ Bottom.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (int) SoilProfileType;
                return hashCode;
            }
        }

        private bool Equals(PipingSoilProfile other)
        {
            return AreLayersEqual(other.layers)
                   && PipingSoilProfileId == other.PipingSoilProfileId
                   && Bottom.Equals(other.Bottom)
                   && string.Equals(Name, other.Name)
                   && SoilProfileType == other.SoilProfileType;
        }

        private bool AreLayersEqual(PipingSoilLayer[] otherLayers)
        {
            int layerCount = layers.Length;
            if (layerCount != otherLayers.Length)
            {
                return false;
            }

            for (int i = 0; i < layerCount; i++)
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
        /// all have values for <see cref="PipingSoilLayer.Top"/> which are greater than or equal to <see cref="Bottom"/>.
        /// </summary>
        /// <param name="collection">The collection of <see cref="PipingSoilLayer"/> to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="collection"/> contains no layers</item>
        /// <item><paramref name="collection"/> contains a layer with the <see cref="PipingSoilLayer.Top"/> less than
        /// <see cref="Bottom"/></item>
        /// </list></exception>
        private void ValidateLayersCollection(IEnumerable<PipingSoilLayer> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection), string.Format(Resources.Error_Cannot_Construct_PipingSoilProfile_Without_Layers));
            }
            if (!collection.Any())
            {
                throw new ArgumentException(Resources.Error_Cannot_Construct_PipingSoilProfile_Without_Layers);
            }
            if (collection.Any(l => l.Top < Bottom))
            {
                throw new ArgumentException(Resources.PipingSoilProfile_Layers_Layer_top_below_profile_bottom);
            }
        }
    }
}