﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Riskeer.HydraRing.IO.Properties;

namespace Riskeer.HydraRing.IO.HydraulicBoundaryDatabase
{
    /// <summary>
    /// Class for reading a HRD file.
    /// </summary>
    public class HrdFileReader : SqLiteDatabaseReaderBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="HrdFileReader"/>.
        /// </summary>
        /// <param name="databaseFilePath">The path of the HRD file to read.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>the <paramref name="databaseFilePath"/> contains invalid characters;</item>
        /// <item>no file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public HrdFileReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Reads the HRD locations from the database.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when the database has an unsupported structure.</exception>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        public IEnumerable<ReadHrdLocation> ReadHrdLocations()
        {
            var readHrdLocations = new List<ReadHrdLocation>();

            try
            {
                using (IDataReader reader = CreateDataReader("SELECT HRDLocationId, Name, XCoordinate, YCoordinate " +
                                                             "FROM HRDLocations " +
                                                             "WHERE LocationTypeId > 1;"))
                {
                    while (MoveNext(reader))
                    {
                        readHrdLocations.Add(ReadHrdLocation(reader));
                    }

                    return readHrdLocations;
                }
            }
            catch (SQLiteException e)
            {
                throw new CriticalFileReadException(
                    new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabase_Unknown_database_structure),
                    e);
            }
        }

        /// <summary>
        /// Reads a HRD location from the database.
        /// </summary>
        /// <returns>A <see cref="ReadHrdLocation"/> based on the data read from the database.</returns>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        private ReadHrdLocation ReadHrdLocation(IDataReader reader)
        {
            try
            {
                return new ReadHrdLocation(reader.Read<long>("HRDLocationId"),
                                           reader.Read<string>("Name"),
                                           reader.Read<double>("XCoordinate"),
                                           reader.Read<double>("YCoordinate"));
            }
            catch (ConversionException e)
            {
                throw new LineParseException(
                    new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabase_Unexpected_value_on_column),
                    e);
            }
        }
    }
}