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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile
{
    /// <summary>
    /// A factor to create configured <see cref="MacroStabilityInwardsStochasticSoilProfile"/> that 
    /// can be used for testing.
    /// </summary>
    public static class MacroStabilityInwardsStochasticSoilProfileTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsStochasticSoilProfile"/> with
        /// a configured 2D soil profile.
        /// </summary>
        /// <returns>A configured <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.</returns>
        public static MacroStabilityInwardsStochasticSoilProfile CreateMacroStabilityInwardsStochasticSoilProfile2D()
        {
            var doubleNestedLayer = new MacroStabilityInwardsSoilLayer2D(
                new Ring(new List<Point2D>
                {
                    new Point2D(4.0, 2.0),
                    new Point2D(0.0, 2.5)
                }),
                new MacroStabilityInwardsSoilLayerData
                {
                    MaterialName = "Soil"
                },
                Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>());

            var nestedLayer = new MacroStabilityInwardsSoilLayer2D(
                new Ring(new List<Point2D>
                {
                    new Point2D(4.0, 2.0),
                    new Point2D(0.0, 2.5)
                }),
                new MacroStabilityInwardsSoilLayerData
                {
                    MaterialName = "Clay"
                },
                new[]
                {
                    doubleNestedLayer
                });

            var layers = new[]
            {
                new MacroStabilityInwardsSoilLayer2D(
                    new Ring(new List<Point2D>
                    {
                        new Point2D(0.0, 1.0),
                        new Point2D(2.0, 4.0)
                    }),
                    new MacroStabilityInwardsSoilLayerData
                    {
                        MaterialName = "Sand"
                    },
                    Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>()),
                new MacroStabilityInwardsSoilLayer2D(
                    new Ring(new List<Point2D>
                    {
                        new Point2D(3.0, 1.0),
                        new Point2D(8.0, 3.0)
                    }),
                    new MacroStabilityInwardsSoilLayerData
                    {
                        MaterialName = "Sand"
                    },
                    new[]
                    {
                        nestedLayer
                    }
                ),
                new MacroStabilityInwardsSoilLayer2D(
                    new Ring(new List<Point2D>
                    {
                        new Point2D(2.0, 4.0),
                        new Point2D(2.0, 8.0)
                    }),
                    new MacroStabilityInwardsSoilLayerData
                    {
                        MaterialName = "Sand"
                    },
                    Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>())
            };

            return new MacroStabilityInwardsStochasticSoilProfile(0.5,
                                                                  new MacroStabilityInwardsSoilProfile2D(
                                                                      "Profile 2D",
                                                                      layers,
                                                                      Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>()));
        }
    }
}