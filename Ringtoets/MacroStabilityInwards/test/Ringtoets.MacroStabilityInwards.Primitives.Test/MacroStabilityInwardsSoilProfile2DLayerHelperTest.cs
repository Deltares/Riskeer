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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfile2DLayerHelperTest
    {
        [Test]
        public void GetLayersRecursively_LayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsSoilProfile2DLayerHelper.GetLayersRecursively(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("layers", paramName);
        }

        [Test]
        public void GetLayersRecursively_WithLayers_ReturnsAllTopLevelAndNestedLayers()
        {
            // Setup
            var nestedLayer1 = new MacroStabilityInwardsSoilLayer2D(new Ring(new List<Point2D>
            {
                new Point2D(4.0, 2.0),
                new Point2D(0.0, 2.5)
            }), new List<Ring>());

            var topLevelLayer1 = new MacroStabilityInwardsSoilLayer2D(new Ring(new List<Point2D>
                                                                      {
                                                                          new Point2D(0.0, 1.0),
                                                                          new Point2D(2.0, 4.0)
                                                                      }),
                                                                      new List<Ring>(),
                                                                      new MacroStabilityInwardsSoilLayerData(),
                                                                      new[]
                                                                      {
                                                                          nestedLayer1
                                                                      });

            var doubleNestedLayer = new MacroStabilityInwardsSoilLayer2D(new Ring(new List<Point2D>
            {
                new Point2D(4.0, 2.0),
                new Point2D(0.0, 2.5)
            }), new List<Ring>());

            var nestedLayer2 = new MacroStabilityInwardsSoilLayer2D(new Ring(new List<Point2D>
                                                                    {
                                                                        new Point2D(4.0, 2.0),
                                                                        new Point2D(0.0, 2.5)
                                                                    }),
                                                                    new List<Ring>(),
                                                                    new MacroStabilityInwardsSoilLayerData(),
                                                                    new[]
                                                                    {
                                                                        doubleNestedLayer
                                                                    });

            var topLevelLayer2 = new MacroStabilityInwardsSoilLayer2D(new Ring(new List<Point2D>
                                                                      {
                                                                          new Point2D(3.0, 1.0),
                                                                          new Point2D(8.0, 3.0)
                                                                      }), new List<Ring>(),
                                                                      new MacroStabilityInwardsSoilLayerData(),
                                                                      new[]
                                                                      {
                                                                          nestedLayer2
                                                                      });
            var layers = new[]
            {
                topLevelLayer1,
                topLevelLayer2
            };

            // Call
            IEnumerable<IMacroStabilityInwardsSoilLayer2D> flatLayers = MacroStabilityInwardsSoilProfile2DLayerHelper.GetLayersRecursively(layers);

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