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

namespace Ringtoets.MacroStabilityInwards.Data.SoilProfile
{
    /// <summary>
    /// A soil profile for which its soil layers have been adapted using a surface line.
    /// </summary>
    public class MacroStabilityInwardsSoilProfileUnderSurfaceLine
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilProfileUnderSurfaceLine"/>.
        /// </summary>
        /// <param name="layersUnderSurfaceLine">The layers in the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="layersUnderSurfaceLine"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilProfileUnderSurfaceLine(IEnumerable<MacroStabilityInwardsSoilLayerUnderSurfaceLine> layersUnderSurfaceLine)
        {
            if (layersUnderSurfaceLine == null)
            {
                throw new ArgumentNullException(nameof(layersUnderSurfaceLine));
            }
            LayersUnderSurfaceLine = layersUnderSurfaceLine;
        }

        /// <summary>
        /// Gets the layers in the profile.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsSoilLayerUnderSurfaceLine> LayersUnderSurfaceLine { get; }
    }
}