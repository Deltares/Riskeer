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

namespace Ringtoets.HydraRing.Calculation.Services
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations.
    /// </summary>
    public class HydraRingCalculationService
    {
        private static Process hydraRingProcess;

        /// <summary>
        /// This method performs a type II calculation via Hydra-Ring:
        /// Iterate towards a target probability, provided as reliability index.
        /// </summary>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="timeIntegrationSchemeType">The <see cref="HydraRingTimeIntegrationSchemeType"/> to use while executing the calculation.</param>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while executing the calculation.</param>
        /// <param name="targetProbabilityCalculationInput">The input of the calculation to perform.</param>
        /// <returns>A <see cref="TargetProbabilityCalculationOutput"/> on a successful calculation, <c>null</c> otherwise.</returns>
        public static TargetProbabilityCalculationOutput PerformCalculation(string hlcdDirectory, string ringId,
                                                                             HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType,
                                                                             HydraRingUncertaintiesType uncertaintiesType,
                                                                             TargetProbabilityCalculationInput targetProbabilityCalculationInput)
        {
            return PerformCalculation(hlcdDirectory, ringId, timeIntegrationSchemeType, uncertaintiesType,
                                      targetProbabilityCalculationInput, (outputFilePath, outputDatabasePath) =>
                                                                         TargetProbabilityCalculationParser.Parse(
                                                                             outputFilePath, targetProbabilityCalculationInput.Section.SectionId));
        }

        /// <summary>
        /// This method performs a type I calculation via Hydra-Ring:
        /// Given a set of random variables, compute the probability of failure.
        /// </summary>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="timeIntegrationSchemeType">The <see cref="HydraRingTimeIntegrationSchemeType"/> to use while executing the calculation.</param>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while executing the calculation.</param>
        /// <param name="exceedanceProbabilityCalculationInput">The input of the calculation to perform.</param>
        /// <returns>A <see cref="ExceedanceProbabilityCalculationOutput"/> on a successful calculation, <c>null</c> otherwise.</returns>
        public static ExceedanceProbabilityCalculationOutput PerformCalculation(string hlcdDirectory, string ringId,
                                                                                 HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType,
                                                                                 HydraRingUncertaintiesType uncertaintiesType,
                                                                                 ExceedanceProbabilityCalculationInput exceedanceProbabilityCalculationInput)
        {
            return PerformCalculation(hlcdDirectory, ringId, timeIntegrationSchemeType, uncertaintiesType,
                                      exceedanceProbabilityCalculationInput, (outputFilePath, outputDatabasePath) =>
                                                                             ExceedanceProbabilityCalculationParser.Parse(
                                                                                 outputDatabasePath, exceedanceProbabilityCalculationInput.Section.SectionId));
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

        private static T PerformCalculation<T>(string hlcdDirectory, string ringId,
                                               HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType, HydraRingUncertaintiesType uncertaintiesType,
                                               HydraRingCalculationInput hydraRingCalculationInput, Func<string, string, T> parseFunction)
        {
            var sectionId = hydraRingCalculationInput.Section.SectionId;

            // Create a working directory
            var workingDirectory = CreateWorkingDirectory();

            // Write the initialization script
            var hydraRingInitializationService = new HydraRingInitializationService(hydraRingCalculationInput.FailureMechanismType, sectionId, hlcdDirectory, workingDirectory);
            File.WriteAllText(hydraRingInitializationService.IniFilePath, hydraRingInitializationService.GenerateInitializationScript());

            // Write the database creation script
            var hydraRingConfigurationService = new HydraRingConfigurationService(ringId, timeIntegrationSchemeType, uncertaintiesType);
            hydraRingConfigurationService.AddHydraRingCalculationInput(hydraRingCalculationInput);
            File.WriteAllText(hydraRingInitializationService.DataBaseCreationScriptFilePath, hydraRingConfigurationService.GenerateDataBaseCreationScript());

            // Perform the calculation
            hydraRingProcess = HydraRingProcessFactory.Create(hydraRingInitializationService.MechanismComputationExeFilePath, hydraRingInitializationService.IniFilePath, workingDirectory);
            hydraRingProcess.Start();
            hydraRingProcess.WaitForExit();

            // Parse and return the output
            return parseFunction(hydraRingInitializationService.OutputFilePath, hydraRingInitializationService.OutputDataBasePath);
        }

        private static string CreateWorkingDirectory()
        {
            var workingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (Directory.Exists(workingDirectory))
            {
                Directory.Delete(workingDirectory, true);
            }

            Directory.CreateDirectory(workingDirectory);

            return workingDirectory;
        }
    }
}