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

        [TestFixture]
        private class PipingSoilLayerEqualsGuideLines : EqualsGuidelinesTestFixture<PipingSoilLayer, TestLayer>
        {
            private const int seed = 21;

            protected override PipingSoilLayer CreateObject()
            {
                return CreateRandomLayer(seed);
            }

            protected override TestLayer CreateDerivedObject()
            {
                PipingSoilLayer baseLayer = CreateRandomLayer(21);
                return new TestLayer(baseLayer);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                PipingSoilLayer baseLayer = CreateRandomLayer(seed);

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = "Different Name",
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevelDeviation = baseLayer.BelowPhreaticLevelDeviation,
                    BelowPhreaticLevelMean = baseLayer.BelowPhreaticLevelMean,
                    BelowPhreaticLevelShift = baseLayer.BelowPhreaticLevelShift,
                    DiameterD70CoefficientOfVariation = baseLayer.DiameterD70CoefficientOfVariation,
                    DiameterD70Mean = baseLayer.DiameterD70Mean,
                    PermeabilityCoefficientOfVariation = baseLayer.PermeabilityCoefficientOfVariation,
                    PermeabilityMean = baseLayer.PermeabilityMean
                }).SetName("Name");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color.ToArgb().Equals(Color.Aqua.ToArgb()) ? Color.Bisque : Color.Aqua,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevelDeviation = baseLayer.BelowPhreaticLevelDeviation,
                    BelowPhreaticLevelMean = baseLayer.BelowPhreaticLevelMean,
                    BelowPhreaticLevelShift = baseLayer.BelowPhreaticLevelShift,
                    DiameterD70CoefficientOfVariation = baseLayer.DiameterD70CoefficientOfVariation,
                    DiameterD70Mean = baseLayer.DiameterD70Mean,
                    PermeabilityCoefficientOfVariation = baseLayer.PermeabilityCoefficientOfVariation,
                    PermeabilityMean = baseLayer.PermeabilityMean
                }).SetName("Color");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = !baseLayer.IsAquifer,
                    BelowPhreaticLevelDeviation = baseLayer.BelowPhreaticLevelDeviation,
                    BelowPhreaticLevelMean = baseLayer.BelowPhreaticLevelMean,
                    BelowPhreaticLevelShift = baseLayer.BelowPhreaticLevelShift,
                    DiameterD70CoefficientOfVariation = baseLayer.DiameterD70CoefficientOfVariation,
                    DiameterD70Mean = baseLayer.DiameterD70Mean,
                    PermeabilityCoefficientOfVariation = baseLayer.PermeabilityCoefficientOfVariation,
                    PermeabilityMean = baseLayer.PermeabilityMean
                }).SetName("IsAquifer");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevelDeviation = baseLayer.BelowPhreaticLevelDeviation + 10,
                    BelowPhreaticLevelMean = baseLayer.BelowPhreaticLevelMean,
                    BelowPhreaticLevelShift = baseLayer.BelowPhreaticLevelShift,
                    DiameterD70CoefficientOfVariation = baseLayer.DiameterD70CoefficientOfVariation,
                    DiameterD70Mean = baseLayer.DiameterD70Mean,
                    PermeabilityCoefficientOfVariation = baseLayer.PermeabilityCoefficientOfVariation,
                    PermeabilityMean = baseLayer.PermeabilityMean
                }).SetName("BelowPhreaticLevelDeviation");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevelDeviation = baseLayer.BelowPhreaticLevelDeviation,
                    BelowPhreaticLevelMean = baseLayer.BelowPhreaticLevelMean + 10,
                    BelowPhreaticLevelShift = baseLayer.BelowPhreaticLevelShift,
                    DiameterD70CoefficientOfVariation = baseLayer.DiameterD70CoefficientOfVariation,
                    DiameterD70Mean = baseLayer.DiameterD70Mean,
                    PermeabilityCoefficientOfVariation = baseLayer.PermeabilityCoefficientOfVariation,
                    PermeabilityMean = baseLayer.PermeabilityMean
                }).SetName("BelowPhreaticLevelMean");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevelDeviation = baseLayer.BelowPhreaticLevelDeviation,
                    BelowPhreaticLevelMean = baseLayer.BelowPhreaticLevelMean,
                    BelowPhreaticLevelShift = baseLayer.BelowPhreaticLevelShift + 10,
                    DiameterD70CoefficientOfVariation = baseLayer.DiameterD70CoefficientOfVariation,
                    DiameterD70Mean = baseLayer.DiameterD70Mean,
                    PermeabilityCoefficientOfVariation = baseLayer.PermeabilityCoefficientOfVariation,
                    PermeabilityMean = baseLayer.PermeabilityMean
                }).SetName("BelowPhreaticLevelShift");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevelDeviation = baseLayer.BelowPhreaticLevelDeviation,
                    BelowPhreaticLevelMean = baseLayer.BelowPhreaticLevelMean,
                    BelowPhreaticLevelShift = baseLayer.BelowPhreaticLevelShift,
                    DiameterD70CoefficientOfVariation = baseLayer.DiameterD70CoefficientOfVariation + 70,
                    DiameterD70Mean = baseLayer.DiameterD70Mean,
                    PermeabilityCoefficientOfVariation = baseLayer.PermeabilityCoefficientOfVariation,
                    PermeabilityMean = baseLayer.PermeabilityMean
                }).SetName("DiameterD70CoefficientOfVariation");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevelDeviation = baseLayer.BelowPhreaticLevelDeviation,
                    BelowPhreaticLevelMean = baseLayer.BelowPhreaticLevelMean,
                    BelowPhreaticLevelShift = baseLayer.BelowPhreaticLevelShift,
                    DiameterD70CoefficientOfVariation = baseLayer.DiameterD70CoefficientOfVariation,
                    DiameterD70Mean = baseLayer.DiameterD70Mean + 70,
                    PermeabilityCoefficientOfVariation = baseLayer.PermeabilityCoefficientOfVariation,
                    PermeabilityMean = baseLayer.PermeabilityMean
                }).SetName("DiameterD70Mean");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevelDeviation = baseLayer.BelowPhreaticLevelDeviation,
                    BelowPhreaticLevelMean = baseLayer.BelowPhreaticLevelMean,
                    BelowPhreaticLevelShift = baseLayer.BelowPhreaticLevelShift,
                    DiameterD70CoefficientOfVariation = baseLayer.DiameterD70CoefficientOfVariation,
                    DiameterD70Mean = baseLayer.DiameterD70Mean,
                    PermeabilityCoefficientOfVariation = baseLayer.PermeabilityCoefficientOfVariation + 10,
                    PermeabilityMean = baseLayer.PermeabilityMean
                }).SetName("PermeabilityCoefficientOfVariation");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevelDeviation = baseLayer.BelowPhreaticLevelDeviation,
                    BelowPhreaticLevelMean = baseLayer.BelowPhreaticLevelMean,
                    BelowPhreaticLevelShift = baseLayer.BelowPhreaticLevelShift,
                    DiameterD70CoefficientOfVariation = baseLayer.DiameterD70CoefficientOfVariation,
                    DiameterD70Mean = baseLayer.DiameterD70Mean,
                    PermeabilityCoefficientOfVariation = baseLayer.PermeabilityCoefficientOfVariation,
                    PermeabilityMean = baseLayer.PermeabilityMean + 10
                }).SetName("PermeabilityMean");
            }
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