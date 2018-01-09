﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.Util;
using log4net;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.DuneErosion.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for dune erosion.
    /// </summary>
    public class DuneErosionBoundaryCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DuneErosionBoundaryCalculationService));

        private bool canceled;
        private IDunesBoundaryConditionsCalculator calculator;

        /// <summary>
        /// Performs validation on the given <paramref name="hydraulicBoundaryDatabaseFilePath"/> and <paramref name="preprocessorDirectory"/>.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database file which to validate.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory to validate.</param>
        /// <returns><c>true</c> if there were no validation errors; <c>false</c> otherwise.</returns>
        public static bool Validate(string hydraulicBoundaryDatabaseFilePath, string preprocessorDirectory)
        {
            var isValid = true;

            CalculationServiceHelper.LogValidationBegin();

            string databaseFilePathValidationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(hydraulicBoundaryDatabaseFilePath,
                                                                                                                   preprocessorDirectory);
            if (!string.IsNullOrEmpty(databaseFilePathValidationProblem))
            {
                CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                                                            new[]
                                                            {
                                                                databaseFilePathValidationProblem
                                                            });

                isValid = false;
            }

            string preprocessorDirectoryValidationProblem = HydraulicBoundaryDatabaseHelper.ValidatePreprocessorDirectory(preprocessorDirectory);
            if (!string.IsNullOrEmpty(preprocessorDirectoryValidationProblem))
            {
                CalculationServiceHelper.LogMessagesAsError(new[]
                {
                    preprocessorDirectoryValidationProblem
                });

                isValid = false;
            }

            CalculationServiceHelper.LogValidationEnd();

            return isValid;
        }

        /// <summary>
        /// Performs a dune erosion calculation based on the supplied <see cref="DuneLocation"/>
        /// and sets the <see cref="DuneLocationCalculation.Output"/> if the calculation is successful.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="duneLocation">The <see cref="DuneLocation"/> that holds information required 
        /// to perform the calculation.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic 
        /// boundary database file.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/> equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="duneLocation"/>,
        /// <paramref name="hydraulicBoundaryDatabaseFilePath"/> or <paramref name="preprocessorDirectory"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="hydraulicBoundaryDatabaseFilePath"/> contains invalid characters.</item>
        /// <item>The contribution of the failure mechanism is zero.</item>
        /// <item>The target probability or the calculated probability falls outside the [0.0, 1.0] 
        /// range and is not <see cref="double.NaN"/>.</item>
        /// </list></exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing 
        /// the calculation.</exception>
        public void Calculate(DuneLocation duneLocation,
                              double norm,
                              string hydraulicBoundaryDatabaseFilePath,
                              string preprocessorDirectory)
        {
            if (duneLocation == null)
            {
                throw new ArgumentNullException(nameof(duneLocation));
            }

            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            string duneLocationName = duneLocation.Name;

            CalculationServiceHelper.LogCalculationBegin();

            calculator = HydraRingCalculatorFactory.Instance.CreateDunesBoundaryConditionsCalculator(hlcdDirectory, preprocessorDirectory);

            var exceptionThrown = false;

            try
            {
                DunesBoundaryConditionsCalculationInput calculationInput = CreateInput(duneLocation,
                                                                                       norm,
                                                                                       hydraulicBoundaryDatabaseFilePath,
                                                                                       !string.IsNullOrEmpty(preprocessorDirectory));
                calculator.Calculate(calculationInput);

                if (string.IsNullOrEmpty(calculator.LastErrorFileContent))
                {
                    duneLocation.Calculation.Output = CreateDuneLocationOutput(duneLocationName, calculationInput.Beta, norm);
                }
            }
            catch (HydraRingCalculationException)
            {
                if (!canceled)
                {
                    string lastErrorContent = calculator.LastErrorFileContent;
                    log.Error(string.IsNullOrEmpty(lastErrorContent)
                                  ? string.Format(Resources.DuneErosionBoundaryCalculationService_Calculate_Error_in_DuneErosionBoundaryCalculation_0_no_error_report,
                                                  duneLocationName)
                                  : string.Format(Resources.DuneErosionBoundaryCalculationService_Calculate_Error_in_DuneErosionBoundaryCalculation_0_click_details_for_last_error_report_1,
                                                  duneLocationName, lastErrorContent));

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
                    log.ErrorFormat(Resources.DuneErosionBoundaryCalculationService_Calculate_Error_in_DuneErosionBoundaryCalculation_0_click_details_for_last_error_report_1,
                                    duneLocationName, lastErrorFileContent);
                }

                log.InfoFormat(Resources.DuneErosionBoundaryCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0,
                               calculator.OutputDirectory);
                CalculationServiceHelper.LogCalculationEnd();

                if (hasErrorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }

        /// <summary>
        /// Cancels any ongoing dune erosion calculation.
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
        /// <returns>A <see cref="DuneLocationOutput"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or the calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        private DuneLocationOutput CreateDuneLocationOutput(string duneLocationName, double targetReliability, double targetProbability)
        {
            double reliability = calculator.ReliabilityIndex;
            double probability = StatisticsConverter.ReliabilityToProbability(reliability);

            CalculationConvergence converged = RingtoetsCommonDataCalculationService.GetCalculationConvergence(calculator.Converged);

            if (converged != CalculationConvergence.CalculatedConverged)
            {
                log.WarnFormat(Resources.DuneErosionBoundaryCalculationService_CreateDuneLocationOutput_Calculation_for_DuneLocation_0_not_converged, duneLocationName);
            }

            return new DuneLocationOutput(converged,
                                          new DuneLocationOutput.ConstructionProperties
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
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path to the hydraulic
        /// boundary database.</param>
        /// <param name="usePreprocessor">Indicator whether to use the preprocessor in the calculation.</param>
        /// <returns>A <see cref="DunesBoundaryConditionsCalculationInput"/> with all needed
        /// input data.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryDatabaseFilePath"/> 
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        private static DunesBoundaryConditionsCalculationInput CreateInput(DuneLocation duneLocation,
                                                                           double norm,
                                                                           string hydraulicBoundaryDatabaseFilePath,
                                                                           bool usePreprocessor)
        {
            var dunesBoundaryConditionsCalculationInput = new DunesBoundaryConditionsCalculationInput(1, duneLocation.Id, norm);
            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(dunesBoundaryConditionsCalculationInput, hydraulicBoundaryDatabaseFilePath, usePreprocessor);
            return dunesBoundaryConditionsCalculationInput;
        }
    }
}