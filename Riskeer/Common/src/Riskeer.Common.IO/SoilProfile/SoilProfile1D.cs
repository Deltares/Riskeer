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
    /// This class represents a one dimensional soil profile.
    /// </summary>
    public class SoilProfile1D : ISoilProfile
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilProfile1D"/>.
        /// </summary>
        /// <param name="id">The database identifier of the soil profile.</param>
        /// <param name="name">The name of the profile.</param>
        /// <param name="bottom">The bottom level of the profile.</param>
        /// <param name="layers">The collection of layers that should be part of the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="layers"/> 
        /// is <c>null</c>.</exception>
        public SoilProfile1D(long id, string name, double bottom, IEnumerable<SoilLayer1D> layers)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (layers == null)
            {
                throw new ArgumentNullException(nameof(layers));
            }

            Id = id;
            Name = name;
            Bottom = bottom;
            Layers = layers;
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
        /// Gets an <see cref="IEnumerable{T}"/> of 
        /// <see cref="SoilLayer1D"/> for the <see cref="SoilLayer1D"/>.
        /// </summary>
        public IEnumerable<SoilLayer1D> Layers { get; }

        public string Name { get; }
    }
}