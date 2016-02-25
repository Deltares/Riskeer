// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Data.SQLite;
using System.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Core.Common.IO.Readers
{
    /// <summary>
    /// Base class for database readers.
    /// </summary>
    public abstract class SqLiteDatabaseReaderBase : IDisposable
    {
        private readonly string fullFilePath;

        /// <summary>
        /// Creates a new instance of <see cref="SqLiteDatabaseReaderBase"/> which will use the <paramref name="databaseFilePath"/>
        /// as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Preparing the queries to read from the database failed.</item>
        /// </list>
        /// </exception>
        protected SqLiteDatabaseReaderBase(string databaseFilePath)
        {
            try
            {
                FileUtils.ValidateFilePath(databaseFilePath);
            }
            catch (ArgumentException e)
            {
                throw new CriticalFileReadException(e.Message, e);
            }
            if (!File.Exists(databaseFilePath))
            {
                var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(UtilsResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            fullFilePath = databaseFilePath;
            OpenConnection(databaseFilePath);
        }

        /// <summary>
        /// Gets the path to the file.
        /// </summary>
        public string Path
        {
            get
            {
                return fullFilePath;
            }
        }

        /// <summary>
        /// Closes and disposes the existing <see cref="Connection"/>.
        /// </summary>
        public virtual void Dispose()
        {
            if (Connection != null)
            {
                Connection.Close();
                Connection.Dispose();
            }
        }

        /// <summary>
        /// Gets the <see cref="Connection"/>.
        /// </summary>
        protected SQLiteConnection Connection { get; private set; }

        /// <summary>
        /// Opens the connection with the <paramref name="databaseFile"/>.
        /// </summary>
        /// <param name="databaseFile">The database file to establish a connection with.</param>
        private void OpenConnection(string databaseFile)
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                FailIfMissing = true,
                DataSource = databaseFile,
                ReadOnly = true,
                ForeignKeys = true
            }.ConnectionString;

            Connection = new SQLiteConnection(connectionStringBuilder);
            Connection.Open();
        }
    }
}