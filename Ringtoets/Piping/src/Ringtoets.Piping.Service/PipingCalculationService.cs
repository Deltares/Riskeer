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
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Service;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service.Properties;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

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
        /// <returns><c>False</c> if <paramref name="calculation"/> contains validation errors; <c>True</c> otherwise.</returns>
        public static bool Validate(PipingCalculation calculation)
        {
            CalculationServiceHelper.LogValidationBeginTime(calculation.Name);
            CalculationServiceHelper.LogMessagesAsWarning(GetInputWarnings(calculation.InputParameters).ToArray());

            List<string> inputValidationResults = ValidateInput(calculation.InputParameters);
            if (inputValidationResults.Count > 0)
            {
                CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, inputValidationResults.ToArray());
                CalculationServiceHelper.LogValidationEndTime(calculation.Name);
                return false;
            }

            var validationResults = new PipingCalculator(CreateInputFromData(calculation.InputParameters), PipingSubCalculatorFactory.Instance).Validate();
            CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, validationResults.ToArray());

            CalculationServiceHelper.LogValidationEndTime(calculation.Name);

            return validationResults.Count == 0;
        }

        /// <summary>
        /// Performs a piping calculation based on the supplied <see cref="PipingCalculation"/> and sets <see cref="PipingCalculation.Output"/>
        /// based on the result if the calculation was successful. Error and status information is logged during
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

                calculation.Output = new PipingOutput(new PipingOutput.ConstructionProperties
                {
                    UpliftZValue = pipingResult.UpliftZValue,
                    UpliftFactorOfSafety = pipingResult.UpliftFactorOfSafety,
                    HeaveZValue = pipingResult.HeaveZValue,
                    HeaveFactorOfSafety = pipingResult.HeaveFactorOfSafety,
                    SellmeijerZValue = pipingResult.SellmeijerZValue,
                    SellmeijerFactorOfSafety = pipingResult.SellmeijerFactorOfSafety,
                    HeaveGradient = pipingResult.HeaveGradient,
                    SellmeijerCreepCoefficient = pipingResult.SellmeijerCreepCoefficient,
                    SellmeijerCriticalFall = pipingResult.SellmeijerCriticalFall,
                    SellmeijerReducedFall = pipingResult.SellmeijerReducedFall
                });
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
            var validationResults = new List<string>();

            validationResults.AddRange(ValidateHydraulics(inputParameters));

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

        private static IEnumerable<string> ValidateHydraulics(PipingInput inputParameters)
        {
            var validationResults = new List<string>();
            if (!inputParameters.UseAssessmentLevelManualInput && inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_No_HydraulicBoundaryLocation_selected);
            }
            else
            {
                validationResults.AddRange(ValidateAssessmentLevel(inputParameters));

                if (IsInconcreteValue(inputParameters.PiezometricHeadExit))
                {
                    validationResults.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_PiezometricHeadExit);
                }
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateAssessmentLevel(PipingInput inputParameters)
        {
            var validationResult = new List<string>();

            if (inputParameters.UseAssessmentLevelManualInput)
            {
                if (IsInconcreteValue(inputParameters.AssessmentLevel))
                {
                    validationResult.Add(string.Format(RingtoetsCommonServiceResources.Validation_ValidateInput_No_concrete_value_entered_for_ParameterName_0_,
                                                       ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.AssessmentLevel_DisplayName)));
                }
            }
            else
            {
                if (double.IsNaN(inputParameters.AssessmentLevel))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_AssessmentLevel);
                }
            }

            return validationResult;
        }

        private static bool IsInconcreteValue(RoundedDouble parameter)
        {
            return double.IsNaN(parameter) || double.IsInfinity(parameter);
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
            if (double.IsNaN(inputParameters.ThicknessAquiferLayer.Mean))
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_thickness_aquifer_layer);
            }

            if (double.IsNaN(inputParameters.ThicknessCoverageLayer.Mean))
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_thickness_coverage_layer);
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
                if (double.IsNaN(PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(inputParameters).GetDesignValue()))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_derive_DarcyPermeability);
                }

                if (double.IsNaN(PipingSemiProbabilisticDesignValueFactory.GetDiameter70(inputParameters).GetDesignValue()))
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
            if (!hasConsecutiveCoverageLayers)
            {
                validationResult.Add(Resources.PipingCalculationService_ValidateInput_No_coverage_layer_at_ExitPointL_under_SurfaceLine);
            }
            else
            {
                RoundedDouble saturatedVolumicWeightOfCoverageLayer =
                    PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters).GetDesignValue();

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
            }

            return warnings;
        }

        private static IEnumerable<string> GetDiameter70Warnings(PipingInput inputParameters)
        {
            List<string> warnings = new List<string>();

            RoundedDouble diameter70Value = PipingSemiProbabilisticDesignValueFactory.GetDiameter70(inputParameters).GetDesignValue();

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
                PipingSemiProbabilisticDesignValueFactory.GetEffectiveThicknessCoverageLayer(inputParameters).GetDesignValue(),
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
                inputParameters.StochasticSoilProfile.SoilProfile);
        }
    }
}