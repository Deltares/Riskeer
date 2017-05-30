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
using Ringtoets.MacroStabilityInwards.IO.Exceptions;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Builders
{
    /// <summary>
    /// Helps in the creation of a <see cref="MacroStabilityInwardsSoilProfile"/>.
    /// </summary>
    internal class SoilProfileBuilder1D
    {
        private readonly Collection<MacroStabilityInwardsSoilLayer> layers;
        private readonly string name;
        private readonly double bottom;
        private readonly long soilProfileId;

        internal SoilProfileBuilder1D(string name, double bottom, long soilProfileId)
        {
            this.name = name;
            this.bottom = bottom;
            this.soilProfileId = soilProfileId;
            layers = new Collection<MacroStabilityInwardsSoilLayer>();
        }

        /// <summary>
        /// Creates a new instances of the <see cref="MacroStabilityInwardsSoilProfile"/> based on the layer definitions.
        /// </summary>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilProfile"/>.</returns>
        /// <exception cref="SoilProfileBuilderException">Thrown when no layers have been added through <see cref="Add"/>.</exception>
        internal MacroStabilityInwardsSoilProfile Build()
        {
            try
            {
                return new MacroStabilityInwardsSoilProfile(name, bottom, layers, SoilProfileType.SoilProfile1D, soilProfileId);
            }
            catch (ArgumentException e)
            {
                throw new SoilProfileBuilderException(e.Message, e);
            }
        }

        /// <summary>
        /// Adds a new <see cref="MacroStabilityInwardsSoilLayer"/>, which will be added to the <see cref="MacroStabilityInwardsSoilProfile"/>.
        /// </summary>
        /// <param name="soilLayer">The <see cref="MacroStabilityInwardsSoilLayer"/> to add.</param>
        internal void Add(MacroStabilityInwardsSoilLayer soilLayer)
        {
            layers.Add(soilLayer);
        }
    }
}