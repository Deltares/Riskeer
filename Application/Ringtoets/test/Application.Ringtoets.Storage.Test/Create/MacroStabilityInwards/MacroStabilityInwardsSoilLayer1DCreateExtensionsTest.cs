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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayer1DCreateExtensionsTest
    {
        [Test]
        public void Create_WithValidProperties_ReturnsEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
            {
                Properties =
                {
                    IsAquifer = random.NextBoolean(),
                    MaterialName = "MaterialName",
                    Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                    UsePop = random.NextBoolean(),
                    ShearStrengthModel = random.NextEnumValue<MacroStabilityInwardsShearStrengthModel>(),
                    AbovePhreaticLevelMean = random.NextDouble(),
                    AbovePhreaticLevelCoefficientOfVariation = random.NextDouble(),
                    AbovePhreaticLevelShift = random.NextDouble(),
                    BelowPhreaticLevelMean = random.NextDouble(),
                    BelowPhreaticLevelCoefficientOfVariation = random.NextDouble(),
                    BelowPhreaticLevelShift = random.NextDouble(),
                    CohesionMean = random.NextDouble(),
                    CohesionCoefficientOfVariation = random.NextDouble(),
                    FrictionAngleMean = random.NextDouble(),
                    FrictionAngleCoefficientOfVariation = random.NextDouble(),
                    ShearStrengthRatioMean = random.NextDouble(),
                    ShearStrengthRatioCoefficientOfVariation = random.NextDouble(),
                    StrengthIncreaseExponentMean = random.NextDouble(),
                    StrengthIncreaseExponentCoefficientOfVariation = random.NextDouble(),
                    PopMean = random.NextDouble(),
                    PopCoefficientOfVariation = random.NextDouble()
                }
            };
            int order = random.Next();

            // Call
            MacroStabilityInwardsSoilLayer1DEntity entity = soilLayer.Create(order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(soilLayer.Top, entity.Top);

            MacroStabilityInwardsSoilLayerProperties properties = soilLayer.Properties;
            Assert.AreEqual(Convert.ToByte(properties.IsAquifer), entity.IsAquifer);
            Assert.AreEqual(properties.MaterialName, entity.MaterialName);
            Assert.AreEqual(properties.Color.ToArgb(), Convert.ToInt32(entity.Color));
            Assert.AreEqual(Convert.ToByte(properties.UsePop), entity.UsePop);
            Assert.AreEqual(Convert.ToByte(properties.ShearStrengthModel), entity.ShearStrengthModel);
            Assert.AreEqual(properties.AbovePhreaticLevelMean, entity.AbovePhreaticLevelMean);
            Assert.AreEqual(properties.AbovePhreaticLevelCoefficientOfVariation, entity.AbovePhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(properties.AbovePhreaticLevelShift, entity.AbovePhreaticLevelShift);
            Assert.AreEqual(properties.BelowPhreaticLevelMean, entity.BelowPhreaticLevelMean);
            Assert.AreEqual(properties.BelowPhreaticLevelCoefficientOfVariation, entity.BelowPhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(properties.BelowPhreaticLevelShift, entity.BelowPhreaticLevelShift);
            Assert.AreEqual(properties.CohesionMean, entity.CohesionMean);
            Assert.AreEqual(properties.CohesionCoefficientOfVariation, entity.CohesionCoefficientOfVariation);
            Assert.AreEqual(properties.FrictionAngleMean, entity.FrictionAngleMean);
            Assert.AreEqual(properties.FrictionAngleCoefficientOfVariation, entity.FrictionAngleCoefficientOfVariation);
            Assert.AreEqual(properties.ShearStrengthRatioMean, entity.ShearStrengthRatioMean);
            Assert.AreEqual(properties.ShearStrengthRatioCoefficientOfVariation, entity.ShearStrengthRatioCoefficientOfVariation);
            Assert.AreEqual(properties.StrengthIncreaseExponentMean, entity.StrengthIncreaseExponentMean);
            Assert.AreEqual(properties.StrengthIncreaseExponentCoefficientOfVariation, entity.StrengthIncreaseExponentCoefficientOfVariation);
            Assert.AreEqual(properties.PopMean, entity.PopMean);
            Assert.AreEqual(properties.PopCoefficientOfVariation, entity.PopCoefficientOfVariation);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_WithNaNProperties_ReturnsEntityWithPropertiesSetToNull()
        {
            // Setup
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(double.NaN)
            {
                Properties =
                {
                    AbovePhreaticLevelMean = double.NaN,
                    AbovePhreaticLevelCoefficientOfVariation = double.NaN,
                    AbovePhreaticLevelShift = double.NaN,
                    BelowPhreaticLevelMean = double.NaN,
                    BelowPhreaticLevelCoefficientOfVariation = double.NaN,
                    BelowPhreaticLevelShift = double.NaN,
                    CohesionMean = double.NaN,
                    CohesionCoefficientOfVariation = double.NaN,
                    FrictionAngleMean = double.NaN,
                    FrictionAngleCoefficientOfVariation = double.NaN,
                    ShearStrengthRatioMean = double.NaN,
                    ShearStrengthRatioCoefficientOfVariation = double.NaN,
                    StrengthIncreaseExponentMean = double.NaN,
                    StrengthIncreaseExponentCoefficientOfVariation = double.NaN,
                    PopMean = double.NaN,
                    PopCoefficientOfVariation = double.NaN
                }
            };

            // Call
            MacroStabilityInwardsSoilLayer1DEntity entity = soilLayer.Create(0);

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
                Properties =
                {
                    MaterialName = materialName
                }
            };

            // Call
            MacroStabilityInwardsSoilLayer1DEntity entity = soilLayer.Create(0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(materialName, entity.MaterialName);
        }
    }
}