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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.MacroStabilityInwards;
using Application.Ringtoets.Storage.Serializers;
using Application.Ringtoets.Storage.TestUtil.MacroStabilityInwards;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;

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
            MacroStabilityInwardsSoilLayerTwoDEntity entity = CreateMacroStabilityInwardsSoilLayerTwoDEntity();

            // Call
            MacroStabilityInwardsSoilLayer2D layer = entity.Read();

            // Assert
            AssertMacroStabilityInwardsSoilLayer2D(entity, layer);
        }

        [Test]
        public void Read_WithNestedLayers_ReturnsMacroStabilityInwardsSoilLayer2DWithNestedLayers()
        {
            // Setup
            MacroStabilityInwardsSoilLayerTwoDEntity entity = MacroStabilityInwardsSoilLayerTwoDEntityTestFactory.CreateMacroStabilityInwardsSoilLayerTwoDEntity();

            entity.MacroStabilityInwardsSoilLayerTwoDEntity1.Add(CreateMacroStabilityInwardsSoilLayerTwoDEntity());
            entity.MacroStabilityInwardsSoilLayerTwoDEntity1.Add(CreateMacroStabilityInwardsSoilLayerTwoDEntity());
            entity.MacroStabilityInwardsSoilLayerTwoDEntity1.Add(CreateMacroStabilityInwardsSoilLayerTwoDEntity());

            // Call
            MacroStabilityInwardsSoilLayer2D layer = entity.Read();

            // Assert
            Assert.IsNotNull(layer);
            Assert.AreEqual(entity.MacroStabilityInwardsSoilLayerTwoDEntity1.Count,
                            layer.NestedLayers.Count());

            for (var i = 0; i < entity.MacroStabilityInwardsSoilLayerTwoDEntity1.Count; i++)
            {
                AssertMacroStabilityInwardsSoilLayer2D(entity.MacroStabilityInwardsSoilLayerTwoDEntity1.ElementAt(i),
                                                       layer.NestedLayers.ElementAt(i));
            }
        }

        [Test]
        public void Read_WithNullValues_ReturnsMacroStabilityInwardsSoilLayer2DWithNaNValues()
        {
            // Setup
            var entity = new MacroStabilityInwardsSoilLayerTwoDEntity
            {
                MaterialName = nameof(MacroStabilityInwardsSoilLayerTwoDEntity),
                OuterRingXml = new Point2DXmlSerializer().ToXml(RingTestFactory.CreateRandomRing().Points)
            };

            // Call
            MacroStabilityInwardsSoilLayer2D layer = entity.Read();

            // Assert
            Assert.IsNotNull(layer);
            MacroStabilityInwardsSoilLayerData data = layer.Data;
            Assert.AreEqual(entity.MaterialName, data.MaterialName);

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

        private static void AssertMacroStabilityInwardsSoilLayer2D(MacroStabilityInwardsSoilLayerTwoDEntity entity,
                                                                   MacroStabilityInwardsSoilLayer2D layer)
        {
            Assert.IsNotNull(layer);
            MacroStabilityInwardsSoilLayerData data = layer.Data;
            Assert.AreEqual(Convert.ToBoolean(entity.IsAquifer), data.IsAquifer);
            Assert.AreEqual(Convert.ToInt64(entity.Color), data.Color.ToInt64());
            Assert.AreEqual(entity.MaterialName, data.MaterialName);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) entity.AbovePhreaticLevelMean.ToNullAsNaN(),
                CoefficientOfVariation = (RoundedDouble) entity.AbovePhreaticLevelCoefficientOfVariation.ToNullAsNaN(),
                Shift = (RoundedDouble) entity.AbovePhreaticLevelShift.ToNullAsNaN()
            }, data.AbovePhreaticLevel);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) entity.BelowPhreaticLevelMean.ToNullAsNaN(),
                CoefficientOfVariation = (RoundedDouble) entity.BelowPhreaticLevelCoefficientOfVariation.ToNullAsNaN(),
                Shift = (RoundedDouble) entity.BelowPhreaticLevelShift.ToNullAsNaN()
            }, data.BelowPhreaticLevel);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) entity.CohesionMean.ToNullAsNaN(),
                CoefficientOfVariation = (RoundedDouble) entity.CohesionCoefficientOfVariation.ToNullAsNaN()
            }, data.Cohesion);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) entity.FrictionAngleMean.ToNullAsNaN(),
                CoefficientOfVariation = (RoundedDouble) entity.FrictionAngleCoefficientOfVariation.ToNullAsNaN()
            }, data.FrictionAngle);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) entity.ShearStrengthRatioMean.ToNullAsNaN(),
                CoefficientOfVariation = (RoundedDouble) entity.ShearStrengthRatioCoefficientOfVariation.ToNullAsNaN()
            }, data.ShearStrengthRatio);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) entity.StrengthIncreaseExponentMean.ToNullAsNaN(),
                CoefficientOfVariation = (RoundedDouble) entity.StrengthIncreaseExponentCoefficientOfVariation.ToNullAsNaN()
            }, data.StrengthIncreaseExponent);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) entity.PopMean.ToNullAsNaN(),
                CoefficientOfVariation = (RoundedDouble) entity.PopCoefficientOfVariation.ToNullAsNaN()
            }, data.Pop);

            CollectionAssert.AreEqual(new Point2DXmlSerializer().FromXml(entity.OuterRingXml),
                                      layer.OuterRing.Points);
            CollectionAssert.IsEmpty(layer.NestedLayers);
        }

        private static MacroStabilityInwardsSoilLayerTwoDEntity CreateMacroStabilityInwardsSoilLayerTwoDEntity()
        {
            var random = new Random(31);
            var entity = new MacroStabilityInwardsSoilLayerTwoDEntity
            {
                IsAquifer = Convert.ToByte(random.NextBoolean()),
                Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()).ToInt64(),
                MaterialName = random.Next().ToString(),
                AbovePhreaticLevelMean = random.GetFromRange(2.0, 3.0),
                AbovePhreaticLevelCoefficientOfVariation = random.NextDouble(),
                AbovePhreaticLevelShift = random.GetFromRange(0.0, 1.0),
                BelowPhreaticLevelMean = random.GetFromRange(2.0, 3.0),
                BelowPhreaticLevelCoefficientOfVariation = random.NextDouble(),
                BelowPhreaticLevelShift = random.GetFromRange(0.0, 1.0),
                CohesionMean = random.NextDouble(),
                CohesionCoefficientOfVariation = random.NextDouble(),
                FrictionAngleMean = random.NextDouble(),
                FrictionAngleCoefficientOfVariation = random.NextDouble(),
                ShearStrengthRatioMean = random.NextDouble(),
                ShearStrengthRatioCoefficientOfVariation = random.NextDouble(),
                StrengthIncreaseExponentMean = random.NextDouble(),
                StrengthIncreaseExponentCoefficientOfVariation = random.NextDouble(),
                PopMean = random.NextDouble(),
                PopCoefficientOfVariation = random.NextDouble(),
                OuterRingXml = new Point2DXmlSerializer().ToXml(RingTestFactory.CreateRandomRing().Points)
            };
            return entity;
        }
    }
}