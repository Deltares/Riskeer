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
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class represents a one dimensional soil profile.
    /// </summary>
    public class SoilProfile1D : ISoilProfile
    {
        private readonly SoilLayer1D[] soilLayers;

        /// <summary>
        /// Creates a new instance of <see cref="SoilProfile1D"/>.
        /// </summary>
        /// <param name="id">The database identifier of the soil profile.</param>
        /// <param name="name">The name of the profile.</param>
        /// <param name="bottom">The bottom level of the profile.</param>
        /// <param name="layers">The collection of layers that should be part of the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="layers"/> 
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="layers"/> contains no layers</item>
        /// <item><paramref name="layers"/> contains a layer with the <see cref="SoilLayer1D.Top"/> 
        /// less than <paramref name="bottom"/></item>
        /// </list>
        /// </exception>
        public SoilProfile1D(long id, string name, double bottom, IEnumerable<SoilLayer1D> layers)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            ValidateLayersCollection(layers, bottom);

            Id = id;
            Name = name;
            Bottom = bottom;
            soilLayers = layers.OrderByDescending(l => l.Top).ToArray();
        }

        /// <summary>
        /// Gets the database identifier of the soil profile.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the bottom level of the <see cref="SoilProfile1D"/>.
        /// </summary>
        public double Bottom { get; }

        /// <summary>
        /// Gets an ordered (by <see cref="SoilLayer1D.Top"/>, descending) <see cref="IEnumerable{T}"/> of 
        /// <see cref="SoilLayer1D"/> for the <see cref="SoilLayer1D"/>.
        /// </summary>
        public IEnumerable<SoilLayer1D> Layers
        {
            get
            {
                return soilLayers;
            }
        }

        public string Name { get; }

        /// <summary>
        /// Validates the given <paramref name="layers"/>. A valid <paramref name="layers"/> has layers which 
        /// all have values for <see cref="SoilLayer1D.Top"/> which are greater than or equal to <see cref="Bottom"/>.
        /// </summary>
        /// <param name="layers">The collection of <see cref="SoilLayer1D"/> to validate.</param>
        /// <param name="bottom">The bottom level of the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="layers"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="layers"/> contains no layers</item>
        /// <item><paramref name="layers"/> contains a layer with the <see cref="SoilLayer1D.Top"/> 
        /// less than <see cref="Bottom"/></item>
        /// </list>
        /// </exception>
        private void ValidateLayersCollection(IEnumerable<SoilLayer1D> layers, double bottom)
        {
            if (layers == null)
            {
                throw new ArgumentNullException(nameof(layers), string.Format(Resources.SoilProfile_Cannot_construct_SoilProfile_without_layers));
            }
            if (!layers.Any())
            {
                throw new ArgumentException(Resources.SoilProfile_Cannot_construct_SoilProfile_without_layers);
            }
            if (layers.Any(l => l.Top < bottom))
            {
                throw new ArgumentException(Resources.SoilProfile_Layers_Layer_top_below_profile_bottom);
            }
        }
    }
}