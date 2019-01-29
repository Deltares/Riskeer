// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// This class represents a two dimensional soil profile.
    /// </summary>
    public class SoilProfile2D : ISoilProfile
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilProfile2D"/>.
        /// </summary>
        /// <param name="id">The database identifier of the soil profile.</param>
        /// <param name="name">The name of the profile.</param>
        /// <param name="layers">The collection of layers that should be part of the profile.</param>
        /// <param name="preconsolidationStresses">The preconsolidation stresses that are defined for the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>, <paramref name="layers"/> or
        /// <paramref name="preconsolidationStresses"/> is <c>null</c>.</exception>
        public SoilProfile2D(long id, string name, IEnumerable<SoilLayer2D> layers, IEnumerable<PreconsolidationStress> preconsolidationStresses)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (preconsolidationStresses == null)
            {
                throw new ArgumentNullException(nameof(preconsolidationStresses));
            }

            if (layers == null)
            {
                throw new ArgumentNullException(nameof(layers));
            }

            Id = id;
            Name = name;
            PreconsolidationStresses = preconsolidationStresses;
            Layers = layers;
            IntersectionX = double.NaN;
        }

        /// <summary>
        /// Gets the database identifier of the soil profile.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the collection of layers that are part of the profile.
        /// </summary>
        public IEnumerable<SoilLayer2D> Layers { get; }

        /// <summary>
        /// The 1d intersection of the profile.
        /// </summary>
        public double IntersectionX { get; set; }

        /// <summary>
        /// Gets the collection of preconsolidation stresses that are defined for the profile.
        /// </summary>
        public IEnumerable<PreconsolidationStress> PreconsolidationStresses { get; }

        public string Name { get; }
    }
}