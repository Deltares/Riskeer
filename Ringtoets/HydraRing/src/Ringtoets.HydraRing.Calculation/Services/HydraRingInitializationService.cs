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
using System.IO;
using System.Reflection;
using System.Security;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Providers;

namespace Ringtoets.HydraRing.Calculation.Services
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
        private const string hydraRingBinariesSubDirectory = "HydraRing";

        private const string iniFileExtension = ".ini";
        private const string databaseFileExtension = ".sql";
        private const string logFileExtension = ".log";

        private readonly int mechanismId;
        private readonly int sectionId;

        private readonly string hlcdFilePath;
        private readonly string hydraRingDirectory;
        private readonly string configurationDatabaseFilePath;
        private readonly string preprocessorDirectory;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingInitializationService"/> class.
        /// </summary>
        /// <param name="failureMechanismType">The failure mechanism type.</param>
        /// <param name="sectionId">The section id.</param>
        /// <param name="hlcdDirectory">The HLCD directory.</param>
        /// <param name="temporaryWorkingDirectory">The working directory.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/>
        /// matches <see cref="string.IsNullOrEmpty"/>.</remarks>
        public HydraRingInitializationService(HydraRingFailureMechanismType failureMechanismType,
                                              int sectionId,
                                              string hlcdDirectory,
                                              string temporaryWorkingDirectory,
                                              string preprocessorDirectory)
        {
            mechanismId = new FailureMechanismDefaultsProvider().GetFailureMechanismDefaults(failureMechanismType).MechanismId;
            this.sectionId = sectionId;
            TemporaryWorkingDirectory = temporaryWorkingDirectory;
            hlcdFilePath = Path.Combine(hlcdDirectory, HydraRingFileConstants.HlcdDatabaseFileName);
            hydraRingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), hydraRingBinariesSubDirectory);
            configurationDatabaseFilePath = Path.Combine(hydraRingDirectory, HydraRingFileConstants.ConfigurationDatabaseFileName);
            this.preprocessorDirectory = preprocessorDirectory;
        }

        /// <summary>
        /// Gets the ini file path.
        /// </summary>
        public string IniFilePath
        {
            get
            {
                return Path.Combine(TemporaryWorkingDirectory, sectionId + iniFileExtension);
            }
        }

        /// <summary>
        /// Gets the database creation script file path.
        /// </summary>
        public string DatabaseCreationScriptFilePath
        {
            get
            {
                return Path.Combine(TemporaryWorkingDirectory, sectionId + databaseFileExtension);
            }
        }

        /// <summary>
        /// Gets the path of the MechanismComputation.exe file.
        /// </summary>
        public string MechanismComputationExeFilePath
        {
            get
            {
                return Path.Combine(hydraRingDirectory, HydraRingFileConstants.HydraRingExecutableFileName);
            }
        }

        /// <summary>
        /// Gets the directory in which Hydra-Ring will place temporary input and output files created during a
        /// calculation.
        /// </summary>
        public string TemporaryWorkingDirectory { get; }

        /// <summary>
        /// Generates the initialization script necessary for performing Hydra-Ring calculations.
        /// </summary>
        /// <returns>The initialization script.</returns>
        /// <exception cref="IOException">Thrown when an I/O error occurred while opening the file.</exception>
        /// <exception cref="SecurityException">Thrown when the path can't be accessed due to missing permissions.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the path can't be accessed due to missing permissions.</exception>
        public void WriteInitializationScript()
        {
            string initializationFileContent = string.Join(Environment.NewLine,
                                                           "section                 = " + sectionId,
                                                           "mechanism               = " + mechanismId,
                                                           "alternative             = 1", // Fixed: no support for piping
                                                           "layer                   = 1", // Fixed: no support for revetments
                                                           "logfile                 = " + sectionId + logFileExtension,
                                                           "outputverbosity         = basic",
                                                           "outputtofile            = file",
                                                           "projectdbfilename       = " + sectionId + databaseFileExtension,
                                                           "outputfilename          = " + HydraRingFileConstants.DesignTablesFileName,
                                                           "configdbfilename        = " + configurationDatabaseFilePath,
                                                           "hydraulicdbfilename     = " + hlcdFilePath,
                                                           "designpointOutput       = sqlite");

            if (!string.IsNullOrEmpty(preprocessorDirectory))
            {
                initializationFileContent += Environment.NewLine + "preprocessordbdirectory = " + preprocessorDirectory;
            }

            File.WriteAllText(IniFilePath, initializationFileContent);
        }
    }
}