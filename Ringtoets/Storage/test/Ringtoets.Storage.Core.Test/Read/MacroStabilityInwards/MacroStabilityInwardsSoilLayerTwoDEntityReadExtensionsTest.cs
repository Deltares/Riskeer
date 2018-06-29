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
using Application.Ringtoets.Storage.TestUtil.MacroStabilityInwards;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;
using Ringtoets.Storage.Core.Read.MacroStabilityInwards;

namespace Ringtoets.Storage.Core.Test.Read.MacroStabilityInwards
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

        private static void AssertMacroStabilityInwardsSoilLayer2D(MacroStabilityInwardsSoilLayerTwoDEntity entity,
                                                                   MacroStabilityInwardsSoilLayer2D layer)
        {
            Assert.IsNotNull(layer);
            MacroStabilityInwardsSoilLayerData data = layer.Data;
            Assert.AreEqual(Convert.ToBoolean(entity.IsAquifer), data.IsAquifer);
            Assert.AreEqual(entity.Color, data.Color.ToInt64());
            Assert.AreEqual(entity.MaterialName, data.MaterialName);

            Assert.AreEqual(entity.AbovePhreaticLevelMean ?? double.NaN, data.AbovePhreaticLevel.Mean,
                            data.AbovePhreaticLevel.GetAccuracy());
            Assert.AreEqual(entity.AbovePhreaticLevelCoefficientOfVariation ?? double.NaN, data.AbovePhreaticLevel.CoefficientOfVariation,
                            data.AbovePhreaticLevel.GetAccuracy());
            Assert.AreEqual(entity.AbovePhreaticLevelShift ?? double.NaN, data.AbovePhreaticLevel.Shift,
                            data.AbovePhreaticLevel.GetAccuracy());

            Assert.AreEqual(entity.BelowPhreaticLevelMean ?? double.NaN, data.BelowPhreaticLevel.Mean,
                            data.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(entity.BelowPhreaticLevelCoefficientOfVariation ?? double.NaN, data.BelowPhreaticLevel.CoefficientOfVariation,
                            data.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(entity.BelowPhreaticLevelShift ?? double.NaN, data.BelowPhreaticLevel.Shift,
                            data.BelowPhreaticLevel.GetAccuracy());

            Assert.AreEqual(entity.CohesionMean ?? double.NaN, data.Cohesion.Mean,
                            data.Cohesion.GetAccuracy());
            Assert.AreEqual(entity.CohesionCoefficientOfVariation ?? double.NaN, data.Cohesion.CoefficientOfVariation,
                            data.Cohesion.GetAccuracy());

            Assert.AreEqual(entity.FrictionAngleMean ?? double.NaN, data.FrictionAngle.Mean,
                            data.FrictionAngle.GetAccuracy());
            Assert.AreEqual(entity.FrictionAngleCoefficientOfVariation ?? double.NaN, data.FrictionAngle.CoefficientOfVariation,
                            data.FrictionAngle.GetAccuracy());

            Assert.AreEqual(entity.ShearStrengthRatioMean ?? double.NaN, data.ShearStrengthRatio.Mean,
                            data.ShearStrengthRatio.GetAccuracy());
            Assert.AreEqual(entity.ShearStrengthRatioCoefficientOfVariation ?? double.NaN, data.ShearStrengthRatio.CoefficientOfVariation,
                            data.ShearStrengthRatio.GetAccuracy());

            Assert.AreEqual(entity.StrengthIncreaseExponentMean ?? double.NaN, data.StrengthIncreaseExponent.Mean,
                            data.StrengthIncreaseExponent.GetAccuracy());
            Assert.AreEqual(entity.StrengthIncreaseExponentCoefficientOfVariation ?? double.NaN, data.StrengthIncreaseExponent.CoefficientOfVariation,
                            data.StrengthIncreaseExponent.GetAccuracy());

            Assert.AreEqual(entity.PopMean ?? double.NaN, data.Pop.Mean, data.Pop.GetAccuracy());
            Assert.AreEqual(entity.PopCoefficientOfVariation ?? double.NaN, data.Pop.CoefficientOfVariation,
                            data.Pop.GetAccuracy());

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
                AbovePhreaticLevelMean = random.NextDouble(2.0, 3.0),
                AbovePhreaticLevelCoefficientOfVariation = random.NextDouble(),
                AbovePhreaticLevelShift = random.NextDouble(0.0, 1.0),
                BelowPhreaticLevelMean = random.NextDouble(2.0, 3.0),
                BelowPhreaticLevelCoefficientOfVariation = random.NextDouble(),
                BelowPhreaticLevelShift = random.NextDouble(0.0, 1.0),
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