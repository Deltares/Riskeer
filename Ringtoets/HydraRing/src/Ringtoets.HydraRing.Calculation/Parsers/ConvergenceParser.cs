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
using System.Collections.Generic;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.HydraRing.Calculation.Properties;

namespace Riskeer.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for the convergence result of Hydra-Ring calculations.
    /// </summary>
    public class ConvergenceParser : IHydraRingFileParser
    {
        private const string convergedColumnName = "Converged";

        private readonly string getLastResultQuery =
            $"SELECT ConvOnBeta OR ConvOnValue OR ConvOnValueZeroQVariant AS {convergedColumnName} " +
            "FROM IterateToGivenBetaConvergence " +
            $"WHERE SectionId = {HydraRingDatabaseConstants.SectionIdParameterName} " +
            "ORDER BY OuterIterationId DESC " +
            "LIMIT 1";

        /// <summary>
        /// Gets the output that was parsed from the output file.
        /// </summary>
        public bool? Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            Dictionary<string, object> result = HydraRingDatabaseParseHelper.ReadSingleLine(workingDirectory,
                                                                                            getLastResultQuery,
                                                                                            sectionId,
                                                                                            Resources.ConvergenceParser_No_convergence_found_in_output_file);

            ReadResult(result);
        }

        /// <summary>
        /// Reads the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The result from the database read.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when the result
        /// cannot be converted to the output format.</exception>
        private void ReadResult(IDictionary<string, object> result)
        {
            try
            {
                Output = Convert.ToBoolean(result[convergedColumnName]);
            }
            catch (InvalidCastException e)
            {
                throw new HydraRingFileParserException(Resources.ConvergenceParser_No_convergence_found_in_output_file, e);
            }
        }
    }
}