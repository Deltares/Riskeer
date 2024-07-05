﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using NUnit.Framework;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Riskeer.HydraRing.IO.Test.HydraulicLocationConfigurationDatabase
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabaseQueryBuilderTest
    {
        [Test]
        public void GetLocationsQuery_Always_ReturnsExpectedValue()
        {
            // Call
            string query = HydraulicLocationConfigurationDatabaseQueryBuilder.GetLocationsQuery();

            // Assert
            const string expectedQuery = "SELECT LocationId, HRDLocationId, TrackId FROM Locations;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetIsScenarioInformationPresentQuery_Always_ReturnsExpectedValue()
        {
            // Call
            string query = HydraulicLocationConfigurationDatabaseQueryBuilder.GetIsScenarioInformationPresentQuery();

            // Assert
            const string expectedQuery = "SELECT COUNT() = 1 AS IsScenarioInformationPresent FROM sqlite_master WHERE type = 'table' AND name='ScenarioInformation';";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetScenarioInformationQuery_Always_ReturnsExpectedValue()
        {
            // Call
            string query = HydraulicLocationConfigurationDatabaseQueryBuilder.GetScenarioInformationQuery();

            // Assert
            const string expectedQuery = "SELECT ScenarioName, Year, Scope, SeaLevel, RiverDischarge, LakeLevel, WindDirection, WindSpeed, Comment " +
                                         "FROM ScenarioInformation;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetTracksQuery_Always_ReturnsExpectedValue()
        {
            // Call
            string query = HydraulicLocationConfigurationDatabaseQueryBuilder.GetTracksQuery();

            // Assert
            const string expectedQuery = "SELECT * FROM Tracks LEFT JOIN Regions ON Tracks.RegionId = Regions.RegionId;";
            Assert.AreEqual(expectedQuery, query);
        }
    }
}