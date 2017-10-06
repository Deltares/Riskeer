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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayerDataTest
    {
        [Test]
        public void DefaultConstructor_DefaultValuesSet()
        {
            // Call
            var properties = new MacroStabilityInwardsSoilLayerData();

            // Assert
            Assert.IsFalse(properties.IsAquifer);
            Assert.IsEmpty(properties.MaterialName);
            Assert.AreEqual(Color.Empty, properties.Color);

            Assert.IsFalse(properties.UsePop);
            Assert.AreEqual(MacroStabilityInwardsShearStrengthModel.CPhi, properties.ShearStrengthModel);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN,
                Shift = RoundedDouble.NaN
            }, properties.AbovePhreaticLevel);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN,
                Shift = RoundedDouble.NaN
            }, properties.BelowPhreaticLevel);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.Cohesion);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.FrictionAngle);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.ShearStrengthRatio);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.StrengthIncreaseExponent);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.Pop);
        }

        [Test]
        public void MaterialName_Null_ThrowsArgumentNullException()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayerData();

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
            var layer = new MacroStabilityInwardsSoilLayerData();

            // Call
            layer.MaterialName = materialName;

            // Assert
            Assert.AreEqual(materialName, layer.MaterialName);
        }

        [Test]
        public void GetHashCode_EqualProperties_AreEqual()
        {
            // Setup
            MacroStabilityInwardsSoilLayerData dataA = CreateRandomProperties(21);
            MacroStabilityInwardsSoilLayerData dataB = CreateRandomProperties(21);

            // Precondition
            Assert.AreEqual(dataA, dataB);
            Assert.AreEqual(dataB, dataA);

            // Call & Assert
            Assert.AreEqual(dataA.GetHashCode(), dataB.GetHashCode());
            Assert.AreEqual(dataB.GetHashCode(), dataA.GetHashCode());
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Setup
            MacroStabilityInwardsSoilLayerData layer = CreateRandomProperties(21);

            // Call
            bool areEqual = layer.Equals(new object());

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            MacroStabilityInwardsSoilLayerData layer = CreateRandomProperties(21);

            // Call
            bool areEqual = layer.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(ChangeSingleProperties))]
        public void Equals_ChangeSingleProperty_ReturnsFalse(Action<MacroStabilityInwardsSoilLayerData> changeProperty)
        {
            // Setup
            MacroStabilityInwardsSoilLayerData layer = CreateRandomProperties(21);
            MacroStabilityInwardsSoilLayerData layerToChange = CreateRandomProperties(21);

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
        public void Equals_DifferentScenarios_ReturnsExpectedResult(MacroStabilityInwardsSoilLayerData layer, MacroStabilityInwardsSoilLayerData otherLayer, bool expectedEqual)
        {
            // Call
            bool areEqualOne = layer.Equals(otherLayer);
            bool areEqualTwo = otherLayer.Equals(layer);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        private static IEnumerable<TestCaseData> ChangeSingleProperties()
        {
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.ShearStrengthModel = (MacroStabilityInwardsShearStrengthModel) 9));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.MaterialName = "interesting"));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.IsAquifer = !lp.IsAquifer));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.UsePop = !lp.UsePop));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.Color = lp.Color.ToArgb().Equals(Color.Aqua.ToArgb()) ? Color.Bisque : Color.Aqua));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.AbovePhreaticLevel.Mean = (RoundedDouble) (11.0 - lp.AbovePhreaticLevel.Mean)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.AbovePhreaticLevel.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.AbovePhreaticLevel.CoefficientOfVariation)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.AbovePhreaticLevel.Shift = (RoundedDouble) (1.0 - lp.AbovePhreaticLevel.Shift)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.BelowPhreaticLevel.Mean = (RoundedDouble) (12.0 - lp.BelowPhreaticLevel.Mean)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.BelowPhreaticLevel.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.BelowPhreaticLevel.CoefficientOfVariation)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.BelowPhreaticLevel.Shift = (RoundedDouble) (1.0 - lp.BelowPhreaticLevel.Shift)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.Cohesion.Mean = (RoundedDouble) (11.0 - lp.Cohesion.Mean)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.Cohesion.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.Cohesion.CoefficientOfVariation)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.FrictionAngle.Mean = (RoundedDouble) (11.0 - lp.FrictionAngle.Mean)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.FrictionAngle.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.FrictionAngle.CoefficientOfVariation)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.ShearStrengthRatio.Mean = (RoundedDouble) (11.0 - lp.ShearStrengthRatio.Mean)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.ShearStrengthRatio.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.ShearStrengthRatio.CoefficientOfVariation)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.StrengthIncreaseExponent.Mean = (RoundedDouble) (11.0 - lp.StrengthIncreaseExponent.Mean)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.StrengthIncreaseExponent.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.StrengthIncreaseExponent.CoefficientOfVariation)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.Pop.Mean = (RoundedDouble) (11.0 - lp.Pop.Mean)));
            yield return new TestCaseData(new Action<MacroStabilityInwardsSoilLayerData>(lp => lp.Pop.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.Pop.CoefficientOfVariation)));
        }

        private static TestCaseData[] PropertiesCombinations()
        {
            MacroStabilityInwardsSoilLayerData dataA = CreateRandomProperties(21);
            MacroStabilityInwardsSoilLayerData dataB = CreateRandomProperties(21);
            MacroStabilityInwardsSoilLayerData dataC = CreateRandomProperties(73);
            MacroStabilityInwardsSoilLayerData dataD = CreateRandomProperties(21);

            return new[]
            {
                new TestCaseData(dataA, dataA, true)
                {
                    TestName = "Equals_LayerALayerA_True"
                },
                new TestCaseData(dataA, dataB, true)
                {
                    TestName = "Equals_LayerALayerB_True"
                },
                new TestCaseData(dataB, dataD, true)
                {
                    TestName = "Equals_LayerBLayerD_True"
                },
                new TestCaseData(dataA, dataD, true)
                {
                    TestName = "Equals_LayerALayerD_True"
                },
                new TestCaseData(dataB, dataC, false)
                {
                    TestName = "Equals_LayerBLayerC_False"
                },
                new TestCaseData(dataA, dataC, false)
                {
                    TestName = "Equals_LayerALayerC_False"
                }
            };
        }

        private static MacroStabilityInwardsSoilLayerData CreateRandomProperties(int randomSeed)
        {
            var random = new Random(randomSeed);
            return new MacroStabilityInwardsSoilLayerData
            {
                MaterialName = string.Join("", Enumerable.Repeat('x', random.Next(0, 40))),
                Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                IsAquifer = random.NextBoolean(),
                UsePop = random.NextBoolean(),
                ShearStrengthModel = random.NextEnumValue<MacroStabilityInwardsShearStrengthModel>(),
                AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 10,
                    CoefficientOfVariation = (RoundedDouble) 0.2,
                    Shift = (RoundedDouble) 1
                },
                BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 11,
                    CoefficientOfVariation = (RoundedDouble) 0.6,
                    Shift = (RoundedDouble) 1
                },
                Cohesion = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 10,
                    CoefficientOfVariation = (RoundedDouble) 0.2
                },
                FrictionAngle = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 10,
                    CoefficientOfVariation = (RoundedDouble) 0.2
                },
                ShearStrengthRatio = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 10,
                    CoefficientOfVariation = (RoundedDouble) 0.2
                },
                StrengthIncreaseExponent = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 10,
                    CoefficientOfVariation = (RoundedDouble) 0.2
                },
                Pop = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 10,
                    CoefficientOfVariation = (RoundedDouble) 0.2
                }
            };
        }
    }
}