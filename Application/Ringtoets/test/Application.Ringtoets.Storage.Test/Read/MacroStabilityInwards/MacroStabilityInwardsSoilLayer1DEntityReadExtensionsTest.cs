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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.MacroStabilityInwards;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayer1DEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((MacroStabilityInwardsSoilLayer1DEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_WithValues_ReturnsMacroStabilityInwardsSoilLayer1DWithDoubleParameterValues()
        {
            // Setup
            var random = new Random(31);
            double top = random.NextDouble();
            int color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()).ToArgb();
            bool isAquifer = random.NextBoolean();
            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelCoefficientOfVariation = random.NextDouble();
            double belowPhreaticLevelShift = random.NextDouble();
            double abovePhreaticLevelMean = random.NextDouble();
            double abovePhreaticLevelCoefficientOfVariation = random.NextDouble();
            double abovePhreaticLevelShift = random.NextDouble();
            double cohesionMean = random.NextDouble();
            double cohesionCoefficientOfVariation = random.NextDouble();
            double frictionAngleMean = random.NextDouble();
            double frictionAngleCoefficientOfVariation = random.NextDouble();
            double shearStrengthRatioMean = random.NextDouble();
            double shearStrengthRatioCoefficientOfVariation = random.NextDouble();
            double strengthIncreaseExponentMean = random.NextDouble();
            double strengthIncreaseExponentCoefficientOfVariation = random.NextDouble();
            double popMean = random.NextDouble();
            double popCoefficientOfVariation = random.NextDouble();

            var entity = new MacroStabilityInwardsSoilLayer1DEntity
            {
                Top = top,
                IsAquifer = Convert.ToByte(isAquifer),
                Color = color,
                MaterialName = random.Next().ToString(),
                AbovePhreaticLevelMean = abovePhreaticLevelMean,
                AbovePhreaticLevelCoefficientOfVariation = abovePhreaticLevelCoefficientOfVariation,
                AbovePhreaticLevelShift = abovePhreaticLevelShift,

                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelCoefficientOfVariation = belowPhreaticLevelCoefficientOfVariation,
                BelowPhreaticLevelShift = belowPhreaticLevelShift,

                CohesionMean = cohesionMean,
                CohesionCoefficientOfVariation = cohesionCoefficientOfVariation,

                FrictionAngleMean = frictionAngleMean,
                FrictionAngleCoefficientOfVariation = frictionAngleCoefficientOfVariation,

                ShearStrengthRatioMean = shearStrengthRatioMean,
                ShearStrengthRatioCoefficientOfVariation = shearStrengthRatioCoefficientOfVariation,

                StrengthIncreaseExponentMean = strengthIncreaseExponentMean,
                StrengthIncreaseExponentCoefficientOfVariation = strengthIncreaseExponentCoefficientOfVariation,

                PopMean = popMean,
                PopCoefficientOfVariation = popCoefficientOfVariation
            };

            // Call
            MacroStabilityInwardsSoilLayer1D layer = entity.Read();

            // Assert
            Assert.IsNotNull(layer);
            Assert.AreEqual(top, layer.Top);
            MacroStabilityInwardsSoilLayerProperties properties = layer.Properties;
            Assert.AreEqual(isAquifer, properties.IsAquifer);
            Assert.AreEqual(Color.FromArgb(color), properties.Color);
            Assert.AreEqual(entity.MaterialName, properties.MaterialName);

            Assert.AreEqual(belowPhreaticLevelMean, properties.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelCoefficientOfVariation, properties.BelowPhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(belowPhreaticLevelShift, properties.BelowPhreaticLevelShift);

            Assert.AreEqual(abovePhreaticLevelMean, properties.AbovePhreaticLevelMean);
            Assert.AreEqual(abovePhreaticLevelCoefficientOfVariation, properties.AbovePhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(abovePhreaticLevelShift, properties.AbovePhreaticLevelShift);

            Assert.AreEqual(cohesionMean, properties.CohesionMean);
            Assert.AreEqual(cohesionCoefficientOfVariation, properties.CohesionCoefficientOfVariation);

            Assert.AreEqual(frictionAngleMean, properties.FrictionAngleMean);
            Assert.AreEqual(frictionAngleCoefficientOfVariation, properties.FrictionAngleCoefficientOfVariation);

            Assert.AreEqual(shearStrengthRatioMean, properties.ShearStrengthRatioMean);
            Assert.AreEqual(shearStrengthRatioCoefficientOfVariation, properties.ShearStrengthRatioCoefficientOfVariation);

            Assert.AreEqual(strengthIncreaseExponentMean, properties.StrengthIncreaseExponentMean);
            Assert.AreEqual(strengthIncreaseExponentCoefficientOfVariation, properties.StrengthIncreaseExponentCoefficientOfVariation);

            Assert.AreEqual(popMean, properties.PopMean);
            Assert.AreEqual(popCoefficientOfVariation, properties.PopCoefficientOfVariation);
        }

        [Test]
        public void Read_WithNullValues_ReturnsMacroStabilityInwardsSoilLayer1DWithNaNValues()
        {
            // Setup
            var entity = new MacroStabilityInwardsSoilLayer1DEntity
            {
                MaterialName = nameof(MacroStabilityInwardsSoilLayer1DEntity)
            };

            // Call
            MacroStabilityInwardsSoilLayer1D layer = entity.Read();

            // Assert
            Assert.IsNotNull(layer);
            MacroStabilityInwardsSoilLayerProperties properties = layer.Properties;
            Assert.AreEqual(entity.MaterialName, properties.MaterialName);

            Assert.IsNaN(layer.Top);
            Assert.IsNaN(properties.AbovePhreaticLevelMean);
            Assert.IsNaN(properties.AbovePhreaticLevelCoefficientOfVariation);
            Assert.IsNaN(properties.AbovePhreaticLevelShift);

            Assert.IsNaN(properties.BelowPhreaticLevelMean);
            Assert.IsNaN(properties.BelowPhreaticLevelCoefficientOfVariation);
            Assert.IsNaN(properties.BelowPhreaticLevelShift);

            Assert.IsNaN(properties.CohesionMean);
            Assert.IsNaN(properties.CohesionCoefficientOfVariation);

            Assert.IsNaN(properties.FrictionAngleMean);
            Assert.IsNaN(properties.FrictionAngleCoefficientOfVariation);

            Assert.IsNaN(properties.ShearStrengthRatioMean);
            Assert.IsNaN(properties.ShearStrengthRatioCoefficientOfVariation);

            Assert.IsNaN(properties.StrengthIncreaseExponentMean);
            Assert.IsNaN(properties.StrengthIncreaseExponentCoefficientOfVariation);

            Assert.IsNaN(properties.PopMean);
            Assert.IsNaN(properties.PopCoefficientOfVariation);
        }
    }
}