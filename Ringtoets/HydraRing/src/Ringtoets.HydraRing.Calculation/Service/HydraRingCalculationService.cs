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
using System.Diagnostics;
using System.IO;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations.
    /// </summary>
    /// <remarks>The current implementation of this service is not thread safe (calculations should be performed one at a time).</remarks>
    public static class HydraRingCalculationService
    {
        private static Process hydraRingProcess;

        /// <summary>
        /// This method performs a type 2 calculation via Hydra-Ring ("iterate towards a target probability, provided as reliability index").
        /// </summary>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="timeIntegrationSchemeType">The <see cref="HydraRingTimeIntegrationSchemeType"/> to use while executing the calculation.</param>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while executing the calculation.</param>
        /// <param name="targetProbabilityCalculationInput">The input of the calculation to perform.</param>
        /// <returns>A <see cref="TargetProbabilityCalculationOutput"/> or <c>null</c> when something went wrong.</returns>
        public static TargetProbabilityCalculationOutput PerformCalculation(string hlcdDirectory, string ringId, HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType, HydraRingUncertaintiesType uncertaintiesType, TargetProbabilityCalculationInput targetProbabilityCalculationInput)
        {
            return PerformCalculation(hlcdDirectory, ringId, timeIntegrationSchemeType, uncertaintiesType, targetProbabilityCalculationInput, (outputFilePath, ouputDatabasePath) => TargetProbabilityCalculationParser.Parse(outputFilePath, targetProbabilityCalculationInput.DikeSection.SectionId));
        }

        /// <summary>
        /// Cancels any currently running Hydra-Ring calculation.
        /// </summary>
        public static void CancelRunningCalculation()
        {
            if (hydraRingProcess != null && !hydraRingProcess.HasExited)
            {
                hydraRingProcess.StandardInput.WriteLine("b");
            }
        }

        private static T PerformCalculation<T>(string hlcdDirectory, string ringId, HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType, HydraRingUncertaintiesType uncertaintiesType, HydraRingCalculationInput hydraRingCalculationInput, Func<string, string, T> parseFunction)
        {
            var hydraulicBoundaryLocationId = hydraRingCalculationInput.HydraulicBoundaryLocationId;

            // Create a working directory
            var workingDirectory = CreateWorkingDirectory(hydraulicBoundaryLocationId.ToString());

            // Write the initialization script
            var hydraRingInitializationService = new HydraRingInitializationService(hydraRingCalculationInput.FailureMechanismType, hydraulicBoundaryLocationId, hlcdDirectory, workingDirectory);
            File.WriteAllText(hydraRingInitializationService.IniFilePath, hydraRingInitializationService.GenerateInitializationScript());

            // Write the database creation script
            var hydraRingConfigurationService = new HydraRingConfigurationService(ringId, timeIntegrationSchemeType, uncertaintiesType);
            hydraRingConfigurationService.AddHydraRingCalculationInput(hydraRingCalculationInput);
            File.WriteAllText(hydraRingInitializationService.DataBaseCreationScriptFilePath, hydraRingConfigurationService.GenerateDataBaseCreationScript());

            // Perform the calculation
            hydraRingProcess = new Process
            {
                StartInfo = new ProcessStartInfo(hydraRingInitializationService.MechanismComputationExeFilePath, hydraRingInitializationService.IniFilePath)
                {
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            hydraRingProcess.Start();
            hydraRingProcess.WaitForExit();

            // Parse and return the output
            return parseFunction(hydraRingInitializationService.OutputFilePath, hydraRingInitializationService.OutputDataBasePath);
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