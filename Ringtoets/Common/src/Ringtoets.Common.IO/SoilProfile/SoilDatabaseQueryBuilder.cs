﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
    }
}