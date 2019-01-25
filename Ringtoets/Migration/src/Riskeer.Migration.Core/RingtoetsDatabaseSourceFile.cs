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
using System.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Storage;
using Core.Common.IO.Readers;
using Riskeer.Migration.Core.Properties;

namespace Riskeer.Migration.Core
{
    /// <summary>
    /// Class that provides methods for the migration database source file.
    /// </summary>
    public class RingtoetsDatabaseSourceFile : SqLiteDatabaseReaderBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RingtoetsDatabaseSourceFile"/> class.
        /// </summary>
        /// <param name="databaseFilePath">The path to the source file.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Unable to open database file.</item>
        /// </list>
        /// </exception>
        public RingtoetsDatabaseSourceFile(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Gets the version of the Ringtoets database source file at location <see cref="SqLiteDatabaseReaderBase.Path"/>.
        /// </summary>
        /// <returns>The version of the file.</returns>
        /// <exception cref="StorageValidationException">Thrown when the file is not a valid 
        /// Ringtoets database source file.</exception>
        public string GetVersion()
        {
            string versionQuery = ProjectDatabaseQueryBuilder.GetVersionQuery();
            try
            {
                using (IDataReader dataReader = CreateDataReader(versionQuery, null))
                {
                    if (!dataReader.Read())
                    {
                        return string.Empty;
                    }

                    return dataReader.Read<string>(VersionTableDefinitions.Version);
                }
            }
            catch (SystemException exception)
            {
                throw new StorageValidationException(string.Format(Resources.RingtoetsDatabaseSourceFile_Invalid_Ringtoets_File_Path_0,
                                                                   Path), exception);
            }
        }
    }
}