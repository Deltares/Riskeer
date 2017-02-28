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
using System.IO;
using System.Linq;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using log4net;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for design water level.
    /// </summary>
    public class DesignWaterLevelCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DesignWaterLevelCalculationService));
        private IDesignWaterLevelCalculator calculator;
        private bool canceled;

        /// <summary>
        /// Performs validation over the values on the given <paramref name="hydraulicBoundaryDatabaseFilePath"/>.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="name">The name of the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database file which to validate.</param>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <returns><c>True</c> if there were no validation errors; <c>False</c> otherwise.</returns>
        public static bool Validate(string name, string hydraulicBoundaryDatabaseFilePath, ICalculationMessageProvider messageProvider)
        {
            string calculationName = messageProvider.GetCalculationName(name);

            CalculationServiceHelper.LogValidationBeginTime(calculationName);

            string[] validationProblems = ValidateInput(hydraulicBoundaryDatabaseFilePath);

            CalculationServiceHelper.LogMessagesAsError(Resources.Hydraulic_boundary_database_connection_failed_0_,
                                                        validationProblems);

            CalculationServiceHelper.LogValidationEndTime(calculationName);

            return !validationProblems.Any();
        }

        /// <summary>
        /// Performs a calculation for the design water level.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location used in the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="norm">The norm of the assessment section.</param>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when 
        /// <list type="bullet">
        /// <item><paramref name="hydraulicBoundaryDatabaseFilePath"/> contains invalid characters.</item>
        /// <item>The target propability or the calculated propability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</item>
        /// </list></exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>        
        public void Calculate(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                              string hydraulicBoundaryDatabaseFilePath,
                              double norm,
                              ICalculationMessageProvider messageProvider)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }
            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            string calculationName = messageProvider.GetCalculationName(hydraulicBoundaryLocation.Name);

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            calculator = HydraRingCalculatorFactory.Instance.CreateDesignWaterLevelCalculator(hlcdDirectory);

            var exceptionThrown = false;

            try
            {
                AssessmentLevelCalculationInput calculationInput = CreateInput(hydraulicBoundaryLocation, norm, hydraulicBoundaryDatabaseFilePath);
                calculator.Calculate(calculationInput);

                if (string.IsNullOrEmpty(calculator.LastErrorFileContent))
                {
                    hydraulicBoundaryLocation.DesignWaterLevelOutput = CreateHydraulicBoundaryLocationOutput(
                        messageProvider, hydraulicBoundaryLocation.Name, calculationInput.Beta, norm, calculator.Converged);
                }
            }
            catch (HydraRingCalculationException)
            {
                if (!canceled)
                {
                    var lastErrorContent = calculator.LastErrorFileContent;
                    log.Error(string.IsNullOrEmpty(lastErrorContent)
                                  ? messageProvider.GetCalculationFailedUnexplainedMessage(hydraulicBoundaryLocation.Name)
                                  : messageProvider.GetCalculationFailedMessage(hydraulicBoundaryLocation.Name, lastErrorContent));

                    exceptionThrown = true;
                    throw;
                }
            }
            finally
            {
                var lastErrorFileContent = calculator.LastErrorFileContent;
                bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(canceled, exceptionThrown, lastErrorFileContent);
                if (errorOccurred)
                {
                    log.Error(messageProvider.GetCalculationFailedMessage(hydraulicBoundaryLocation.Name, lastErrorFileContent));
                }

                log.InfoFormat(Resources.DesignWaterLevelCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                CalculationServiceHelper.LogCalculationEndTime(calculationName);

                if (errorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }

        /// <summary>
        /// Cancels the currently running design water level calculation.
        /// </summary>
        public void Cancel()
        {
            calculator?.Cancel();
            canceled = true;
        }

        /// <summary>
        /// Creates the output of the calculation.
        /// </summary>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <param name="hydraulicBoundaryLocationName">The name of the hydraulic boundary location.</param>
        /// <param name="targetReliability">The target reliability for the calculation.</param>
        /// <param name="targetProbability">The target probability for the calculation.</param>
        /// <returns>A <see cref="HydraulicBoundaryLocationOutput"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or the calculated propability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        private HydraulicBoundaryLocationOutput CreateHydraulicBoundaryLocationOutput(
            ICalculationMessageProvider messageProvider,
            string hydraulicBoundaryLocationName,
            double targetReliability,
            double targetProbability,
            bool? calculatorConverged)
        {
            var designWaterLevel = calculator.DesignWaterLevel;
            var reliability = calculator.ReliabilityIndex;
            var probability = StatisticsConverter.ReliabilityToProbability(reliability);

            CalculationConvergence converged = RingtoetsCommonDataCalculationService.GetCalculationConvergence(calculatorConverged);

            if (converged != CalculationConvergence.CalculatedConverged)
            {
                log.Warn(messageProvider.GetCalculatedNotConvergedMessage(hydraulicBoundaryLocationName));
            }

            return new HydraulicBoundaryLocationOutput(designWaterLevel, targetProbability,
                                                       targetReliability, probability, reliability,
                                                       converged);
        }

        /// <summary>
        /// Creates the input for an wave height calculation.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> to create the input from.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The filepath to the hydraulic boundary database.</param>
        /// <returns>An <see cref="AssessmentLevelCalculationInput"/>.</returns>
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
        private static AssessmentLevelCalculationInput CreateInput(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                   double norm,
                                                                   string hydraulicBoundaryDatabaseFilePath)
        {
            var assessmentLevelCalculationInput = new AssessmentLevelCalculationInput(1, hydraulicBoundaryLocation.Id, norm);

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(assessmentLevelCalculationInput, hydraulicBoundaryDatabaseFilePath);

            return assessmentLevelCalculationInput;
        }

        private static string[] ValidateInput(string hydraulicBoundaryDatabaseFilePath)
        {
            var validationResult = new List<string>();

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabaseFilePath);

            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResult.Add(validationProblem);
            }

            return validationResult.ToArray();
        }
    }
}