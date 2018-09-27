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

using System;
using System.Data.SQLite;
using Core.Common.IO;
using NUnit.Framework;
using Ringtoets.Common.Util;
using Ringtoets.Integration.Data;
using Ringtoets.Storage.Core.Properties;

namespace Ringtoets.Storage.Core.TestUtil
{
    /// <summary>
    /// This class is used to prepare test databases for test.
    /// </summary>
    public static class SqLiteDatabaseHelper
    {
        /// <summary>
        /// Creates a corrupt database file based on <see cref="GetCompleteSchema"/>.
        /// </summary>
        /// <param name="databaseFilePath">The database file path.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="databaseFilePath"/> 
        /// is <c>null</c> or whitespace.</exception>
        public static void CreateCorruptDatabaseFile(string databaseFilePath)
        {
            CreateDatabaseFile(databaseFilePath, GetCorruptSchema());
            AddVersionEntity(databaseFilePath, RingtoetsVersionHelper.GetCurrentDatabaseVersion());
        }

        /// <summary>
        /// Creates the complete database file with a VersionEntity row but no project data.
        /// </summary>
        /// <param name="databaseFilePath">The database file path.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="databaseFilePath"/> 
        /// is <c>null</c> or whitespace.</exception>
        public static void CreateCompleteDatabaseFileWithoutProjectData(string databaseFilePath)
        {
            CreateCompleteDatabaseFileEmpty(databaseFilePath);
            AddVersionEntity(databaseFilePath, RingtoetsVersionHelper.GetCurrentDatabaseVersion());
        }

        /// <summary>
        /// Creates the complete database file without any rows in any table.
        /// </summary>
        /// <param name="databaseFilePath">The database file path.</param>
        public static void CreateCompleteDatabaseFileEmpty(string databaseFilePath)
        {
            CreateDatabaseFile(databaseFilePath, GetCompleteSchema());
        }

        /// <summary>
        /// Adds a row to the <c>VersionEntity</c> table with a given database version.
        /// </summary>
        /// <param name="databaseFilePath">The database file path.</param>
        /// <param name="databaseVersion">The database version.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="databaseFilePath"/> 
        /// is <c>null</c> or whitespace.</exception>
        public static void AddVersionEntity(string databaseFilePath, string databaseVersion)
        {
            string addVersionRowCommand = GetAddVersionRowCommandText(databaseVersion);
            PerformCommandOnDatabase(databaseFilePath, addVersionRowCommand);
        }

        /// <summary>
        /// Creates a new Sqlite database file with the structure defined in <paramref name="databaseSchemaQuery"/>.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="databaseSchemaQuery">Script that contains the schema to execute on the database.</param>
        /// <exception cref="SQLiteException">Thrown when executing <paramref name="databaseSchemaQuery"/> failed.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="databaseSchemaQuery"/>
        /// or <paramref name="databaseFilePath"/> is <c>null</c> or whitespace.</exception>
        public static void CreateDatabaseFile(string databaseFilePath, string databaseSchemaQuery)
        {
            if (string.IsNullOrWhiteSpace(databaseSchemaQuery))
            {
                throw new ArgumentNullException(nameof(databaseSchemaQuery));
            }

            if (string.IsNullOrWhiteSpace(databaseFilePath))
            {
                throw new ArgumentNullException(nameof(databaseFilePath));
            }

            SQLiteConnection.CreateFile(databaseFilePath);
            PerformCommandOnDatabase(databaseFilePath, databaseSchemaQuery);
        }

        /// <summary>
        /// Converts the <paramref name="project"/> into a new Ringtoets database file.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="project"><see cref="RingtoetsProject"/> to save.</param>
        public static void CreateValidRingtoetsDatabase(string databaseFilePath, RingtoetsProject project)
        {
            try
            {
                var storageSqLite = new StorageSqLite();
                storageSqLite.StageProject(project);
                storageSqLite.SaveProjectAs(databaseFilePath);
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
                   "CREATE TABLE 'VersionEntity' ('VersionId' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                   "'Version' VARCHAR(20) NOT NULL,'Timestamp' DATETIME NOT NULL, 'FingerPrint' BLOB NOT NULL);";
        }

        private static string GetCompleteSchema()
        {
            return Resources.DatabaseStructure;
        }

        /// <summary>
        /// Performs the command on a database.
        /// </summary>
        /// <param name="databaseFilePath">The file path to the database.</param>
        /// <param name="commandText">The command text/query.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="databaseFilePath"/> is <c>null</c> or whitespace.</exception>
        private static void PerformCommandOnDatabase(string databaseFilePath, string commandText)
        {
            string connectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(databaseFilePath, false);
            using (var dbContext = new SQLiteConnection(connectionString, true))
            {
                dbContext.Open();
                using (SQLiteCommand command = dbContext.CreateCommand())
                {
                    try
                    {
                        command.CommandText = commandText;
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        SQLiteConnection.ClearAllPools();
                        dbContext.Close();
                    }
                }
            }
        }

        private static string GetAddVersionRowCommandText(string databaseVersion)
        {
            return "INSERT INTO VersionEntity (Version, Timestamp, FingerPrint) "
                   + $"VALUES (\"{databaseVersion}\", '2016-08-10 10:55:48', 'QWERTY')";
        }
    }
}