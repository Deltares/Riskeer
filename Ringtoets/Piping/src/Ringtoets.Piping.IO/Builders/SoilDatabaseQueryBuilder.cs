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

using Ringtoets.Piping.IO.SoilProfile;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// Defines queries to execute on the DSoil-Model database.
    /// </summary>
    public static class SoilDatabaseQueryBuilder
    {
        /// <summary>
        /// Returns the SQL query to execute to fetch Stochastic Soil Models 
        /// of the Piping Mechanism from the DSoil-Model database.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        /// <remarks><see cref="System.Data.SQLite.SQLiteParameter"/> ME_Name needs to be 
        /// defined as <see cref="MechanismDatabaseColumns.MechanismName"/>.</remarks>
        public static string GetStochasticSoilModelOfMechanismQuery()
        {
            return string.Format(@"SELECT SP.{8}, SP.{9}, S.{10}, SSM.{11}, SSM.{12} " +
                                 "FROM {0} M " +
                                 "INNER JOIN {1} S USING({4}) " +
                                 "INNER JOIN {2} SSM USING({5}) " +
                                 "INNER JOIN {3} SP USING({6}) " +
                                 "WHERE M.{7} = @{7} ORDER BY SSM.{12};",
                                 MechanismDatabaseColumns.TableName,
                                 SegmentDatabaseColumns.TableName,
                                 StochasticSoilModelDatabaseColumns.TableName,
                                 SegmentPointsDatabaseColumns.TableName,
                                 MechanismDatabaseColumns.MechanismId,
                                 StochasticSoilModelDatabaseColumns.StochasticSoilModelId,
                                 SegmentPointsDatabaseColumns.SegmentId,
                                 MechanismDatabaseColumns.MechanismName,
                                 SegmentPointsDatabaseColumns.CoordinateX,
                                 SegmentPointsDatabaseColumns.CoordinateY,
                                 SegmentDatabaseColumns.SegmentName,
                                 StochasticSoilModelDatabaseColumns.StochasticSoilModelName,
                                 StochasticSoilModelDatabaseColumns.StochasticSoilModelId
                );
        }

        /// <summary>
        /// Returns the query to get the amount of <see cref="Ringtoets.Piping.Data.StochasticSoilModel"/> 
        /// that can be read from the database.
        /// </summary>
        /// <returns>The query to get the amount of <see cref="Ringtoets.Piping.Data.StochasticSoilModel"/> 
        /// from the database.</returns>
        public static string GetStochasticSoilModelOfMechanismCountQuery()
        {
            return string.Format(@"SELECT COUNT('1') AS {8} " +
                                 "FROM (" +
                                 "SELECT '1' FROM {0} M " +
                                 "INNER JOIN {1} S USING({4}) " +
                                 "INNER JOIN {2} SSM USING({5}) " +
                                 "INNER JOIN {3} SP USING({6}) " +
                                 "WHERE M.{7} = @{7} GROUP BY {5}" +
                                 ");",
                                 MechanismDatabaseColumns.TableName,
                                 SegmentDatabaseColumns.TableName,
                                 StochasticSoilModelDatabaseColumns.TableName,
                                 SegmentPointsDatabaseColumns.TableName,
                                 MechanismDatabaseColumns.MechanismId,
                                 StochasticSoilModelDatabaseColumns.StochasticSoilModelId,
                                 SegmentPointsDatabaseColumns.SegmentId,
                                 MechanismDatabaseColumns.MechanismName,
                                 StochasticSoilModelDatabaseColumns.Count
                );
        }

        /// <summary>
        /// Returns the SQL query to execute to fetch all Stochastic Soil Profiles 
        /// from the DSoil-Model database.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetAllStochasticSoilProfileQuery()
        {
            return string.Format("SELECT {1}, {2}, {3}, {4} FROM {0} ORDER BY {1};",
                                 StochasticSoilProfileDatabaseColumns.TableName,
                                 StochasticSoilProfileDatabaseColumns.StochasticSoilModelId,
                                 StochasticSoilProfileDatabaseColumns.Probability,
                                 StochasticSoilProfileDatabaseColumns.SoilProfile1DId,
                                 StochasticSoilProfileDatabaseColumns.SoilProfile2DId
                );
        }

        /// <summary>
        /// Returns the query to get the amount of SoilProfile1D and SoilProfile2D
        /// that can be read from the database.
        /// </summary>
        /// <returns>The query to get the amount of SoilProfile1D and SoilProfile2D
        /// that can be read from the database.</returns>
        public static string GetPipingSoilProfileCountQuery()
        {
            return string.Format(
                "SELECT (" +
                "SELECT COUNT(DISTINCT sl1D.SP1D_ID) " +
                "FROM Mechanism AS m " +
                "JOIN Segment AS segment USING(ME_ID) " +
                "JOIN StochasticSoilProfile ssp USING(SSM_ID) " +
                "JOIN SoilLayer1D sl1D USING(SP1D_ID) " +
                "WHERE m.ME_Name = @{0}" +
                ") + (" +
                "SELECT COUNT(DISTINCT sl2D.SP2D_ID) " +
                "FROM Mechanism AS m " +
                "JOIN Segment AS segment USING(ME_ID) " +
                "JOIN StochasticSoilProfile ssp USING(SSM_ID) " +
                "JOIN SoilLayer2D sl2D USING(SP2D_ID) " +
                "JOIN MechanismPointLocation mpl USING(ME_ID, SP2D_ID) " +
                "WHERE m.ME_Name = @{0}" +
                ") " +
                "AS {1};",
                MechanismDatabaseColumns.MechanismName,
                SoilProfileDatabaseColumns.ProfileCount);
        }

        /// <summary>
        /// Returns the SQL query to execute to check if version of the DSoil-Model database is as expected.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        /// <remarks><see cref="System.Data.SQLite.SQLiteParameter"/> Value needs to be 
        /// defined as the required database version.</remarks>
        public static string GetCheckVersionQuery()
        {
            return string.Format(
                "SELECT {2} FROM {0} WHERE {1} = 'VERSION' AND {2} = @{2};",
                MetaDataDatabaseColumns.TableName,
                MetaDataDatabaseColumns.Key,
                MetaDataDatabaseColumns.Value
                );
        }
    }
}