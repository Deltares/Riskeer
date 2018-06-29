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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Ringtoets.Migration.Core.Properties;

namespace Ringtoets.Migration.Core
{
    /// <summary>
    /// This class reads an SqLite database file and constructs <see cref="MigrationLogMessage"/> 
    /// instances from this database.
    /// </summary>
    public class MigrationLogDatabaseReader : SqLiteDatabaseReaderBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="MigrationLogDatabaseReader"/> that will use the 
        /// <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file was found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Unable to open the database file.</item>
        /// <item>The database contains invalid data.</item>
        /// </list>
        /// </exception>
        public MigrationLogDatabaseReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Gets the migration log messages from the database.
        /// </summary>
        /// <returns>The read log messages.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when trying to get the migration 
        /// messages failed.</exception>
        public ReadOnlyCollection<MigrationLogMessage> GetMigrationLogMessages()
        {
            string query = "SELECT " +
                           $"{MigrationLogEntityTableDefinitions.FromVersion}, " +
                           $"{MigrationLogEntityTableDefinitions.ToVersion}, " +
                           $"{MigrationLogEntityTableDefinitions.LogMessage} " +
                           $"FROM {MigrationLogEntityTableDefinitions.TableName}";

            try
            {
                using (IDataReader dataReader = CreateDataReader(query))
                {
                    return ReadMigrationLogMessages(dataReader).AsReadOnly();
                }
            }
            catch (SystemException e) when (e is ArgumentException || e is SQLiteException)
            {
                string message = Resources.MigrationLogDatabaseReader_GetMigrationLogMessages_failed;
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Reads and returns the log messages from the <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="dataReader">The <see cref="IDataReader"/> that is used to read the log 
        /// messages from.</param>
        /// <returns>The read log messages.</returns>
        /// <exception cref="ArgumentException">Thrown when the read data is <c>null</c> or empty.</exception>
        private static List<MigrationLogMessage> ReadMigrationLogMessages(IDataReader dataReader)
        {
            var messages = new List<MigrationLogMessage>();

            while (dataReader.Read())
            {
                string fromVersion = ReadStringOrNull(dataReader, MigrationLogEntityTableDefinitions.FromVersion);
                string toVersion = ReadStringOrNull(dataReader, MigrationLogEntityTableDefinitions.ToVersion);
                string message = ReadStringOrNull(dataReader, MigrationLogEntityTableDefinitions.LogMessage);

                messages.Add(new MigrationLogMessage(fromVersion, toVersion, message));
            }

            return messages;
        }

        private static string ReadStringOrNull(IDataReader dataReader, string columnName)
        {
            object valueObject = dataReader[columnName];
            if (valueObject.Equals(DBNull.Value))
            {
                return null;
            }

            return (string) valueObject;
        }
    }
}