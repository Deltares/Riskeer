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
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class HydraulicBoundaryLocationCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "testName", 2, 3);

            // Call
            TestDelegate test = () => hydraulicBoundaryLocation.Create(null, 0);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_WithPersistenceRegistry_ReturnsHydraulicLocationEntityWithPropertiesSet()
        {
            // Setup
            var testName = "testName";
            var random = new Random(21);
            var coordinateX = random.NextDouble();
            var coordinateY = random.NextDouble();
            var id = random.Next(0, 150);
            int order = random.Next();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, testName, coordinateX, coordinateY);
            var registry = new PersistenceRegistry();

            // Call
            var entity = hydraulicBoundaryLocation.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(coordinateX, entity.LocationX);
            Assert.AreEqual(coordinateY, entity.LocationY);
            Assert.AreEqual(id, entity.LocationId);
            Assert.IsEmpty(entity.HydraulicLocationOutputEntities);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReferences()
        {
            // Setup
            var testName = "original name";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, testName, 0, 0);
            var registry = new PersistenceRegistry();

            // Call
            HydraulicLocationEntity entity = hydraulicBoundaryLocation.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreNotSame(testName, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(testName, entity.Name);
        }

        [Test]
        public void Create_WithPersistenceRegistryAndInitializer_ReturnsHydraulicLocationEntityWithOutputSet(
            [Values(CalculationConvergence.CalculatedConverged, CalculationConvergence.CalculatedNotConverged,
                CalculationConvergence.NotCalculated)] CalculationConvergence designWaterLevelConvergence,
            [Values(CalculationConvergence.CalculatedConverged, CalculationConvergence.CalculatedNotConverged,
                CalculationConvergence.NotCalculated)] CalculationConvergence waveHeightConvergence)
        {
            // Setup
            var random = new Random(21);
            HydraulicBoundaryLocationOutput hydraulicBoundaryLocationDesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), designWaterLevelConvergence);
            HydraulicBoundaryLocationOutput hydraulicBoundaryLocationWaveHeightOutput = new HydraulicBoundaryLocationOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), waveHeightConvergence);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "testName", random.NextDouble(), random.NextDouble())
            {
                DesignWaterLevelOutput = hydraulicBoundaryLocationDesignWaterLevelOutput,
                WaveHeightOutput = hydraulicBoundaryLocationWaveHeightOutput
            };
            var registry = new PersistenceRegistry();

            // Call
            HydraulicLocationEntity entity = hydraulicBoundaryLocation.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);

            var designWaterLevelOutputEntity = GetHydraulicLocationOutputEntity(entity, HydraulicLocationOutputType.DesignWaterLevel);
            Assert.IsNotNull(designWaterLevelOutputEntity);
            AssertHydraulicBoundaryLocationOutput(hydraulicBoundaryLocationDesignWaterLevelOutput, designWaterLevelOutputEntity);

            var waveheightOutputEntity = GetHydraulicLocationOutputEntity(entity, HydraulicLocationOutputType.DesignWaterLevel);
            Assert.IsNotNull(waveheightOutputEntity);
            AssertHydraulicBoundaryLocationOutput(hydraulicBoundaryLocationDesignWaterLevelOutput, waveheightOutputEntity);
        }

        [Test]
        public void Create_HydraulicBoundaryLocationSavedMultipleTimes_ReturnSameEntity()
        {
            // Setup
            var hydraulicBoundaryLocations = new HydraulicBoundaryLocation(1, "A", 1.1, 2.2);

            var registry = new PersistenceRegistry();

            // Call
            HydraulicLocationEntity entity1 = hydraulicBoundaryLocations.Create(registry, 0);
            HydraulicLocationEntity entity2 = hydraulicBoundaryLocations.Create(registry, 1);

            // Assert
            Assert.AreSame(entity1, entity2);
        }

        [Test]
        public void CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "testName", 2, 3);

            // Call
            TestDelegate test = () => hydraulicBoundaryLocation.CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation(null, 0);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation_WithPersistenceRegistry_ReturnsGrassCoverErosionOutwardsHydraulicLocationEntityWithPropertiesSet()
        {
            // Setup
            var testName = "testName";
            var random = new Random(21);
            var coordinateX = random.NextDouble();
            var coordinateY = random.NextDouble();
            var id = random.Next(0, 150);
            int order = random.Next();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, testName, coordinateX, coordinateY);
            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionOutwardsHydraulicLocationEntity entity =
                hydraulicBoundaryLocation.CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(coordinateX, entity.LocationX);
            Assert.AreEqual(coordinateY, entity.LocationY);
            Assert.AreEqual(id, entity.LocationId);
            Assert.IsEmpty(entity.GrassCoverErosionOutwardsHydraulicLocationOutputEntities);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation_StringPropertiesDoNotShareReferences()
        {
            // Setup
            var testName = "original name";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, testName, 0, 0);
            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionOutwardsHydraulicLocationEntity entity =
                hydraulicBoundaryLocation.CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreNotSame(testName, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(testName, entity.Name);
        }

        [Test]
        public void CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation_WithPersistenceRegistryAndInitializer_ReturnsGrassCoverErosionOutwardsHydraulicLocationEntityWithOutputSet(
            [Values(CalculationConvergence.CalculatedConverged, CalculationConvergence.CalculatedNotConverged,
                CalculationConvergence.NotCalculated)] CalculationConvergence designWaterLevelConvergence,
            [Values(CalculationConvergence.CalculatedConverged, CalculationConvergence.CalculatedNotConverged,
                CalculationConvergence.NotCalculated)] CalculationConvergence waveHeightConvergence)
        {
            // Setup
            var random = new Random(21);
            HydraulicBoundaryLocationOutput hydraulicBoundaryLocationDesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), designWaterLevelConvergence);
            HydraulicBoundaryLocationOutput hydraulicBoundaryLocationWaveHeightOutput = new HydraulicBoundaryLocationOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), waveHeightConvergence);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "testName", random.NextDouble(), random.NextDouble())
            {
                DesignWaterLevelOutput = hydraulicBoundaryLocationDesignWaterLevelOutput,
                WaveHeightOutput = hydraulicBoundaryLocationWaveHeightOutput
            };
            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionOutwardsHydraulicLocationEntity entity =
                hydraulicBoundaryLocation.CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            var designWaterLevelOutputEntity = GetHydraulicLocationOutputEntity(entity, HydraulicLocationOutputType.DesignWaterLevel);
            Assert.IsNotNull(designWaterLevelOutputEntity);
            AssertHydraulicBoundaryLocationOutput(hydraulicBoundaryLocationDesignWaterLevelOutput, designWaterLevelOutputEntity);

            var waveheightOutputEntity = GetHydraulicLocationOutputEntity(entity, HydraulicLocationOutputType.DesignWaterLevel);
            Assert.IsNotNull(waveheightOutputEntity);
            AssertHydraulicBoundaryLocationOutput(hydraulicBoundaryLocationDesignWaterLevelOutput, waveheightOutputEntity);
        }

        [Test]
        public void CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation_HydraulicBoundaryLocationSavedMultipleTimes_ReturnSameEntity()
        {
            // Setup
            var hydraulicBoundaryLocations = new HydraulicBoundaryLocation(1, "A", 1.1, 2.2);

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionOutwardsHydraulicLocationEntity entity1 =
                hydraulicBoundaryLocations.CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation(registry, 0);
            GrassCoverErosionOutwardsHydraulicLocationEntity entity2 =
                hydraulicBoundaryLocations.CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation(registry, 1);

            // Assert
            Assert.AreSame(entity1, entity2);
        }

        private static IHydraulicLocationOutputEntity GetHydraulicLocationOutputEntity(
            HydraulicLocationEntity entity, HydraulicLocationOutputType outputType)
        {
            return entity.HydraulicLocationOutputEntities.SingleOrDefault(
                e => e.HydraulicLocationOutputType == (byte) outputType);
        }

        private static IHydraulicLocationOutputEntity GetHydraulicLocationOutputEntity(
            GrassCoverErosionOutwardsHydraulicLocationEntity entity, HydraulicLocationOutputType outputType)
        {
            return entity.GrassCoverErosionOutwardsHydraulicLocationOutputEntities.SingleOrDefault(
                e => e.HydraulicLocationOutputType == (byte) outputType);
        }

        private static void AssertHydraulicBoundaryLocationOutput(HydraulicBoundaryLocationOutput output, IHydraulicLocationOutputEntity entity)
        {
            Assert.AreEqual(output.Result, entity.Result, output.Result.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability, output.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(output.CalculationConvergence, (CalculationConvergence) entity.CalculationConvergence);
        }
    }
}