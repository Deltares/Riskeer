// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.IO;

namespace Core.Common.IO.Readers
{
    /// <summary>
    /// This interface can be used for data bases to implement a row/column based way of reading records.
    /// </summary>
    public interface IRowBasedDatabaseReader
    {
        /// <summary>
        /// Gets the path of the database being read.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Moves the reader to the next record in the database.
        /// </summary>
        void MoveNext();

        /// <summary>
        /// Reads a value at column <paramref name="columnName"/> from the database.
        /// </summary>
        /// <typeparam name="T">The expected type of value in the column with name <paramref name="columnName"/>.</typeparam>
        /// <param name="columnName">The name of the column to read from.</param>
        /// <returns>The read value from the column with name <paramref name="columnName"/>.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the value in the column was not of type <typeparamref name="T"/>.</exception>
        T Read<T>(string columnName);

        /// <summary>
        /// Reads the value in the column with name <paramref name="columnName"/> from the 
        /// current row that's being pointed at. If the column has a value of <see cref="DBNull.Value"/>,
        /// then the default value for <typeparamref name="T"/> is returned.
        /// </summary>
        /// <typeparam name="T">The type of object to read.</typeparam>
        /// <param name="columnName">The name of the column to read from.</param>
        /// <returns>The value in the column, or <c>null</c> if the value was <see cref="DBNull.Value"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when the value in the column could not be casted to type <typeparamref name="T"/>.</exception>
        T ReadOrDefault<T>(string columnName);
    }
}