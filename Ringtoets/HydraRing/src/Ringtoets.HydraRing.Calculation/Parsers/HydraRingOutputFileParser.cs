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

using System.IO;
using Ringtoets.HydraRing.Calculation.Properties;
using Ringtoets.HydraRing.IO;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for a Hydra-Ring log file.
    /// </summary>
    public class HydraRingOutputFileParser : IHydraRingFileParser
    {
        /// <summary>
        /// Gets the output file content from a Hydra-Ring calculation, or the log file content in case of an error.
        /// </summary>
        public string OutputFileContent { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            string outputFileName = sectionId + HydraRingFileConstants.OutputFileSuffix;
            string outputFilePath = Path.Combine(workingDirectory, outputFileName);

            if (!File.Exists(outputFilePath))
            {
                outputFileName = sectionId + HydraRingFileConstants.LogFileExtension;
                outputFilePath = Path.Combine(workingDirectory, outputFileName);
            }

            try
            {
                OutputFileContent = File.ReadAllText(outputFilePath);
            }
            catch
            {
                var message = string.Format(Resources.Parse_Cannot_read_FileName_0_nor_FileName_1_from_FolderPath_2_,
                                            sectionId + HydraRingFileConstants.OutputFileSuffix,
                                            outputFileName,
                                            workingDirectory);
                throw new HydraRingFileParserException(message);
            }
        }
    }
}