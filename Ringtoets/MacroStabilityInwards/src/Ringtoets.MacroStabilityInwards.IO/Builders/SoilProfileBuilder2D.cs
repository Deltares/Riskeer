﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.ObjectModel;
using Ringtoets.MacroStabilityInwards.IO.Exceptions;
using Ringtoets.MacroStabilityInwards.IO.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Builders
{
    /// <summary>
    /// Constructs a 1d Soil Profile based on definitions of <see cref="SoilLayer2D"/>.
    /// </summary>
    internal class SoilProfileBuilder2D
    {
        private readonly ICollection<MacroStabilityInwardsSoilLayer1D> layers = new Collection<MacroStabilityInwardsSoilLayer1D>();
        private readonly double atX;
        private readonly string profileName;
        private readonly long soilProfileId;

        private double bottom;

        /// <summary>
        /// Creates a new instance of <see cref="SoilProfileBuilder2D"/> with the supposed name for the new <see cref="MacroStabilityInwardsSoilProfile1D"/>
        /// and the point at which a 1D profile should be obtained from the 2D profile.
        /// </summary>
        /// <param name="profileName">The name for the <see cref="MacroStabilityInwardsSoilProfile1D"/> constructed by the <see cref="SoilProfileBuilder2D"/>.</param>
        /// <param name="atX">The x position from which to obtain a 1D profile.</param>
        /// <param name="soilProfileId">The identifier of the profile in the database.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="atX"/> can not be used to determine intersections with
        /// (is <see cref="double.NaN"/>).</exception>
        internal SoilProfileBuilder2D(string profileName, double atX, long soilProfileId)
        {
            if (double.IsNaN(atX))
            {
                string message = string.Format(Resources.Error_SoilProfileBuilder_cant_determine_intersect_SoilProfileName_0_at_double_NaN, profileName);
                throw new ArgumentException(message);
            }
            this.profileName = profileName;
            this.atX = atX;
            bottom = double.MaxValue;
            this.soilProfileId = soilProfileId;
        }

        /// <summary>
        /// Creates a new instances of the <see cref="MacroStabilityInwardsSoilProfile1D"/> based on the layer definitions.
        /// </summary>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilProfile1D"/>.</returns>
        /// <exception cref="SoilProfileBuilderException">Thrown when trying to build a 
        /// <see cref="MacroStabilityInwardsSoilProfile1D"/> and not having added any layers using <see cref="Add"/>.
        /// </exception>
        internal MacroStabilityInwardsSoilProfile1D Build()
        {
            try
            {
                return new MacroStabilityInwardsSoilProfile1D(profileName, bottom, layers, SoilProfileType.SoilProfile2D, soilProfileId);
            }
            catch (ArgumentException e)
            {
                throw new SoilProfileBuilderException(e.Message, e);
            }
        }

        /// <summary>
        /// Adds a new <see cref="SoilLayer2D"/> to the profile.
        /// </summary>
        /// <param name="soilLayer">The <see cref="SoilLayer2D"/> to add to the profile.</param>
        /// <returns>The <see cref="SoilProfileBuilder2D"/>.</returns>
        /// <exception cref="SoilProfileBuilderException">Thrown when either:
        /// <list type="bullet">
        /// <item>the <paramref name="soilLayer"/>'s geometry contains vertical segments at the 
        /// X-coordinate given for the construction of the <see cref="SoilProfileBuilder2D(string,double,long)"/>.</item>
        /// <item>any of the distributions of the stochastic parameters for <paramref name="soilLayer"/> is not defined 
        /// as lognormal or is shifted when it should not be</item>
        /// </list></exception>
        internal SoilProfileBuilder2D Add(SoilLayer2D soilLayer)
        {
            double newBottom;

            try
            {
                IEnumerable<MacroStabilityInwardsSoilLayer1D> soilLayers = soilLayer.AsMacroStabilityInwardsSoilLayers(atX, out newBottom);
                foreach (MacroStabilityInwardsSoilLayer1D layer in soilLayers)
                {
                    layers.Add(layer);
                }
            }
            catch (SoilLayerConversionException e)
            {
                throw new SoilProfileBuilderException(e.Message, e);
            }

            bottom = Math.Min(bottom, newBottom);
            return this;
        }
    }
}