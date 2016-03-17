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

using NUnit.Framework;
using Ringtoets.Piping.IO.Builders;

namespace Ringtoets.Piping.IO.Test.Builders
{
    [TestFixture]
    public class SoilDatabaseQueryBuilderTest
    {
        [Test]
        public void GetStochasticSoilModelOfMechanismQuery_Always_ReturnsExpectedValues()
        {
            // Setup
            const string expectedQuery = "SELECT SP.XWorld, SP.YWorld, S.SE_Name, SSM.SSM_Name, SSM.SSM_ID " +
                                         "FROM Mechanism M " +
                                         "INNER JOIN Segment S USING(ME_ID) " +
                                         "INNER JOIN StochasticSoilModel SSM USING(SSM_ID) " +
                                         "INNER JOIN SegmentPoints SP USING(SE_ID) " +
                                         "WHERE M.ME_Name = @ME_Name ORDER BY SSM.SSM_ID;";

            // Call
            string query = SoilDatabaseQueryBuilder.GetStochasticSoilModelOfMechanismQuery();

            // Assert
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetAllStochasticSoilProfileQuery_Always_ReturnsExpectedValues()
        {
            // Setup
            const string expectedQuery = "SELECT SSM_ID, Probability, SP1D_ID, SP2D_ID " +
                                         "FROM StochasticSoilProfile " +
                                         "ORDER BY SSM_ID;";

            // Call
            string query = SoilDatabaseQueryBuilder.GetAllStochasticSoilProfileQuery();

            // Assert
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetCheckVersionQuery_Always_ReturnsExpectedValues()
        {
            // Setup
            const string expectedQuery = "SELECT Value " +
                                         "FROM _MetaData " +
                                         "WHERE Key = 'VERSION' AND Value = @Value;";

            // Call
            string query = SoilDatabaseQueryBuilder.GetCheckVersionQuery();

            // Assert
            Assert.AreEqual(expectedQuery, query);
        }
    }
}