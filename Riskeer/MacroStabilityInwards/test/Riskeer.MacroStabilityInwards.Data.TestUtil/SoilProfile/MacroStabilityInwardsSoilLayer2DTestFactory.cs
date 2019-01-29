﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile
{
    /// <summary>
    /// Factory to create simple <see cref="MacroStabilityInwardsSoilLayer2D"/> instances that 
    /// can be used for testing.
    /// </summary>
    public static class MacroStabilityInwardsSoilLayer2DTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        /// <returns>The created <see cref="MacroStabilityInwardsSoilLayer2D"/>.</returns>
        public static MacroStabilityInwardsSoilLayer2D CreateMacroStabilityInwardsSoilLayer2D()
        {
            return CreateMacroStabilityInwardsSoilLayer2D(new MacroStabilityInwardsSoilLayer2D[0]);
        }

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        /// <param name="nestedLayers">The nested <see cref="MacroStabilityInwardsSoilLayer2D"/>.</param>
        /// <returns>The created <see cref="MacroStabilityInwardsSoilLayer2D"/>.</returns>
        public static MacroStabilityInwardsSoilLayer2D CreateMacroStabilityInwardsSoilLayer2D(IEnumerable<MacroStabilityInwardsSoilLayer2D> nestedLayers)
        {
            return new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                        {
                                                            new Point2D(0, 0),
                                                            new Point2D(1, 1)
                                                        }),
                                                        new MacroStabilityInwardsSoilLayerData
                                                        {
                                                            MaterialName = "Valid"
                                                        },
                                                        nestedLayers);
        }
    }
}