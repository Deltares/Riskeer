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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Utils;
using log4net;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service.IllustrationPoints;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;
using HydraRingGeneralResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for marginal wave statistics.
    /// </summary>
    public class WaveHeightCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WaveHeightCalculationService));

        private IWaveHeightCalculator calculator;
        private bool canceled;

        /// <summary>
        /// Performs validation over the values on the given <paramref name="hydraulicBoundaryDatabaseFilePath"/>.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database file which to validate.</param>
        /// <returns><c>True</c> if there were no validation errors; <c>False</c> otherwise.</returns>
        public static bool Validate(string hydraulicBoundaryDatabaseFilePath)
        {
            CalculationServiceHelper.LogValidationBegin();

            string[] validationProblems = ValidateInput(hydraulicBoundaryDatabaseFilePath);

            CalculationServiceHelper.LogMessagesAsError(Resources.Hydraulic_boundary_database_connection_failed_0_,
                                                        validationProblems);

            CalculationServiceHelper.LogValidationEnd();

            return !validationProblems.Any();
        }

        /// <summary>
        /// Performs a calculation for wave height.
        /// </summary>
        /// <param name="waveHeightCalculation">The wave height calculation to use.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="norm">The norm of the assessment section.</param>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveHeightCalculation"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when 
        /// <list type="bullet">
        /// <item><paramref name="hydraulicBoundaryDatabaseFilePath"/> contains invalid characters.</item>
        /// <item>The target probability or the calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</item>
        /// </list></exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        public void Calculate(IHydraulicBoundaryWrapperCalculation waveHeightCalculation,
                              string hydraulicBoundaryDatabaseFilePath,
                              double norm,
                              ICalculationMessageProvider messageProvider)
        {
            if (waveHeightCalculation == null)
            {
                throw new ArgumentNullException(nameof(waveHeightCalculation));
            }

            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);

            CalculationServiceHelper.LogCalculationBegin();

            calculator = HydraRingCalculatorFactory.Instance.CreateWaveHeightCalculator(hlcdDirectory);

            var exceptionThrown = false;

            try
            {
                PerformCalculation(waveHeightCalculation,
                                   hydraulicBoundaryDatabaseFilePath,
                                   norm,
                                   messageProvider);
            }
            catch (HydraRingCalculationException e)
            {
                if (!canceled)
                {
                    string lastErrorContent = calculator.LastErrorFileContent;
                    log.Error(!string.IsNullOrEmpty(lastErrorContent)
                                  ? messageProvider.GetCalculationFailedMessage(waveHeightCalculation.Name, lastErrorContent)
                                  : messageProvider.GetCalculationFailedMessage(waveHeightCalculation.Name, e.Message),
                              e);

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
                    log.Error(messageProvider.GetCalculationFailedMessage(waveHeightCalculation.Name, lastErrorFileContent));
                }

                log.InfoFormat(Resources.WaveHeightCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                CalculationServiceHelper.LogCalculationEnd();

                if (errorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }

        /// <summary>
        /// Cancels any currently running wave height calculation.
        /// </summary>
        public void Cancel()
        {
            calculator?.Cancel();
            canceled = true;
        }

        /// <summary>
        /// Performs a calculation for the wave height.
        /// </summary>
        /// <param name="waveHeightCalculation">The wave height calculation to use.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="norm">The norm of the assessment section.</param>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        private void PerformCalculation(IHydraulicBoundaryWrapperCalculation waveHeightCalculation,
                                        string hydraulicBoundaryDatabaseFilePath,
                                        double norm,
                                        ICalculationMessageProvider messageProvider)
        {
            WaveHeightCalculationInput calculationInput = CreateInput(waveHeightCalculation, norm, hydraulicBoundaryDatabaseFilePath);

            calculator.Calculate(calculationInput);

            if (canceled || !string.IsNullOrEmpty(calculator.LastErrorFileContent))
            {
                return;
            }

            HydraulicBoundaryLocationOutput hydraulicBoundaryLocationOutput = CreateHydraulicBoundaryLocationOutput(
                messageProvider, waveHeightCalculation.Name, calculationInput.Beta, norm, calculator.Converged);

            if (waveHeightCalculation.CalculateIllustrationPoints)
            {
                SetIllustrationPointsResult(hydraulicBoundaryLocationOutput, calculator.IllustrationPointsResult);
            }

            waveHeightCalculation.Output = hydraulicBoundaryLocationOutput;
        }

        /// <summary>
        /// Sets a <see cref="GeneralResult{T}"/> based on the information 
        /// of <paramref name="hydraRingGeneralResult"/> to the <paramref name="hydraulicBoundaryLocationOutput"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationOutput">The <see cref="HydraulicBoundaryLocationOutput"/> 
        /// for which to set the <see cref="GeneralResult{T}"/>.</param>
        /// <param name="hydraRingGeneralResult">The <see cref="HydraRingGeneralResult"/> to base the 
        /// <see cref="GeneralResult{T}"/> to create on.</param>
        private static void SetIllustrationPointsResult(HydraulicBoundaryLocationOutput hydraulicBoundaryLocationOutput,
                                                        HydraRingGeneralResult hydraRingGeneralResult)
        {
            if (hydraRingGeneralResult != null)
            {
                GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult =
                    GeneralResultConverter.CreateGeneralResultTopLevelSubMechanismIllustrationPoint(hydraRingGeneralResult);
                hydraulicBoundaryLocationOutput.SetIllustrationPoints(generalResult);
            }
        }

        /// <summary>
        /// Creates the output of the calculation.
        /// </summary>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <param name="hydraulicBoundaryLocationName">The name of the hydraulic boundary location.</param>
        /// <param name="targetReliability">The target reliability for the calculation.</param>
        /// <param name="targetProbability">The target probability for the calculation.</param>
        /// <param name="calculatorConverged">The value indicating whether the calculation converged.</param>
        /// <returns>A <see cref="HydraulicBoundaryLocationOutput"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or the calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        private HydraulicBoundaryLocationOutput CreateHydraulicBoundaryLocationOutput(
            ICalculationMessageProvider messageProvider,
            string hydraulicBoundaryLocationName,
            double targetReliability,
            double targetProbability,
            bool? calculatorConverged)
        {
            double waveHeight = calculator.WaveHeight;
            double reliability = calculator.ReliabilityIndex;
            double probability = StatisticsConverter.ReliabilityToProbability(reliability);

            CalculationConvergence converged = RingtoetsCommonDataCalculationService.GetCalculationConvergence(calculatorConverged);

            if (converged != CalculationConvergence.CalculatedConverged)
            {
                log.Warn(messageProvider.GetCalculatedNotConvergedMessage(hydraulicBoundaryLocationName));
            }

            return new HydraulicBoundaryLocationOutput(waveHeight, targetProbability,
                                                       targetReliability, probability, reliability,
                                                       converged);
        }

        /// <summary>
        /// Creates the input for an wave height calculation.
        /// </summary>
        /// <param name="waveHeightCalculation">The <see cref="IHydraulicBoundaryWrapperCalculation"/>
        /// to create the input from.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path to the hydraulic
        /// boundary database.</param>
        /// <returns>A <see cref="WaveHeightCalculationInput"/>.</returns>
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
        private static WaveHeightCalculationInput CreateInput(IHydraulicBoundaryWrapperCalculation waveHeightCalculation,
                                                              double norm,
                                                              string hydraulicBoundaryDatabaseFilePath)
        {
            var waveHeightCalculationInput = new WaveHeightCalculationInput(1, waveHeightCalculation.Id, norm);

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(waveHeightCalculationInput, hydraulicBoundaryDatabaseFilePath);

            return waveHeightCalculationInput;
        }

        private static string[] ValidateInput(string hydraulicBoundaryDatabaseFilePath)
        {
            var validationResult = new List<string>();

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabaseFilePath);

            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResult.Add(validationProblem);
            }

            return validationResult.ToArray();
        }
    }
}