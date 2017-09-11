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
using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.Piping
{
    [TestFixture]
    public class PipingSoilLayerCreateExtensionsTest
    {
        [Test]
        public void Create_WithValidProperties_ReturnsEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            int order = random.Next();
            var soilLayer = new PipingSoilLayer(random.NextDouble())
            {
                IsAquifer = random.NextBoolean(),
                Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                MaterialName = "MaterialName",
                BelowPhreaticLevelMean = random.NextDouble(),
                BelowPhreaticLevelDeviation = random.NextDouble(),
                BelowPhreaticLevelShift = random.NextDouble(),
                DiameterD70Mean = random.NextDouble(),
                DiameterD70CoefficientOfVariation = random.NextDouble(),
                PermeabilityMean = random.NextDouble(),
                PermeabilityCoefficientOfVariation = random.NextDouble()
            };

            // Call
            PipingSoilLayerEntity entity = soilLayer.Create(order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(soilLayer.Top, entity.Top);
            Assert.AreEqual(Convert.ToByte(soilLayer.IsAquifer), entity.IsAquifer);
            Assert.AreEqual(soilLayer.Color.ToArgb(), Convert.ToInt32(entity.Color));
            Assert.AreEqual(soilLayer.BelowPhreaticLevelMean, entity.BelowPhreaticLevelMean);
            Assert.AreEqual(soilLayer.BelowPhreaticLevelDeviation, entity.BelowPhreaticLevelDeviation);
            Assert.AreEqual(soilLayer.BelowPhreaticLevelShift, entity.BelowPhreaticLevelShift);
            Assert.AreEqual(soilLayer.DiameterD70Mean, entity.DiameterD70Mean);
            Assert.AreEqual(soilLayer.DiameterD70CoefficientOfVariation, entity.DiameterD70CoefficientOfVariation);
            Assert.AreEqual(soilLayer.PermeabilityMean, entity.PermeabilityMean);
            Assert.AreEqual(soilLayer.PermeabilityCoefficientOfVariation, entity.PermeabilityCoefficientOfVariation);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_WithNaNProperties_ReturnsEntityWithPropertiesSetToNull()
        {
            // Setup
            var soilLayer = new PipingSoilLayer(double.NaN)
            {
                BelowPhreaticLevelMean = double.NaN,
                BelowPhreaticLevelDeviation = double.NaN,
                BelowPhreaticLevelShift = double.NaN,
                DiameterD70Mean = double.NaN,
                DiameterD70CoefficientOfVariation = double.NaN,
                PermeabilityMean = double.NaN,
                PermeabilityCoefficientOfVariation = double.NaN
            };

            // Call
            PipingSoilLayerEntity entity = soilLayer.Create(0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.Top);
            Assert.IsNull(entity.BelowPhreaticLevelMean);
            Assert.IsNull(entity.BelowPhreaticLevelDeviation);
            Assert.IsNull(entity.BelowPhreaticLevelShift);
            Assert.IsNull(entity.DiameterD70Mean);
            Assert.IsNull(entity.DiameterD70CoefficientOfVariation);
            Assert.IsNull(entity.PermeabilityMean);
            Assert.IsNull(entity.PermeabilityCoefficientOfVariation);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string materialName = "MaterialName";
            var soilLayer = new PipingSoilLayer(0)
            {
                MaterialName = materialName
            };

            // Call
            PipingSoilLayerEntity entity = soilLayer.Create(0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(materialName, entity.MaterialName);
        }
    }
}