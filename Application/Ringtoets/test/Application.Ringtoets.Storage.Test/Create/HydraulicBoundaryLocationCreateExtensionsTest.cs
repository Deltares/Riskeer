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
using Application.Ringtoets.Storage.Create;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class HydraulicBoundaryLocationCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "testName", 2, 3);

            // Call
            TestDelegate test = () => hydraulicBoundaryLocation.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsHydraulicBoundaryLocationEntityWithPropertiesSet()
        {
            // Setup
            var testName = "testName";
            var random = new Random(21);
            var coordinateX = random.NextDouble();
            var coordinateY = random.NextDouble();
            var id = random.Next(0,150);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, testName, coordinateX, coordinateY);
            var registry = new PersistenceRegistry();

            // Call
            var entity = hydraulicBoundaryLocation.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(Convert.ToDecimal(coordinateX), entity.LocationX);
            Assert.AreEqual(Convert.ToDecimal(coordinateY), entity.LocationY);
            Assert.AreEqual(id, entity.LocationId);
            Assert.IsNull(entity.DesignWaterLevel);
        }

        [Test]
        public void Create_WithCollectorAndDesignWaterLevel_ReturnsHydraulicBoundaryLocationEntityWithDesignWaterLevelSet()
        {
            // Setup
            var random = new Random(21);
            var waterLevel = random.NextDouble();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "testName", random.NextDouble(), random.NextDouble())
            {
                DesignWaterLevel = waterLevel
            };
            var registry = new PersistenceRegistry();

            // Call
            var entity = hydraulicBoundaryLocation.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(Convert.ToDecimal(waterLevel), entity.DesignWaterLevel);
        }
    }
}