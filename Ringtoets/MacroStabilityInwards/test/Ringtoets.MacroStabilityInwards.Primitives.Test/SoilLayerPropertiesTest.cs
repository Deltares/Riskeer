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

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class SoilLayerPropertiesTest
    {
        [Test]
        public void DefaultConstructor_DefaultValuesSet()
        {
            // Call
            var properties = new SoilLayerProperties();

            // Assert
            Assert.IsFalse(properties.IsAquifer);
            Assert.IsEmpty(properties.MaterialName);
            Assert.AreEqual(Color.Empty, properties.Color);

            Assert.IsFalse(properties.UsePop);
            Assert.AreEqual(ShearStrengthModel.None, properties.ShearStrengthModel);

            Assert.IsNaN(properties.AbovePhreaticLevelMean);
            Assert.IsNaN(properties.AbovePhreaticLevelDeviation);

            Assert.IsNaN(properties.BelowPhreaticLevelMean);
            Assert.IsNaN(properties.BelowPhreaticLevelDeviation);

            Assert.IsNaN(properties.CohesionMean);
            Assert.IsNaN(properties.CohesionDeviation);
            Assert.IsNaN(properties.CohesionShift);

            Assert.IsNaN(properties.FrictionAngleMean);
            Assert.IsNaN(properties.FrictionAngleDeviation);
            Assert.IsNaN(properties.FrictionAngleShift);

            Assert.IsNaN(properties.ShearStrengthRatioMean);
            Assert.IsNaN(properties.ShearStrengthRatioDeviation);
            Assert.IsNaN(properties.ShearStrengthRatioShift);

            Assert.IsNaN(properties.StrengthIncreaseExponentMean);
            Assert.IsNaN(properties.StrengthIncreaseExponentDeviation);
            Assert.IsNaN(properties.StrengthIncreaseExponentShift);

            Assert.IsNaN(properties.PopMean);
            Assert.IsNaN(properties.PopDeviation);
            Assert.IsNaN(properties.PopShift);
        }

        [Test]
        public void MaterialName_Null_ThrowsArgumentNullException()
        {
            // Setup
            double top = new Random(22).NextDouble();
            var layer = new MacroStabilityInwardsSoilLayer1D(top);

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
            var layer = new MacroStabilityInwardsSoilLayer1D(top);

            // Call
            layer.MaterialName = materialName;

            // Assert
            Assert.AreEqual(materialName, layer.MaterialName);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            SoilLayerProperties layer = CreateRandomProperties(21);

            // Call
            bool areEqual = layer.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        private static IEnumerable<TestCaseData> ChangeSingleProperties()
        {
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.ShearStrengthModel = (ShearStrengthModel) 9));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.MaterialName = "interesting"));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.IsAquifer = !lp.IsAquifer));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.UsePop = !lp.IsAquifer));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.Color = lp.Color == Color.Aqua ? Color.Bisque : Color.Aqua));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.AbovePhreaticLevelMean = 1.0 - lp.AbovePhreaticLevelMean));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.AbovePhreaticLevelDeviation = 1.0 - lp.AbovePhreaticLevelDeviation));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.BelowPhreaticLevelMean = 1.0 - lp.BelowPhreaticLevelMean));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.BelowPhreaticLevelDeviation = 1.0 - lp.BelowPhreaticLevelDeviation));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.CohesionMean = 1.0 - lp.CohesionMean));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.CohesionDeviation = 1.0 - lp.CohesionDeviation));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.CohesionShift = 1.0 - lp.CohesionShift));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.FrictionAngleMean = 1.0 - lp.FrictionAngleMean));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.FrictionAngleDeviation = 1.0 - lp.FrictionAngleDeviation));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.FrictionAngleShift = 1.0 - lp.FrictionAngleShift));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.ShearStrengthRatioMean = 1.0 - lp.ShearStrengthRatioMean));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.ShearStrengthRatioDeviation = 1.0 - lp.ShearStrengthRatioDeviation));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.ShearStrengthRatioShift = 1.0 - lp.ShearStrengthRatioShift));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.StrengthIncreaseExponentMean = 1.0 - lp.StrengthIncreaseExponentMean));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.StrengthIncreaseExponentDeviation = 1.0 - lp.StrengthIncreaseExponentDeviation));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.StrengthIncreaseExponentShift = 1.0 - lp.StrengthIncreaseExponentShift));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.PopMean = 1.0 - lp.PopMean));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.PopDeviation = 1.0 - lp.PopDeviation));
            yield return new TestCaseData(new Action<SoilLayerProperties>(lp => lp.PopShift = 1.0 - lp.PopShift));
        }

        [Test]
        [TestCaseSource(nameof(ChangeSingleProperties))]
        public void Equals_ChangeSingleProperty_ReturnsFalse(Action<SoilLayerProperties> changeProperty)
        {
            // Setup
            SoilLayerProperties layer = CreateRandomProperties(21);
            SoilLayerProperties layerToChange = CreateRandomProperties(21);

            changeProperty(layerToChange);

            // Call
            bool areEqualOne = layer.Equals(layerToChange);
            bool areEqualTwo = layerToChange.Equals(layer);

            // Assert
            Assert.IsFalse(areEqualOne);
            Assert.IsFalse(areEqualTwo);
        }

        [Test]
        [TestCaseSource(nameof(PropertiesCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(SoilLayerProperties layer, SoilLayerProperties otherLayer, bool expectedEqual)
        {
            // Call
            bool areEqualOne = layer.Equals(otherLayer);
            bool areEqualTwo = otherLayer.Equals(layer);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        private static TestCaseData[] PropertiesCombinations()
        {
            SoilLayerProperties propertiesA = CreateRandomProperties(21);
            SoilLayerProperties propertiesB = CreateRandomProperties(21);
            SoilLayerProperties propertiesC = CreateRandomProperties(73);

            return new[]
            {
                new TestCaseData(propertiesA, propertiesA, true)
                {
                    TestName = "Equals_LayerALayerA_True"
                },
                new TestCaseData(propertiesA, propertiesB, true)
                {
                    TestName = "Equals_LayerALayerB_True"
                },
                new TestCaseData(propertiesB, propertiesC, false)
                {
                    TestName = "Equals_LayerBLayerC_False"
                },
                new TestCaseData(propertiesC, propertiesC, true)
                {
                    TestName = "Equals_LayerCLayerC_True"
                }
            };
        }

        private static SoilLayerProperties CreateRandomProperties(int randomSeed)
        {
            var random = new Random(randomSeed);
            return new SoilLayerProperties
            {
                MaterialName = string.Join("", Enumerable.Repeat('x', random.Next(0, 40))),
                Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                IsAquifer = random.NextBoolean(),
                UsePop = random.NextBoolean(),
                ShearStrengthModel = random.NextEnumValue<ShearStrengthModel>(),
                AbovePhreaticLevelMean = random.NextDouble(),
                AbovePhreaticLevelDeviation = random.NextDouble(),
                BelowPhreaticLevelMean = random.NextDouble(),
                BelowPhreaticLevelDeviation = random.NextDouble(),
                CohesionMean = random.NextDouble(),
                CohesionDeviation = random.NextDouble(),
                CohesionShift = random.NextDouble(),
                FrictionAngleMean = random.NextDouble(),
                FrictionAngleDeviation = random.NextDouble(),
                FrictionAngleShift = random.NextDouble(),
                ShearStrengthRatioMean = random.NextDouble(),
                ShearStrengthRatioDeviation = random.NextDouble(),
                ShearStrengthRatioShift = random.NextDouble(),
                StrengthIncreaseExponentMean = random.NextDouble(),
                StrengthIncreaseExponentDeviation = random.NextDouble(),
                StrengthIncreaseExponentShift = random.NextDouble(),
                PopMean = random.NextDouble(),
                PopDeviation = random.NextDouble(),
                PopShift = random.NextDouble()
            };
        }
    }
}