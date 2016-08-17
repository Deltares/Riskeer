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
using log4net;
using Ringtoets.HydraRing.Calculation.Properties;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for a Hydra-Ring log file.
    /// </summary>
    public class HydraRingLogFileParser : IHydraRingFileParser
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraRingLogFileParser));

        /// <summary>
        /// Gets the log file content.
        /// </summary>
        public string LogFileContent { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            string logFileName = sectionId + ".log";

            try
            {
                LogFileContent = File.ReadAllText(Path.Combine(workingDirectory, sectionId + ".log"));
            }
            catch
            {
                log.ErrorFormat(Resources.Parse_Cannot_read_file_0_from_folder_1_, logFileName, workingDirectory);
            }
        }
    }
}