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

using System;
using System.Data.SQLite;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Base.Data;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.TestUtil
{
    /// <summary>
    /// This class is used to prepare test databases for test.
    /// </summary>
    public static class SqLiteDatabaseHelper
    {
        /// <summary>
        /// Creates a new Sqlite database file with the structure defined in <paramref name="databaseSchemaQuery"/>.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="databaseSchemaQuery">Script that contains the schema to execute on the database.</param>
        /// <exception cref="SQLiteException">Thrown when executing <paramref name="databaseSchemaQuery"/> failed.</exception>
        public static void CreateDatabaseFile(string databaseFilePath, string databaseSchemaQuery)
        {
            if (databaseSchemaQuery == null)
            {
                throw new ArgumentNullException("databaseSchemaQuery");
            }

            SQLiteConnection.CreateFile(databaseFilePath);

            var connectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(databaseFilePath);
            using (var dbContext = new SQLiteConnection(connectionString, true))
            {
                dbContext.Open();
                using (var command = dbContext.CreateCommand())
                {
                    try
                    {
                        command.CommandText = databaseSchemaQuery;
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        SQLiteConnection.ClearAllPools();
                    }
                }
                dbContext.Close();
            }
        }

        /// <summary>
        /// Converts the <paramref name="project"/> into a new Ringtoets database file.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="project"><see cref="Project"/> to save.</param>
        public static void CreateValidRingtoetsDatabase(string databaseFilePath, Project project)
        {
            try
            {
                var storageSqLite = new StorageSqLite();
                storageSqLite.SaveProjectAs(databaseFilePath, project);
            }
            catch (Exception exception)
            {
                Assert.Fail("Precondition failed: creating database file failed due to {0}", exception);
            }
            finally
            {
                SQLiteConnection.ClearAllPools();
            }
        }

        /// <summary>
        /// Returns a corrupt database schema that will pass validation.
        /// </summary>
        /// <returns>The corrupt database schema that will pass validation.</returns>
        public static string GetCorruptSchema()
        {
            return "DROP TABLE IF EXISTS 'VersionEntity'; " +
                   "CREATE TABLE VersionEntity (VersionId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                   " FromVersion VARCHAR (16), ToVersion VARCHAR (16),Timestamp NUMERIC); ";
        }

        /// <summary>
        /// Returns a corrupt database schema that will pass validation.
        /// </summary>
        /// <returns>The corrupt database schema that will pass validation.</returns>
        public static string GetCompleteSchema()
        {
            return Resources.DatabaseStructure;
        }
    }
}