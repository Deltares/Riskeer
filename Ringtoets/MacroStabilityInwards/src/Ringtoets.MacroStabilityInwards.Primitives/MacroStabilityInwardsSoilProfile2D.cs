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

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// This class represents a soil profile, which was imported for use in a macro stability inwards calculation.
    /// </summary>
    public class MacroStabilityInwardsSoilProfile2D : ISoilProfile
    {
        private MacroStabilityInwardsSoilLayer2D[] layers;

        /// <summary>
        /// Creates a new instance ofL <see cref="MacroStabilityInwardsSoilProfile2D"/>, with the given <paramref name="name"/> and <paramref name="layers"/>.
        /// A new collection is created for <paramref name="layers"/> and used in the <see cref="MacroStabilityInwardsSoilProfile2D"/>.
        /// </summary>
        /// <param name="name">The name of the profile.</param>
        /// <param name="layers">The collection of layers that should be part of the profile.</param>
        /// <param name="sourceProfileType">The type of soil profile used as data source
        /// to build this instance.</param>
        /// <param name="soilProfileId">Identifier of the profile.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="layers"/> contains no layers.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="layers"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilProfile2D(string name, IEnumerable<MacroStabilityInwardsSoilLayer2D> layers, SoilProfileType sourceProfileType, long soilProfileId)
        {
            Name = name;
            Layers = layers;
            SoilProfileType = sourceProfileType;
            MacroStabilityInwardsSoilProfileId = soilProfileId;
        }

        /// <summary>
        /// Gets the database identifier of the <see cref="MacroStabilityInwardsSoilProfile2D"/>.
        /// </summary>
        public long MacroStabilityInwardsSoilProfileId { get; }

        /// <summary>
        /// Gets the name of <see cref="MacroStabilityInwardsSoilProfile2D"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of <see cref="MacroStabilityInwardsSoilLayer2D"/> for 
        /// the <see cref="MacroStabilityInwardsSoilProfile2D"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the value is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the value contains no layers.</exception>
        public IEnumerable<MacroStabilityInwardsSoilLayer2D> Layers
        {
            get
            {
                return layers;
            }
            private set
            {
                ValidateLayersCollection(value);
                layers = value.ToArray();
            }
        }

        /// <summary>
        /// Gets the type of soil profile used as data source to build this instance.
        /// </summary>
        public SoilProfileType SoilProfileType { get; }

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
            var other = obj as MacroStabilityInwardsSoilProfile2D;
            return other != null && Equals((MacroStabilityInwardsSoilProfile2D) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 0;
                foreach (MacroStabilityInwardsSoilLayer2D layer in layers)
                {
                    hashCode = (hashCode * 397) ^ layer.GetHashCode();
                }
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (int) SoilProfileType;
                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsSoilProfile2D other)
        {
            return layers.SequenceEqual(other.layers)
                   && string.Equals(Name, other.Name)
                   && SoilProfileType == other.SoilProfileType;
        }

        /// <summary>
        /// Validates the given <paramref name="collection"/>. A valid <paramref name="collection"/> has layers.
        /// </summary>
        /// <param name="collection">The collection of <see cref="MacroStabilityInwardsSoilLayer2D"/> to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="collection"/> contains no layers.</exception>
        private void ValidateLayersCollection(IEnumerable<MacroStabilityInwardsSoilLayer2D> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection), string.Format(Resources.Error_Cannot_Construct_MacroStabilityInwardsSoilProfile_Without_Layers));
            }
            if (!collection.Any())
            {
                throw new ArgumentException(Resources.Error_Cannot_Construct_MacroStabilityInwardsSoilProfile_Without_Layers);
            }
        }
    }
}