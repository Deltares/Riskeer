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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class HydraulicLocationEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new HydraulicLocationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_WithCollector_ReturnsHydraulicBoundaryLocationWithPropertiesSetAndEntityRegistered()
        {
            // Setup
            var random = new Random(21);
            long testId = random.Next(0, 400);
            var testName = "testName";
            double x = random.NextDouble();
            double y = random.NextDouble();
            var entity = new HydraulicLocationEntity
            {
                LocationId = testId,
                Name = testName,
                LocationX = x,
                LocationY = y,
                DesignWaterLevelCalculationConvergence = (byte) CalculationConvergence.CalculatedConverged,
                WaveHeightCalculationConvergence = (byte) CalculationConvergence.CalculatedConverged
            };

            var collector = new ReadConversionCollector();

            // Call
            var location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual(testId, location.Id);
            Assert.AreEqual(testName, location.Name);
            Assert.AreEqual(x, location.Location.X, 1e-6);
            Assert.AreEqual(y, location.Location.Y, 1e-6);
            Assert.IsNaN(location.DesignWaterLevel);
            Assert.IsNaN(location.WaveHeight);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, location.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, location.WaveHeightCalculationConvergence);

            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        [TestCase(null, double.NaN)]
        [TestCase(double.MaxValue, double.MaxValue)]
        [TestCase(double.MinValue, double.MinValue)]
        [TestCase(1.5, 1.5)]
        [TestCase(double.NaN, double.NaN)]
        public void Read_DifferentDesignWaterLevel_ReturnHydraulicBoundaryLocationWithExpectedWaterLevel(double? waterLevel, double expectedWaterLevel)
        {
            // Setup
            var entity = new HydraulicLocationEntity
            {
                Name = "someName",
                DesignWaterLevel = waterLevel
            };

            var collector = new ReadConversionCollector();

            // Call
            var location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual((RoundedDouble) expectedWaterLevel, location.DesignWaterLevel, location.DesignWaterLevel.GetAccuracy());
        }

        [Test]
        [TestCase(null, double.NaN)]
        [TestCase(double.MaxValue, double.MaxValue)]
        [TestCase(double.MinValue, double.MinValue)]
        [TestCase(1.5, 1.5)]
        [TestCase(double.NaN, double.NaN)]
        public void Read_DifferentWaveHeight_ReturnHydraulicBoundaryLocationWithExpectedWaveHeight(double? waveHeight, double expectedWaveHeight)
        {
            // Setup
            var entity = new HydraulicLocationEntity
            {
                Name = "someName",
                WaveHeight = waveHeight
            };

            var collector = new ReadConversionCollector();

            // Call
            var location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual((RoundedDouble) expectedWaveHeight, location.WaveHeight, location.WaveHeight.GetAccuracy());
        }

        [Test]
        public void Read_SameHydraulicLocationEntityTwice_ReturnSameHydraulicBoundaryLocation()
        {
            // Setup
            var entity = new HydraulicLocationEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();

            // Call
            HydraulicBoundaryLocation location1 = entity.Read(collector);
            HydraulicBoundaryLocation location2 = entity.Read(collector);

            // Assert
            Assert.AreSame(location1, location2);
        }
    }
}