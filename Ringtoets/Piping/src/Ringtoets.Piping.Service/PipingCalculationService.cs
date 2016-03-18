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

using log4net;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.Service.Properties;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// This class is responsible for invoking operations on the <see cref="PipingCalculator"/>. Error and status information is 
    /// logged during the execution of the operation. At the end of an operation, a <see cref="PipingCalculationResult"/> is returned,
    /// representing the result of the operation.
    /// </summary>
    public static class PipingCalculationService
    {
        private static readonly ILog pipingCalculationLogger = LogManager.GetLogger(typeof(PipingCalculation));
        public static IPipingSubCalculatorFactory SubCalculatorFactory = new PipingSubCalculatorFactory();

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="PipingCalculation"/> for which to validate the values.</param>
        /// <returns>False if <paramref name="calculation"/> contains validation errors; True otherwise.</returns>
        public static bool Validate(PipingCalculation calculation)
        {
            pipingCalculationLogger.Info(String.Format(Resources.Validation_Subject_0_started_Time_1_,
                                                       calculation.Name, DateTimeService.CurrentTimeAsString));

            var validationResults = new PipingCalculator(CreateInputFromData(calculation.InputParameters), SubCalculatorFactory).Validate();
            LogMessagesAsError(Resources.Error_in_piping_validation_0, validationResults.ToArray());

            pipingCalculationLogger.Info(String.Format(Resources.Validation_Subject_0_ended_Time_1_,
                                                calculation.Name, DateTimeService.CurrentTimeAsString));

            return validationResults.Count == 0;
        }

        /// <summary>
        /// Performs a piping calculation based on the supplied <see cref="PipingCalculation"/> and sets <see cref="PipingCalculation.Output"/>
        /// to the <see cref="PipingCalculationResult"/> if the calculation was successful. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="PipingCalculation"/> to base the input for the calculation upon.</param>
        /// <remarks>Consider calling <see cref="Validate"/> first to see if calculation is possible.</remarks>
        public static void Calculate(PipingCalculation calculation)
        {
            pipingCalculationLogger.Info(String.Format(Resources.Calculation_Subject_0_started_Time_1_,
                                                       calculation.Name, DateTimeService.CurrentTimeAsString));

            try
            {
                var pipingResult = new PipingCalculator(CreateInputFromData(calculation.InputParameters), SubCalculatorFactory).Calculate();

                calculation.Output = new PipingOutput(pipingResult.UpliftZValue,
                                                     pipingResult.UpliftFactorOfSafety,
                                                     pipingResult.HeaveZValue,
                                                     pipingResult.HeaveFactorOfSafety,
                                                     pipingResult.SellmeijerZValue,
                                                     pipingResult.SellmeijerFactorOfSafety);
            }
            catch (PipingCalculatorException e)
            {
                LogMessagesAsError(Resources.Error_in_piping_calculation_0, e.Message);
            }
            finally
            {
                pipingCalculationLogger.Info(String.Format(Resources.Calculation_Subject_0_ended_Time_1_,
                                                    calculation.Name, DateTimeService.CurrentTimeAsString));
            }
        }

        private static void LogMessagesAsError(string format, params string[] errorMessages)
        {
            foreach (var errorMessage in errorMessages)
            {
                pipingCalculationLogger.ErrorFormat(format, errorMessage);
            }
        }

        /// <summary>
        /// Calculates the thickness of the coverage layer based on the values of the <see cref="PipingInput"/>.
        /// </summary>
        /// <returns>The thickness of the coverage layer, or -1 if the thickness could not be calculated.</returns>
        public static double CalculateThicknessCoverageLayer(PipingInput input)
        {
            try
            {
                return new PipingCalculator(CreateInputFromData(input), SubCalculatorFactory).CalculateThicknessCoverageLayer();
            }
            catch (PipingCalculatorException)
            {
                return double.NaN;
            }
        }

        private static PipingCalculatorInput CreateInputFromData(PipingInput inputParameters)
        {
            return new PipingCalculatorInput(
                inputParameters.WaterVolumetricWeight,
                PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters).GetDesignValue(),
                inputParameters.UpliftModelFactor,
                inputParameters.AssessmentLevel,
                inputParameters.PiezometricHeadExit,
                PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(inputParameters).GetDesignValue(),
                PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(inputParameters).GetDesignValue(),
                inputParameters.CriticalHeaveGradient,
                PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(inputParameters).GetDesignValue(),
                inputParameters.SellmeijerModelFactor,
                inputParameters.SellmeijerReductionFactor,
                PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(inputParameters).GetDesignValue(),
                inputParameters.SandParticlesVolumicWeight,
                inputParameters.WhitesDragCoefficient,
                PipingSemiProbabilisticDesignValueFactory.GetDiameter70(inputParameters).GetDesignValue(),
                PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(inputParameters).GetDesignValue(),
                inputParameters.WaterKinematicViscosity,
                inputParameters.Gravity,
                PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(inputParameters).GetDesignValue(),
                inputParameters.MeanDiameter70,
                inputParameters.BeddingAngle,
                inputParameters.ExitPointL,
                inputParameters.SurfaceLine,
                inputParameters.SoilProfile
                );
        }

        public static double CalculatePiezometricHeadAtExit(PipingInput input)
        {
            return new PipingCalculator(CreateInputFromData(input), SubCalculatorFactory).CalculatePiezometricHeadAtExit();
        }
    }
}