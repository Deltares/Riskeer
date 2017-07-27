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
using System.Drawing;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayer2DTest
    {
        [Test]
        public void Constructor_WithoutOuterRing_ArgumentNullException()
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
            TestDelegate test= () => new MacroStabilityInwardsSoilLayer2D(null, holes);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("outerRing", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutHoles_ReturnsNewInstance()
        {
            // Setup
            var outerRing = new Ring(new[]
            {
                new Point2D(0, 2),
                new Point2D(2, 2)
            });

            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer2D(outerRing, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("holes", exception.ParamName);
        }

        [Test]
        public void Constructor_WithOuterRingAndHoles_ReturnsNewInstance()
        {
            // Setup
            var outerRing = new Ring(new[]
            {
                new Point2D(0, 2),
                new Point2D(2, 2)
            });
            var holes = new[]
            {
                new Ring(new[]
                {
                    new Point2D(0, 2),
                    new Point2D(2, 2)
                })
            };

            // Call
            var layer = new MacroStabilityInwardsSoilLayer2D(outerRing, holes);

            // Assert
            Assert.NotNull(layer);
            Assert.AreSame(outerRing, layer.OuterRing);
            Assert.AreNotSame(holes, layer.Holes);
            TestHelper.AssertCollectionsAreEqual(holes, layer.Holes, new ReferenceEqualityComparer<Ring>());
            Assert.NotNull(layer.Properties);
        }

        [Test]
        public void GetHashCode_EqualLayers_AreEqual()
        {
            // Setup
            var layerA = CreateRandomLayer(21);
            var layerB = CreateRandomLayer(21);

            // Precondition
            Assert.AreEqual(layerA, layerB);
            Assert.AreEqual(layerB, layerA);

            // Call & Assert
            Assert.AreEqual(layerA.GetHashCode(), layerB.GetHashCode());
            Assert.AreEqual(layerB.GetHashCode(), layerA.GetHashCode());
        }

        [Test]
        public void Equals_DerivedClassWithEqualProperties_ReturnsTrue()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D layer = CreateRandomLayer(2);
            var derivedProfile = new TestLayer(layer);

            // Call
            bool areEqual = layer.Equals(derivedProfile);

            // Assert
            Assert.IsTrue(areEqual);
        }

        private class TestLayer : MacroStabilityInwardsSoilLayer2D
        {
            public TestLayer(MacroStabilityInwardsSoilLayer2D layer)
                : base(layer.OuterRing, layer.Holes)
            {
                Properties.IsAquifer = layer.Properties.IsAquifer;
                Properties.Color = layer.Properties.Color;
                Properties.MaterialName = layer.Properties.MaterialName;
                Properties.ShearStrengthModel = layer.Properties.ShearStrengthModel;
                Properties.AbovePhreaticLevelMean = layer.Properties.AbovePhreaticLevelMean;
                Properties.AbovePhreaticLevelDeviation = layer.Properties.AbovePhreaticLevelDeviation;
                Properties.BelowPhreaticLevelMean = layer.Properties.BelowPhreaticLevelMean;
                Properties.BelowPhreaticLevelDeviation = layer.Properties.BelowPhreaticLevelDeviation;
                Properties.CohesionMean = layer.Properties.CohesionMean;
                Properties.CohesionDeviation = layer.Properties.CohesionDeviation;
                Properties.CohesionShift = layer.Properties.CohesionShift;
                Properties.FrictionAngleMean = layer.Properties.FrictionAngleMean;
                Properties.FrictionAngleDeviation = layer.Properties.FrictionAngleDeviation;
                Properties.FrictionAngleShift = layer.Properties.FrictionAngleShift;
                Properties.StrengthIncreaseExponentMean = layer.Properties.StrengthIncreaseExponentMean;
                Properties.StrengthIncreaseExponentDeviation = layer.Properties.StrengthIncreaseExponentDeviation;
                Properties.StrengthIncreaseExponentShift = layer.Properties.StrengthIncreaseExponentShift;
                Properties.ShearStrengthRatioMean = layer.Properties.ShearStrengthRatioMean;
                Properties.ShearStrengthRatioDeviation = layer.Properties.ShearStrengthRatioDeviation;
                Properties.ShearStrengthRatioShift = layer.Properties.ShearStrengthRatioShift;
                Properties.PopMean = layer.Properties.PopMean;
                Properties.PopDeviation = layer.Properties.PopDeviation;
                Properties.PopShift = layer.Properties.PopShift;
            }
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
                new TestCaseData(layerB, layerC, false)
                {
                    TestName = "Equals_LayerBLayerC_False"
                },
                new TestCaseData(layerC, layerC, true)
                {
                    TestName = "Equals_LayerCLayerC_True"
                }
            };
        }

        private static MacroStabilityInwardsSoilLayer2D CreateRandomLayer(int randomSeed)
        {
            var random = new Random(randomSeed);
            return new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            }), new[]
            {
                new Ring(new[]
                {
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble())
                })
            })
            {
                Properties =
                {
                    Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>())
                }
            };
        }
    }
}