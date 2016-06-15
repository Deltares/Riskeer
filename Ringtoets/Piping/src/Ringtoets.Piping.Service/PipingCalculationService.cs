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
using Ringtoets.Common.Service;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.Service.Properties;

using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// This class is responsible for invoking operations on the <see cref="PipingCalculator"/>. Error and status information is 
    /// logged during the execution of the operation. At the end of an operation, a <see cref="PipingCalculationResult"/> is returned,
    /// representing the result of the operation.
    /// </summary>
    public static class PipingCalculationService
    {
        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="PipingCalculation"/> for which to validate the values.</param>
        /// <returns><c>False</c> if <paramref name="calculation"/> contains validation errors; <c>True</c> otherwise.</returns>
        public static bool Validate(PipingCalculation calculation)
        {
            CalculationServiceHelper.LogValidationBeginTime(calculation.Name);

            var inputValidationResults = ValidateInput(calculation.InputParameters);

            if (inputValidationResults.Count > 0)
            {
                CalculationServiceHelper.LogMessagesAsError(Resources.Error_in_piping_validation_0, inputValidationResults.ToArray());
                CalculationServiceHelper.LogValidationEndTime(calculation.Name);
                return false;
            }

            var validationResults = new PipingCalculator(CreateInputFromData(calculation.InputParameters), PipingSubCalculatorFactory.Instance).Validate();
            CalculationServiceHelper.LogMessagesAsError(Resources.Error_in_piping_validation_0, validationResults.ToArray());

            CalculationServiceHelper.LogValidationEndTime(calculation.Name);

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
            CalculationServiceHelper.LogCalculationBeginTime(calculation.Name);

            try
            {
                var pipingResult = new PipingCalculator(CreateInputFromData(calculation.InputParameters), PipingSubCalculatorFactory.Instance).Calculate();

                calculation.Output = new PipingOutput(pipingResult.UpliftZValue,
                                                      pipingResult.UpliftFactorOfSafety,
                                                      pipingResult.HeaveZValue,
                                                      pipingResult.HeaveFactorOfSafety,
                                                      pipingResult.SellmeijerZValue,
                                                      pipingResult.SellmeijerFactorOfSafety);
            }
            catch (PipingCalculatorException e)
            {
                CalculationServiceHelper.LogMessagesAsError(Resources.Error_in_piping_calculation_0, e.Message);
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEndTime(calculation.Name);
            }
        }

        private static List<string> ValidateInput(PipingInput inputParameters)
        {
            List<string> validationResult = new List<string>();

            if (double.IsNaN(inputParameters.ThicknessAquiferLayer.Mean))
            {
                validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_thickness_aquifer_layer);
            }

            if (double.IsNaN(inputParameters.ThicknessCoverageLayer.Mean))
            {
                validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_thickness_coverage_layer);
            }

            return validationResult;
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
                inputParameters.StochasticSoilProfile.SoilProfile
                );
        }
    }
}