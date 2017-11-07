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
using Application.Ringtoets.Storage.Create.MacroStabilityInwards;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.MacroStabilityInwards
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
            Assert.AreEqual(data.AbovePhreaticLevel.Mean, entity.AbovePhreaticLevelMean);
            Assert.AreEqual(data.AbovePhreaticLevel.CoefficientOfVariation, entity.AbovePhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(data.AbovePhreaticLevel.Shift, entity.AbovePhreaticLevelShift);
            Assert.AreEqual(data.BelowPhreaticLevel.Mean, entity.BelowPhreaticLevelMean);
            Assert.AreEqual(data.BelowPhreaticLevel.CoefficientOfVariation, entity.BelowPhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(data.BelowPhreaticLevel.Shift, entity.BelowPhreaticLevelShift);
            Assert.AreEqual(data.Cohesion.Mean, entity.CohesionMean);
            Assert.AreEqual(data.Cohesion.CoefficientOfVariation, entity.CohesionCoefficientOfVariation);
            Assert.AreEqual(data.FrictionAngle.Mean, entity.FrictionAngleMean);
            Assert.AreEqual(data.FrictionAngle.CoefficientOfVariation, entity.FrictionAngleCoefficientOfVariation);
            Assert.AreEqual(data.ShearStrengthRatio.Mean, entity.ShearStrengthRatioMean);
            Assert.AreEqual(data.ShearStrengthRatio.CoefficientOfVariation, entity.ShearStrengthRatioCoefficientOfVariation);
            Assert.AreEqual(data.StrengthIncreaseExponent.Mean, entity.StrengthIncreaseExponentMean);
            Assert.AreEqual(data.StrengthIncreaseExponent.CoefficientOfVariation, entity.StrengthIncreaseExponentCoefficientOfVariation);
            Assert.AreEqual(data.Pop.Mean, entity.PopMean);
            Assert.AreEqual(data.Pop.CoefficientOfVariation, entity.PopCoefficientOfVariation);
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