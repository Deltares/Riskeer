﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Service.IllustrationPoints;
using Riskeer.Common.Service.MessageProviders;
using Riskeer.Common.Service.Properties;
using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.Exceptions;
using HydraRingGeneralResult = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for design water level.
    /// </summary>
    public class DesignWaterLevelCalculationService : TargetProbabilityCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DesignWaterLevelCalculationService));
        private IDesignWaterLevelCalculator calculator;
        private bool canceled;

        /// <summary>
        /// Performs a calculation for the design water level.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationCalculation">The hydraulic boundary location calculation to perform.</param>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> with the
        /// hydraulic boundary calculation settings.</param>
        /// <param name="targetProbability">The target probability to use during the calculation.</param>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <remarks>Preprocessing is disabled when the preprocessor directory equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocationCalculation"/>,
        /// <paramref name="calculationSettings"/> or <paramref name="messageProvider"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>the hydraulic boundary database file path contains invalid characters;</item>
        /// <item>the target probability or the calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>no hydraulic boundary settings database could be found;</item>
        /// <item>the hydraulic boundary settings database cannot be opened;</item>
        /// <item>the required data cannot be read from the hydraulic boundary settings database.</item>
        /// </list>
        /// </exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        public void Calculate(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                              HydraulicBoundaryCalculationSettings calculationSettings,
                              double targetProbability,
                              ICalculationMessageProvider messageProvider)
        {
            if (hydraulicBoundaryLocationCalculation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocationCalculation));
            }

            if (calculationSettings == null)
            {
                throw new ArgumentNullException(nameof(calculationSettings));
            }

            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryLocationCalculation.HydraulicBoundaryLocation;

            CalculationServiceHelper.LogCalculationBegin();

            HydraRingCalculationSettings hydraRingCalculationSettings = HydraRingCalculationSettingsFactory.CreateSettings(calculationSettings);
            calculator = HydraRingCalculatorFactory.Instance.CreateDesignWaterLevelCalculator(hydraRingCalculationSettings);

            var exceptionThrown = false;

            try
            {
                PerformCalculation(hydraulicBoundaryLocationCalculation,
                                   calculationSettings,
                                   targetProbability,
                                   messageProvider);
            }
            catch (HydraRingCalculationException)
            {
                if (!canceled)
                {
                    string lastErrorContent = calculator.LastErrorFileContent;
                    log.Error(string.IsNullOrEmpty(lastErrorContent)
                                  ? messageProvider.GetCalculationFailedMessage(hydraulicBoundaryLocation.Name)
                                  : messageProvider.GetCalculationFailedWithErrorReportMessage(hydraulicBoundaryLocation.Name, lastErrorContent));

                    exceptionThrown = true;
                    throw;
                }
            }
            finally
            {
                string lastErrorFileContent = calculator.LastErrorFileContent;
                bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(canceled, exceptionThrown, lastErrorFileContent);
                if (errorOccurred)
                {
                    log.Error(messageProvider.GetCalculationFailedWithErrorReportMessage(hydraulicBoundaryLocation.Name, lastErrorFileContent));
                }

                log.InfoFormat(Resources.DesignWaterLevelCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                CalculationServiceHelper.LogCalculationEnd();

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
        /// Performs a calculation for the design water level.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationCalculation">The hydraulic boundary location calculation to perform.</param>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> with the
        /// hydraulic boundary calculation settings.</param>
        /// <param name="targetProbability">The target probability to use during the calculation.</param>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>no hydraulic boundary settings database could be found;</item>
        /// <item>the hydraulic boundary settings database cannot be opened;</item>
        /// <item>the required data cannot be read from the hydraulic boundary settings database.</item>
        /// </list>
        /// </exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        private void PerformCalculation(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                        HydraulicBoundaryCalculationSettings calculationSettings,
                                        double targetProbability,
                                        ICalculationMessageProvider messageProvider)
        {
            HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryLocationCalculation.HydraulicBoundaryLocation;

            AssessmentLevelCalculationInput calculationInput = CreateInput(hydraulicBoundaryLocation.Id, targetProbability, calculationSettings);

            calculator.Calculate(calculationInput);

            if (canceled || !string.IsNullOrEmpty(calculator.LastErrorFileContent))
            {
                return;
            }

            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult = null;
            try
            {
                generalResult = hydraulicBoundaryLocationCalculation.InputParameters.ShouldIllustrationPointsBeCalculated
                                    ? GetGeneralResult(calculator.IllustrationPointsResult)
                                    : null;
            }
            catch (ArgumentException e)
            {
                log.Warn(string.Format(Resources.CalculationService_Error_in_reading_illustrationPoints_for_CalculationName_0_with_ErrorMessage_1,
                                       hydraulicBoundaryLocation.Name,
                                       e.Message));
            }

            HydraulicBoundaryLocationCalculationOutput hydraulicBoundaryLocationCalculationOutput = CreateOutput(
                messageProvider, hydraulicBoundaryLocation.Name, calculationInput.Beta, targetProbability, calculator.Converged, generalResult);

            hydraulicBoundaryLocationCalculation.Output = hydraulicBoundaryLocationCalculationOutput;
        }

        /// <summary>
        /// Gets a <see cref="GeneralResult{T}"/> based on the information of <paramref name="hydraRingGeneralResult"/>. 
        /// </summary>
        /// <param name="hydraRingGeneralResult">The <see cref="HydraRingGeneralResult"/> to base the 
        /// <see cref="GeneralResult{T}"/> to create on.</param>
        /// <returns>A <see cref="GeneralResult{T}"/>.</returns>
        private GeneralResult<TopLevelSubMechanismIllustrationPoint> GetGeneralResult(HydraRingGeneralResult hydraRingGeneralResult)
        {
            if (hydraRingGeneralResult == null)
            {
                log.Warn(calculator.IllustrationPointsParserErrorMessage);
                return null;
            }

            try
            {
                return GeneralResultConverter.ConvertToGeneralResultTopLevelSubMechanismIllustrationPoint(hydraRingGeneralResult);
            }
            catch (IllustrationPointConversionException e)
            {
                log.Warn(Resources.SetGeneralResult_Converting_IllustrationPointResult_Failed, e);
            }

            return null;
        }

        /// <summary>
        /// Creates the output of the calculation.
        /// </summary>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <param name="hydraulicBoundaryLocationName">The name of the hydraulic boundary location.</param>
        /// <param name="targetReliability">The target reliability for the calculation.</param>
        /// <param name="targetProbability">The target probability for the calculation.</param>
        /// <param name="calculatorConverged">The value indicating whether the calculation converged.</param>
        /// <param name="generalResult">The general result with illustration points.</param>
        /// <returns>A <see cref="HydraulicBoundaryLocationCalculationOutput"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or the calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        private HydraulicBoundaryLocationCalculationOutput CreateOutput(ICalculationMessageProvider messageProvider,
                                                                        string hydraulicBoundaryLocationName,
                                                                        double targetReliability,
                                                                        double targetProbability,
                                                                        bool? calculatorConverged,
                                                                        GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult)
        {
            double designWaterLevel = calculator.DesignWaterLevel;
            double reliability = calculator.ReliabilityIndex;
            double probability = StatisticsConverter.ReliabilityToProbability(reliability);

            CalculationConvergence converged = RiskeerCommonDataCalculationService.GetCalculationConvergence(calculatorConverged);

            if (converged != CalculationConvergence.CalculatedConverged)
            {
                log.Warn(messageProvider.GetCalculatedNotConvergedMessage(hydraulicBoundaryLocationName));
            }

            return new HydraulicBoundaryLocationCalculationOutput(designWaterLevel, targetProbability,
                                                                  targetReliability, probability, reliability,
                                                                  converged,
                                                                  generalResult);
        }

        /// <summary>
        /// Creates the input for a design water level calculation.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="targetProbability">The target probability to use during the calculation.</param>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> with the
        /// hydraulic boundary calculation settings.</param>
        /// <returns>An <see cref="AssessmentLevelCalculationInput"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the hydraulic boundary database file path
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>no hydraulic boundary settings database could be found;</item>
        /// <item>the hydraulic boundary settings database cannot be opened;</item>
        /// <item>the required data cannot be read from the hydraulic boundary settings database.</item>
        /// </list>
        /// </exception>
        private static AssessmentLevelCalculationInput CreateInput(long hydraulicBoundaryLocationId,
                                                                   double targetProbability,
                                                                   HydraulicBoundaryCalculationSettings calculationSettings)
        {
            var assessmentLevelCalculationInput = new AssessmentLevelCalculationInput(1, hydraulicBoundaryLocationId, targetProbability);

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(assessmentLevelCalculationInput, calculationSettings.HrdFilePath, false);

            return assessmentLevelCalculationInput;
        }
    }
}