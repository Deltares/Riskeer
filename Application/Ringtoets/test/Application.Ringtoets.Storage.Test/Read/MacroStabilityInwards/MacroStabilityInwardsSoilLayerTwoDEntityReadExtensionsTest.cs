﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayerTwoDEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((MacroStabilityInwardsSoilLayerTwoDEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_WithValues_ReturnsMacroStabilityInwardsSoilLayer2DWithDoubleParameterValues()
        {
            // Setup
            var random = new Random(31);
            int color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()).ToArgb();
            bool isAquifer = random.NextBoolean();
            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelCoefficientOfVariation = random.NextDouble();
            double belowPhreaticLevelShift = random.NextDouble();
            const double abovePhreaticLevelMean = 0.3;
            const double abovePhreaticLevelCoefficientOfVariation = 0.2;
            const double abovePhreaticLevelShift = 0.1;
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

            var outerRingPoints = new[]
            {
                CreateRandomPoint2D(random),
                CreateRandomPoint2D(random),
                CreateRandomPoint2D(random),
                CreateRandomPoint2D(random)
            };
            var holes = new[]
            {
                CreateRandomRing(random),
                CreateRandomRing(random),
                CreateRandomRing(random)
            };

            var entity = new MacroStabilityInwardsSoilLayerTwoDEntity
            {
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
                PopCoefficientOfVariation = popCoefficientOfVariation,
                OuterRingXml = new Point2DXmlSerializer().ToXml(outerRingPoints),
                HolesXml = new RingXmlSerializer().ToXml(holes)
            };

            // Call
            MacroStabilityInwardsSoilLayer2D layer = entity.Read();

            // Assert
            Assert.IsNotNull(layer);
            MacroStabilityInwardsSoilLayerProperties properties = layer.Properties;
            Assert.AreEqual(isAquifer, properties.IsAquifer);
            Assert.AreEqual(Color.FromArgb(color), properties.Color);
            Assert.AreEqual(entity.MaterialName, properties.MaterialName);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) abovePhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) abovePhreaticLevelCoefficientOfVariation,
                Shift = (RoundedDouble) abovePhreaticLevelShift
            }, properties.AbovePhreaticLevel);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) belowPhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) belowPhreaticLevelCoefficientOfVariation,
                Shift = (RoundedDouble) belowPhreaticLevelShift
            }, properties.BelowPhreaticLevel);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) cohesionMean,
                CoefficientOfVariation = (RoundedDouble) cohesionCoefficientOfVariation
            }, properties.Cohesion);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) frictionAngleMean,
                CoefficientOfVariation = (RoundedDouble) frictionAngleCoefficientOfVariation
            }, properties.FrictionAngle);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) shearStrengthRatioMean,
                CoefficientOfVariation = (RoundedDouble) shearStrengthRatioCoefficientOfVariation
            }, properties.ShearStrengthRatio);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) strengthIncreaseExponentMean,
                CoefficientOfVariation = (RoundedDouble) strengthIncreaseExponentCoefficientOfVariation
            }, properties.StrengthIncreaseExponent);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) popMean,
                CoefficientOfVariation = (RoundedDouble) popCoefficientOfVariation
            }, properties.Pop);

            CollectionAssert.AreEqual(outerRingPoints, layer.OuterRing.Points);
            CollectionAssert.AreEqual(holes, layer.Holes);
        }

        [Test]
        public void Read_WithNullValues_ReturnsMacroStabilityInwardsSoilLayer2DWithNaNValues()
        {
            // Setup
            var entity = new MacroStabilityInwardsSoilLayerTwoDEntity
            {
                MaterialName = nameof(MacroStabilityInwardsSoilLayerTwoDEntity),
                OuterRingXml = new Point2DXmlSerializer().ToXml(CreateRandomRing(new Random(31)).Points),
                HolesXml = new RingXmlSerializer().ToXml(new Ring[0])
            };

            // Call
            MacroStabilityInwardsSoilLayer2D layer = entity.Read();

            // Assert
            Assert.IsNotNull(layer);
            MacroStabilityInwardsSoilLayerProperties properties = layer.Properties;
            Assert.AreEqual(entity.MaterialName, properties.MaterialName);

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

        private static Ring CreateRandomRing(Random random)
        {
            return new Ring(new[]
            {
                CreateRandomPoint2D(random),
                CreateRandomPoint2D(random)
            });
        }

        private static Point2D CreateRandomPoint2D(Random random)
        {
            return new Point2D(random.NextDouble(), random.NextDouble());
        }
    }
}