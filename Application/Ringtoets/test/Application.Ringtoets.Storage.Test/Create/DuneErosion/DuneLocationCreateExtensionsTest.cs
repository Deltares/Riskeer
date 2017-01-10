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
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.DuneErosion;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create.DuneErosion
{
    [TestFixture]
    public class DuneLocationCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var location = new TestDuneLocation();

            // Call
            TestDelegate test = () => location.Create(null, 0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_WithPersistenceRegistry_ReturnsDuneLocationEntityWithPropertiesSet()
        {
            // Setup
            var testName = "testName";
            var random = new Random(21);
            double coordinateX = random.NextDouble();
            double coordinateY = random.NextDouble();
            int id = random.Next(0, 150);
            int order = random.Next();
            var registry = new PersistenceRegistry();

            var location = new DuneLocation(id, testName, new Point2D(coordinateX, coordinateY),
                                            new DuneLocation.ConstructionProperties());

            // Call
            DuneLocationEntity entity = location.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(coordinateX, entity.LocationX);
            Assert.AreEqual(coordinateY, entity.LocationY);
            Assert.AreEqual(id, entity.LocationId);
            Assert.IsEmpty(entity.DuneLocationOutputEntities);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReferences()
        {
            // Setup
            var testName = "original name";
            var location = new DuneLocation(1, testName, new Point2D(0, 0), new DuneLocation.ConstructionProperties());
            var registry = new PersistenceRegistry();

            // Call
            DuneLocationEntity entity = location.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreNotSame(testName, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(testName, entity.Name);
        }

        [Test]
        public void Create_WithPersistenceRegistryAndInitializer_ReturnsDuneLocationEntityWithOutputSet()
        {
            // Setup
            var random = new Random(21);
            var duneLocationOutput = new DuneLocationOutput(random.NextEnumValue<CalculationConvergence>(),
                                                            new DuneLocationOutput.ConstructionProperties
                                                            {
                                                                WaterLevel = random.NextDouble(),
                                                                WaveHeight = random.NextDouble(),
                                                                WavePeriod = random.NextDouble(),
                                                                TargetProbability = random.NextDouble(),
                                                                TargetReliability = random.NextDouble(),
                                                                CalculatedProbability = random.NextDouble(),
                                                                CalculatedReliability = random.NextDouble()
                                                            });

            var location = new TestDuneLocation
            {
                Output = duneLocationOutput
            };
            var registry = new PersistenceRegistry();

            // Call
            DuneLocationEntity entity = location.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);

            DuneLocationOutputEntity duneLocationOutputEntity = entity.DuneLocationOutputEntities.SingleOrDefault();
            Assert.IsNotNull(duneLocationOutputEntity);
            AssertDuneBoundaryLocationOutput(duneLocationOutput, duneLocationOutputEntity);
        }

        [Test]
        public void Create_DuneLocationSavedMultipleTimes_ReturnSameEntity()
        {
            // Setup
            var location = new TestDuneLocation();

            var registry = new PersistenceRegistry();

            // Call
            DuneLocationEntity entity1 = location.Create(registry, 0);
            DuneLocationEntity entity2 = location.Create(registry, 1);

            // Assert
            Assert.AreSame(entity1, entity2);
        }

        private static void AssertDuneBoundaryLocationOutput(DuneLocationOutput output, DuneLocationOutputEntity entity)
        {
            Assert.AreEqual(output.WaterLevel, entity.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(output.WaveHeight, entity.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(output.WavePeriod, entity.WavePeriod, output.WavePeriod.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability, output.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(output.CalculationConvergence, (CalculationConvergence) entity.CalculationConvergence);
        }
    }
}