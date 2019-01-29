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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Piping.Primitives.Test
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

            DistributionAssert.AreEqual(new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN,
                Shift = RoundedDouble.NaN
            }, layer.BelowPhreaticLevel);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(6)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, layer.DiameterD70);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(6)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, layer.Permeability);
        }

        [Test]
        public void BelowPhreaticLevel_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var layer = new PipingSoilLayer(random.NextDouble());

            var distributionToSet = new LogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            layer.BelowPhreaticLevel = distributionToSet;

            // Assert
            var expectedDistribution = new LogNormalDistribution(2)
            {
                Mean = distributionToSet.Mean,
                StandardDeviation = distributionToSet.StandardDeviation,
                Shift = distributionToSet.Shift
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(layer.BelowPhreaticLevel, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Diameter70_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var layer = new PipingSoilLayer(random.NextDouble());

            var distributionToSet = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            layer.DiameterD70 = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(6)
            {
                Mean = distributionToSet.Mean,
                CoefficientOfVariation = distributionToSet.CoefficientOfVariation,
                Shift = layer.DiameterD70.Shift
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(layer.DiameterD70, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Permeability_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var layer = new PipingSoilLayer(random.NextDouble());

            var distributionToSet = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            layer.Permeability = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(6)
            {
                Mean = distributionToSet.Mean,
                CoefficientOfVariation = distributionToSet.CoefficientOfVariation,
                Shift = layer.DiameterD70.Shift
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(layer.Permeability, distributionToSet, expectedDistribution);
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
        private class PipingSoilLayerEqualsTest : EqualsTestFixture<PipingSoilLayer, TestLayer>
        {
            private const int seed = 21;

            protected override PipingSoilLayer CreateObject()
            {
                return CreateRandomLayer(seed);
            }

            protected override TestLayer CreateDerivedObject()
            {
                PipingSoilLayer baseLayer = CreateRandomLayer(seed);
                return new TestLayer(baseLayer);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                PipingSoilLayer baseLayer = CreateRandomLayer(seed);

                var random = new Random(seed);
                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = "Different Name",
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevel = baseLayer.BelowPhreaticLevel,
                    DiameterD70 = baseLayer.DiameterD70,
                    Permeability = baseLayer.Permeability
                }).SetName("Name");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color.ToArgb().Equals(Color.Aqua.ToArgb()) ? Color.Bisque : Color.Aqua,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevel = baseLayer.BelowPhreaticLevel,
                    DiameterD70 = baseLayer.DiameterD70,
                    Permeability = baseLayer.Permeability
                }).SetName("Color");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = !baseLayer.IsAquifer,
                    BelowPhreaticLevel = baseLayer.BelowPhreaticLevel,
                    DiameterD70 = baseLayer.DiameterD70,
                    Permeability = baseLayer.Permeability
                }).SetName("IsAquifer");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevel = new LogNormalDistribution
                    {
                        Mean = baseLayer.BelowPhreaticLevel.Mean,
                        StandardDeviation = baseLayer.BelowPhreaticLevel.StandardDeviation + random.NextRoundedDouble(),
                        Shift = baseLayer.BelowPhreaticLevel.Shift
                    },
                    DiameterD70 = baseLayer.DiameterD70,
                    Permeability = baseLayer.Permeability
                }).SetName("BelowPhreaticLevelDeviation");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevel = new LogNormalDistribution
                    {
                        Mean = baseLayer.BelowPhreaticLevel.Mean + random.NextRoundedDouble(),
                        StandardDeviation = baseLayer.BelowPhreaticLevel.StandardDeviation,
                        Shift = baseLayer.BelowPhreaticLevel.Shift
                    },
                    DiameterD70 = baseLayer.DiameterD70,
                    Permeability = baseLayer.Permeability
                }).SetName("BelowPhreaticLevelMean");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevel = new LogNormalDistribution
                    {
                        Mean = baseLayer.BelowPhreaticLevel.Mean,
                        StandardDeviation = baseLayer.BelowPhreaticLevel.StandardDeviation,
                        Shift = baseLayer.BelowPhreaticLevel.Shift + random.NextRoundedDouble()
                    },
                    DiameterD70 = baseLayer.DiameterD70,
                    Permeability = baseLayer.Permeability
                }).SetName("BelowPhreaticLevelShift");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevel = baseLayer.BelowPhreaticLevel,
                    DiameterD70 = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = baseLayer.DiameterD70.Mean,
                        CoefficientOfVariation = baseLayer.DiameterD70.CoefficientOfVariation + random.NextRoundedDouble()
                    },
                    Permeability = baseLayer.Permeability
                }).SetName("DiameterD70CoefficientOfVariation");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevel = baseLayer.BelowPhreaticLevel,
                    DiameterD70 = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = baseLayer.DiameterD70.Mean + random.NextRoundedDouble(),
                        CoefficientOfVariation = baseLayer.DiameterD70.CoefficientOfVariation
                    },
                    Permeability = baseLayer.Permeability
                }).SetName("DiameterD70Mean");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevel = baseLayer.BelowPhreaticLevel,
                    DiameterD70 = baseLayer.DiameterD70,
                    Permeability = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = baseLayer.Permeability.Mean,
                        CoefficientOfVariation = baseLayer.Permeability.CoefficientOfVariation + random.NextRoundedDouble()
                    }
                }).SetName("PermeabilityCoefficientOfVariation");

                yield return new TestCaseData(new PipingSoilLayer(double.NaN)
                {
                    MaterialName = baseLayer.MaterialName,
                    Color = baseLayer.Color,
                    IsAquifer = baseLayer.IsAquifer,
                    BelowPhreaticLevel = baseLayer.BelowPhreaticLevel,
                    DiameterD70 = baseLayer.DiameterD70,
                    Permeability = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = baseLayer.Permeability.Mean + random.NextRoundedDouble(),
                        CoefficientOfVariation = baseLayer.Permeability.CoefficientOfVariation
                    }
                }).SetName("PermeabilityMean");
            }

            private static PipingSoilLayer CreateRandomLayer(int randomSeed)
            {
                var random = new Random(randomSeed);
                return new PipingSoilLayer(random.NextDouble())
                {
                    MaterialName = string.Join("", Enumerable.Repeat('x', random.Next(0, 40))),
                    Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                    IsAquifer = random.NextBoolean(),
                    BelowPhreaticLevel = new LogNormalDistribution
                    {
                        Mean = random.NextRoundedDouble(1, double.MaxValue),
                        StandardDeviation = random.NextRoundedDouble(),
                        Shift = random.NextRoundedDouble()
                    },
                    DiameterD70 = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    Permeability = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    }
                };
            }
        }

        private class TestLayer : PipingSoilLayer
        {
            public TestLayer(PipingSoilLayer layer)
                : base(layer.Top)
            {
                IsAquifer = layer.IsAquifer;
                MaterialName = layer.MaterialName;
                Color = layer.Color;
                BelowPhreaticLevel = layer.BelowPhreaticLevel;
                DiameterD70 = layer.DiameterD70;
                Permeability = layer.Permeability;
            }
        }
    }
}