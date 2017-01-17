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
using Migration.Core.Storage.Properties;

namespace Migration.Core.Storage
{
    public class TargetFile : IDisposable
    {
        private SQLiteConnection connection;

        public TargetFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            FilePath = filePath;
        }

        public string FilePath { get; }

        public void OpenDatabaseConnection()
        {
            SQLiteConnection.CreateFile(FilePath);
            connection = new SQLiteConnection(SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(FilePath, false));
            connection.Open();
        }

        public void CreateStructure()
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = Resources.DatabaseStructure;
                command.ExecuteNonQuery();
            }
        }

        public void Execute(string sql)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occured");
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void Dispose()
        {
            connection?.Dispose();
        }
    }
}