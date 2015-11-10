using System;
using Ringtoets.Piping.IO.Exceptions;

namespace Ringtoets.Piping.IO.SoilProfile
{
    internal interface IRowBasedReader
    {
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
        /// current row that's being pointed at.
        /// </summary>
        /// <typeparam name="T">The type of object to read.</typeparam>
        /// <param name="columnName">The name of the column to read from.</param>
        /// <returns>The value in the column, or <c>null</c> if the value was <see cref="DBNull.Value"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when the value in the column could not be casted to type <typeparamref name="T"/>.</exception>
        T? ReadOrNull<T>(string columnName) where T : struct;
    }
}