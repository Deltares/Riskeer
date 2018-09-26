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
using System.Globalization;
using Core.Common.IO.Exceptions;

namespace Core.Common.IO.Readers
{
    public static class IDataReaderExtensions
    {
        /// <summary>
        /// Reads a value at column <paramref name="columnName"/> from the database.
        /// </summary>
        /// <typeparam name="T">The expected type of value in the column with name <paramref name="columnName"/>.</typeparam>
        /// <param name="dataReader">The data reader from which to read a column of a certain type.</param>
        /// <param name="columnName">The name of the column to read from.</param>
        /// <returns>The read value from the column with name <paramref name="columnName"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="columnName"/> is not present in the read
        /// data row.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataReader"/> or 
        /// <paramref name="columnName"/> is <c>null</c>.</exception>
        /// <exception cref="ConversionException">Thrown when the value in the column could not be converted
        /// to type <typeparamref name="T"/>.</exception>
        public static T Read<T>(this IDataReader dataReader, string columnName)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException(nameof(dataReader));
            }

            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            Type conversionType = typeof(T);
            object value;

            try
            {
                value = dataReader[columnName];
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException($@"Column '{columnName}' not defined for data row.", nameof(columnName));
            }

            try
            {
                return (T) Convert.ChangeType(value, conversionType);
            }
            catch (InvalidCastException)
            {
                throw new ConversionException(
                    string.Format(CultureInfo.CurrentCulture, "Value read from data reader ('{0}') could not be cast to desired type {1}.",
                                  value,
                                  conversionType));
            }
            catch (FormatException)
            {
                throw new ConversionException(
                    string.Format(CultureInfo.CurrentCulture, "Value read from data reader ('{0}') is an incorrect format to transform to type {1}.",
                                  value,
                                  conversionType));
            }
            catch (OverflowException)
            {
                throw new ConversionException(
                    string.Format(CultureInfo.CurrentCulture, "Value read from data reader ('{0}') was too large to convert to type {1}.",
                                  value,
                                  conversionType));
            }
        }
    }
}