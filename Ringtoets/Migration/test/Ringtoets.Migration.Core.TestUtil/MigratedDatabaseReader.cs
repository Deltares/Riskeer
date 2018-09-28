// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Data;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using NUnit.Framework;

namespace Ringtoets.Migration.Core.TestUtil
{
    /// <summary>
    /// Database reader for a migrated database.
    /// </summary>
    public class MigratedDatabaseReader : SqLiteDatabaseReaderBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="MigratedDatabaseReader"/>.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Unable to open database file.</item>
        /// </list>
        /// </exception>
        public MigratedDatabaseReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Asserts that the <paramref name="queryString"/> results in one field with the value <c>true</c>.
        /// </summary>
        /// <param name="queryString">The query to execute.</param>
        /// <exception cref="System.Data.SQLite.SQLiteException">The execution of <paramref name="queryString"/> 
        /// failed.</exception>
        public void AssertReturnedDataIsValid(string queryString)
        {
            using (IDataReader dataReader = CreateDataReader(queryString))
            {
                Assert.IsTrue(dataReader.Read(), "No data can be read from the data reader " +
                                                 $"when using query '{queryString}'.");
                Assert.AreEqual(1, dataReader.FieldCount, $"Expected one field, was {dataReader.FieldCount} " +
                                                          $"fields when using query '{queryString}'.");
                Assert.AreEqual(1, dataReader[0], $"Result should be 1 when using query '{queryString}'.");
            }
        }
    }
}