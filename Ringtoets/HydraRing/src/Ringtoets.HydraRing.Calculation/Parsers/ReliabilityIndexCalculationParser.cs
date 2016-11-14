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
using System.Globalization;
using System.IO;
using System.Linq;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.IO;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for the output of Hydra-Ring calculations that iterate towards a reliability index.
    /// </summary>
    public class ReliabilityIndexCalculationParser : IHydraRingFileParser
    {
        /// <summary>
        /// Gets a <see cref="ReliabilityIndexCalculationOutput"/> 
        /// corresponding to the section id if <see cref="Parse"/> executed successfully; or <c>null</c> otherwise.
        /// </summary>
        public ReliabilityIndexCalculationOutput Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            try
            {
                using (var streamReader = new StreamReader(Path.Combine(workingDirectory, HydraRingFileConstants.DesignTablesFileName)))
                {
                    var fileContents = streamReader.ReadToEnd();
                    var lines = fileContents.Split('\n');

                    foreach (var resultLine in lines.Skip(3)) // Skip the header lines
                    {
                        var results = resultLine.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);

                        if (results.Any() && results.ElementAt(0) == sectionId.ToString())
                        {
                            Output = new ReliabilityIndexCalculationOutput(GetDoubleValueFromElement(results.ElementAt(results.Length - 2)), GetDoubleValueFromElement(results.ElementAt(results.Length - 1)));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new HydraRingFileParserException("", e);
            }
        }

        private static double GetDoubleValueFromElement(string element)
        {
            return double.Parse(element, CultureInfo.InvariantCulture);
        }
    }
}