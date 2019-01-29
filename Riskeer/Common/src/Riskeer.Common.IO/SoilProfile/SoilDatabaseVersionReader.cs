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
using System.Data.SQLite;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Riskeer.Common.IO.Properties;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a soil database file and reads version from this database.
    /// </summary>
    public class SoilDatabaseVersionReader : SqLiteDatabaseReaderBase
    {
        private const string databaseRequiredVersion = "17.2.0.0";

        /// <summary>
        /// Creates a new instance of <see cref="SoilDatabaseVersionReader"/> 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public SoilDatabaseVersionReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Verifies if the database has the required version.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The database version could not be read.</item>
        /// <item>The database version is incorrect.</item>
        /// </list>
        /// </exception>
        public void VerifyVersion()
        {
            string checkVersionQuery = SoilDatabaseQueryBuilder.GetCheckVersionQuery();
            var sqliteParameter = new SQLiteParameter
            {
                DbType = DbType.String,
                ParameterName = $"@{MetaTableDefinitions.Value}",
                Value = databaseRequiredVersion
            };

            try
            {
                ReadVersion(checkVersionQuery, sqliteParameter);
            }
            catch (SQLiteException exception)
            {
                string exceptionMessage = new FileReaderErrorMessageBuilder(Path).Build(
                    Resources.SoilProfileReader_Critical_Unexpected_value_on_column);
                throw new CriticalFileReadException(exceptionMessage, exception);
            }
        }

        /// <summary>
        /// Reads if the required version was found in the database.
        /// </summary>
        /// <param name="checkVersionQuery">The query to execute.</param>
        /// <param name="sqliteParameter">The parameter containing the required version.</param>
        /// <exception cref="SQLiteException">Thrown when the execution of <paramref name="checkVersionQuery"/> 
        /// failed.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when the database version is incorrect.</exception>
        private void ReadVersion(string checkVersionQuery, SQLiteParameter sqliteParameter)
        {
            using (IDataReader dataReader = CreateDataReader(checkVersionQuery, sqliteParameter))
            {
                if (!dataReader.Read())
                {
                    string exceptionMessage = new FileReaderErrorMessageBuilder(Path).Build(
                        string.Format(Resources.SoilProfileReader_Database_incorrect_version_requires_Version_0,
                                      databaseRequiredVersion));
                    throw new CriticalFileReadException(exceptionMessage);
                }
            }
        }
    }
}