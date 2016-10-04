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
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations.
    /// </summary>
    public abstract class HydraRingCalculator
    {
        private readonly HydraRingOutputFileParser outputFileParser;

        private Process hydraRingProcess;

        protected HydraRingCalculator()
        {
            outputFileParser = new HydraRingOutputFileParser();
        }

        public string OutputFileContent { get; private set; }

        /// <summary>
        /// Cancels any currently running Hydra-Ring calculation.
        /// </summary>
        public void Cancel()
        {
            if (hydraRingProcess != null && !hydraRingProcess.HasExited)
            {
                hydraRingProcess.StandardInput.WriteLine("b");
            }
        }

        protected virtual IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield break;
        }

        protected void Calculate(
            string hlcdDirectory,
            string ringId,
            HydraRingUncertaintiesType uncertaintiesType,
            HydraRingCalculationInput hydraRingCalculationInput)
        {
            var sectionId = hydraRingCalculationInput.Section.SectionId;
            var workingDirectory = CreateWorkingDirectory();

            var hydraRingConfigurationService = new HydraRingConfigurationService(ringId, uncertaintiesType);
            hydraRingConfigurationService.AddHydraRingCalculationInput(hydraRingCalculationInput);

            var hydraRingInitializationService = new HydraRingInitializationService(hydraRingCalculationInput.FailureMechanismType, sectionId, hlcdDirectory, workingDirectory);
            hydraRingInitializationService.WriteInitializationScript();
            hydraRingConfigurationService.WriteDataBaseCreationScript(hydraRingInitializationService.DatabaseCreationScriptFilePath);

            PerformCalculation(workingDirectory, hydraRingInitializationService);
            ExecuteParsers(hydraRingInitializationService.TemporaryWorkingDirectory, sectionId);
            SetAllOutputs();
        }

        protected virtual void SetOutputs() {}

        private void SetAllOutputs()
        {
            OutputFileContent = outputFileParser.OutputFileContent;
            SetOutputs();
        }

        private IEnumerable<IHydraRingFileParser> GetAllParsers()
        {
            yield return outputFileParser;
            foreach (var parser in GetParsers())
            {
                yield return parser;
            }
        }

        private void ExecuteParsers(string temporaryWorkingDirectory, int sectionId)
        {
            foreach (var parser in GetAllParsers())
            {
                parser.Parse(temporaryWorkingDirectory, sectionId);
            }
        }

        private void PerformCalculation(string workingDirectory, HydraRingInitializationService hydraRingInitializationService)
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