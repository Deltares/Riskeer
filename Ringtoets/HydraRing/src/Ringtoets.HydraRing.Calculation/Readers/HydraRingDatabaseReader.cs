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
using System.Data;
using System.Data.SQLite;
using System.IO;
using Core.Common.Util;

namespace Ringtoets.HydraRing.Calculation.Readers
{
    /// <summary>
    /// Class for reading the output database of a Hydra-Ring calculation.
    /// </summary>
    internal class HydraRingDatabaseReader : IDisposable
    {
        private readonly string workingDirectory;

        private SQLiteConnection connection;
        private SQLiteCommand command;
        private SQLiteDataReader reader;

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingDatabaseReader"/>.
        /// </summary>
        /// <param name="workingDirectory">The path to the directory which contains the
        /// output of the Hydra-Ring calculation.</param>
        /// <param name="query">The query to perform when reading the database.</param>
        /// <param name="sectionId">The section id to get the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="workingDirectory"/>
        /// or <paramref name="query"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="workingDirectory"/>
        /// <list type="bullet">
        /// <item>is zero-length, or</item>
        /// <item>contains only whitespace, or</item>
        /// <item>contains illegal characters, or</item>
        /// <item>contains a colon which is not part of a volume identifier, or</item>
        /// <item>is too long.</item>
        /// </list>
        /// </exception>
        /// <exception cref="SQLiteException">Thrown when the reader encounters
        /// an error while connecting to the database.</exception>
        public HydraRingDatabaseReader(string workingDirectory, string query, int sectionId)
        {
            if (workingDirectory == null)
            {
                throw new ArgumentNullException(nameof(workingDirectory));
            }

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            IOUtils.ValidateFilePath(workingDirectory);

            this.workingDirectory = workingDirectory;

            CreateConnection(sectionId);
            CreateCommand(query, sectionId);
            OpenConnection();
            GetReader();
        }

        /// <summary>
        /// Executes the query on the database and reads the next row.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> with the key 
        /// of the column and the value. Or <c>null</c> if no row could
        /// be read from the reader.</returns>
        public Dictionary<string, object> ReadLine()
        {
            if (reader.Read())
            {
                var results = new Dictionary<string, object>();

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    results.Add(reader.GetName(i), reader[i]);
                }

                return results;
            }

            return null;
        }

        /// <summary>
        /// Progresses the reader to the next result in the data set.
        /// </summary>
        /// <returns><c>true</c> if there was another result in the data set, <c>false</c>
        /// otherwise.</returns>
        public bool NextResult()
        {
            return reader.NextResult();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                reader?.Dispose();
                connection?.Dispose();
                command?.Dispose();
            }
        }

        private void CreateConnection(int sectionId)
        {
            string databaseFile = Path.Combine(workingDirectory, $"{sectionId}{HydraRingFileConstants.OutputDatabaseFileNameSuffix}");

            string connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                FailIfMissing = true,
                DataSource = databaseFile,
                ReadOnly = true
            }.ConnectionString;

            connection = new SQLiteConnection(connectionStringBuilder);
        }

        private void CreateCommand(string query, int sectionId)
        {
            command = new SQLiteCommand(query, connection);
            command.Parameters.Add(new SQLiteParameter
            {
                DbType = DbType.Int64,
                ParameterName = HydraRingDatabaseConstants.SectionIdParameterName,
                Value = sectionId
            });
        }

        private void GetReader()
        {
            reader = command.ExecuteReader();
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <exception cref="SQLiteException">Thrown when the connection could not be opened.</exception>
        private void OpenConnection()
        {
            connection.Open();
        }
    }
}