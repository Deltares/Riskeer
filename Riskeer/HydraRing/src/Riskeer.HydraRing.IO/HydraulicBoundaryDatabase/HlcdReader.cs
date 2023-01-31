// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
    /// Class for reading a hydraulic location configuration database.
    /// </summary>
    public class HlcdReader : SqLiteDatabaseReaderBase
    {
        private readonly string trackName;

        /// <summary>
        /// Creates a new instance of <see cref="HlcdReader"/>.
        /// </summary>
        /// <param name="databaseFilePath">The path of the hydraulic location configuration database to open.</param>
        /// <param name="trackName">The name of the track at stake.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>the <paramref name="databaseFilePath"/> contains invalid characters;</item>
        /// <item>no file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public HlcdReader(string databaseFilePath, string trackName) : base(databaseFilePath)
        {
            this.trackName = trackName;
        }

        /// <summary>
        /// Reads the hydraulic location configuration database.
        /// </summary>
        /// <returns>A <see cref="ReadHydraulicBoundaryDatabase"/>.</returns>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when the data cannot be read.</exception>
        public ReadHydraulicBoundaryDatabase Read()
        {
            IEnumerable<string> hrdFileNames = ReadHrdFileNames();
            
            return null;
        }

        /// <summary>
        /// Reads the HRD file names from the hydraulic location configuration database.
        /// </summary>
        /// <returns>The HRD file names found in the database.</returns>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        public IEnumerable<string> ReadHrdFileNames()
        {
            using (IDataReader reader = CreateDataReader(HlcdQueryBuilder.GetHrdFileNamesQuery(trackName),
                                                         new SQLiteParameter
                                                         {
                                                             DbType = DbType.String
                                                         }))
            {
                while (MoveNext(reader))
                {
                    yield return ReadHrdFileName(reader);
                }
            }
        }
        
        /// <summary>
        /// Reads a HRD file name from the database.
        /// </summary>
        /// <returns>A HRD file name based on the data read from the database.</returns>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        private string ReadHrdFileName(IDataReader reader)
        {
            try
            {
                return reader.Read<string>(HlcdDefinitions.HrdFileNameColumn);
            }
            catch (ConversionException e)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, e);
            }
        }
    }
}