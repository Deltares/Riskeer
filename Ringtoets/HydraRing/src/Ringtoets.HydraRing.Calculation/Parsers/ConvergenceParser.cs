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
            "ORDER BY OuterIterationId DESC " +
            "LIMIT 1";

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

            HydraRingDatabaseParseHelper.Parse(workingDirectory,
                                               getLastResultQuery,
                                               sectionId,
                                               Resources.ParseFile_No_convergence_found_in_output_file,
                                               ReadResult);
        }

        /// <summary>
        /// Reads the result of the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The reader to get the result from.</param>
        /// <exception cref="InvalidCastException">Thrown when the the result
        /// cannot be converted to the output format.</exception>
        private void ReadResult(HydraRingDatabaseReader reader)
        {
            Output = Convert.ToBoolean(reader.ReadColumn(convergedColumnName));
        }
    }
}