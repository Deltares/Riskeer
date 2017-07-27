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

using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilDatabaseQueryBuilderTest
    {
        [Test]
        public void GetCheckVersionQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetCheckVersionQuery();

            // Assert
            const string expectedQuery =
                "SELECT Value " +
                "FROM _MetaData " +
                "WHERE Key = 'VERSION' " +
                "AND Value = @Value;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetSoilModelNamesUniqueQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetSoilModelNamesUniqueQuery();

            // Assert
            const string expectedQuery =
                "SELECT [All].nameCount == [Distinct].nameCount AS AreSegmentsUnique " +
                "FROM (SELECT COUNT(SSM_Name) nameCount FROM StochasticSoilModel) AS [All] " +
                "JOIN (SELECT COUNT(DISTINCT SSM_Name) nameCount FROM StochasticSoilModel) AS [Distinct];";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetStochasticSoilProfileProbabilitiesDefinedQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetStochasticSoilProfileProbabilitiesValidQuery();

            // Assert
            const string expectedQuery =
                "SELECT COUNT(Probability) == 0 AS AllProbabilitiesValid " +
                "FROM StochasticSoilProfile " +
                "WHERE Probability NOT BETWEEN 0 AND 1 OR Probability ISNULL;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetStochasticSoilModelOfMechanismQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetStochasticSoilModelOfMechanismQuery();

            // Assert
            const string expectedQuery =
                "SELECT M.ME_Name, SP.XWorld, SP.YWorld, S.SE_Name, SSM.SSM_Name, SSM.SSM_ID " +
                "FROM Mechanism M " +
                "INNER JOIN Segment S USING(ME_ID) " +
                "INNER JOIN StochasticSoilModel SSM USING(SSM_ID) " +
                "INNER JOIN SegmentPoints SP USING(SE_ID) " +
                "ORDER BY M.ME_Name, SSM.SSM_ID;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetStochasticSoilModelPerMechanismQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetStochasticSoilModelPerMechanismQuery();

            // Assert
            const string expectedQuery =
                "SELECT M.ME_Name, SSM.SSM_ID, SSM.SSM_Name, SSP.Probability, SSP.SP1D_ID, SSP.SP2D_ID " +
                "FROM Mechanism M " +
                "INNER JOIN Segment S USING(ME_ID) " +
                "INNER JOIN StochasticSoilModel SSM USING(SSM_ID) " +
                "INNER JOIN StochasticSoilProfile SSP USING(SSM_ID) " +
                "ORDER BY M.ME_Name, SSM.SSM_ID;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetSegmentPointsQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetSegmentPointsQuery();

            // Assert
            const string expectedQuery =
                "SELECT SSM.SSM_ID, SP.XWorld, SP.YWorld " +
                "FROM Segment S " +
                "INNER JOIN StochasticSoilModel SSM USING(SSM_ID) " +
                "INNER JOIN SegmentPoints SP USING(SE_ID) " +
                "ORDER BY SSM.SSM_ID;";
            Assert.AreEqual(expectedQuery, query);
        }
    }
}