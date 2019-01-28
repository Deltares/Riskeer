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

using NUnit.Framework;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;

namespace Riskeer.HydraRing.IO.Test.HydraulicBoundaryDatabase
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseQueryBuilderTest
    {
        [Test]
        public void GetVersionQuery_Always_ReturnsExpectedValues()
        {
            // Call
            string query = HydraulicBoundaryDatabaseQueryBuilder.GetVersionQuery();

            // Assert
            const string expectedQuery = "SELECT (NameRegion || CreationDate || TrackId) as GeneratedVersion FROM General LIMIT 0,1;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetTrackIdQuery_Always_ReturnsExpectedValues()
        {
            // Call
            string query = HydraulicBoundaryDatabaseQueryBuilder.GetTrackIdQuery();

            // Assert
            const string expectedQuery = "SELECT TrackId FROM General LIMIT 0,1;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetRelevantLocationsQuery_Always_ReturnsExpectedValues()
        {
            // Call
            string query = HydraulicBoundaryDatabaseQueryBuilder.GetRelevantLocationsQuery();

            // Assert
            const string expectedQuery = "SELECT HRDLocationId, Name, XCoordinate, YCoordinate FROM HRDLocations WHERE LocationTypeId > 1;";
            Assert.AreEqual(expectedQuery, query);
        }
    }
}