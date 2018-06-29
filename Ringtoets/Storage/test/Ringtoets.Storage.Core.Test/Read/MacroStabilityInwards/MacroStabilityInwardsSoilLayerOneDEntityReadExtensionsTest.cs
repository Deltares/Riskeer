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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read.MacroStabilityInwards;

namespace Ringtoets.Storage.Core.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayerOneDEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((MacroStabilityInwardsSoilLayerOneDEntity) null).Read();

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
            Color color = Color.FromKnownColor(random.NextEnumValue<KnownColor>());
            bool isAquifer = random.NextBoolean();
            const double abovePhreaticLevelMean = 0.3;
            const double abovePhreaticLevelCoefficientOfVariation = 0.2;
            const double abovePhreaticLevelShift = 0.1;
            const double belowPhreaticLevelMean = 0.6;
            const double belowPhreaticLevelCoefficientOfVariation = 0.5;
            const double belowPhreaticLevelShift = 0.4;
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

            var entity = new MacroStabilityInwardsSoilLayerOneDEntity
            {
                Top = top,
                IsAquifer = Convert.ToByte(isAquifer),
                Color = color.ToInt64(),
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
            MacroStabilityInwardsSoilLayerData data = layer.Data;
            Assert.AreEqual(isAquifer, data.IsAquifer);
            Assert.IsNotNull(color);
            Assert.AreEqual(color.ToArgb(), data.Color.ToArgb());
            Assert.AreEqual(entity.MaterialName, data.MaterialName);

            Assert.AreEqual(abovePhreaticLevelMean, data.AbovePhreaticLevel.Mean,
                            data.AbovePhreaticLevel.GetAccuracy());
            Assert.AreEqual(abovePhreaticLevelCoefficientOfVariation, data.AbovePhreaticLevel.CoefficientOfVariation,
                            data.AbovePhreaticLevel.GetAccuracy());
            Assert.AreEqual(abovePhreaticLevelShift, data.AbovePhreaticLevel.Shift,
                            data.AbovePhreaticLevel.GetAccuracy());

            Assert.AreEqual(belowPhreaticLevelMean, data.BelowPhreaticLevel.Mean,
                            data.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(belowPhreaticLevelCoefficientOfVariation, data.BelowPhreaticLevel.CoefficientOfVariation,
                            data.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(belowPhreaticLevelShift, data.BelowPhreaticLevel.Shift,
                            data.BelowPhreaticLevel.GetAccuracy());

            Assert.AreEqual(cohesionMean, data.Cohesion.Mean,
                            data.Cohesion.GetAccuracy());
            Assert.AreEqual(cohesionCoefficientOfVariation, data.Cohesion.CoefficientOfVariation,
                            data.Cohesion.GetAccuracy());

            Assert.AreEqual(frictionAngleMean, data.FrictionAngle.Mean,
                            data.FrictionAngle.GetAccuracy());
            Assert.AreEqual(frictionAngleCoefficientOfVariation, data.FrictionAngle.CoefficientOfVariation,
                            data.FrictionAngle.GetAccuracy());

            Assert.AreEqual(shearStrengthRatioMean, data.ShearStrengthRatio.Mean,
                            data.ShearStrengthRatio.GetAccuracy());
            Assert.AreEqual(shearStrengthRatioCoefficientOfVariation, data.ShearStrengthRatio.CoefficientOfVariation,
                            data.ShearStrengthRatio.GetAccuracy());

            Assert.AreEqual(strengthIncreaseExponentMean, data.StrengthIncreaseExponent.Mean,
                            data.StrengthIncreaseExponent.GetAccuracy());
            Assert.AreEqual(strengthIncreaseExponentCoefficientOfVariation, data.StrengthIncreaseExponent.CoefficientOfVariation,
                            data.StrengthIncreaseExponent.GetAccuracy());

            Assert.AreEqual(popMean, data.Pop.Mean, data.Pop.GetAccuracy());
            Assert.AreEqual(popCoefficientOfVariation, data.Pop.CoefficientOfVariation,
                            data.Pop.GetAccuracy());
        }

        [Test]
        public void Read_WithNullValues_ReturnsMacroStabilityInwardsSoilLayer1DWithNaNValues()
        {
            // Setup
            var entity = new MacroStabilityInwardsSoilLayerOneDEntity
            {
                MaterialName = nameof(MacroStabilityInwardsSoilLayerOneDEntity)
            };

            // Call
            MacroStabilityInwardsSoilLayer1D layer = entity.Read();

            // Assert
            Assert.IsNotNull(layer);
            MacroStabilityInwardsSoilLayerData data = layer.Data;
            Assert.AreEqual(entity.MaterialName, data.MaterialName);

            Assert.IsNaN(layer.Top);

            Assert.IsNaN(data.AbovePhreaticLevel.Mean);
            Assert.IsNaN(data.AbovePhreaticLevel.CoefficientOfVariation);
            Assert.IsNaN(data.AbovePhreaticLevel.Shift);

            Assert.IsNaN(data.BelowPhreaticLevel.Mean);
            Assert.IsNaN(data.BelowPhreaticLevel.CoefficientOfVariation);
            Assert.IsNaN(data.BelowPhreaticLevel.Shift);

            Assert.IsNaN(data.Cohesion.Mean);
            Assert.IsNaN(data.Cohesion.CoefficientOfVariation);

            Assert.IsNaN(data.FrictionAngle.Mean);
            Assert.IsNaN(data.FrictionAngle.CoefficientOfVariation);

            Assert.IsNaN(data.ShearStrengthRatio.Mean);
            Assert.IsNaN(data.ShearStrengthRatio.CoefficientOfVariation);

            Assert.IsNaN(data.StrengthIncreaseExponent.Mean);
            Assert.IsNaN(data.StrengthIncreaseExponent.CoefficientOfVariation);

            Assert.IsNaN(data.Pop.Mean);
            Assert.IsNaN(data.Pop.CoefficientOfVariation);
        }
    }
}