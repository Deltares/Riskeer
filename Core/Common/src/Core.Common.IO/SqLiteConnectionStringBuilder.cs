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
using System.Data.SQLite;

namespace Core.Common.IO
{
    /// <summary>
    /// This class builds a connection string to a SQLite database file.
    /// </summary>
    public static class SqLiteConnectionStringBuilder
    {
        /// <summary>
        /// Constructs a connection string to connect to <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Location of the storage file.</param>
        /// <param name="readOnly">When <c>true</c>, the database will be opened for read-only 
        /// access and writing will be disabled.</param>
        /// <returns>A new connection string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is 
        /// <c>null</c> or empty (only whitespaces).</exception>
        public static string BuildSqLiteConnectionString(string filePath, bool readOnly)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                const string message = @"Cannot create a connection string without the path to the file to connect to.";
                throw new ArgumentNullException(nameof(filePath), message);
            }

            return new SQLiteConnectionStringBuilder
            {
                FailIfMissing = true,
                DataSource = filePath,
                ReadOnly = readOnly,
                ForeignKeys = true,
                Version = 3,
                Pooling = false
            }.ConnectionString;
        }
    }
}