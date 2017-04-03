// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Data.SQLite;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Properties;
using Ringtoets.HydraRing.Calculation.Readers;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for the convergence of Hydra-Ring calculations.
    /// </summary>
    public class ConvergenceParser : IHydraRingFileParser
    {
        private const string sectionIdParameterName = "@sectionId";
        private const string convergedColumnName = "Converged";

        private readonly string getLastResultQuery =
            $"SELECT ConvOnBeta OR ConvOnValue AS {convergedColumnName} " +
            "FROM IterateToGivenBetaConvergence " +
            $"WHERE SectionId = {sectionIdParameterName} " +
            "AND OuterIterationId = (SELECT MAX(OuterIterationId) FROM IterateToGivenBetaConvergence);";

        /// <summary>
        /// Gets the value indicating whether the calculation for a section has converged.
        /// </summary>
        public bool? Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            if (workingDirectory == null)
            {
                throw new ArgumentNullException(nameof(workingDirectory));
            }

            ParseFile(workingDirectory, sectionId);
        }

        /// <summary>
        /// Parses the file.
        /// </summary>
        /// <param name="workingDirectory">The path to the directory which contains
        /// the output of the Hydra-Ring calculation.</param>
        /// <param name="sectionId">The section id to get the output for.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when the reader
        /// encounters an error while reading the database.</exception>
        private void ParseFile(string workingDirectory, int sectionId)
        {
            try
            {
                using (var reader = new HydraRingDatabaseReader(workingDirectory, getLastResultQuery, sectionId))
                {
                    reader.Execute();
                    ReadResult(reader);
                }
            }
            catch (SQLiteException e)
            {
                throw new HydraRingFileParserException(Resources.Parse_Cannot_read_convergence_in_output_file, e);
            }
            catch (HydraRingDatabaseReaderException)
            {
                throw new HydraRingFileParserException(Resources.Parse_No_convergence_found_in_output_file);
            }
        }

        /// <summary>
        /// Reads the result of the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The reader to get the result from.</param>
        private void ReadResult(HydraRingDatabaseReader reader)
        {
            object result = reader.ReadColumn(convergedColumnName);
            if (result != null)
            {
                Output = Convert.ToBoolean(result);
            }
        }
    }
}