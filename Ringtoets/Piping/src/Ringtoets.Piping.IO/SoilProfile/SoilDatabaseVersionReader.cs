﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class reads a soil database file and reads version from this database.
    /// </summary>
    public class SoilDatabaseVersionReader : SqLiteDatabaseReaderBase
    {
        private const string databaseRequiredVersion = "15.0.6.0";

        /// <summary>
        /// Creates a new instance of <see cref="SoilDatabaseVersionReader"/>, 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>The database version could not be read.</item>
        /// <item>The database version is incorrect.</item>
        /// </list></exception>
        public SoilDatabaseVersionReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Verifies if the database has the required version.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when: <list type="bullet">
        /// <item>The database version could not be read.</item>
        /// <item>The database version is incorrect.</item>
        /// </list></exception>
        public void VerifyVersion()
        {
            var checkVersionQuery = SoilDatabaseQueryBuilder.GetCheckVersionQuery();
            var sqliteParameter = new SQLiteParameter
            {
                DbType = DbType.String,
                ParameterName = string.Format("@{0}", MetaDataDatabaseColumns.Value),
                Value = databaseRequiredVersion
            };

            try
            {
                ReadVersion(checkVersionQuery, sqliteParameter);
            }
            catch (SQLiteException exception)
            {
                var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column);
                throw new CriticalFileReadException(message, exception);
            }
        }

        private void ReadVersion(string checkVersionQuery, SQLiteParameter sqliteParameter)
        {
            using (SQLiteDataReader dataReader = CreateDataReader(checkVersionQuery, sqliteParameter))
            {
                if (!dataReader.HasRows)
                {
                    throw new CriticalFileReadException(string.Format(
                        Resources.PipingSoilProfileReader_Database_incorrect_version_requires_Version_0_,
                        databaseRequiredVersion));
                }
            }
        }
    }
}