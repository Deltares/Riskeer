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
using System.IO;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;

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
        /// <param name="databaseSchemaQuery">Script that containts the schema to execute on the database.</param>
        /// <exception cref="SQLiteException">Thrown when executing <paramref name="databaseSchemaQuery"/> failed.</exception>
        public static void CreateDatabaseFile(string databaseFilePath, string databaseSchemaQuery)
        {
            if (databaseSchemaQuery == null)
            {
                throw new ArgumentNullException("databaseSchemaQuery");
            }
            if (File.Exists(databaseFilePath))
            {
                TearDownTempFile(databaseFilePath);
            }

            SQLiteConnection.CreateFile(databaseFilePath);

            var connectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(databaseFilePath);
            using (var dbContext = new SQLiteConnection(connectionString))
            {
                using (var command = dbContext.CreateCommand())
                {
                    dbContext.Open();
                    command.CommandText = databaseSchemaQuery;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Converts the <paramref name="project"/> into a new Ringtoets database file.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="project"><see cref="Project"/> to save.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when:
        /// <list type="bullet">
        /// <item>The database does not contain the table <c>version</c></item>
        /// <item>THe file <paramref name="databaseFilePath"/> was not created.</item>
        /// <item>Saving the <paramref name="project"/> to the database failed.</item>
        /// <item>The connection to the database file failed.</item>
        /// </list>
        /// </exception>
        public static void CreateValidRingtoetsDatabase(string databaseFilePath, Project project)
        {
            var storageSqLite = new StorageSqLite();
            storageSqLite.SaveProjectAs(databaseFilePath, project);
        }

        /// <summary>
        /// Returns a corrupt databaseschema that will pass validation.
        /// </summary>
        /// <returns>The corrupt databaseschema that will pass validation.</returns>
        public static string GetCorruptSchema()
        {
            return "DROP TABLE IF EXISTS 'Version'; " +
                   "CREATE TABLE Version (VersionId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                   " FromVersion VARCHAR (16), ToVersion VARCHAR (16),Timestamp NUMERIC); ";
        }

        /// <summary>
        /// Removes the <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The file to delete.</param>
        public static void TearDownTempFile(string filePath)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}