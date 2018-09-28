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
using System.Drawing;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.Create.MacroStabilityInwards;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayer1DCreateExtensionsTest
    {
        [Test]
        public void Create_SoilLayerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((MacroStabilityInwardsSoilLayer1D) null).Create(0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("soilLayer", parameterName);
        }

        [Test]
        public void Create_WithValidProperties_ReturnsEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
            {
                Data =
                {
                    IsAquifer = random.NextBoolean(),
                    MaterialName = "MaterialName",
                    Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                    UsePop = random.NextBoolean(),
                    ShearStrengthModel = random.NextEnumValue<MacroStabilityInwardsShearStrengthModel>(),
                    AbovePhreaticLevel =
                    {
                        Mean = (RoundedDouble) 0.3,
                        CoefficientOfVariation = (RoundedDouble) 0.2,
                        Shift = (RoundedDouble) 0.1
                    },
                    BelowPhreaticLevel =
                    {
                        Mean = (RoundedDouble) 10,
                        CoefficientOfVariation = (RoundedDouble) 0.5,
                        Shift = (RoundedDouble) 2
                    },
                    Cohesion =
                    {
                        Mean = (RoundedDouble) 10,
                        CoefficientOfVariation = (RoundedDouble) 1
                    },
                    FrictionAngle =
                    {
                        Mean = (RoundedDouble) 12,
                        CoefficientOfVariation = (RoundedDouble) 0.8
                    },
                    ShearStrengthRatio =
                    {
                        Mean = (RoundedDouble) 10,
                        CoefficientOfVariation = (RoundedDouble) 0.6
                    },
                    StrengthIncreaseExponent =
                    {
                        Mean = (RoundedDouble) 11,
                        CoefficientOfVariation = (RoundedDouble) 0.7
                    },
                    Pop =
                    {
                        Mean = (RoundedDouble) 14,
                        CoefficientOfVariation = (RoundedDouble) 0.9
                    }
                }
            };
            int order = random.Next();

            // Call
            MacroStabilityInwardsSoilLayerOneDEntity entity = soilLayer.Create(order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(soilLayer.Top, entity.Top);

            MacroStabilityInwardsSoilLayerData data = soilLayer.Data;
            Assert.AreEqual(Convert.ToByte(data.IsAquifer), entity.IsAquifer);
            Assert.AreEqual(data.MaterialName, entity.MaterialName);
            Assert.AreEqual(data.Color.ToInt64(), Convert.ToInt64(entity.Color));
            Assert.AreEqual(Convert.ToByte(data.UsePop), entity.UsePop);
            Assert.AreEqual(Convert.ToByte(data.ShearStrengthModel), entity.ShearStrengthModel);

            VariationCoefficientLogNormalDistribution abovePhreaticLevelDistribution = data.AbovePhreaticLevel;
            Assert.AreEqual(abovePhreaticLevelDistribution.Mean, entity.AbovePhreaticLevelMean,
                            abovePhreaticLevelDistribution.GetAccuracy());
            Assert.AreEqual(abovePhreaticLevelDistribution.CoefficientOfVariation, entity.AbovePhreaticLevelCoefficientOfVariation,
                            abovePhreaticLevelDistribution.GetAccuracy());
            Assert.AreEqual(abovePhreaticLevelDistribution.Shift, entity.AbovePhreaticLevelShift,
                            abovePhreaticLevelDistribution.GetAccuracy());

            VariationCoefficientLogNormalDistribution belowPhreaticLevelDistribution = data.BelowPhreaticLevel;
            Assert.AreEqual(belowPhreaticLevelDistribution.Mean, entity.BelowPhreaticLevelMean,
                            belowPhreaticLevelDistribution.GetAccuracy());
            Assert.AreEqual(belowPhreaticLevelDistribution.CoefficientOfVariation, entity.BelowPhreaticLevelCoefficientOfVariation,
                            belowPhreaticLevelDistribution.GetAccuracy());
            Assert.AreEqual(belowPhreaticLevelDistribution.Shift, entity.BelowPhreaticLevelShift,
                            belowPhreaticLevelDistribution.GetAccuracy());

            VariationCoefficientLogNormalDistribution cohesionDistribution = data.Cohesion;
            Assert.AreEqual(cohesionDistribution.Mean, entity.CohesionMean,
                            cohesionDistribution.GetAccuracy());
            Assert.AreEqual(cohesionDistribution.CoefficientOfVariation, entity.CohesionCoefficientOfVariation,
                            cohesionDistribution.GetAccuracy());

            VariationCoefficientLogNormalDistribution frictionAngleDistribution = data.FrictionAngle;
            Assert.AreEqual(frictionAngleDistribution.Mean, entity.FrictionAngleMean,
                            frictionAngleDistribution.GetAccuracy());
            Assert.AreEqual(frictionAngleDistribution.CoefficientOfVariation, entity.FrictionAngleCoefficientOfVariation,
                            frictionAngleDistribution.GetAccuracy());

            VariationCoefficientLogNormalDistribution shearStrengthRatioDistribution = data.ShearStrengthRatio;
            Assert.AreEqual(shearStrengthRatioDistribution.Mean, entity.ShearStrengthRatioMean,
                            shearStrengthRatioDistribution.GetAccuracy());
            Assert.AreEqual(shearStrengthRatioDistribution.CoefficientOfVariation, entity.ShearStrengthRatioCoefficientOfVariation,
                            shearStrengthRatioDistribution.GetAccuracy());

            VariationCoefficientLogNormalDistribution strengthIncreaseExponentDistribution = data.StrengthIncreaseExponent;
            Assert.AreEqual(strengthIncreaseExponentDistribution.Mean, entity.StrengthIncreaseExponentMean,
                            strengthIncreaseExponentDistribution.GetAccuracy());
            Assert.AreEqual(strengthIncreaseExponentDistribution.CoefficientOfVariation, entity.StrengthIncreaseExponentCoefficientOfVariation,
                            strengthIncreaseExponentDistribution.GetAccuracy());

            VariationCoefficientLogNormalDistribution popDistribution = data.Pop;
            Assert.AreEqual(popDistribution.Mean, entity.PopMean, popDistribution.GetAccuracy());
            Assert.AreEqual(popDistribution.CoefficientOfVariation, entity.PopCoefficientOfVariation, popDistribution.GetAccuracy());
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_WithNaNProperties_ReturnsEntityWithPropertiesSetToNull()
        {
            // Setup
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(double.NaN)
            {
                Data =
                {
                    AbovePhreaticLevel =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN,
                        Shift = RoundedDouble.NaN
                    },
                    BelowPhreaticLevel =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN,
                        Shift = RoundedDouble.NaN
                    },
                    Cohesion =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    FrictionAngle =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    ShearStrengthRatio =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    StrengthIncreaseExponent =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    Pop =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    }
                }
            };

            // Call
            MacroStabilityInwardsSoilLayerOneDEntity entity = soilLayer.Create(0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.Top);
            Assert.IsNull(entity.AbovePhreaticLevelMean);
            Assert.IsNull(entity.AbovePhreaticLevelCoefficientOfVariation);
            Assert.IsNull(entity.AbovePhreaticLevelShift);
            Assert.IsNull(entity.BelowPhreaticLevelMean);
            Assert.IsNull(entity.BelowPhreaticLevelCoefficientOfVariation);
            Assert.IsNull(entity.BelowPhreaticLevelShift);
            Assert.IsNull(entity.CohesionMean);
            Assert.IsNull(entity.CohesionCoefficientOfVariation);
            Assert.IsNull(entity.FrictionAngleMean);
            Assert.IsNull(entity.FrictionAngleCoefficientOfVariation);
            Assert.IsNull(entity.ShearStrengthRatioMean);
            Assert.IsNull(entity.ShearStrengthRatioCoefficientOfVariation);
            Assert.IsNull(entity.StrengthIncreaseExponentMean);
            Assert.IsNull(entity.StrengthIncreaseExponentCoefficientOfVariation);
            Assert.IsNull(entity.PopMean);
            Assert.IsNull(entity.PopCoefficientOfVariation);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string materialName = "MaterialName";
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(0)
            {
                Data =
                {
                    MaterialName = materialName
                }
            };

            // Call
            MacroStabilityInwardsSoilLayerOneDEntity entity = soilLayer.Create(0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(materialName, entity.MaterialName);
        }
    }
}