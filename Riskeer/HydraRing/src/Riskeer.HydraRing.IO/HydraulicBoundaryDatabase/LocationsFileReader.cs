// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;

namespace Riskeer.HydraRing.IO.HydraulicBoundaryDatabase
{
    /// <summary>
    /// Class for reading a locations file.
    /// </summary>
    public class LocationsFileReader : SqLiteDatabaseReaderBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="LocationsFileReader"/>.
        /// </summary>
        /// <param name="databaseFilePath">The path of the locations file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public LocationsFileReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Reads the locations from the database.
        /// </summary>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        public IEnumerable<ReadLocation> ReadLocations()
        {
            using (IDataReader reader = CreateDataReader(GetLocationsQuery()))
            {
                while (MoveNext(reader))
                {
                    yield return new ReadLocation(reader.Read<long>("LocationId"),
                                                  reader.Read<string>("Segment"),
                                                  reader.Read<string>("HRDFileName"));
                }
            }
        }

        private static string GetLocationsQuery()
        {
            return "SELECT L.LocationId, L.Segment, T.HRDFileName " +
                   "FROM Locations L " +
                   "INNER JOIN Tracks T USING(TrackId) " +
                   "WHERE L.TypeOfHydraulicDataId > 1;"; // Value > 1 makes it relevant
        }
    }
}
