﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Properties;
using Ringtoets.HydraRing.IO;

namespace Ringtoets.HydraRing.Calculation.Readers
{
    /// <summary>
    /// Class for reading the output database of a Hydra-Ring calculation.
    /// </summary>
    internal class HydraRingDatabaseReader : IDisposable
    {
        private const string sectionIdParameterName = "@sectionId";
        private readonly string workingDirectory;
        private readonly SQLiteConnection connection;

        private SQLiteDataReader reader;
        private SQLiteCommand command;

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

            connection = CreateConnection();
            CreateCommand(query, sectionId);
            OpenConnection();
            GetReader();
        }

        /// <summary>
        /// Executes the query on the database and reads the next row.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> with the key 
        /// of the column and the value.</returns>
        /// <exception cref="HydraRingDatabaseReaderException">Thrown when 
        /// an error encounters while reading the database.</exception>
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

            throw new HydraRingDatabaseReaderException(Resources.HydraRingDatabaseReader_Execute_No_result_found_in_output_file);
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

        private void GetReader()
        {
            reader = command.ExecuteReader();
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <exception cref="SQLiteException">Thrown when 
        /// the connection could not be opened.</exception>
        private void OpenConnection()
        {
            connection.Open();
        }

        private void CreateCommand(string query, int sectionId)
        {
            command = new SQLiteCommand(query, connection);
            command.Parameters.Add(new SQLiteParameter
            {
                DbType = DbType.Int64,
                ParameterName = sectionIdParameterName,
                Value = sectionId
            });
        }

        private SQLiteConnection CreateConnection()
        {
            string databaseFile = Path.Combine(workingDirectory, HydraRingFileConstants.OutputDatabaseFileName);

            string connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                FailIfMissing = true,
                DataSource = databaseFile,
                ReadOnly = true
            }.ConnectionString;

            return new SQLiteConnection(connectionStringBuilder);
        }
    }
}