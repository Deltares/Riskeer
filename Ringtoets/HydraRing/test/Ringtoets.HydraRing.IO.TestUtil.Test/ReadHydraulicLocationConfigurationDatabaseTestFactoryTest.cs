// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Ringtoets.HydraRing.IO.TestUtil.Test
{
    [TestFixture]
    public class ReadHydraulicLocationConfigurationDatabaseTestFactoryTest
    {
        [Test]
        public void Create_ExpectedValues()
        {
            // Call
            ReadHydraulicLocationConfigurationDatabase database = ReadHydraulicLocationConfigurationDatabaseTestFactory.Create();

            // Assert
            Assert.AreEqual(2, database.LocationIdMappings.Count());
            Assert.IsNull(database.ReadHydraulicLocationConfigurationDatabaseSettings);

            var i = 1;
            foreach (ReadHydraulicLocationMapping databaseLocationIdMapping in database.LocationIdMappings)
            {
                Assert.AreEqual(i, databaseLocationIdMapping.HrdLocationId);
                Assert.AreEqual(i + 100, databaseLocationIdMapping.HlcdLocationId);
                i++;
            }
        }

        [Test]
        public void Create_WithLocationIds_ExpectedValues()
        {
            // Setup
            var locationsIds = new long[]
            {
                4,
                6,
                8
            };

            // Call
            ReadHydraulicLocationConfigurationDatabase database = ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(locationsIds);

            // Assert
            Assert.AreEqual(locationsIds.Length, database.LocationIdMappings.Count());
            Assert.IsNull(database.ReadHydraulicLocationConfigurationDatabaseSettings);

            var i = 0;
            foreach (ReadHydraulicLocationMapping databaseLocationIdMapping in database.LocationIdMappings)
            {
                Assert.AreEqual(locationsIds[i], databaseLocationIdMapping.HrdLocationId);
                Assert.AreEqual(locationsIds[i] + 100, databaseLocationIdMapping.HlcdLocationId);
                i++;
            }
        }

        [Test]
        public void CreateWithConfigurationSettings_ExpectedValues()
        {
            // Call
            ReadHydraulicLocationConfigurationDatabase database = ReadHydraulicLocationConfigurationDatabaseTestFactory.CreateWithConfigurationSettings();

            // Assert
            var i = 1;
            foreach (ReadHydraulicLocationMapping databaseLocationIdMapping in database.LocationIdMappings)
            {
                Assert.AreEqual(i, databaseLocationIdMapping.HrdLocationId);
                Assert.AreEqual(i + 100, databaseLocationIdMapping.HlcdLocationId);
                i++;
            }

            ReadHydraulicLocationConfigurationDatabaseSettings setting = database.ReadHydraulicLocationConfigurationDatabaseSettings.Single();
            Assert.AreEqual("scenarioName", setting.ScenarioName);
            Assert.AreEqual(1337, setting.Year);
            Assert.AreEqual("scope", setting.Scope);
            Assert.AreEqual("seaLevel", setting.SeaLevel);
            Assert.AreEqual("riverDischarge", setting.RiverDischarge);
            Assert.AreEqual("lakeLevel", setting.LakeLevel);
            Assert.AreEqual("windDirection", setting.WindDirection);
            Assert.AreEqual("windSpeed", setting.WindSpeed);
            Assert.AreEqual("comment", setting.Comment);
        }

        [Test]
        public void CreateWithConfigurationSettings_WithReadHydraulicLocationConfigurationDatabaseSettings_ExpectedValues()
        {
            // Setup
            IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> settings = Enumerable.Empty<ReadHydraulicLocationConfigurationDatabaseSettings>();

            // Call
            ReadHydraulicLocationConfigurationDatabase database = ReadHydraulicLocationConfigurationDatabaseTestFactory.CreateWithConfigurationSettings(settings);

            // Assert
            var i = 1;
            foreach (ReadHydraulicLocationMapping databaseLocationIdMapping in database.LocationIdMappings)
            {
                Assert.AreEqual(i, databaseLocationIdMapping.HrdLocationId);
                Assert.AreEqual(i + 100, databaseLocationIdMapping.HlcdLocationId);
                i++;
            }

            Assert.AreSame(settings, database.ReadHydraulicLocationConfigurationDatabaseSettings);
        }
    }
}