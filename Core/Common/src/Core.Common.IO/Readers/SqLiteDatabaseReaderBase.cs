// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Common.Util.Builders;
using UtilResources = Core.Common.Util.Properties.Resources;

namespace Core.Common.IO.Readers
{
    /// <summary>
    /// Base class for database readers.
    /// </summary>
    public abstract class SqLiteDatabaseReaderBase : IDisposable
    {
        private bool disposed;

        /// <summary>
        /// Creates a new instance of <see cref="SqLiteDatabaseReaderBase"/> which will use the <paramref name="databaseFilePath"/>
        /// as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Unable to open database file.</item>
        /// </list>
        /// </exception>
        protected SqLiteDatabaseReaderBase(string databaseFilePath)
        {
            try
            {
                IOUtils.ValidateFilePath(databaseFilePath);
                Path = databaseFilePath;

                if (!File.Exists(databaseFilePath))
                {
                    string message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(UtilResources.Error_File_does_not_exist);
                    throw new CriticalFileReadException(message);
                }

                OpenConnection(databaseFilePath);
            }
            catch (ArgumentException e)
            {
                throw new CriticalFileReadException(e.Message, e);
            }
            catch (SQLiteException e)
            {
                throw new CriticalFileReadException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets the path to the file.
        /// </summary>
        public string Path { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the <see cref="Connection"/>.
        /// </summary>
        protected SQLiteConnection Connection { get; private set; }

        /// <summary>
        /// Closes and disposes the existing <see cref="Connection"/>.
        /// When <paramref name="disposing"/> is <c>true</c>, the managed resources are freed as well.
        /// </summary>
        /// <param name="disposing">Indicates whether the method call comes from the <see cref="Dispose"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            CloseConnection();

            disposed = true;
        }

        /// <summary>
        /// Moves to and reads the next result set in multiple row-returning SQL command. 
        /// </summary>
        /// <param name="sqliteDataReader">The <see cref="IDataReader"/> to process.</param>
        /// <returns><c>true</c> if the command was successful and a new result set is available, <c>false</c> otherwise.</returns>
        protected static bool MoveNext(IDataReader sqliteDataReader)
        {
            return sqliteDataReader.Read() || sqliteDataReader.NextResult() && sqliteDataReader.Read();
        }

        /// <summary>
        /// Creates a new <see cref="SQLiteDataReader"/>, based upon <paramref name="queryString"/> and <paramref name="parameters"/>.
        /// </summary>
        /// <param name="queryString">The query to execute.</param>
        /// <param name="parameters">Parameters the <paramref name="queryString"/> is dependent on.</param>
        /// <returns>A new instance of <see cref="IDataReader"/>.</returns>
        /// <exception cref="SQLiteException">Thrown when the execution of <paramref name="queryString"/> failed.</exception>
        protected IDataReader CreateDataReader(string queryString, params SQLiteParameter[] parameters)
        {
            using (var query = new SQLiteCommand(Connection)
            {
                CommandText = queryString
            })
            {
                if (parameters != null)
                {
                    query.Parameters.AddRange(parameters);
                }

                return query.ExecuteReader();
            }
        }

        private void CloseConnection()
        {
            if (Connection != null)
            {
                Connection.Close();
                Connection.Dispose();
                Connection = null;
            }
        }

        /// <summary>
        /// Opens the connection with the <paramref name="databaseFile"/>.
        /// </summary>
        /// <param name="databaseFile">The database file to establish a connection with.</param>
        private void OpenConnection(string databaseFile)
        {
            string connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                FailIfMissing = true,
                DataSource = databaseFile,
                ReadOnly = true,
                ForeignKeys = true
            }.ConnectionString;

            Connection = new SQLiteConnection(connectionStringBuilder, true);
            Connection.Open();
        }
    }
}