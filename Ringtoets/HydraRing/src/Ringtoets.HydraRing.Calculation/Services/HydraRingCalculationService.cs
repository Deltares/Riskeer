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
using System.Linq;
using log4net;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Properties;

namespace Ringtoets.HydraRing.Calculation.Services
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations.
    /// </summary>
    public static class HydraRingCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraRingCalculationService));

        private static Process hydraRingProcess;

        /// <summary>
        /// This method performs a type II calculation via Hydra-Ring:
        /// Iterate towards a target probability, provided as reliability index.
        /// </summary>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="timeIntegrationSchemeType">The <see cref="HydraRingTimeIntegrationSchemeType"/> to use while executing the calculation.</param>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while executing the calculation.</param>
        /// <param name="hydraRingCalculationInput">The input of the calculation to perform.</param>
        /// <param name="parsers">Parsers that will be invoked after the Hydra-Ring calculation has ran.</param>
        public static void PerformCalculation(
            string hlcdDirectory,
            string ringId,
            HydraRingUncertaintiesType uncertaintiesType,
            HydraRingCalculationInput hydraRingCalculationInput,
            IEnumerable<IHydraRingFileParser> parsers)
        {
            var sectionId = hydraRingCalculationInput.Section.SectionId;
            var workingDirectory = CreateWorkingDirectory();

            var hydraRingConfigurationService = new HydraRingConfigurationService(ringId, uncertaintiesType);
            hydraRingConfigurationService.AddHydraRingCalculationInput(hydraRingCalculationInput);

            var hydraRingInitializationService = new HydraRingInitializationService(hydraRingCalculationInput.FailureMechanismType, sectionId, hlcdDirectory, workingDirectory);
            hydraRingInitializationService.WriteInitializationScript();
            hydraRingConfigurationService.WriteDataBaseCreationScript(hydraRingInitializationService.DatabaseCreationScriptFilePath);

            PerformCalculation(workingDirectory, hydraRingInitializationService);

            PerformPostProcessing(hydraRingCalculationInput, parsers.ToList(), hydraRingInitializationService);
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

        private static void PerformPostProcessing(HydraRingCalculationInput hydraRingCalculationInput,
                                                  ICollection<IHydraRingFileParser> parsers,
                                                  HydraRingInitializationService hydraRingInitializationService)
        {
            var outputFileParser = new HydraRingOutputFileParser();
            parsers.Add(outputFileParser);

            ExecuteParsers(parsers, hydraRingInitializationService.TemporaryWorkingDirectory, hydraRingCalculationInput.Section.SectionId);

            string outputFileContent = outputFileParser.OutputFileContent;
            if (!string.IsNullOrEmpty(outputFileContent))
            {
                log.InfoFormat(Resources.HydraRingCalculationService_HydraRing_calculation_report_message_text_0, outputFileContent);
            }
        }

        private static void ExecuteParsers(IEnumerable<IHydraRingFileParser> parsers, string workingDirectory, int sectionId)
        {
            foreach (var parser in parsers)
            {
                parser.Parse(workingDirectory, sectionId);
            }
        }

        private static void PerformCalculation(string workingDirectory, HydraRingInitializationService hydraRingInitializationService)
        {
            hydraRingProcess = HydraRingProcessFactory.Create(
                hydraRingInitializationService.MechanismComputationExeFilePath,
                hydraRingInitializationService.IniFilePath,
                workingDirectory);
            hydraRingProcess.Start();
            hydraRingProcess.WaitForExit();
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