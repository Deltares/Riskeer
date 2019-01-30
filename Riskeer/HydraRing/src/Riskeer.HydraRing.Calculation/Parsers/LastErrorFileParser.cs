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

using System.IO;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.HydraRing.Calculation.Properties;

namespace Riskeer.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for the Hydra-Ring last_err file.
    /// </summary>
    public class LastErrorFileParser : IHydraRingFileParser
    {
        /// <summary>
        /// Gets the error output file content from a failed Hydra-Ring calculation.
        /// </summary>
        public string ErrorFileContent { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            string errorOutputFilePath = Path.Combine(workingDirectory, HydraRingFileConstants.LastErrorFileName);

            if (File.Exists(errorOutputFilePath))
            {
                try
                {
                    ErrorFileContent = File.ReadAllText(errorOutputFilePath);
                }
                catch
                {
                    string message = string.Format(Resources.Parse_Cannot_read_last_error_Filename_0_from_FolderPath_1_,
                                                   HydraRingFileConstants.LastErrorFileName,
                                                   workingDirectory);
                    throw new HydraRingFileParserException(message);
                }
            }
        }
    }
}