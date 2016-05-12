﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class HydraulicLocationEntityTest
    {
        [Test]
        public void Read_Always_ReturnsHydraulicBoundaryLocationWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            long testId = random.Next(0, 400);
            var entityId = new Random(21).Next(1, 502);
            var testName = "testName";
            double x = random.NextDouble();
            double y = random.NextDouble();
            var entity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = entityId,
                LocationId = testId,
                Name = testName,
                LocationX = Convert.ToDecimal(x),
                LocationY = Convert.ToDecimal(y)
            };

            // Call
            var location = entity.Read();

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual(entityId, location.StorageId);
            Assert.AreEqual(testId, location.Id);
            Assert.AreEqual(testName, location.Name);
            Assert.AreEqual(x, location.Location.X, 1e-6);
            Assert.AreEqual(y, location.Location.Y, 1e-6);
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

            // Call
            var location = entity.Read();

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual(expectedWaterLevel, location.DesignWaterLevel);
        } 
    }
}