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

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayerTest
    {
        [Test]
        public void Constructor_WithTop_ReturnsNewInstanceWithTopSet()
        {
            // Setup
            double top = new Random(22).NextDouble();

            // Call
            var layer = new MacroStabilityInwardsSoilLayer(top);

            // Assert
            Assert.NotNull(layer);
            Assert.AreEqual(top, layer.Top);
            Assert.IsFalse(layer.IsAquifer);
            Assert.IsEmpty(layer.MaterialName);
            Assert.AreEqual(Color.Empty, layer.Color);

            Assert.IsFalse(layer.UsePop);
            Assert.AreEqual(ShearStrengthModel.None, layer.ShearStrengthModel);

            Assert.IsNaN(layer.AbovePhreaticLevelMean);
            Assert.IsNaN(layer.AbovePhreaticLevelDeviation);

            Assert.IsNaN(layer.BelowPhreaticLevelMean);
            Assert.IsNaN(layer.BelowPhreaticLevelDeviation);

            Assert.IsNaN(layer.CohesionMean);
            Assert.IsNaN(layer.CohesionDeviation);
            Assert.IsNaN(layer.CohesionShift);

            Assert.IsNaN(layer.FrictionAngleMean);
            Assert.IsNaN(layer.FrictionAngleDeviation);
            Assert.IsNaN(layer.FrictionAngleShift);

            Assert.IsNaN(layer.ShearStrengthRatioMean);
            Assert.IsNaN(layer.ShearStrengthRatioDeviation);
            Assert.IsNaN(layer.ShearStrengthRatioShift);

            Assert.IsNaN(layer.StrengthIncreaseExponentMean);
            Assert.IsNaN(layer.StrengthIncreaseExponentDeviation);
            Assert.IsNaN(layer.StrengthIncreaseExponentShift);

            Assert.IsNaN(layer.PopMean);
            Assert.IsNaN(layer.PopDeviation);
            Assert.IsNaN(layer.PopShift);
        }

        [Test]
        public void MaterialName_Null_ThrowsArgumentNullException()
        {
            // Setup
            double top = new Random(22).NextDouble();
            var layer = new MacroStabilityInwardsSoilLayer(top);

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
            var layer = new MacroStabilityInwardsSoilLayer(top);

            // Call
            layer.MaterialName = materialName;

            // Assert
            Assert.AreEqual(materialName, layer.MaterialName);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            MacroStabilityInwardsSoilLayer layer = CreateRandomLayer(21);

            // Call
            bool areEqual = layer.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(LayerCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(MacroStabilityInwardsSoilLayer layer, MacroStabilityInwardsSoilLayer otherLayer, bool expectedEqual)
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
            MacroStabilityInwardsSoilLayer layerA = CreateRandomLayer(21);
            MacroStabilityInwardsSoilLayer layerB = CreateRandomLayer(21);
            MacroStabilityInwardsSoilLayer layerC = CreateRandomLayer(73);

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

        private static MacroStabilityInwardsSoilLayer CreateRandomLayer(int randomSeed)
        {
            var random = new Random(randomSeed);
            return new MacroStabilityInwardsSoilLayer(random.NextDouble())
            {
                MaterialName = string.Join("", Enumerable.Repeat('x', random.Next(0, 40))),
                Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                IsAquifer = random.NextBoolean()
            };
        }
    }
}