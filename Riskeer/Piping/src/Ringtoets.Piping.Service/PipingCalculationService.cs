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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// This class is responsible for invoking operations on the <see cref="PipingCalculator"/>. Error and status information is 
    /// logged during the execution of the operation.
    /// </summary>
    public static class PipingCalculationService
    {
        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="PipingCalculation"/> for which to validate the values.</param>
        /// <param name="normativeAssessmentLevel">The normative assessment level to use in case the manual assessment level is not applicable.</param>
        /// <returns><c>false</c> if <paramref name="calculation"/> contains validation errors; <c>true</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static bool Validate(PipingCalculation calculation, RoundedDouble normativeAssessmentLevel)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            CalculationServiceHelper.LogValidationBegin();

            CalculationServiceHelper.LogMessagesAsWarning(GetInputWarnings(calculation.InputParameters).ToArray());

            string[] inputValidationResults = ValidateInput(calculation.InputParameters, normativeAssessmentLevel).ToArray();

            if (inputValidationResults.Length > 0)
            {
                CalculationServiceHelper.LogMessagesAsError(inputValidationResults);
                CalculationServiceHelper.LogValidationEnd();
                return false;
            }

            List<string> validationResults = new PipingCalculator(CreateInputFromData(calculation.InputParameters, normativeAssessmentLevel),
                                                                  PipingSubCalculatorFactory.Instance).Validate();

            CalculationServiceHelper.LogMessagesAsError(validationResults.ToArray());

            CalculationServiceHelper.LogValidationEnd();

            return validationResults.Count == 0;
        }

        /// <summary>
        /// Performs a piping calculation based on the supplied <see cref="PipingCalculation"/> and sets <see cref="PipingCalculation.Output"/>
        /// based on the result if the calculation was successful. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="PipingCalculation"/> to base the input for the calculation upon.</param>
        /// <param name="normativeAssessmentLevel">The normative assessment level to use in case the manual assessment level is not applicable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        /// <exception cref="PipingCalculatorException">Thrown when an unexpected error occurred during the calculation.</exception>
        /// <remarks>Consider calling <see cref="Validate"/> first to see if calculation is possible.</remarks>
        public static void Calculate(PipingCalculation calculation, RoundedDouble normativeAssessmentLevel)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            CalculationServiceHelper.LogCalculationBegin();

            try
            {
                PipingCalculatorResult pipingResult = new PipingCalculator(CreateInputFromData(calculation.InputParameters, normativeAssessmentLevel),
                                                                           PipingSubCalculatorFactory.Instance).Calculate();

                calculation.Output = new PipingOutput(new PipingOutput.ConstructionProperties
                {
                    UpliftZValue = pipingResult.UpliftZValue,
                    UpliftFactorOfSafety = pipingResult.UpliftFactorOfSafety,
                    HeaveZValue = pipingResult.HeaveZValue,
                    HeaveFactorOfSafety = pipingResult.HeaveFactorOfSafety,
                    SellmeijerZValue = pipingResult.SellmeijerZValue,
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
                CalculationServiceHelper.LogExceptionAsError(RingtoetsCommonServiceResources.CalculationService_Calculate_unexpected_error, e);

                throw;
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEnd();
            }
        }

        private static List<string> ValidateInput(PipingInput inputParameters, RoundedDouble normativeAssessmentLevel)
        {
            var validationResults = new List<string>();

            validationResults.AddRange(ValidateHydraulics(inputParameters, normativeAssessmentLevel));

            IEnumerable<string> coreValidationError = ValidateCoreSurfaceLineAndSoilProfileProperties(inputParameters);
            validationResults.AddRange(coreValidationError);

            if (double.IsNaN(inputParameters.EntryPointL))
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_No_value_for_EntryPointL);
            }

            if (!coreValidationError.Any())
            {
                validationResults.AddRange(ValidateSoilLayers(inputParameters));
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateHydraulics(PipingInput inputParameters, RoundedDouble normativeAssessmentLevel)
        {
            var validationResults = new List<string>();
            if (!inputParameters.UseAssessmentLevelManualInput && inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }
            else
            {
                validationResults.AddRange(ValidateAssessmentLevel(inputParameters, normativeAssessmentLevel));

                RoundedDouble piezometricHeadExit = DerivedPipingInput.GetPiezometricHeadExit(inputParameters, GetEffectiveAssessmentLevel(inputParameters, normativeAssessmentLevel));
                if (double.IsNaN(piezometricHeadExit) || double.IsInfinity(piezometricHeadExit))
                {
                    validationResults.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_PiezometricHeadExit);
                }
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateAssessmentLevel(PipingInput inputParameters, RoundedDouble normativeAssessmentLevel)
        {
            var validationResult = new List<string>();

            if (inputParameters.UseAssessmentLevelManualInput)
            {
                validationResult.AddRange(new NumericInputRule(inputParameters.AssessmentLevel, ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.WaterLevel_DisplayName)).Validate());
            }
            else
            {
                if (double.IsNaN(normativeAssessmentLevel))
                {
                    validationResult.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_Cannot_determine_AssessmentLevel);
                }
            }

            return validationResult;
        }

        private static IEnumerable<string> ValidateCoreSurfaceLineAndSoilProfileProperties(PipingInput inputParameters)
        {
            var validationResults = new List<string>();
            if (inputParameters.SurfaceLine == null)
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_No_SurfaceLine_selected);
            }

            if (inputParameters.StochasticSoilProfile == null)
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_No_StochasticSoilProfile_selected);
            }

            if (double.IsNaN(inputParameters.ExitPointL))
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_No_value_for_ExitPointL);
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateSoilLayers(PipingInput inputParameters)
        {
            var validationResults = new List<string>();
            if (double.IsNaN(DerivedPipingInput.GetThicknessAquiferLayer(inputParameters).Mean))
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_thickness_aquifer_layer);
            }

            PipingSoilProfile pipingSoilProfile = inputParameters.StochasticSoilProfile.SoilProfile;
            double surfaceLevel = inputParameters.SurfaceLine.GetZAtL(inputParameters.ExitPointL);

            validationResults.AddRange(ValidateAquiferLayers(inputParameters, pipingSoilProfile, surfaceLevel));
            validationResults.AddRange(ValidateCoverageLayers(inputParameters, pipingSoilProfile, surfaceLevel));
            return validationResults;
        }

        private static IEnumerable<string> ValidateAquiferLayers(PipingInput inputParameters, PipingSoilProfile pipingSoilProfile, double surfaceLevel)
        {
            var validationResult = new List<string>();

            bool hasConsecutiveAquiferLayers = pipingSoilProfile.GetConsecutiveAquiferLayersBelowLevel(surfaceLevel).Any();
            if (!hasConsecutiveAquiferLayers)
            {
                validationResult.Add(Resources.PipingCalculationService_ValidateInput_No_aquifer_layer_at_ExitPointL_under_SurfaceLine);
            }
            else
            {
                if (double.IsNaN(PipingSemiProbabilisticDesignVariableFactory.GetDarcyPermeability(inputParameters).GetDesignValue()))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_derive_DarcyPermeability);
                }

                if (double.IsNaN(PipingSemiProbabilisticDesignVariableFactory.GetDiameter70(inputParameters).GetDesignValue()))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_derive_Diameter70);
                }
            }

            return validationResult;
        }

        private static IEnumerable<string> ValidateCoverageLayers(PipingInput inputParameters, PipingSoilProfile pipingSoilProfile, double surfaceLevel)
        {
            var validationResult = new List<string>();

            bool hasConsecutiveCoverageLayers = pipingSoilProfile.GetConsecutiveCoverageLayersBelowLevel(surfaceLevel).Any();
            if (hasConsecutiveCoverageLayers)
            {
                RoundedDouble saturatedVolumicWeightOfCoverageLayer =
                    PipingSemiProbabilisticDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters).GetDesignValue();

                if (double.IsNaN(saturatedVolumicWeightOfCoverageLayer))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_derive_SaturatedVolumicWeight);
                }
                else if (saturatedVolumicWeightOfCoverageLayer < inputParameters.WaterVolumetricWeight)
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_SaturatedVolumicWeightCoverageLayer_must_be_larger_than_WaterVolumetricWeight);
                }
            }

            return validationResult;
        }

        private static List<string> GetInputWarnings(PipingInput inputParameters)
        {
            var warnings = new List<string>();

            if (IsSurfaceLineProfileDefinitionComplete(inputParameters))
            {
                double surfaceLineLevel = inputParameters.SurfaceLine.GetZAtL(inputParameters.ExitPointL);

                warnings.AddRange(GetMultipleAquiferLayersWarning(inputParameters, surfaceLineLevel));
                warnings.AddRange(GetMultipleCoverageLayersWarning(inputParameters, surfaceLineLevel));
                warnings.AddRange(GetDiameter70Warnings(inputParameters));
                warnings.AddRange(GetThicknessCoverageLayerWarnings(inputParameters));
            }

            return warnings;
        }

        private static IEnumerable<string> GetThicknessCoverageLayerWarnings(PipingInput inputParameters)
        {
            var warnings = new List<string>();

            PipingSoilProfile pipingSoilProfile = inputParameters.StochasticSoilProfile.SoilProfile;
            double surfaceLevel = inputParameters.SurfaceLine.GetZAtL(inputParameters.ExitPointL);

            bool hasConsecutiveCoverageLayers = pipingSoilProfile.GetConsecutiveCoverageLayersBelowLevel(surfaceLevel).Any();
            if (!hasConsecutiveCoverageLayers)
            {
                warnings.Add(Resources.PipingCalculationService_ValidateInput_No_coverage_layer_at_ExitPointL_under_SurfaceLine);
            }

            if (double.IsNaN(DerivedPipingInput.GetThicknessCoverageLayer(inputParameters).Mean))
            {
                warnings.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_thickness_coverage_layer);
            }

            return warnings;
        }

        private static IEnumerable<string> GetDiameter70Warnings(PipingInput inputParameters)
        {
            var warnings = new List<string>();

            RoundedDouble diameter70Value = PipingSemiProbabilisticDesignVariableFactory.GetDiameter70(inputParameters).GetDesignValue();

            if (!double.IsNaN(diameter70Value) && (diameter70Value < 6.3e-5 || diameter70Value > 0.5e-3))
            {
                warnings.Add(string.Format(Resources.PipingCalculationService_GetInputWarnings_Specified_DiameterD70_value_0_not_in_valid_range_of_model, diameter70Value));
            }

            return warnings;
        }

        private static IEnumerable<string> GetMultipleCoverageLayersWarning(PipingInput inputParameters, double surfaceLineLevel)
        {
            var warnings = new List<string>();

            bool hasMoreThanOneCoverageLayer = inputParameters.StochasticSoilProfile.SoilProfile.GetConsecutiveCoverageLayersBelowLevel(surfaceLineLevel).Count() > 1;
            if (hasMoreThanOneCoverageLayer)
            {
                warnings.Add(Resources.PipingCalculationService_GetInputWarnings_Multiple_coverage_layers_Attempt_to_determine_value_from_combination);
            }

            return warnings;
        }

        private static IEnumerable<string> GetMultipleAquiferLayersWarning(PipingInput inputParameters, double surfaceLineLevel)
        {
            var warnings = new List<string>();

            bool hasMoreThanOneAquiferLayer = inputParameters.StochasticSoilProfile.SoilProfile.GetConsecutiveAquiferLayersBelowLevel(surfaceLineLevel).Count() > 1;
            if (hasMoreThanOneAquiferLayer)
            {
                warnings.Add(Resources.PipingCalculationService_GetInputWarnings_Multiple_aquifer_layers_Attempt_to_determine_values_for_DiameterD70_and_DarcyPermeability_from_top_layer);
            }

            return warnings;
        }

        private static bool IsSurfaceLineProfileDefinitionComplete(PipingInput surfaceLineMissing)
        {
            return surfaceLineMissing.SurfaceLine != null &&
                   surfaceLineMissing.StochasticSoilProfile != null &&
                   !double.IsNaN(surfaceLineMissing.ExitPointL);
        }

        private static PipingCalculatorInput CreateInputFromData(PipingInput inputParameters, RoundedDouble normativeAssessmentLevel)
        {
            RoundedDouble effectiveAssessmentLevel = GetEffectiveAssessmentLevel(inputParameters, normativeAssessmentLevel);

            return new PipingCalculatorInput(
                new PipingCalculatorInput.ConstructionProperties
                {
                    WaterVolumetricWeight = inputParameters.WaterVolumetricWeight,
                    SaturatedVolumicWeightOfCoverageLayer = PipingSemiProbabilisticDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters).GetDesignValue(),
                    UpliftModelFactor = inputParameters.UpliftModelFactor,
                    AssessmentLevel = effectiveAssessmentLevel,
                    PiezometricHeadExit = DerivedPipingInput.GetPiezometricHeadExit(inputParameters, effectiveAssessmentLevel),
                    DampingFactorExit = PipingSemiProbabilisticDesignVariableFactory.GetDampingFactorExit(inputParameters).GetDesignValue(),
                    PhreaticLevelExit = PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(inputParameters).GetDesignValue(),
                    CriticalHeaveGradient = inputParameters.CriticalHeaveGradient,
                    ThicknessCoverageLayer = PipingSemiProbabilisticDesignVariableFactory.GetThicknessCoverageLayer(inputParameters).GetDesignValue(),
                    EffectiveThicknessCoverageLayer = PipingSemiProbabilisticDesignVariableFactory.GetEffectiveThicknessCoverageLayer(inputParameters).GetDesignValue(),
                    SellmeijerModelFactor = inputParameters.SellmeijerModelFactor,
                    SellmeijerReductionFactor = inputParameters.SellmeijerReductionFactor,
                    SeepageLength = PipingSemiProbabilisticDesignVariableFactory.GetSeepageLength(inputParameters).GetDesignValue(),
                    SandParticlesVolumicWeight = inputParameters.SandParticlesVolumicWeight,
                    WhitesDragCoefficient = inputParameters.WhitesDragCoefficient,
                    Diameter70 = PipingSemiProbabilisticDesignVariableFactory.GetDiameter70(inputParameters).GetDesignValue(),
                    DarcyPermeability = PipingSemiProbabilisticDesignVariableFactory.GetDarcyPermeability(inputParameters).GetDesignValue(),
                    WaterKinematicViscosity = inputParameters.WaterKinematicViscosity,
                    Gravity = inputParameters.Gravity,
                    ThicknessAquiferLayer = PipingSemiProbabilisticDesignVariableFactory.GetThicknessAquiferLayer(inputParameters).GetDesignValue(),
                    MeanDiameter70 = inputParameters.MeanDiameter70,
                    BeddingAngle = inputParameters.BeddingAngle,
                    ExitPointXCoordinate = inputParameters.ExitPointL,
                    SurfaceLine = inputParameters.SurfaceLine,
                    SoilProfile = inputParameters.StochasticSoilProfile?.SoilProfile
                });
        }

        private static RoundedDouble GetEffectiveAssessmentLevel(PipingInput input, RoundedDouble normativeAssessmentLevel)
        {
            return input.UseAssessmentLevelManualInput
                       ? input.AssessmentLevel
                       : normativeAssessmentLevel;
        }
    }
}