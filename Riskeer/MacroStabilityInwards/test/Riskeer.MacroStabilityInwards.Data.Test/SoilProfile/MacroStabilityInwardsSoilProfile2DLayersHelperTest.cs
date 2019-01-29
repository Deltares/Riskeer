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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfile2DLayersHelperTest
    {
        [Test]
        public void GetLayersRecursively_LayersNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer2D> layers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(null);

            // Assert
            CollectionAssert.IsEmpty(layers);
        }

        [Test]
        public void GetLayersRecursively_WithNestedLayers_ReturnsAllTopLevelAndNestedLayers()
        {
            // Setup
            var nestedLayer1 = new MacroStabilityInwardsSoilLayer2D(
                new Ring(new List<Point2D>
                {
                    new Point2D(4.0, 2.0),
                    new Point2D(0.0, 2.5)
                }));

            var topLevelLayer1 = new MacroStabilityInwardsSoilLayer2D(
                new Ring(new List<Point2D>
                {
                    new Point2D(0.0, 1.0),
                    new Point2D(2.0, 4.0)
                }),
                new MacroStabilityInwardsSoilLayerData(),
                new[]
                {
                    nestedLayer1
                });

            var doubleNestedLayer = new MacroStabilityInwardsSoilLayer2D(
                new Ring(new List<Point2D>
                {
                    new Point2D(4.0, 2.0),
                    new Point2D(0.0, 2.5)
                }));

            var nestedLayer2 = new MacroStabilityInwardsSoilLayer2D(
                new Ring(new List<Point2D>
                {
                    new Point2D(4.0, 2.0),
                    new Point2D(0.0, 2.5)
                }),
                new MacroStabilityInwardsSoilLayerData(),
                new[]
                {
                    doubleNestedLayer
                });

            var topLevelLayer2 = new MacroStabilityInwardsSoilLayer2D(
                new Ring(new List<Point2D>
                {
                    new Point2D(3.0, 1.0),
                    new Point2D(8.0, 3.0)
                }),
                new MacroStabilityInwardsSoilLayerData(),
                new[]
                {
                    nestedLayer2
                });

            MacroStabilityInwardsSoilLayer2D[] layers =
            {
                topLevelLayer1,
                topLevelLayer2
            };

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer2D> flatLayers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(layers);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                topLevelLayer1,
                nestedLayer1,
                topLevelLayer2,
                nestedLayer2,
                doubleNestedLayer
            }, flatLayers);
        }
    }
}