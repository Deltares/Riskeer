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
    /// Base implementation for a calculator which uses Hydra-Ring to perform different calculations.
    /// </summary>
    internal abstract class HydraRingCalculatorBase
    {
        private readonly LastErrorFileParser lastErrorFileParser;
        private Process hydraRingProcess;

        private readonly string hlcdDirectory;
        private readonly string ringId;

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingCalculatorBase"/> with a default Hydra-Ring file parser
        /// initialized.
        /// </summary>
        /// <param name="hlcdDirectory">The directory in which the Hydraulic Boundary Database can be found.</param>
        /// <param name="ringId">The id of the assessment section which is used in the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hlcdDirectory"/> is <c>null</c>.</exception>
        protected HydraRingCalculatorBase(string hlcdDirectory, string ringId)
        {
            if (hlcdDirectory == null)
            {
                throw new ArgumentNullException("hlcdDirectory");
            }
            this.hlcdDirectory = hlcdDirectory;
            this.ringId = ringId;

            lastErrorFileParser = new LastErrorFileParser();
        }

        /// <summary>
        /// Gets the directory of the output used during the calculation.
        /// </summary>
        public string OutputDirectory { get; private set; }

        /// <summary>
        /// Gets the content of the last error file generated during the Hydra-Ring calculation.
        /// </summary>
        public string LastErrorContent { get; private set; }

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

        /// <summary>
        /// Gets the parsers that are executed on the output file(s) of Hydra-Ring.
        /// </summary>
        /// <returns>The parsers to execute.</returns>
        protected virtual IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield break;
        }

        /// <summary>
        /// Sets the values on the output parameters of the calculation.
        /// </summary>
        protected abstract void SetOutputs();

        /// <summary>
        /// Performs the actual calculation by running the Hydra-Ring executable.
        /// </summary>
        /// <param name="uncertaintiesType">The uncertainty type used in the calculation.</param>
        /// <param name="hydraRingCalculationInput">The object containing input data.</param>
        protected void Calculate(
            HydraRingUncertaintiesType uncertaintiesType,
            HydraRingCalculationInput hydraRingCalculationInput)
        {
            var sectionId = hydraRingCalculationInput.Section.SectionId;
            OutputDirectory = CreateWorkingDirectory();

            var hydraRingConfigurationService = new HydraRingConfigurationService(ringId, uncertaintiesType);
            hydraRingConfigurationService.AddHydraRingCalculationInput(hydraRingCalculationInput);

            var hydraRingInitializationService = new HydraRingInitializationService(hydraRingCalculationInput.FailureMechanismType, sectionId, hlcdDirectory, OutputDirectory);
            hydraRingInitializationService.WriteInitializationScript();
            hydraRingConfigurationService.WriteDataBaseCreationScript(hydraRingInitializationService.DatabaseCreationScriptFilePath);

            PerformCalculation(OutputDirectory, hydraRingInitializationService);
            ExecuteGenericParsers(hydraRingInitializationService, sectionId);
            ExecuteCustomParsers(hydraRingInitializationService.TemporaryWorkingDirectory, sectionId);
        }

        private void ExecuteGenericParsers(HydraRingInitializationService hydraRingInitializationService, int sectionId)
        {
            lastErrorFileParser.Parse(hydraRingInitializationService.TemporaryWorkingDirectory, sectionId);
            LastErrorContent = lastErrorFileParser.ErrorFileContent;
        }

        private void ExecuteCustomParsers(string temporaryWorkingDirectory, int sectionId)
        {
            foreach (var parser in GetParsers())
            {
                parser.Parse(temporaryWorkingDirectory, sectionId);
            }

            SetOutputs();
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