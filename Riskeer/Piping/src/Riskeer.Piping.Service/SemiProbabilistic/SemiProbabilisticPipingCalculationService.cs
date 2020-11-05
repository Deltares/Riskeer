// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Service;
using Riskeer.Common.Service.ValidationRules;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.KernelWrapper;
using Riskeer.Piping.KernelWrapper.SubCalculator;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;

namespace Riskeer.Piping.Service.SemiProbabilistic
{
    /// <summary>
    /// This class is responsible for invoking operations on the <see cref="PipingCalculator"/>. Error and status information is 
    /// logged during the execution of the operation.
    /// </summary>
    public static class SemiProbabilisticPipingCalculationService
    {
        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="SemiProbabilisticPipingCalculation"/> for which to validate the values.</param>
        /// <param name="generalInput">The <see cref="GeneralPipingInput"/> to derive values from during the validation.</param>
        /// <param name="normativeAssessmentLevel">The normative assessment level to use in case the manual assessment level is not applicable.</param>
        /// <returns><c>false</c> if <paramref name="calculation"/> contains validation errors; <c>true</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> or <paramref name="generalInput"/> is <c>null</c>.</exception>
        public static bool Validate(SemiProbabilisticPipingCalculation calculation,
                                    GeneralPipingInput generalInput,
                                    RoundedDouble normativeAssessmentLevel)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            CalculationServiceHelper.LogValidationBegin();

            LogAnyWarnings(calculation);

            bool hasErrors = LogAnyErrors(calculation, generalInput, normativeAssessmentLevel);

            CalculationServiceHelper.LogValidationEnd();

            return !hasErrors;
        }

        /// <summary>
        /// Performs a piping calculation based on the supplied <see cref="SemiProbabilisticPipingCalculation"/> and sets
        /// <see cref="SemiProbabilisticPipingCalculation.Output"/> if the calculation was successful. Error and status
        /// information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="SemiProbabilisticPipingCalculation"/> to base the input for the calculation upon.</param>
        /// <param name="generalInput">The <see cref="GeneralPipingInput"/> to derive values from during the calculation.</param>
        /// <param name="normativeAssessmentLevel">The normative assessment level to use in case the manual assessment level is not applicable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> or <paramref name="generalInput"/> is <c>null</c>.</exception>
        /// <exception cref="PipingCalculatorException">Thrown when an unexpected error occurred during the calculation.</exception>
        /// <remarks>Consider calling <see cref="Validate"/> first to see if calculation is possible.</remarks>
        public static void Calculate(SemiProbabilisticPipingCalculation calculation,
                                     GeneralPipingInput generalInput,
                                     RoundedDouble normativeAssessmentLevel)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            CalculationServiceHelper.LogCalculationBegin();

            try
            {
                PipingCalculatorResult pipingResult = new PipingCalculator(CreateInputFromData(calculation.InputParameters,
                                                                                               generalInput,
                                                                                               normativeAssessmentLevel),
                                                                           PipingSubCalculatorFactory.Instance).Calculate();

                calculation.Output = new SemiProbabilisticPipingOutput(new SemiProbabilisticPipingOutput.ConstructionProperties
                {
                    UpliftFactorOfSafety = pipingResult.UpliftFactorOfSafety,
                    HeaveFactorOfSafety = pipingResult.HeaveFactorOfSafety,
                    SellmeijerFactorOfSafety = pipingResult.SellmeijerFactorOfSafety,
                    UpliftEffectiveStress = pipingResult.UpliftEffectiveStress,
                    HeaveGradient = pipingResult.HeaveGradient,
                    SellmeijerCreepCoefficient = pipingResult.SellmeijerCreepCoefficient,
                    SellmeijerCriticalFall = pipingResult.SellmeijerCriticalFall,
                    SellmeijerReducedFall = pipingResult.SellmeijerReducedFall
                });
            }
            catch (PipingCalculatorException e)
            {
                CalculationServiceHelper.LogExceptionAsError(RiskeerCommonServiceResources.CalculationService_Calculate_unexpected_error, e);

                throw;
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEnd();
            }
        }

        private static void LogAnyWarnings(SemiProbabilisticPipingCalculation calculation)
        {
            CalculationServiceHelper.LogMessagesAsWarning(PipingCalculationValidationHelper.GetValidationWarnings(calculation.InputParameters).ToArray());

            RoundedDouble diameter70Value = SemiProbabilisticPipingDesignVariableFactory.GetDiameter70(calculation.InputParameters).GetDesignValue();
            if (!double.IsNaN(diameter70Value) && (diameter70Value < 6.3e-5 || diameter70Value > 0.5e-3))
            {
                CalculationServiceHelper.LogMessagesAsWarning(new[]
                {
                    string.Format(Resources.PipingCalculationService_GetInputWarnings_Specified_DiameterD70_value_0_not_in_valid_range_of_model, diameter70Value)
                });
            }
        }

        private static bool LogAnyErrors(SemiProbabilisticPipingCalculation calculation, GeneralPipingInput generalInput, RoundedDouble normativeAssessmentLevel)
        {
            string[] messages = ValidateInput(calculation.InputParameters, generalInput, normativeAssessmentLevel).ToArray();

            if (messages.Length > 0)
            {
                CalculationServiceHelper.LogMessagesAsError(messages);
                return true;
            }

            messages = new PipingCalculator(CreateInputFromData(calculation.InputParameters, generalInput, normativeAssessmentLevel),
                                            PipingSubCalculatorFactory.Instance).Validate().ToArray();

            if (messages.Length > 0)
            {
                CalculationServiceHelper.LogMessagesAsError(messages);
                return true;
            }

            return false;
        }

        private static List<string> ValidateInput(SemiProbabilisticPipingInput input, GeneralPipingInput generalInput, RoundedDouble normativeAssessmentLevel)
        {
            var validationResults = new List<string>();

            validationResults.AddRange(ValidateHydraulics(input, normativeAssessmentLevel));
            validationResults.AddRange(PipingCalculationValidationHelper.GetValidationErrors(input));

            if (!validationResults.Any())
            {
                validationResults.AddRange(ValidateCoverageLayers(input, generalInput));
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateHydraulics(SemiProbabilisticPipingInput input, RoundedDouble normativeAssessmentLevel)
        {
            var validationResults = new List<string>();
            if (!input.UseAssessmentLevelManualInput && input.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(RiskeerCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }
            else
            {
                validationResults.AddRange(ValidateAssessmentLevel(input, normativeAssessmentLevel));

                RoundedDouble piezometricHeadExit = DerivedSemiProbabilisticPipingInput.GetPiezometricHeadExit(input, GetEffectiveAssessmentLevel(input, normativeAssessmentLevel));
                if (double.IsNaN(piezometricHeadExit) || double.IsInfinity(piezometricHeadExit))
                {
                    validationResults.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_PiezometricHeadExit);
                }
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateAssessmentLevel(SemiProbabilisticPipingInput input, RoundedDouble normativeAssessmentLevel)
        {
            var validationResult = new List<string>();

            if (input.UseAssessmentLevelManualInput)
            {
                validationResult.AddRange(new NumericInputRule(input.AssessmentLevel, ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.WaterLevel_DisplayName)).Validate());
            }
            else
            {
                if (double.IsNaN(normativeAssessmentLevel))
                {
                    validationResult.Add(RiskeerCommonServiceResources.CalculationService_ValidateInput_Cannot_determine_AssessmentLevel);
                }
            }

            return validationResult;
        }

        private static IEnumerable<string> ValidateCoverageLayers(PipingInput input, GeneralPipingInput generalInput)
        {
            double surfaceLevel = input.SurfaceLine.GetZAtL(input.ExitPointL);
            PipingSoilProfile pipingSoilProfile = input.StochasticSoilProfile.SoilProfile;

            if (pipingSoilProfile.GetConsecutiveCoverageLayersBelowLevel(surfaceLevel).Any())
            {
                LogNormalDistribution thicknessCoverageLayer = DerivedPipingInput.GetThicknessCoverageLayer(input);
                if (!double.IsNaN(thicknessCoverageLayer.Mean))
                {
                    RoundedDouble saturatedVolumicWeightOfCoverageLayer = SemiProbabilisticPipingDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(input).GetDesignValue();
                    if (!double.IsNaN(saturatedVolumicWeightOfCoverageLayer) && saturatedVolumicWeightOfCoverageLayer < generalInput.WaterVolumetricWeight)
                    {
                        yield return Resources.PipingCalculationService_ValidateInput_SaturatedVolumicWeightCoverageLayer_must_be_larger_than_WaterVolumetricWeight;
                    }
                }
            }
        }

        private static PipingCalculatorInput CreateInputFromData(SemiProbabilisticPipingInput input,
                                                                 GeneralPipingInput generalPipingInput,
                                                                 RoundedDouble normativeAssessmentLevel)
        {
            RoundedDouble effectiveAssessmentLevel = GetEffectiveAssessmentLevel(input, normativeAssessmentLevel);

            return new PipingCalculatorInput(
                new PipingCalculatorInput.ConstructionProperties
                {
                    WaterVolumetricWeight = generalPipingInput.WaterVolumetricWeight,
                    SaturatedVolumicWeightOfCoverageLayer = SemiProbabilisticPipingDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(input).GetDesignValue(),
                    UpliftModelFactor = SemiProbabilisticPipingDesignVariableFactory.GetUpliftModelFactorDesignVariable(generalPipingInput).GetDesignValue(),
                    AssessmentLevel = effectiveAssessmentLevel,
                    PiezometricHeadExit = DerivedSemiProbabilisticPipingInput.GetPiezometricHeadExit(input, effectiveAssessmentLevel),
                    DampingFactorExit = SemiProbabilisticPipingDesignVariableFactory.GetDampingFactorExit(input).GetDesignValue(),
                    PhreaticLevelExit = PipingDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                    CriticalHeaveGradient = SemiProbabilisticPipingDesignVariableFactory.GetCriticalHeaveGradientDesignVariable(generalPipingInput).GetDesignValue(),
                    ThicknessCoverageLayer = SemiProbabilisticPipingDesignVariableFactory.GetThicknessCoverageLayer(input).GetDesignValue(),
                    EffectiveThicknessCoverageLayer = SemiProbabilisticPipingDesignVariableFactory.GetEffectiveThicknessCoverageLayer(input, generalPipingInput).GetDesignValue(),
                    SellmeijerModelFactor = SemiProbabilisticPipingDesignVariableFactory.GetSellmeijerModelFactorDesignVariable(generalPipingInput).GetDesignValue(),
                    SellmeijerReductionFactor = generalPipingInput.SellmeijerReductionFactor,
                    SeepageLength = SemiProbabilisticPipingDesignVariableFactory.GetSeepageLength(input).GetDesignValue(),
                    SandParticlesVolumicWeight = generalPipingInput.SandParticlesVolumicWeight,
                    WhitesDragCoefficient = generalPipingInput.WhitesDragCoefficient,
                    Diameter70 = SemiProbabilisticPipingDesignVariableFactory.GetDiameter70(input).GetDesignValue(),
                    DarcyPermeability = SemiProbabilisticPipingDesignVariableFactory.GetDarcyPermeability(input).GetDesignValue(),
                    WaterKinematicViscosity = generalPipingInput.WaterKinematicViscosity,
                    Gravity = generalPipingInput.Gravity,
                    ThicknessAquiferLayer = SemiProbabilisticPipingDesignVariableFactory.GetThicknessAquiferLayer(input).GetDesignValue(),
                    MeanDiameter70 = generalPipingInput.MeanDiameter70,
                    BeddingAngle = generalPipingInput.BeddingAngle,
                    ExitPointXCoordinate = input.ExitPointL,
                    SurfaceLine = input.SurfaceLine,
                    SoilProfile = input.StochasticSoilProfile?.SoilProfile
                });
        }

        private static RoundedDouble GetEffectiveAssessmentLevel(SemiProbabilisticPipingInput input, RoundedDouble normativeAssessmentLevel)
        {
            return input.UseAssessmentLevelManualInput
                       ? input.AssessmentLevel
                       : normativeAssessmentLevel;
        }
    }
}