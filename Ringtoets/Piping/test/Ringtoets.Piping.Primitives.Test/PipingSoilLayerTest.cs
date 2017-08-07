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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.Piping.Primitives.Test
{
    [TestFixture]
    public class PipingSoilLayerTest
    {
        [Test]
        public void Constructor_WithTop_ReturnsNewInstanceWithTopSet()
        {
            // Setup
            double top = new Random(22).NextDouble();

            // Call
            var layer = new PipingSoilLayer(top);

            // Assert
            Assert.NotNull(layer);
            Assert.AreEqual(top, layer.Top);
            Assert.IsFalse(layer.IsAquifer);
            Assert.IsEmpty(layer.MaterialName);
            Assert.AreEqual(Color.Empty, layer.Color);

            Assert.IsNaN(layer.BelowPhreaticLevelMean);
            Assert.IsNaN(layer.BelowPhreaticLevelDeviation);
            Assert.IsNaN(layer.BelowPhreaticLevelShift);

            Assert.IsNaN(layer.DiameterD70Mean);
            Assert.IsNaN(layer.DiameterD70CoefficientOfVariation);

            Assert.IsNaN(layer.PermeabilityMean);
            Assert.IsNaN(layer.PermeabilityCoefficientOfVariation);
        }

        [Test]
        public void MaterialName_Null_ThrowsArgumentNullException()
        {
            // Setup
            double top = new Random(22).NextDouble();
            var layer = new PipingSoilLayer(top);

            // Call
            TestDelegate test = () => layer.MaterialName = null;

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase("A name")]
        public void MaterialName_NotNullValue_ValueSet(string materialName)
        {
            // Setup
            double top = new Random(22).NextDouble();
            var layer = new PipingSoilLayer(top);

            // Call
            layer.MaterialName = materialName;

            // Assert
            Assert.AreEqual(materialName, layer.MaterialName);
        }

        [Test]
        public void Equals_DerivedClassWithEqualProperties_ReturnsTrue()
        {
            // Setup
            PipingSoilLayer layer = CreateRandomLayer(2);
            var derivedLayer = new TestLayer(layer);

            // Call
            bool areEqual = layer.Equals(derivedLayer);

            // Assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Setup
            PipingSoilLayer layer = CreateRandomLayer(21);

            // Call
            bool areEqual = layer.Equals(new object());

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            PipingSoilLayer layer = CreateRandomLayer(21);

            // Call
            bool areEqual = layer.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(LayerCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(PipingSoilLayer layer, PipingSoilLayer otherLayer, bool expectedEqual)
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
            PipingSoilLayer layerA = CreateRandomLayer(21);
            PipingSoilLayer layerB = CreateRandomLayer(21);
            PipingSoilLayer layerC = CreateRandomLayer(73);

            PipingSoilLayer layerD = CreateNaNLayer("C", Color.Aqua, true);
            PipingSoilLayer layerE = CreateNaNLayer("C", Color.Aqua, false);
            PipingSoilLayer layerF = CreateNaNLayer("C", Color.AliceBlue, false);
            PipingSoilLayer layerG = CreateNaNLayer("A", Color.Aqua, false);

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
                },
                new TestCaseData(layerD, layerE, false)
                {
                    TestName = "Equals_LayerDLayerE_False"
                },
                new TestCaseData(layerD, layerF, false)
                {
                    TestName = "Equals_LayerDLayerF_False"
                },
                new TestCaseData(layerD, layerG, false)
                {
                    TestName = "Equals_LayerDLayerG_False"
                }
            };
        }

        private static PipingSoilLayer CreateRandomLayer(int randomSeed)
        {
            var random = new Random(randomSeed);
            return new PipingSoilLayer(random.NextDouble())
            {
                MaterialName = string.Join("", Enumerable.Repeat('x', random.Next(0, 40))),
                Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                IsAquifer = random.NextBoolean(),
                BelowPhreaticLevelDeviation = random.NextDouble(),
                BelowPhreaticLevelMean = random.NextDouble(),
                BelowPhreaticLevelShift = random.NextDouble(),
                DiameterD70CoefficientOfVariation = random.NextDouble(),
                DiameterD70Mean = random.NextDouble(),
                PermeabilityCoefficientOfVariation = random.NextDouble(),
                PermeabilityMean = random.NextDouble()
            };
        }

        private static PipingSoilLayer CreateNaNLayer(string name, Color color, bool isAquifer)
        {
            return new PipingSoilLayer(double.NaN)
            {
                MaterialName = name,
                Color = color,
                IsAquifer = isAquifer,
                BelowPhreaticLevelDeviation = double.NaN,
                BelowPhreaticLevelMean = double.NaN,
                BelowPhreaticLevelShift = double.NaN,
                DiameterD70CoefficientOfVariation = double.NaN,
                DiameterD70Mean = double.NaN,
                PermeabilityCoefficientOfVariation = double.NaN,
                PermeabilityMean = double.NaN
            };
        }

        private class TestLayer : PipingSoilLayer
        {
            public TestLayer(PipingSoilLayer layer)
                : base(layer.Top)
            {
                IsAquifer = layer.IsAquifer;
                MaterialName = layer.MaterialName;
                Color = layer.Color;
                BelowPhreaticLevelMean = layer.BelowPhreaticLevelMean;
                BelowPhreaticLevelDeviation = layer.BelowPhreaticLevelDeviation;
                BelowPhreaticLevelShift = layer.BelowPhreaticLevelShift;
                PermeabilityCoefficientOfVariation = layer.PermeabilityCoefficientOfVariation;
                PermeabilityMean = layer.PermeabilityMean;
                DiameterD70Mean = layer.DiameterD70Mean;
                DiameterD70CoefficientOfVariation = layer.DiameterD70CoefficientOfVariation;
            }
        }
    }
}