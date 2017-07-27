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

using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Defines queries to execute on the DSoil-Model database.
    /// </summary>
    internal static class SoilDatabaseQueryBuilder
    {
        /// <summary>
        /// Returns the SQL query to execute to check if version of the DSoil-Model database is as expected.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        /// <remarks><see cref="System.Data.SQLite.SQLiteParameter"/> "@<see cref="MetaTableDefinitions.Value"/>"
        /// needs to be defined as the required database version.</remarks>
        public static string GetCheckVersionQuery()
        {
            return $"SELECT {MetaTableDefinitions.Value} " +
                   $"FROM {MetaTableDefinitions.TableName} " +
                   $"WHERE {MetaTableDefinitions.Key} = 'VERSION' " +
                   $"AND {MetaTableDefinitions.Value} = @{MetaTableDefinitions.Value};";
        }

        /// <summary>
        /// Returns the SQL query to execute to check if segment names in the DSoil-Model database 
        /// are unique.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetSoilModelNamesUniqueQuery()
        {
            return string.Format(
                "SELECT [All].nameCount == [Distinct].nameCount AS {0} " +
                "FROM (SELECT COUNT({1}) nameCount FROM {2}) AS [All] " +
                "JOIN (SELECT COUNT(DISTINCT {1}) nameCount FROM {2}) AS [Distinct];",
                StochasticSoilModelTableDefinitions.AreSegmentsUnique,
                StochasticSoilModelTableDefinitions.StochasticSoilModelName,
                StochasticSoilModelTableDefinitions.TableName);
        }

        /// <summary>
        /// Returns the SQL query to execute to check if the probabilities of the stochastic
        /// soil profiles are valid.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetStochasticSoilProfileProbabilitiesValidQuery()
        {
            return string.Format(
                "SELECT COUNT({1}) == 0 AS {0} " +
                "FROM {2} " +
                "WHERE {1} NOT BETWEEN 0 AND 1 OR {1} ISNULL;",
                StochasticSoilProfileTableDefinitions.AllProbabilitiesValid,
                StochasticSoilProfileTableDefinitions.Probability,
                StochasticSoilProfileTableDefinitions.TableName);
        }

        /// <summary>
        /// Returns the SQL query to execute to fetch stochastic soil models 
        /// per failure mechanism from the DSoil-Model database.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetStochasticSoilModelOfMechanismQuery()
        {
            return string.Format(@"SELECT M.{7}, SP.{8}, SP.{9}, S.{10}, SSM.{11}, SSM.{12} " +
                                 "FROM {0} M " +
                                 "INNER JOIN {1} S USING({4}) " +
                                 "INNER JOIN {2} SSM USING({5}) " +
                                 "INNER JOIN {3} SP USING({6}) " +
                                 "ORDER BY M.{7}, SSM.{12};",
                                 MechanismTableDefinitions.TableName,
                                 SegmentTableDefinitions.TableName,
                                 StochasticSoilModelTableDefinitions.TableName,
                                 SegmentPointsTableDefinitions.TableName,
                                 MechanismTableDefinitions.MechanismId,
                                 StochasticSoilModelTableDefinitions.StochasticSoilModelId,
                                 SegmentPointsTableDefinitions.SegmentId,
                                 MechanismTableDefinitions.MechanismName,
                                 SegmentPointsTableDefinitions.CoordinateX,
                                 SegmentPointsTableDefinitions.CoordinateY,
                                 SegmentTableDefinitions.SegmentName,
                                 StochasticSoilModelTableDefinitions.StochasticSoilModelName,
                                 StochasticSoilModelTableDefinitions.StochasticSoilModelId
            );
        }

        /// <summary>
        /// Returns the SQL query to execute to fetch stochastic soil models 
        /// per failure mechanism from the DSoil-Model database.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetStochasticSoilModelPerMechanismQuery()
        {
            return $"SELECT M.{MechanismTableDefinitions.MechanismName}, " +
                   $"SSM.{StochasticSoilModelTableDefinitions.StochasticSoilModelId}, " +
                   $"SSM.{StochasticSoilModelTableDefinitions.StochasticSoilModelName}, " +
                   $"SSP.{StochasticSoilProfileTableDefinitions.Probability}, " +
                   $"SSP.{StochasticSoilProfileTableDefinitions.SoilProfile1DId}, " +
                   $"SSP.{StochasticSoilProfileTableDefinitions.SoilProfile2DId} " +
                   $"FROM {MechanismTableDefinitions.TableName} M " +
                   $"INNER JOIN {SegmentTableDefinitions.TableName} S USING({MechanismTableDefinitions.MechanismId}) " +
                   $"INNER JOIN {StochasticSoilModelTableDefinitions.TableName} SSM USING({StochasticSoilModelTableDefinitions.StochasticSoilModelId}) " +
                   $"INNER JOIN {StochasticSoilProfileTableDefinitions.TableName} SSP USING({StochasticSoilModelTableDefinitions.StochasticSoilModelId}) " +
                   $"ORDER BY M.{MechanismTableDefinitions.MechanismName}, SSM.{StochasticSoilModelTableDefinitions.StochasticSoilModelId};";
        }

        /// <summary>
        /// Returns the SQL query to execute to fetch segments and segment points
        /// per stochastic soil model from the DSoil-Model database.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetSegmentPointsQuery()
        {
            return $"SELECT SSM.{StochasticSoilModelTableDefinitions.StochasticSoilModelId}, " +
                   $"SP.{SegmentPointsTableDefinitions.CoordinateX}, " +
                   $"SP.{SegmentPointsTableDefinitions.CoordinateY} " +
                   $"FROM {SegmentTableDefinitions.TableName} S " +
                   $"INNER JOIN {StochasticSoilModelTableDefinitions.TableName} SSM USING({StochasticSoilModelTableDefinitions.StochasticSoilModelId}) " +
                   $"INNER JOIN {SegmentPointsTableDefinitions.TableName} SP USING({SegmentTableDefinitions.SegmentId}) " +
                   $"ORDER BY SSM.{StochasticSoilModelTableDefinitions.StochasticSoilModelId};";
        }
    }
}