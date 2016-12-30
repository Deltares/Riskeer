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
using System.IO;
using Core.Common.Utils;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;

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
        /// Performs a dune erosion calculation based on the supplied <see cref="DuneLocation"/>
        /// and sets the <see cref="DuneLocation.Output"/> if the calculation is successful.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="duneLocation">The <see cref="DuneLocation"/> that holds information required to perform the calculation.</param>
        /// <param name="failureMechanism">The <see cref="DuneErosionFailureMechanism"/> that holds information about the contribution and
        /// the general inputs used in the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that hold information about the norm used in the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path wich points to the hydraulic boundary database file.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when an error occurs during parsing of the Hydra-Ring output.</exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs during the calculation.</exception>
        public void Calculate(DuneLocation duneLocation,
                              DuneErosionFailureMechanism failureMechanism,
                              IAssessmentSection assessmentSection,
                              string hydraulicBoundaryDatabaseFilePath)
        {
            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            calculator = HydraRingCalculatorFactory.Instance.CreateDunesBoundaryConditionsCalculator(hlcdDirectory, assessmentSection.Id);

            string calculationName = duneLocation.Name;

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            var exceptionThrown = false;
            var inputValid = true;
            try
            {
                double mechanismSpecificNorm = failureMechanism.GetMechanismSpecificNorm(assessmentSection);
                DunesBoundaryConditionsCalculationInput calculationInput = CreateInput(duneLocation, mechanismSpecificNorm, hydraulicBoundaryDatabaseFilePath);
                calculator.Calculate(calculationInput);

                if (string.IsNullOrEmpty(calculator.LastErrorFileContent))
                {
                    duneLocation.Output = CreateDuneLocationOutput(duneLocation.Name, calculationInput.Beta, mechanismSpecificNorm);
                }
            }
            catch (ArgumentException e)
            {
                log.Error(e.Message);
                exceptionThrown = true;
                inputValid = false;
                throw;
            }
            catch (HydraRingFileParserException)
            {
                if (!canceled)
                {
                    var lastErrorContent = calculator.LastErrorFileContent;
                    if (string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(Resources.DuneErosionBoundaryCalculationService_Calculate_Error_in_dune_erosion_0_calculation_no_error_report,
                                        calculationName);
                    }
                    else
                    {
                        log.ErrorFormat(Resources.DuneErosionBoundaryCalculationService_Calculate_Error_in_dune_erosion_0_calculation_click_details_for_last_error_report_1,
                                        calculationName, lastErrorContent);
                    }

                    exceptionThrown = true;
                    throw;
                }
            }
            finally
            {
                var lastErrorFileContent = calculator.LastErrorFileContent;
                bool errorOccurred = CalculationServiceHelper.ErrorOccurred(canceled, exceptionThrown, lastErrorFileContent);
                if (errorOccurred)
                {
                    log.ErrorFormat(Resources.DuneErosionBoundaryCalculationService_Calculate_Error_in_dune_erosion_0_calculation_click_details_for_last_error_report_1,
                                    calculationName, lastErrorFileContent);
                }

                if (inputValid)
                {
                    log.InfoFormat(Resources.DuneErosionBoundaryCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                }

                CalculationServiceHelper.LogCalculationEndTime(calculationName);

                if (errorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }

        /// <summary>
        /// Cancels any ongoing structures stability point calculation.
        /// </summary>
        public void Cancel()
        {
            if (calculator != null)
            {
                calculator.Cancel();
            }

            canceled = true;
        }

        private DuneLocationOutput CreateDuneLocationOutput(string duneLocationName, double targetReliability, double targetProbability)
        {
            var reliability = calculator.ReliabilityIndex;
            var probability = StatisticsConverter.ReliabilityToProbability(reliability);

            CalculationConvergence converged = RingtoetsCommonDataCalculationService.CalculationConverged(
                calculator.ReliabilityIndex, targetProbability);

            if (converged != CalculationConvergence.CalculatedConverged)
            {
                log.WarnFormat(Resources.DuneErosionBoundaryCalculationService_CreateDuneLocationOutput_Calculation_for_location_0_not_converged, duneLocationName);
            }

            return new DuneLocationOutput(calculator.WaterLevel, calculator.WaveHeight,
                                          calculator.WavePeriod, targetProbability,
                                          targetReliability, probability,
                                          reliability, converged);
        }

        private static DunesBoundaryConditionsCalculationInput CreateInput(DuneLocation duneLocation,
                                                                           double norm,
                                                                           string hydraulicBoundaryDatabaseFilePath)
        {
            var dunesBoundaryConditionsCalculationInput = new DunesBoundaryConditionsCalculationInput(1, duneLocation.Id, norm, duneLocation.Orientation);
            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(dunesBoundaryConditionsCalculationInput, hydraulicBoundaryDatabaseFilePath);
            return dunesBoundaryConditionsCalculationInput;
        }
    }
}