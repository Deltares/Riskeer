﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Core.Common.Base.IO;
using Core.Common.IO.Readers;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a DSoil database file and reads stochastic soil model from this database.
    /// </summary>
    public class StochasticSoilModelReader : SqLiteDatabaseReaderBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelReader"/>, 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>The database version could not be read.</item>
        /// <item>The database version is incorrect.</item>
        /// </list>
        /// </exception>
        public StochasticSoilModelReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Prepares a new data reader with a query for version validation.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The database version could not be read.</item>
        /// <item>The database version is incorrect.</item>
        /// </list>
        /// </exception>
        public void Initialize()
        {
            VerifyVersion(Path);
        }

        /// <summary>
        /// Verifies that the <paramref name="databaseFilePath"/> has the required version.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The database version could not be read.</item>
        /// <item>The database version is incorrect.</item>
        /// </list>
        /// </exception>
        private static void VerifyVersion(string databaseFilePath)
        {
            using (var reader = new SoilDatabaseVersionReader(databaseFilePath))
            {
                reader.VerifyVersion();
            }
        }
    }
}