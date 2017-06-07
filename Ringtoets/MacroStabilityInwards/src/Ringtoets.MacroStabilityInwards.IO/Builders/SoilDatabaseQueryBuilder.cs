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

using Ringtoets.MacroStabilityInwards.IO.SoilProfile.Schema;

namespace Ringtoets.MacroStabilityInwards.IO.Builders
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
        /// defined as <see cref="MechanismTableColumns.MechanismName"/>.</remarks>
        public static string GetStochasticSoilModelOfMechanismQuery()
        {
            return string.Format(@"SELECT SP.{8}, SP.{9}, S.{10}, SSM.{11}, SSM.{12} " +
                                 "FROM {0} M " +
                                 "INNER JOIN {1} S USING({4}) " +
                                 "INNER JOIN {2} SSM USING({5}) " +
                                 "INNER JOIN {3} SP USING({6}) " +
                                 "WHERE M.{7} = @{7} ORDER BY SSM.{12};",
                                 MechanismTableColumns.TableName,
                                 SegmentTableColumns.TableName,
                                 StochasticSoilModelTableColumns.TableName,
                                 SegmentPointsTableColumns.TableName,
                                 MechanismTableColumns.MechanismId,
                                 StochasticSoilModelTableColumns.StochasticSoilModelId,
                                 SegmentPointsTableColumns.SegmentId,
                                 MechanismTableColumns.MechanismName,
                                 SegmentPointsTableColumns.CoordinateX,
                                 SegmentPointsTableColumns.CoordinateY,
                                 SegmentTableColumns.SegmentName,
                                 StochasticSoilModelTableColumns.StochasticSoilModelName,
                                 StochasticSoilModelTableColumns.StochasticSoilModelId
            );
        }

        /// <summary>
        /// Returns the query to get the amount of <see cref="Data.StochasticSoilModel"/> 
        /// that can be read from the database.
        /// </summary>
        /// <returns>The query to get the amount of <see cref="Data.StochasticSoilModel"/> 
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
                                 MechanismTableColumns.TableName,
                                 SegmentTableColumns.TableName,
                                 StochasticSoilModelTableColumns.TableName,
                                 SegmentPointsTableColumns.TableName,
                                 MechanismTableColumns.MechanismId,
                                 StochasticSoilModelTableColumns.StochasticSoilModelId,
                                 SegmentPointsTableColumns.SegmentId,
                                 MechanismTableColumns.MechanismName,
                                 StochasticSoilModelTableColumns.Count
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
                                 StochasticSoilProfileTableColumns.TableName,
                                 StochasticSoilProfileTableColumns.StochasticSoilModelId,
                                 StochasticSoilProfileTableColumns.Probability,
                                 StochasticSoilProfileTableColumns.SoilProfile1DId,
                                 StochasticSoilProfileTableColumns.SoilProfile2DId
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
                "SELECT COUNT(DISTINCT sl1D.{5}) " +
                "FROM {0} m " +
                "JOIN {1} segment USING({2}) " +
                "JOIN {3} ssp USING({4}) " +
                "JOIN {9} sl1D USING({5}) " +
                "WHERE m.{7} = @{7}" +
                ") + (" +
                "SELECT COUNT(DISTINCT sl2D.{6}) " +
                "FROM {0} m " +
                "JOIN {1} segment USING({2}) " +
                "JOIN {3} ssp USING({4}) " +
                "JOIN {10} sl2D USING({6}) " +
                "JOIN {8} mpl USING({2}, {6}) " +
                "WHERE m.{7} = @{7}" +
                ") " +
                "AS {11};",
                MechanismTableColumns.TableName,
                SegmentTableColumns.TableName,
                MechanismTableColumns.MechanismId,
                StochasticSoilProfileTableColumns.TableName,
                StochasticSoilProfileTableColumns.StochasticSoilModelId,
                StochasticSoilProfileTableColumns.SoilProfile1DId,
                StochasticSoilProfileTableColumns.SoilProfile2DId,
                MechanismTableColumns.MechanismName,
                MechanismPointLocationTableColumns.TableName,
                SoilLayer1DTableColumns.TableName,
                SoilLayer2DTableColumns.TableName,
                SoilProfileTableColumns.ProfileCount);
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
                MetaDataTableColumns.TableName,
                MetaDataTableColumns.Key,
                MetaDataTableColumns.Value
            );
        }

        /// <summary>
        /// Returns the SQL query to execute to check if segment names in the DSoil-Model database 
        /// are unique.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetSoilModelNamesUniqueQuery()
        {
            return string.Format(
                "SELECT [All].nameCount == [Distinct].nameCount as {0} " +
                "FROM(SELECT COUNT({1}) nameCount FROM {2}) AS [All] " +
                "JOIN(SELECT COUNT(DISTINCT {1}) nameCount FROM {2}) AS [Distinct];",
                StochasticSoilModelTableColumns.AreSegmentsUnique,
                StochasticSoilModelTableColumns.StochasticSoilModelName,
                StochasticSoilModelTableColumns.TableName);
        }

        /// <summary>
        /// Returns the SQL query to execute to check if the probabilities of the stochastic
        /// soil profiles are valid.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetStochasticSoilProfileProbabilitiesValidQuery()
        {
            return string.Format(
                "SELECT COUNT({1}) == 0 as {0} " +
                "FROM {2} " +
                "WHERE {1} NOT BETWEEN 0 AND 1 OR {1} ISNULL;",
                StochasticSoilProfileTableColumns.HasNoInvalidProbabilities,
                StochasticSoilProfileTableColumns.Probability,
                StochasticSoilProfileTableColumns.TableName);
        }
    }
}