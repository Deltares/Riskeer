﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Properties;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Base implementation for a calculator which uses Hydra-Ring to perform different calculations.
    /// </summary>
    internal abstract class HydraRingCalculatorBase
    {
        private readonly LastErrorFileParser lastErrorFileParser;

        private readonly string hlcdDirectory;
        private Process hydraRingProcess;

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingCalculatorBase"/> with a default Hydra-Ring file parser
        /// initialized.
        /// </summary>
        /// <param name="hlcdDirectory">The directory in which the hydraulic boundary database can be found.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hlcdDirectory"/> is <c>null</c>.</exception>
        protected HydraRingCalculatorBase(string hlcdDirectory)
        {
            if (hlcdDirectory == null)
            {
                throw new ArgumentNullException(nameof(hlcdDirectory));
            }
            this.hlcdDirectory = hlcdDirectory;

            lastErrorFileParser = new LastErrorFileParser();
        }

        /// <summary>
        /// Gets the temporary output directory that is generated during the calculation.
        /// </summary>
        public string OutputDirectory { get; private set; }

        /// <summary>
        /// Gets the content of the last error file generated during the Hydra-Ring calculation.
        /// </summary>
        public string LastErrorFileContent { get; private set; }

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
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        protected void Calculate(HydraRingUncertaintiesType uncertaintiesType,
                                 HydraRingCalculationInput hydraRingCalculationInput)
        {
            try
            {
                var sectionId = hydraRingCalculationInput.Section.SectionId;
                OutputDirectory = CreateWorkingDirectory();

                var hydraRingConfigurationService = new HydraRingConfigurationService(uncertaintiesType);
                hydraRingConfigurationService.AddHydraRingCalculationInput(hydraRingCalculationInput);

                var hydraRingInitializationService = new HydraRingInitializationService(hydraRingCalculationInput.FailureMechanismType, sectionId, hlcdDirectory, OutputDirectory);
                hydraRingInitializationService.WriteInitializationScript();
                hydraRingConfigurationService.WriteDatabaseCreationScript(hydraRingInitializationService.DatabaseCreationScriptFilePath);

                PerformCalculation(OutputDirectory, hydraRingInitializationService);
                ExecuteGenericParsers(hydraRingInitializationService, sectionId);
                ExecuteCustomParsers(hydraRingInitializationService.TemporaryWorkingDirectory, sectionId);
            }
            catch (Exception e) when (IsSupportedCalculatedException(e))
            {
                throw new HydraRingCalculationException(Resources.HydraRingCalculatorBase_Calculate_Critical_error_during_calculation, e);
            }
        }

        private static bool IsSupportedCalculatedException(Exception e)
        {
            return e is SecurityException
                   || e is IOException
                   || e is UnauthorizedAccessException
                   || e is ArgumentException
                   || e is NotSupportedException
                   || e is Win32Exception
                   || e is HydraRingFileParserException;
        }

        /// <summary>
        /// Executes the generic parsers of the calculation.
        /// </summary>
        /// <param name="hydraRingInitializationService">The <see cref="HydraRingInitializationService"/> to get the directory from.</param>
        /// <param name="sectionId">The id of the section of the calculation.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when the HydraRing file parser 
        /// encounters an error while parsing HydraRing output.</exception>
        private void ExecuteGenericParsers(HydraRingInitializationService hydraRingInitializationService, int sectionId)
        {
            lastErrorFileParser.Parse(hydraRingInitializationService.TemporaryWorkingDirectory, sectionId);
            LastErrorFileContent = lastErrorFileParser.ErrorFileContent;
        }

        /// <summary>
        /// Executes the custom parsers of the calculation.
        /// </summary>
        /// <param name="temporaryWorkingDirectory">The temporary directory of the calculation output files.</param>
        /// <param name="sectionId">The id of the section of the calculation.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when the HydraRing file parser 
        /// encounters an error while parsing HydraRing output.</exception>
        private void ExecuteCustomParsers(string temporaryWorkingDirectory, int sectionId)
        {
            foreach (var parser in GetParsers())
            {
                parser.Parse(temporaryWorkingDirectory, sectionId);
            }

            SetOutputs();
        }

        /// <summary>
        /// Performs the calculation by starting a Hydra-Ring process.
        /// </summary>
        /// <param name="workingDirectory">The directory of the process.</param>
        /// <param name="hydraRingInitializationService">The <see cref="HydraRingConfigurationService"/>.</param>
        /// <exception cref="Win32Exception">Thrown when there was an error in opening the associated file
        /// or the wait setting could not be accessed.</exception>
        private void PerformCalculation(string workingDirectory, HydraRingInitializationService hydraRingInitializationService)
        {
            hydraRingProcess = HydraRingProcessFactory.Create(
                hydraRingInitializationService.MechanismComputationExeFilePath,
                hydraRingInitializationService.IniFilePath,
                workingDirectory);
            hydraRingProcess.Start();
            hydraRingProcess.WaitForExit();
            hydraRingProcess.Close();
            hydraRingProcess = null;
        }

        /// <summary>
        /// Creates the working directory of the calculation.
        /// </summary>
        /// <returns>The created working directory.</returns>
        /// <exception cref="SecurityException">Thrown when the temporary path can't be accessed due to missing permissions.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the directory can't be created due to missing
        /// the required permissions.</exception>
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