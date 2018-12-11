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
using Core.Common.Base.IO;
using Core.Common.Util;
using log4net;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;

namespace Ringtoets.DuneErosion.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for dune locations.
    /// </summary>
    public class DuneLocationCalculationService : TargetProbabilityCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DuneLocationCalculationService));

        private bool canceled;
        private IDunesBoundaryConditionsCalculator calculator;

        /// <summary>
        /// Performs the provided <see cref="DuneLocationCalculation"/> and sets its output if the calculation is successful.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="duneLocationCalculation">The <see cref="DuneLocationCalculation"/> to perform.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> containing all data
        /// to perform a hydraulic boundary calculation.</param>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <remarks>Preprocessing is disabled when the preprocessor directory equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="duneLocationCalculation"/>,
        /// <paramref name="calculationSettings"/> or <paramref name="messageProvider"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>The hydraulic boundary location database file path contains invalid characters.</item>
        /// <item>The contribution of the failure mechanism is zero.</item>
        /// <item>The target probability or the calculated probability falls outside the [0.0, 1.0] 
        /// range and is not <see cref="double.NaN"/>.</item>
        /// </list></exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of the hydraulic boundary database
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing 
        /// the calculation.</exception>
        public void Calculate(DuneLocationCalculation duneLocationCalculation,
                              double norm,
                              HydraulicBoundaryCalculationSettings calculationSettings,
                              ICalculationMessageProvider messageProvider)
        {
            if (duneLocationCalculation == null)
            {
                throw new ArgumentNullException(nameof(duneLocationCalculation));
            }

            if (calculationSettings == null)
            {
                throw new ArgumentNullException(nameof(calculationSettings));
            }

            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            DuneLocation duneLocation = duneLocationCalculation.DuneLocation;
            string duneLocationName = duneLocation.Name;

            CalculationServiceHelper.LogCalculationBegin();

            HydraRingCalculationSettings hydraRingCalculationSettings = HydraRingCalculationSettingsFactory.CreateSettings(calculationSettings);
            calculator = HydraRingCalculatorFactory.Instance.CreateDunesBoundaryConditionsCalculator(hydraRingCalculationSettings);

            var exceptionThrown = false;

            try
            {
                DunesBoundaryConditionsCalculationInput calculationInput = CreateInput(duneLocation,
                                                                                       norm,
                                                                                       calculationSettings);
                calculator.Calculate(calculationInput);

                if (string.IsNullOrEmpty(calculator.LastErrorFileContent))
                {
                    duneLocationCalculation.Output = CreateDuneLocationCalculationOutput(duneLocationName,
                                                                                         calculationInput.Beta,
                                                                                         norm,
                                                                                         messageProvider);
                }
            }
            catch (HydraRingCalculationException)
            {
                if (!canceled)
                {
                    string lastErrorContent = calculator.LastErrorFileContent;
                    log.Error(string.IsNullOrEmpty(lastErrorContent)
                                  ? messageProvider.GetCalculationFailedMessage(duneLocationName)
                                  : messageProvider.GetCalculationFailedWithErrorReportMessage(duneLocationName, lastErrorContent));

                    exceptionThrown = true;
                    throw;
                }
            }
            finally
            {
                string lastErrorFileContent = calculator.LastErrorFileContent;
                bool hasErrorOccurred = CalculationServiceHelper.HasErrorOccurred(canceled, exceptionThrown, lastErrorFileContent);
                if (hasErrorOccurred)
                {
                    log.Error(messageProvider.GetCalculationFailedWithErrorReportMessage(duneLocationName, lastErrorFileContent));
                }

                log.InfoFormat(Resources.DuneLocationCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0,
                               calculator.OutputDirectory);
                CalculationServiceHelper.LogCalculationEnd();

                if (hasErrorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }

        /// <summary>
        /// Cancels any ongoing dune location calculation.
        /// </summary>
        public void Cancel()
        {
            calculator?.Cancel();
            canceled = true;
        }

        /// <summary>
        /// Create the output of the calculation.
        /// </summary>
        /// <param name="duneLocationName">The name of the location.</param>
        /// <param name="targetReliability">The target reliability for the calculation.</param>
        /// <param name="targetProbability">The target probability for the calculation.</param>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <returns>A <see cref="DuneLocationCalculationOutput"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or the calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        private DuneLocationCalculationOutput CreateDuneLocationCalculationOutput(string duneLocationName,
                                                                                  double targetReliability,
                                                                                  double targetProbability,
                                                                                  ICalculationMessageProvider messageProvider)
        {
            double reliability = calculator.ReliabilityIndex;
            double probability = StatisticsConverter.ReliabilityToProbability(reliability);

            CalculationConvergence converged = RingtoetsCommonDataCalculationService.GetCalculationConvergence(calculator.Converged);

            if (converged != CalculationConvergence.CalculatedConverged)
            {
                log.WarnFormat(messageProvider.GetCalculatedNotConvergedMessage(duneLocationName));
            }

            return new DuneLocationCalculationOutput(converged,
                                                     new DuneLocationCalculationOutput.ConstructionProperties
                                                     {
                                                         WaterLevel = calculator.WaterLevel,
                                                         WaveHeight = calculator.WaveHeight,
                                                         WavePeriod = calculator.WavePeriod,
                                                         TargetProbability = targetProbability,
                                                         TargetReliability = targetReliability,
                                                         CalculatedProbability = probability,
                                                         CalculatedReliability = reliability
                                                     });
        }

        /// <summary>
        /// Creates the input used in the calculation.
        /// </summary>
        /// <param name="duneLocation">The <see cref="DuneLocation"/> to create the input for.</param>
        /// <param name="norm">The norm of the failure mechanism to use.</param>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> containing
        /// all data to perform a hydraulic boundary calculation.</param>
        /// <returns>A <see cref="DunesBoundaryConditionsCalculationInput"/> with all needed
        /// input data.</returns>
        /// <exception cref="ArgumentException">Thrown when the hydraulic boundary database file path.
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of the hydraulic boundary database file path.
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        private static DunesBoundaryConditionsCalculationInput CreateInput(DuneLocation duneLocation,
                                                                           double norm,
                                                                           HydraulicBoundaryCalculationSettings calculationSettings)
        {
            var dunesBoundaryConditionsCalculationInput = new DunesBoundaryConditionsCalculationInput(1, duneLocation.Id, norm);
            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(dunesBoundaryConditionsCalculationInput, 
                                                                       calculationSettings.HydraulicBoundaryDatabaseFilePath, 
                                                                       !string.IsNullOrEmpty(calculationSettings.PreprocessorDirectory));
            return dunesBoundaryConditionsCalculationInput;
        }
    }
}