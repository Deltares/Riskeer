// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System.Data.SQLite;
using System.IO;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Base.Storage;
using Core.Common.Utils;
using Core.Common.Utils.Builders;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class interacts with an empty or new SQLite database file.
    /// </summary>
    public static class StorageSqliteCreator
    {
        /// <summary>
        /// Creates the basic database structure for a Ringtoets database file.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when executing <c>DatabaseStructure</c> script fails.</exception>
        public static void CreateDatabaseStructure(string databaseFilePath)
        {
            FileUtils.ValidateFilePath(databaseFilePath);

            if (!File.Exists(databaseFilePath))
            {
                SQLiteConnection.CreateFile(databaseFilePath);
            }
            var connectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(databaseFilePath);
            using (var dbContext = new SQLiteConnection(connectionString))
            {
                using (var command = dbContext.CreateCommand())
                {
                    try
                    {
                        dbContext.Open();
                        command.CommandText = Resources.DatabaseStructure;
                        command.ExecuteNonQuery();
                    }
                    catch (SQLiteException exception)
                    {
                        var message = new FileWriterErrorMessageBuilder(databaseFilePath).Build(Resources.Error_Write_Structure_to_Database);
                        throw new StorageException(message, new UpdateStorageException("", exception));
                    }
                }
            }
        }
    }
}