// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Create;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class PipingSoilLayerCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var soilLayer = new PipingSoilLayer(new Random(21).NextDouble());

            // Call
            TestDelegate test = () => soilLayer.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCollector_ReturnsFailureMechanismEntityWithPropertiesSet(bool isAquifer)
        {
            // Setup
            double top = new Random(21).NextDouble();
            var random = new Random(21);
            var soilLayer = new PipingSoilLayer(top)
            {
                IsAquifer = isAquifer,
                Color = Color.AliceBlue,
                MaterialName = "MaterialName",
                BelowPhreaticLevelMean = random.NextDouble(),
                BelowPhreaticLevelDeviation = random.NextDouble(),
                BelowPhreaticLevelShift = random.NextDouble(),
                DiameterD70Mean = double.NaN,
                DiameterD70Deviation = double.NaN,
                PermeabilityMean = random.NextDouble(),
                PermeabilityDeviation = random.NextDouble()

            };
            var registry = new PersistenceRegistry();

            // Call
            var entity = soilLayer.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(Convert.ToDecimal(top), entity.Top);
            Assert.AreEqual(Convert.ToByte(isAquifer), entity.IsAquifer);
            Assert.AreEqual(soilLayer.Color.ToArgb(), Convert.ToInt32(entity.Color));
            Assert.AreEqual(soilLayer.BelowPhreaticLevelMean.ToNaNAsNull(), entity.BelowPhreaticLevelMean);
            Assert.AreEqual(soilLayer.BelowPhreaticLevelDeviation.ToNaNAsNull(), entity.BelowPhreaticLevelDeviation);
            Assert.AreEqual(soilLayer.BelowPhreaticLevelShift.ToNaNAsNull(), entity.BelowPhreaticLevelShift);
            Assert.AreEqual(soilLayer.DiameterD70Mean.ToNaNAsNull(), entity.DiameterD70Mean);
            Assert.AreEqual(soilLayer.DiameterD70Deviation.ToNaNAsNull(), entity.DiameterD70Deviation);
            Assert.AreEqual(soilLayer.PermeabilityMean.ToNaNAsNull(), entity.PermeabilityMean);
            Assert.AreEqual(soilLayer.PermeabilityDeviation.ToNaNAsNull(), entity.PermeabilityDeviation);
        }
    }
}