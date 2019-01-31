// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Storage;
using Core.Common.IO;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Riskeer.Storage.Core.Properties;

namespace Riskeer.Storage.Core
{
    /// <summary>
    /// This class interacts with an empty or new SQLite database file.
    /// </summary>
    public static class StorageSqliteCreator
    {
        /// <summary>
        /// Creates a new file with the basic database structure for a Riskeer database at
        /// <paramref name="databaseFilePath"/>.
        /// </summary>
        /// <param name="databaseFilePath">Path of the new database file.</param>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="databaseFilePath"/> is invalid</item>
        /// <item><paramref name="databaseFilePath"/> points to an existing file</item>
        /// </list></exception>
        /// <exception cref="StorageException">Thrown when executing <c>DatabaseStructure</c> script fails.</exception>
        public static void CreateDatabaseStructure(string databaseFilePath)
        {
            IOUtils.ValidateFilePath(databaseFilePath);

            if (File.Exists(databaseFilePath))
            {
                string message = $"File '{databaseFilePath}' already exists.";
                throw new ArgumentException(message);
            }

            SQLiteConnection.CreateFile(databaseFilePath);
            string connectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(databaseFilePath, false);
            try
            {
                using (var dbContext = new SQLiteConnection(connectionString, true))
                {
                    dbContext.Open();
                    using (SQLiteCommand command = dbContext.CreateCommand())
                    {
                        command.CommandText = Resources.DatabaseStructure;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException exception)
            {
                string message = new FileWriterErrorMessageBuilder(databaseFilePath).Build(Resources.Error_writing_structure_to_database);
                throw new StorageException(message, new UpdateStorageException("", exception));
            }
            finally
            {
                SQLiteConnection.ClearAllPools();
            }
        }
    }
}