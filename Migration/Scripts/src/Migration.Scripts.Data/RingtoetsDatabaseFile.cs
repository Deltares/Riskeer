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
using System.Data.SQLite;
using System.IO;
using Core.Common.IO;
using Core.Common.Utils;

namespace Migration.Scripts.Data
{
    /// <summary>
    /// Class that provides methods for the migration database target file.
    /// </summary>
    public class RingtoetsDatabaseFile : IDisposable
    {
        private readonly string filePath;
        private SQLiteConnection connection;
        private bool disposed;

        /// <summary>
        /// Creates a new instance of the <see cref="RingtoetsDatabaseFile"/> class.
        /// </summary>
        /// <param name="path">The path to the target file.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/>:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name),</item>
        /// <item>is not writable.</item>
        /// </list></exception>
        public RingtoetsDatabaseFile(string path)
        {
            IOUtils.ValidateFilePathIsWritable(path);
            filePath = path;
        }

        /// <summary>
        /// Opens the connection to the file.
        /// </summary>
        /// <remarks>Creates the file if it does not exist.</remarks>
        public void OpenDatabaseConnection()
        {
            if (!File.Exists(filePath))
            {
                SQLiteConnection.CreateFile(filePath);
            }
            connection = new SQLiteConnection(SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(filePath, false));
            connection.Open();
        }

        /// <summary>
        /// Executes the <paramref name="query"/> that will create the database structure.
        /// </summary>
        /// <param name="query">Create structure query to execute.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="query"/> is <c>null</c> 
        /// or consist out of only whitespace characters.</exception>
        /// <exception cref="SQLiteException">Thrown when executing <paramref name="query"/> failed.</exception>
        public void CreateStructure(string query)
        {
            PerformExecuteQuery(query);
        }

        /// <summary>
        /// Executes the <paramref name="query"/> that will migrate the data.
        /// </summary>
        /// <param name="query">Migration query to execute.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="query"/> is <c>null</c> 
        /// or consist out of only whitespace characters.</exception>
        /// <exception cref="SQLiteException">Thrown when executing <paramref name="query"/> failed.</exception>
        public void ExecuteMigration(string query)
        {
            PerformExecuteQuery(query);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void PerformExecuteQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException(@"Parameter must be a valid database script.", nameof(query));
            }
            using (var command = new SQLiteCommand(connection)
            {
                CommandText = query
            })
            {
                command.ExecuteNonQuery();
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                connection?.Close();
                connection?.Dispose();
                connection = null;
            }
            disposed = true;
        }
    }
}