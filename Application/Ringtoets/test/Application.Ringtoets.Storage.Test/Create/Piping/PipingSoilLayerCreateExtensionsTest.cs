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
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.Piping
{
    [TestFixture]
    public class PipingSoilLayerCreateExtensionsTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCollector_ReturnsFailureMechanismEntityWithPropertiesSet(bool isAquifer)
        {
            // Setup
            var random = new Random(21);
            double top = random.NextDouble();
            int order = random.Next();
            var soilLayer = new PipingSoilLayer(top)
            {
                IsAquifer = isAquifer,
                Color = Color.AliceBlue,
                MaterialName = "MaterialName",
                BelowPhreaticLevelMean = random.NextDouble(),
                BelowPhreaticLevelDeviation = random.NextDouble(),
                BelowPhreaticLevelShift = random.NextDouble(),
                DiameterD70Mean = double.NaN,
                DiameterD70CoefficientOfVariation = double.NaN,
                PermeabilityMean = random.NextDouble(),
                PermeabilityCoefficientOfVariation = random.NextDouble()
            };

            // Call
            PipingSoilLayerEntity entity = soilLayer.Create(order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(top, entity.Top);
            Assert.AreEqual(Convert.ToByte(isAquifer), entity.IsAquifer);
            Assert.AreEqual(soilLayer.Color.ToArgb(), Convert.ToInt32(entity.Color));
            Assert.AreEqual(soilLayer.BelowPhreaticLevelMean.ToNaNAsNull(), entity.BelowPhreaticLevelMean);
            Assert.AreEqual(soilLayer.BelowPhreaticLevelDeviation.ToNaNAsNull(), entity.BelowPhreaticLevelDeviation);
            Assert.AreEqual(soilLayer.BelowPhreaticLevelShift.ToNaNAsNull(), entity.BelowPhreaticLevelShift);
            Assert.AreEqual(soilLayer.DiameterD70Mean.ToNaNAsNull(), entity.DiameterD70Mean);
            Assert.AreEqual(soilLayer.DiameterD70CoefficientOfVariation.ToNaNAsNull(), entity.DiameterD70CoefficientOfVariation);
            Assert.AreEqual(soilLayer.PermeabilityMean.ToNaNAsNull(), entity.PermeabilityMean);
            Assert.AreEqual(soilLayer.PermeabilityCoefficientOfVariation.ToNaNAsNull(), entity.PermeabilityCoefficientOfVariation);
            Assert.AreEqual(order, entity.Order);
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
            Assert.AreNotSame(materialName, entity.MaterialName,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(materialName, entity.MaterialName);
        }
    }
}