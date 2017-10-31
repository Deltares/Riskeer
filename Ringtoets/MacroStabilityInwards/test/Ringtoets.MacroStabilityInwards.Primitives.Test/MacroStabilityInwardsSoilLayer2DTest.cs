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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayer2DTest
    {
        [Test]
        public void Constructor_OuterRingNullWithoutDataAndNestedLayers_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer2D(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("outerRing", exception.ParamName);
        }

        [Test]
        public void Constructor_OuterRingNullWithDataAndNestedLayers_ThrowsArgumentNullException()
        {
            // Setup
            var holes = new[]
            {
                new Ring(new[]
                {
                    new Point2D(0, 2),
                    new Point2D(2, 2)
                })
            };

            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer2D(null,
                                                                           holes,
                                                                           new MacroStabilityInwardsSoilLayerData(),
                                                                           Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("outerRing", exception.ParamName);
        }

        [Test]
        public void Constructor_HolesNullWithDataAndNestedLayers_ThrowsArgumentNullException()
        {
            // Setup
            Ring outerRing = RingTestFactory.CreateRandomRing();

            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer2D(outerRing,
                                                                           null,
                                                                           new MacroStabilityInwardsSoilLayerData(),
                                                                           Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("holes", exception.ParamName);
        }

        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            Ring outerRing = RingTestFactory.CreateRandomRing();
            var holes = new[]
            {
                RingTestFactory.CreateRandomRing()
            };

            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer2D(outerRing, holes, null, Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_NestedLayersNull_ThrowsArgumentNullException()
        {
            // Setup
            Ring outerRing = RingTestFactory.CreateRandomRing();
            var holes = new[]
            {
                RingTestFactory.CreateRandomRing()
            };

            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer2D(outerRing, holes, new MacroStabilityInwardsSoilLayerData(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("nestedLayers", exception.ParamName);
        }

        [Test]
        public void Constructor_WithOuterRing_ReturnsNewInstance()
        {
            // Setup
            Ring outerRing = RingTestFactory.CreateRandomRing();

            // Call
            var layer = new MacroStabilityInwardsSoilLayer2D(outerRing);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilLayer>(layer);
            Assert.AreSame(outerRing, layer.OuterRing);
            CollectionAssert.IsEmpty(layer.Holes);
            Assert.IsNotNull(layer.Data);
            Assert.IsEmpty(layer.NestedLayers);
        }

        [Test]
        public void Constructor_WithOuterRingHolesDataAndNestedLayers_ReturnsNewInstance()
        {
            // Setup
            Ring outerRing = RingTestFactory.CreateRandomRing();
            var holes = new[]
            {
                RingTestFactory.CreateRandomRing()
            };
            var data = new MacroStabilityInwardsSoilLayerData();
            IEnumerable<MacroStabilityInwardsSoilLayer2D> nestedLayers = Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>();

            // Call
            var layer = new MacroStabilityInwardsSoilLayer2D(outerRing, holes, data, nestedLayers);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilLayer>(layer);
            Assert.AreSame(outerRing, layer.OuterRing);
            Assert.AreNotSame(holes, layer.Holes);
            TestHelper.AssertCollectionsAreEqual(holes, layer.Holes, new ReferenceEqualityComparer<Ring>());
            Assert.AreSame(data, layer.Data);
            Assert.AreSame(nestedLayers, layer.NestedLayers);
        }

        [Test]
        public void GetHashCode_EqualLayers_AreEqual()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D layerA = CreateRandomLayer(21);
            MacroStabilityInwardsSoilLayer2D layerB = CreateRandomLayer(21);

            // Precondition
            Assert.AreEqual(layerA, layerB);
            Assert.AreEqual(layerB, layerA);

            // Call & Assert
            Assert.AreEqual(layerA.GetHashCode(), layerB.GetHashCode());
            Assert.AreEqual(layerB.GetHashCode(), layerA.GetHashCode());
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D layer = CreateRandomLayer(21);

            // Call
            bool areEqual = layer.Equals(new object());

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D layer = CreateRandomLayer(21);

            // Call
            bool areEqual = layer.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(LayerCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(MacroStabilityInwardsSoilLayer2D layer, MacroStabilityInwardsSoilLayer2D otherLayer, bool expectedEqual)
        {
            // Call
            bool areEqualOne = layer.Equals(otherLayer);
            bool areEqualTwo = otherLayer.Equals(layer);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        private static TestCaseData[] LayerCombinations()
        {
            MacroStabilityInwardsSoilLayer2D layerA = CreateRandomLayer(21);
            MacroStabilityInwardsSoilLayer2D layerB = CreateRandomLayer(21);
            MacroStabilityInwardsSoilLayer2D layerC = CreateRandomLayer(73);
            MacroStabilityInwardsSoilLayer2D layerD = CreateRandomLayer(21);

            var layerE = new MacroStabilityInwardsSoilLayer2D(
                RingTestFactory.CreateRandomRing(21),
                Enumerable.Empty<Ring>(),
                new MacroStabilityInwardsSoilLayerData
                {
                    Color = Color.Blue
                },
                new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(22))
                });

            var layerF = new MacroStabilityInwardsSoilLayer2D(
                RingTestFactory.CreateRandomRing(31),
                Enumerable.Empty<Ring>(),
                new MacroStabilityInwardsSoilLayerData
                {
                    Color = Color.Blue
                },
                new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(22))
                });

            var layerG = new MacroStabilityInwardsSoilLayer2D(
                RingTestFactory.CreateRandomRing(21),
                Enumerable.Empty<Ring>(),
                new MacroStabilityInwardsSoilLayerData
                {
                    Color = Color.Blue
                },
                new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(32))
                });

            var layerH = new MacroStabilityInwardsSoilLayer2D(
                RingTestFactory.CreateRandomRing(21),
                Enumerable.Empty<Ring>(),
                new MacroStabilityInwardsSoilLayerData
                {
                    Color = Color.Gold
                },
                new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(22))
                });

            return new[]
            {
                new TestCaseData(layerA, layerA, true)
                {
                    TestName = "Equals_LayerALayerA_True"
                },
                new TestCaseData(layerA, layerB, true)
                {
                    TestName = "Equals_LayerALayerB_True"
                },
                new TestCaseData(layerB, layerD, true)
                {
                    TestName = "Equals_LayerALayerD_True"
                },
                new TestCaseData(layerA, layerD, true)
                {
                    TestName = "Equals_LayerALayerD_True"
                },
                new TestCaseData(layerB, layerC, false)
                {
                    TestName = "Equals_LayerBLayerC_False"
                },
                new TestCaseData(layerA, layerC, false)
                {
                    TestName = "Equals_LayerALayerC_False"
                },
                new TestCaseData(layerC, layerC, true)
                {
                    TestName = "Equals_LayerCLayerC_True"
                },
                new TestCaseData(layerE, layerF, false)
                {
                    TestName = "Equals_DifferentOuterRing_False"
                },
                new TestCaseData(layerE, layerG, false)
                {
                    TestName = "Equals_DifferentHoles_False"
                },
                new TestCaseData(layerE, layerH, false)
                {
                    TestName = "Equals_DifferentProperties_False"
                }
            };
        }

        private static MacroStabilityInwardsSoilLayer2D CreateRandomLayer(int randomSeed)
        {
            var random = new Random(randomSeed);

            return new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(randomSeed),
                                                        Enumerable.Empty<Ring>(),
                                                        new MacroStabilityInwardsSoilLayerData
                                                        {
                                                            Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>())
                                                        },
                                                        new[]
                                                        {
                                                            new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(randomSeed))
                                                        });
        }
    }
}