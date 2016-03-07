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
using System.IO;
using System.Reflection;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Providers;

namespace Ringtoets.HydraRing.Calculation.Service
{
    /// <summary>
    /// Service for:
    /// <list type="bullet">
    /// <item>
    /// generating an initialization script that is necessary for performing Hydra-Ring calculations;
    /// </item>
    /// <item>
    /// providing the corresponding file paths.
    /// </item>
    /// </list>
    /// </summary>
    internal class HydraRingInitializationService
    {
        private readonly int mechanismId;
        private readonly int sectionId;
        private readonly string iniFilePath;
        private readonly string dataBaseCreationScriptFileName;
        private readonly string dataBaseCreationScriptFilePath;
        private readonly string logFileName;
        private readonly string logFilePath;
        private readonly string outputFileName;
        private readonly string outputFilePath;
        private readonly string outputDataBasePath;
        private readonly string hlcdFilePath;
        private readonly string mechanismComputationExeFilePath;
        private readonly string configurationDatabaseFilePath;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingInitializationService"/> class.
        /// </summary>
        /// <param name="failureMechanismType">The failure mechanism type.</param>
        /// <param name="sectionId">The section id.</param>
        /// <param name="hlcdDirectory">The HLCD directory.</param>
        /// <param name="workingDirectory">The working directory.</param>
        public HydraRingInitializationService(HydraRingFailureMechanismType failureMechanismType, int sectionId, string hlcdDirectory, string workingDirectory)
        {
            mechanismId = new FailureMechanismDefaultsProvider().GetFailureMechanismDefaults(failureMechanismType).MechanismId;
            this.sectionId = sectionId;

            // Initialize input/output file paths
            var iniFileName = sectionId + ".ini";
            iniFilePath = Path.Combine(workingDirectory, iniFileName);
            dataBaseCreationScriptFileName = sectionId + ".sql";
            dataBaseCreationScriptFilePath = Path.Combine(workingDirectory, dataBaseCreationScriptFileName);
            logFileName = sectionId + ".log";
            logFilePath = Path.Combine(workingDirectory, logFileName);
            outputFileName = "designTable.txt";
            outputFilePath = Path.Combine(workingDirectory, outputFileName);
            outputDataBasePath = Path.Combine(workingDirectory, "temp.sqlite");
            hlcdFilePath = Path.Combine(hlcdDirectory, "HLCD.sqlite");

            // Initialize Hydra-Ring file paths
            var hydraRingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"HydraRing");
            mechanismComputationExeFilePath = Path.Combine(hydraRingDirectory, "MechanismComputation.exe");
            configurationDatabaseFilePath = Path.Combine(hydraRingDirectory, "config.sqlite");
        }

        /// <summary>
        /// Gets the ini file path.
        /// </summary>
        public string IniFilePath
        {
            get
            {
                return iniFilePath;
            }
        }

        /// <summary>
        /// Gets the database creation script file path.
        /// </summary>
        public string DataBaseCreationScriptFilePath
        {
            get
            {
                return dataBaseCreationScriptFilePath;
            }
        }

        /// <summary>
        /// Gets the log file path.
        /// </summary>
        public string LogFilePath
        {
            get
            {
                return logFilePath;
            }
        }

        /// <summary>
        /// Gets the output file path.
        /// </summary>
        public string OutputFilePath
        {
            get
            {
                return outputFilePath;
            }
        }

        /// <summary>
        /// Gets the output database path.
        /// </summary>
        public string OutputDataBasePath
        {
            get
            {
                return outputDataBasePath;
            }
        }

        /// <summary>
        /// Gets the HLCD file path.
        /// </summary>
        public string HlcdFilePath
        {
            get
            {
                return outputDataBasePath;
            }
        }

        /// <summary>
        /// Gets the path of the MechanismComputation.exe file.
        /// </summary>
        public string MechanismComputationExeFilePath
        {
            get
            {
                return mechanismComputationExeFilePath;
            }
        }

        /// <summary>
        /// Generates the initialization script necessary for performing Hydra-Ring calculations.
        /// </summary>
        /// <returns>The initialization script.</returns>
        public string GenerateInitializationScript()
        {
            return string.Join(Environment.NewLine,
                               "section             = " + sectionId,
                               "mechanism           = " + mechanismId,
                               "alternative         = 1", // Fixed: no support for piping
                               "layer               = 1", // Fixed: no support for revetments
                               "logfile             = " + logFileName,
                               "outputverbosity     = basic",
                               "outputtofile        = file",
                               "projectdbfilename   = " + dataBaseCreationScriptFileName,
                               "outputfilename      = " + outputFileName,
                               "configdbfilename    = " + configurationDatabaseFilePath,
                               "hydraulicdbfilename = " + hlcdFilePath);
        }
    }
}
