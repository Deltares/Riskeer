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

namespace Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase
{
    /// <summary>
    /// Defines queries to execute on a hydraulic location configuration database.
    /// </summary>
    public static class HydraulicLocationConfigurationDatabaseQueryBuilder
    {
        /// <summary>
        /// Gets the query to get location ids from the database.
        /// </summary>
        /// <returns>The query to get location ids from the database.</returns>
        public static string GetLocationIdsByTrackIdQuery()
        {
            return $"SELECT {LocationsTableDefinitions.LocationId}, {LocationsTableDefinitions.HrdLocationId} " +
                   $"FROM {LocationsTableDefinitions.TableName} " +
                   $"WHERE {LocationsTableDefinitions.TrackId} = @{LocationsTableDefinitions.TrackId} " +
                   $"ORDER BY {LocationsTableDefinitions.HrdLocationId};";
        }

        /// <summary>
        /// Gets the query to determine whether data related to the scenario information is present in the database.
        /// </summary>
        /// <returns>The query to determine the presence of the scenario information in the database.</returns>
        public static string GetIsScenarioInformationPresentQuery()
        {
            return $"SELECT COUNT() = 1 AS {ScenarioInformationTableDefinitions.IsScenarioInformationPresent} " +
                   "FROM sqlite_master WHERE type = 'table' " +
                   $"AND name='{ScenarioInformationTableDefinitions.TableName}';";
        }

        /// <summary>
        /// Gets the query to get the scenario information from the database.
        /// </summary>
        /// <returns>The query to get the scenario information from the database.</returns>
        public static string GetScenarioInformationQuery()
        {
            return $"SELECT {ScenarioInformationTableDefinitions.ScenarioName}, " +
                   $"{ScenarioInformationTableDefinitions.Year}, " +
                   $"{ScenarioInformationTableDefinitions.Scope}, " +
                   $"{ScenarioInformationTableDefinitions.SeaLevel}, " +
                   $"{ScenarioInformationTableDefinitions.RiverDischarge}, " +
                   $"{ScenarioInformationTableDefinitions.LakeLevel}, " +
                   $"{ScenarioInformationTableDefinitions.WindDirection}, " +
                   $"{ScenarioInformationTableDefinitions.WindSpeed}, " +
                   $"{ScenarioInformationTableDefinitions.Comment} " +
                   $"FROM {ScenarioInformationTableDefinitions.TableName};";
        }
    }
}