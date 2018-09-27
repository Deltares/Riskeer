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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayerDataTest
    {
        [Test]
        public void DefaultConstructor_DefaultValuesSet()
        {
            // Call
            var data = new MacroStabilityInwardsSoilLayerData();

            // Assert
            Assert.IsFalse(data.IsAquifer);
            Assert.IsEmpty(data.MaterialName);
            Assert.AreEqual(Color.Empty, data.Color);

            Assert.IsFalse(data.UsePop);
            Assert.AreEqual(MacroStabilityInwardsShearStrengthModel.CPhi, data.ShearStrengthModel);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN,
                Shift = RoundedDouble.NaN
            }, data.AbovePhreaticLevel);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN,
                Shift = RoundedDouble.NaN
            }, data.BelowPhreaticLevel);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, data.Cohesion);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, data.FrictionAngle);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, data.ShearStrengthRatio);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, data.StrengthIncreaseExponent);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, data.Pop);
        }

        [Test]
        public void MaterialName_Null_ThrowsArgumentNullException()
        {
            // Setup
            var data = new MacroStabilityInwardsSoilLayerData();

            // Call
            TestDelegate test = () => data.MaterialName = null;

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
            var data = new MacroStabilityInwardsSoilLayerData();

            // Call
            data.MaterialName = materialName;

            // Assert
            Assert.AreEqual(materialName, data.MaterialName);
        }

        [Test]
        public void AbovePhreaticLevel_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var data = new MacroStabilityInwardsSoilLayerData();

            var distributionToSet = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            data.AbovePhreaticLevel = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = distributionToSet.Mean,
                CoefficientOfVariation = distributionToSet.CoefficientOfVariation,
                Shift = distributionToSet.Shift
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(data.AbovePhreaticLevel, distributionToSet, expectedDistribution);
        }

        [Test]
        public void BelowPhreaticLevel_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var data = new MacroStabilityInwardsSoilLayerData();

            var distributionToSet = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            data.BelowPhreaticLevel = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = distributionToSet.Mean,
                CoefficientOfVariation = distributionToSet.CoefficientOfVariation,
                Shift = distributionToSet.Shift
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(data.BelowPhreaticLevel, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Cohesion_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var data = new MacroStabilityInwardsSoilLayerData();

            var distributionToSet = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            data.Cohesion = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = distributionToSet.Mean,
                CoefficientOfVariation = distributionToSet.CoefficientOfVariation,
                Shift = data.Cohesion.Shift
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(data.Cohesion, distributionToSet, expectedDistribution);
        }

        [Test]
        public void FrictionAngle_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var data = new MacroStabilityInwardsSoilLayerData();

            var distributionToSet = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            data.FrictionAngle = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = distributionToSet.Mean,
                CoefficientOfVariation = distributionToSet.CoefficientOfVariation,
                Shift = data.FrictionAngle.Shift
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(data.FrictionAngle, distributionToSet, expectedDistribution);
        }

        [Test]
        public void StrengthIncreaseExponent_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var data = new MacroStabilityInwardsSoilLayerData();

            var distributionToSet = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            data.StrengthIncreaseExponent = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = distributionToSet.Mean,
                CoefficientOfVariation = distributionToSet.CoefficientOfVariation,
                Shift = data.StrengthIncreaseExponent.Shift
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(data.StrengthIncreaseExponent, distributionToSet, expectedDistribution);
        }

        [Test]
        public void ShearStrengthRatio_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var data = new MacroStabilityInwardsSoilLayerData();

            var distributionToSet = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            data.ShearStrengthRatio = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = distributionToSet.Mean,
                CoefficientOfVariation = distributionToSet.CoefficientOfVariation,
                Shift = data.StrengthIncreaseExponent.Shift
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(data.ShearStrengthRatio, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Pop_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var data = new MacroStabilityInwardsSoilLayerData();

            var distributionToSet = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            data.Pop = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = distributionToSet.Mean,
                CoefficientOfVariation = distributionToSet.CoefficientOfVariation,
                Shift = data.Pop.Shift
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(data.Pop, distributionToSet, expectedDistribution);
        }

        [TestFixture]
        private class MacroStabilityInwardsSoilLayerDataEqualsTest
            : EqualsTestFixture<MacroStabilityInwardsSoilLayerData, DerivedMacroStabilityInwardsSoilLayerData>
        {
            protected override MacroStabilityInwardsSoilLayerData CreateObject()
            {
                return CreateRandomData(21);
            }

            protected override DerivedMacroStabilityInwardsSoilLayerData CreateDerivedObject()
            {
                return new DerivedMacroStabilityInwardsSoilLayerData(CreateRandomData(21));
            }

            private static MacroStabilityInwardsSoilLayerData CreateRandomData(int randomSeed)
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

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                foreach (ChangePropertyData<MacroStabilityInwardsSoilLayerData> changeSingleDataProperty in ChangeSingleDataProperties())
                {
                    MacroStabilityInwardsSoilLayerData differentSoilLayerData = CreateRandomData(21);
                    changeSingleDataProperty.ActionToChangeProperty(differentSoilLayerData);
                    yield return new TestCaseData(differentSoilLayerData).SetName(changeSingleDataProperty.PropertyName);
                }
            }

            private static IEnumerable<ChangePropertyData<MacroStabilityInwardsSoilLayerData>> ChangeSingleDataProperties()
            {
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.ShearStrengthModel = (MacroStabilityInwardsShearStrengthModel) 9,
                                                                                        "ShearStrengthModel");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.MaterialName = "interesting",
                                                                                        "MaterialName");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.IsAquifer = !lp.IsAquifer,
                                                                                        "IsAquifer");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.UsePop = !lp.UsePop,
                                                                                        "UsePoP");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.Color = lp.Color.ToArgb().Equals(Color.Aqua.ToArgb()) ? Color.Bisque : Color.Aqua,
                                                                                        "Color");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.AbovePhreaticLevel.Mean = (RoundedDouble) (11.0 - lp.AbovePhreaticLevel.Mean),
                                                                                        "AbovePhreaticLevelMean");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.AbovePhreaticLevel.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.AbovePhreaticLevel.CoefficientOfVariation),
                                                                                        "AbovePhreaticLevelCoefficientOfVariation");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.AbovePhreaticLevel.Shift = (RoundedDouble) (1.0 - lp.AbovePhreaticLevel.Shift),
                                                                                        "AbovePhreaticLevelShift");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.BelowPhreaticLevel.Mean = (RoundedDouble) (12.0 - lp.BelowPhreaticLevel.Mean),
                                                                                        "BelowPhreaticLevelMean");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.BelowPhreaticLevel.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.BelowPhreaticLevel.CoefficientOfVariation),
                                                                                        "BelowPhreaticLevelCoefficientOFVariation");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.BelowPhreaticLevel.Shift = (RoundedDouble) (1.0 - lp.BelowPhreaticLevel.Shift),
                                                                                        "BelowPhreaticLevelShift");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.Cohesion.Mean = (RoundedDouble) (11.0 - lp.Cohesion.Mean),
                                                                                        "CohesionMean");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.Cohesion.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.Cohesion.CoefficientOfVariation),
                                                                                        "CohesionCoefficientOfVariation");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.FrictionAngle.Mean = (RoundedDouble) (11.0 - lp.FrictionAngle.Mean),
                                                                                        "FrictionAngleMean");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.FrictionAngle.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.FrictionAngle.CoefficientOfVariation),
                                                                                        "FrictionAngleCoefficientOfVariation");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.ShearStrengthRatio.Mean = (RoundedDouble) (11.0 - lp.ShearStrengthRatio.Mean),
                                                                                        "ShearStrengthRatioMean");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.ShearStrengthRatio.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.ShearStrengthRatio.CoefficientOfVariation),
                                                                                        "ShearStrengthRatioCoefficientOfVariation");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.StrengthIncreaseExponent.Mean = (RoundedDouble) (11.0 - lp.StrengthIncreaseExponent.Mean),
                                                                                        "StrengthIncreaseExponentMean");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.StrengthIncreaseExponent.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.StrengthIncreaseExponent.CoefficientOfVariation),
                                                                                        "StrengthIncreaseExponentCoefficientOfVariation");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.Pop.Mean = (RoundedDouble) (11.0 - lp.Pop.Mean),
                                                                                        "PopMean");
                yield return new ChangePropertyData<MacroStabilityInwardsSoilLayerData>(lp => lp.Pop.CoefficientOfVariation = (RoundedDouble) (1.0 - lp.Pop.CoefficientOfVariation),
                                                                                        "PopCoefficientOfVariation");
            }
        }

        private class DerivedMacroStabilityInwardsSoilLayerData : MacroStabilityInwardsSoilLayerData
        {
            public DerivedMacroStabilityInwardsSoilLayerData(MacroStabilityInwardsSoilLayerData data)
            {
                MaterialName = data.MaterialName;
                Color = data.Color;
                IsAquifer = data.IsAquifer;
                UsePop = data.UsePop;
                ShearStrengthModel = data.ShearStrengthModel;
                AbovePhreaticLevel = data.AbovePhreaticLevel;
                BelowPhreaticLevel = data.BelowPhreaticLevel;
                Cohesion = data.Cohesion;
                FrictionAngle = data.FrictionAngle;
                ShearStrengthRatio = data.ShearStrengthRatio;
                StrengthIncreaseExponent = data.StrengthIncreaseExponent;
                Pop = data.Pop;
            }
        }
    }
}