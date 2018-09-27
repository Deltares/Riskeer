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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.HydraRing
{
    /// <summary>
    /// Validator to validate the Hydra-Ring settings database.
    /// </summary>
    internal class HydraRingSettingsDatabaseValidator : SqLiteDatabaseReaderBase
    {
        private readonly bool usePreprocessor;

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingSettingsDatabaseValidator"/>.
        /// </summary>
        /// <param name="databaseFilePath">The full path to the database file to use when reading settings.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Unable to open database file.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="preprocessorDirectory"/>
        /// is <c>null</c>.</exception>
        public HydraRingSettingsDatabaseValidator(string databaseFilePath, string preprocessorDirectory)
            : base(databaseFilePath)
        {
            if (preprocessorDirectory == null)
            {
                throw new ArgumentNullException(nameof(preprocessorDirectory));
            }

            usePreprocessor = preprocessorDirectory != string.Empty;
        }

        /// <summary>
        /// Verifies that the schema of the opened settings database is valid.
        /// </summary>
        /// <returns><c>true</c> when the schema is valid; <c>false</c> otherwise.</returns>
        public bool ValidateSchema()
        {
            return ContainsRequiredTables(GetColumnDefinitions(Connection));
        }

        private bool ContainsRequiredTables(IEnumerable<Tuple<string, string>> definitions)
        {
            return GetValidSchema().All(definitions.Contains);
        }

        private IEnumerable<Tuple<string, string>> GetValidSchema()
        {
            using (var validSchemaConnection = new SQLiteConnection("Data Source=:memory:"))
            using (SQLiteCommand command = validSchemaConnection.CreateCommand())
            {
                validSchemaConnection.Open();
                command.CommandText = usePreprocessor
                                          ? Resources.settings_schema_preprocessor
                                          : Resources.settings_schema;
                command.ExecuteNonQuery();
                return GetColumnDefinitions(validSchemaConnection);
            }
        }

        private static IEnumerable<Tuple<string, string>> GetColumnDefinitions(SQLiteConnection connection)
        {
            DataTable columns = connection.GetSchema("COLUMNS");

            var definitions = new List<Tuple<string, string>>();
            for (var i = 0; i < columns.Rows.Count; i++)
            {
                DataRow dataRow = columns.Rows[i];
                definitions.Add(
                    Tuple.Create(
                        ((string) dataRow["TABLE_NAME"]).ToLower(),
                        ((string) dataRow["COLUMN_NAME"]).ToLower()
                    ));
            }

            return definitions;
        }
    }
}