// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Data.SQLite;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Properties;
using Ringtoets.HydraRing.Calculation.Readers;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Helper class for methods that apply for parsing output from a Hydra-Ring database.
    /// </summary>
    internal static class HydraRingDatabaseParseHelper
    {
        /// <summary>
        /// Parses the Hydra-Ring output database.
        /// </summary>
        /// <param name="workingDirectory">The path to the directory which contains
        /// the output of the Hydra-Ring calculation.</param>
        /// <param name="query">The query to perform when reading the database.</param>
        /// <param name="sectionId">The section id to get the output for.</param>
        /// <param name="exceptionMessage">The exception message when there is no result.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> with the key of the column and the value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="HydraRingFileParserException">Thrown when the reader encounters an error while 
        /// reading the database.</exception>
        public static Dictionary<string, object> ReadSingleLine(string workingDirectory,
                                                                string query,
                                                                int sectionId,
                                                                string exceptionMessage)
        {
            ValidateParameters(workingDirectory, query, exceptionMessage);

            try
            {
                using (var reader = new HydraRingDatabaseReader(workingDirectory, query, sectionId))
                {
                    return reader.ReadLine();
                }
            }
            catch (SQLiteException e)
            {
                throw new HydraRingFileParserException(Resources.Parse_Cannot_read_result_in_output_file, e);
            }
            catch (HydraRingDatabaseReaderException e)
            {
                throw new HydraRingFileParserException(exceptionMessage, e);
            }
        }

        private static void ValidateParameters(string workingDirectory,
                                               string query,
                                               string exceptionMessage)
        {
            if (workingDirectory == null)
            {
                throw new ArgumentNullException(nameof(workingDirectory));
            }
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (exceptionMessage == null)
            {
                throw new ArgumentNullException(nameof(exceptionMessage));
            }
        }
    }
}