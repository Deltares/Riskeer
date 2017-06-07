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
using Ringtoets.MacroStabilityInwards.IO.Builders;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Builders
{
    [TestFixture]
    public class SoilDatabaseQueryBuilderTest
    {
        [Test]
        public void GetStochasticSoilModelOfMechanismQuery_Always_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetStochasticSoilModelOfMechanismQuery();

            // Assert
            const string expectedQuery = "SELECT SP.XWorld, SP.YWorld, S.SE_Name, SSM.SSM_Name, SSM.SSM_ID " +
                                         "FROM Mechanism M " +
                                         "INNER JOIN Segment S USING(ME_ID) " +
                                         "INNER JOIN StochasticSoilModel SSM USING(SSM_ID) " +
                                         "INNER JOIN SegmentPoints SP USING(SE_ID) " +
                                         "WHERE M.ME_Name = @ME_Name ORDER BY SSM.SSM_ID;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetAllStochasticSoilProfileQuery_Always_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetAllStochasticSoilProfileQuery();

            // Assert
            const string expectedQuery = "SELECT SSM_ID, Probability, SP1D_ID, SP2D_ID " +
                                         "FROM StochasticSoilProfile " +
                                         "ORDER BY SSM_ID;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetPipingSoilProfileCountQuery_Always_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetPipingSoilProfileCountQuery();

            // Assert
            const string expectedQuery = "SELECT (" +
                                         "SELECT COUNT(DISTINCT sl1D.SP1D_ID) " +
                                         "FROM Mechanism m " +
                                         "JOIN Segment segment USING(ME_ID) " +
                                         "JOIN StochasticSoilProfile ssp USING(SSM_ID) " +
                                         "JOIN SoilLayer1D sl1D USING(SP1D_ID) " +
                                         "WHERE m.ME_Name = @ME_Name" +
                                         ") + (" +
                                         "SELECT COUNT(DISTINCT sl2D.SP2D_ID) " +
                                         "FROM Mechanism m " +
                                         "JOIN Segment segment USING(ME_ID) " +
                                         "JOIN StochasticSoilProfile ssp USING(SSM_ID) " +
                                         "JOIN SoilLayer2D sl2D USING(SP2D_ID) " +
                                         "JOIN MechanismPointLocation mpl USING(ME_ID, SP2D_ID) " +
                                         "WHERE m.ME_Name = @ME_Name" +
                                         ") AS nrOfRows;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetStochasticSoilModelOfMechanismCountQuery_Always_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetStochasticSoilModelOfMechanismCountQuery();

            // Assert
            const string expectedQuery = "SELECT COUNT('1') AS nrOfRows FROM (" +
                                         "SELECT '1' FROM Mechanism M " +
                                         "INNER JOIN Segment S USING(ME_ID) " +
                                         "INNER JOIN StochasticSoilModel SSM USING(SSM_ID) " +
                                         "INNER JOIN SegmentPoints SP USING(SE_ID) " +
                                         "WHERE M.ME_Name = @ME_Name GROUP BY SSM_ID);";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetCheckVersionQuery_Always_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetCheckVersionQuery();

            // Assert
            const string expectedQuery = "SELECT Value " +
                                         "FROM _MetaData " +
                                         "WHERE Key = 'VERSION' AND Value = @Value;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetSoilModelNamesUniqueQuery_Always_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetSoilModelNamesUniqueQuery();

            // Assert
            const string expectedQuery =
                "SELECT [All].nameCount == [Distinct].nameCount as AreSegmentsUnique " +
                "FROM(SELECT COUNT(SSM_Name) nameCount FROM StochasticSoilModel) AS [All] " +
                "JOIN(SELECT COUNT(DISTINCT SSM_Name) nameCount FROM StochasticSoilModel) AS [Distinct];";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetStochasticSoilProfileProbabilitiesDefinedQuery_Always_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetStochasticSoilProfileProbabilitiesValidQuery();

            // Assert
            const string expectedQuery = "SELECT COUNT(Probability) == 0 as HasNoInvalidProbabilities " +
                                         "FROM StochasticSoilProfile " +
                                         "WHERE Probability NOT BETWEEN 0 AND 1 OR Probability ISNULL;";
            Assert.AreEqual(expectedQuery, query);
        }
    }
}