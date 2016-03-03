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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Settings;

namespace Ringtoets.HydraRing.Calculation
{
    /// <summary>
    /// Static class that provides methods for performing Hydra-Ring calculations.
    /// </summary>
    public static class HydraRingCalculator
    {
        /// <summary>
        /// This method performs a single failure mechanism calculation via Hydra-Ring.
        /// </summary>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the failure mechanism calculation.</param>
        /// <param name="ringId">The id of the ring to perform the failure mechanism calculation for.</param>
        /// <param name="timeIntegrationSchemeType">The <see cref="HydraRingTimeIntegrationSchemeType"/> to use while executing the failure mechanism calculation.</param>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while executing the failure mechanism calculation.</param>
        /// <param name="hydraRingCalculation">The failure mechanism calculation to perform.</param>
        public static void PerformFailureMechanismCalculation(string hlcdDirectory, string ringId, HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType, HydraRingUncertaintiesType uncertaintiesType, HydraRingCalculation hydraRingCalculation)
        {
            var hydraulicBoundaryLocationId = hydraRingCalculation.HydraulicBoundaryLocationId;
            var mechanismId = new FailureMechanismDefaultsProvider().GetFailureMechanismDefaults(hydraRingCalculation.FailureMechanismType).MechanismId;

            // Create a Hydra-Ring configuration
            var hydraRingConfiguration = new HydraRingConfiguration(ringId, timeIntegrationSchemeType, uncertaintiesType);
            hydraRingConfiguration.AddHydraRingCalculation(hydraRingCalculation);

            // Calculation file names
            var outputFileName = "designTable.txt";
            var logFileName = hydraulicBoundaryLocationId + ".log";
            var iniFileName = hydraulicBoundaryLocationId + ".ini";
            var dataBaseCreationScriptFileName = hydraulicBoundaryLocationId + ".sql";

            // Obtain some calculation file paths
            var workingDirectory = CreateWorkingDirectory(hydraulicBoundaryLocationId.ToString());
            var iniFilePath = Path.Combine(workingDirectory, iniFileName);
            var dataBaseCreationScriptFilePath = Path.Combine(workingDirectory, dataBaseCreationScriptFileName);
            var hlcdFilePath = Path.Combine(hlcdDirectory, "HLCD.sqlite");

            // Obtain some Hydra-Ring related paths
            var hydraRingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"HydraRing");
            var mechanismComputationExeFilePath = Path.Combine(hydraRingDirectory, "MechanismComputation.exe");
            var configurationDatabaseFilePath = Path.Combine(hydraRingDirectory, "config.sqlite");

            // Write the ini file
            File.WriteAllLines(Path.Combine(workingDirectory, iniFilePath), new List<string>
            {
                "section             = " + hydraulicBoundaryLocationId,
                "mechanism           = " + mechanismId,
                "alternative         = 1", // Fixed: no support for piping
                "layer               = 1", // Fixed: no support for revetments
                "logfile             = " + logFileName,
                "outputverbosity     = basic",
                "outputtofile        = file",
                "projectdbfilename   = " + dataBaseCreationScriptFileName,
                "outputfilename      = " + outputFileName,
                "configdbfilename    = " + configurationDatabaseFilePath,
                "hydraulicdbfilename = " + hlcdFilePath
            });

            // Write the database creation script
            File.WriteAllText(dataBaseCreationScriptFilePath, hydraRingConfiguration.GenerateDataBaseCreationScript());

            // Perform the calculation
            var hydraRingProcess = new Process
            {
                StartInfo = new ProcessStartInfo(mechanismComputationExeFilePath, iniFilePath)
                {
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            hydraRingProcess.Start();
            hydraRingProcess.WaitForExit();

            // TODO: Parse output
        }

        private static string CreateWorkingDirectory(string folderName)
        {
            var workingDirectory = Path.Combine(Path.GetTempPath(), folderName);

            if (Directory.Exists(workingDirectory))
            {
                Directory.Delete(workingDirectory, true);
            }

            Directory.CreateDirectory(workingDirectory);

            return workingDirectory;
        }
    }
}