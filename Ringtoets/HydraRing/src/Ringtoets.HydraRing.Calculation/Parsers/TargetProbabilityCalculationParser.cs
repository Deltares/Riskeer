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

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for the output of a Hydra-Ring type 2 calculation ("iterate towards a target probability, provided as reliability index").
    /// </summary>
    internal static class TargetProbabilityCalculationParser
    {
        /// <summary>
        /// Tries to parse a <see cref="TargetProbabilityCalculationOutput"/> object from the provided output file path for the provided section id.
        /// </summary>
        /// <param name="outputFilePath">The path to the file which contains the output of the Hydra-Ring type 2 calculation.</param>
        /// <param name="sectionId">The section id to get the <see cref="TargetProbabilityCalculationOutput"/> object for.</param>
        /// <returns>A <see cref="TargetProbabilityCalculationOutput"/> corresponding to the section id or <c>null</c> otherwise.</returns>
        public static TargetProbabilityCalculationOutput Parse(string outputFilePath, int sectionId)
        {
            try
            {
                using (var streamReader = new StreamReader(outputFilePath))
                {
                    var fileContents = streamReader.ReadToEnd();
                    var lines = fileContents.Split('\n');

                    foreach (var resultLine in lines.Skip(3)) // Skip the header lines
                    {
                        var results = resultLine.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);

                        if (results.ElementAt(0) == sectionId.ToString())
                        {
                            return new TargetProbabilityCalculationOutput(GetDoubleValueFromElement(results.ElementAt(results.Length - 2)), GetDoubleValueFromElement(results.ElementAt(results.Length - 1)));
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        private static double GetDoubleValueFromElement(string element)
        {
            return double.Parse(element, CultureInfo.InvariantCulture);
        }
    }
}