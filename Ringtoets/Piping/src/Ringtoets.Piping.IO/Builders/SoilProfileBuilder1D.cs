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
using System.Collections.ObjectModel;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// Helps in the creation of a <see cref="PipingSoilProfile"/>.
    /// </summary>
    internal class SoilProfileBuilder1D
    {
        private readonly Collection<PipingSoilLayer> layers;
        private readonly string name;
        private readonly double bottom;

        /// <summary>
        /// Creates a new instance of <see cref="SoilProfileBuilder1D"/>.
        /// </summary>
        /// <param name="name">The name of the profile.</param>
        /// <param name="bottom">The bottom level of the profile.</param>
        internal SoilProfileBuilder1D(string name, double bottom)
        {
            this.name = name;
            this.bottom = bottom;
            layers = new Collection<PipingSoilLayer>();
        }

        /// <summary>
        /// Creates a new instances of the <see cref="PipingSoilProfile"/> based on the layer definitions.
        /// </summary>
        /// <returns>A new <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="SoilProfileBuilderException">Thrown when no layers have been added through <see cref="Add"/>.</exception>
        internal PipingSoilProfile Build()
        {
            try
            {
                return new PipingSoilProfile(name, bottom, layers, SoilProfileType.SoilProfile1D);
            }
            catch (ArgumentException e)
            {
                throw new SoilProfileBuilderException(e.Message, e);
            }
        }

        /// <summary>
        /// Adds a new <see cref="PipingSoilLayer"/>, which will be added to the <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <param name="soilLayer">The <see cref="PipingSoilLayer"/> to add.</param>
        internal void Add(PipingSoilLayer soilLayer)
        {
            layers.Add(soilLayer);
        }
    }
}