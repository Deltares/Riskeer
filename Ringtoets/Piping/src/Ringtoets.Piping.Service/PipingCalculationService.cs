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
using Ringtoets.Common.Service;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service.Properties;
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
        /// <returns><c>False</c> if <paramref name="calculation"/> contains validation errors; <c>True</c> otherwise.</returns>
        public static bool Validate(PipingCalculation calculation)
        {
            CalculationServiceHelper.LogValidationBeginTime(calculation.Name);
            CalculationServiceHelper.LogMessagesAsWarning(GetInputWarnings(calculation.InputParameters).ToArray());

            var inputValidationResults = ValidateInput(calculation.InputParameters);
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

            var isHydraulicBoundaryLocationMissing = inputParameters.HydraulicBoundaryLocation == null;
            var isSoilProfileMissing = inputParameters.StochasticSoilProfile == null;
            var isSurfaceLineMissing = inputParameters.SurfaceLine == null;
            var isExitPointLMissing = double.IsNaN(inputParameters.ExitPointL);
            var isEntryPointLMissing = double.IsNaN(inputParameters.EntryPointL);

            if (isHydraulicBoundaryLocationMissing)
            {
                validationResult.Add(Resources.PipingCalculationService_ValidateInput_No_HydraulicBoundaryLocation_selected);
            }

            if (isSurfaceLineMissing)
            {
                validationResult.Add(Resources.PipingCalculationService_ValidateInput_No_SurfaceLine_selected);
            }

            if (isSoilProfileMissing)
            {
                validationResult.Add(Resources.PipingCalculationService_ValidateInput_No_StochasticSoilProfile_selected);
            }

            if (isEntryPointLMissing)
            {
                validationResult.Add(Resources.PipingCalculationService_ValidateInput_No_value_for_EntryPointL);
            }

            if (isExitPointLMissing)
            {
                validationResult.Add(Resources.PipingCalculationService_ValidateInput_No_value_for_ExitPointL);
            }

            if (!isHydraulicBoundaryLocationMissing)
            {
                if (double.IsNaN(inputParameters.AssessmentLevel))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_AssessmentLevel);
                }
                if (double.IsNaN(inputParameters.PiezometricHeadExit))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_PiezometricHeadExit);
                }
            }

            if (!isSurfaceLineMissing && !isSoilProfileMissing && !isExitPointLMissing)
            {
                if (double.IsNaN(inputParameters.ThicknessAquiferLayer.Mean))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_thickness_aquifer_layer);
                }

                if (double.IsNaN(inputParameters.ThicknessCoverageLayer.Mean))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_thickness_coverage_layer);
                }

                var pipingSoilProfile = inputParameters.StochasticSoilProfile.SoilProfile;
                var surfaceLevel = inputParameters.SurfaceLine.GetZAtL(inputParameters.ExitPointL);

                if (pipingSoilProfile != null)
                {
                    IEnumerable<PipingSoilLayer> consecutiveAquiferLayers = pipingSoilProfile.GetConsecutiveAquiferLayersBelowLevel(surfaceLevel).ToArray();
                    IEnumerable<PipingSoilLayer> consecutiveCoverageLayers = pipingSoilProfile.GetConsecutiveCoverageLayersBelowLevel(surfaceLevel).ToArray();

                    var hasAquiferLayers = consecutiveAquiferLayers.Any();
                    var hasCoverageLayers = consecutiveCoverageLayers.Any();

                    if (!hasAquiferLayers)
                    {
                        validationResult.Add(Resources.PipingCalculationService_ValidateInput_No_aquifer_layer_at_ExitPointL_under_SurfaceLine);
                    }
                    if (!hasCoverageLayers)
                    {
                        validationResult.Add(Resources.PipingCalculationService_ValidateInput_No_coverage_layer_at_ExitPointL_under_SurfaceLine);
                    }

                    if (hasAquiferLayers)
                    {
                        if (double.IsNaN(inputParameters.DarcyPermeability.Mean)
                            || double.IsNaN(inputParameters.DarcyPermeability.StandardDeviation))
                        {
                            validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_derive_DarcyPermeability);
                        }
                        if (double.IsNaN(inputParameters.Diameter70.Mean)
                            || double.IsNaN(inputParameters.Diameter70.StandardDeviation))
                        {
                            validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_derive_Diameter70);
                        }
                    }
                    if (hasCoverageLayers)
                    {
                        if (double.IsNaN(inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean)
                            || double.IsNaN(inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation)
                            || double.IsNaN(inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift))
                        {
                            validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_derive_SaturatedVolumicWeight);
                        }
                    }
                }
            }

            return validationResult;
        }

        private static List<string> GetInputWarnings(PipingInput inputParameters)
        {
            List<string> warnings = new List<string>();

            var exitPointL = inputParameters.ExitPointL;

            var isSoilProfileMissing = inputParameters.StochasticSoilProfile == null;
            var isSurfaceLineMissing = inputParameters.SurfaceLine == null;
            var isExitPointLMissing = double.IsNaN(exitPointL);

            if (!isSurfaceLineMissing && !isSoilProfileMissing && !isExitPointLMissing)
            {
                var pipingSoilProfile = inputParameters.StochasticSoilProfile.SoilProfile;
                var surfaceLevel = inputParameters.SurfaceLine.GetZAtL(exitPointL);

                IEnumerable<PipingSoilLayer> consecutiveAquiferLayersBelowLevel = pipingSoilProfile.GetConsecutiveAquiferLayersBelowLevel(surfaceLevel);
                IEnumerable<PipingSoilLayer> consecutiveAquitardLayersBelowLevel = pipingSoilProfile.GetConsecutiveCoverageLayersBelowLevel(surfaceLevel);

                if (consecutiveAquiferLayersBelowLevel.Count() > 1)
                {
                    warnings.Add(Resources.PipingCalculationService_GetInputWarnings_Multiple_aquifer_layers_Attempt_to_determine_values_for_DiameterD70_and_DarcyPermeability_from_top_layer);
                }
                if (consecutiveAquitardLayersBelowLevel.Count() > 1)
                {
                    warnings.Add(Resources.PipingCalculationService_GetInputWarnings_Multiple_coverage_layers_Attempt_to_determine_value_from_combination);
                }
                if (!double.IsNaN(inputParameters.Diameter70.Mean) && !double.IsNaN(inputParameters.Diameter70.StandardDeviation))
                {
                    var diameter70Value = PipingSemiProbabilisticDesignValueFactory.GetDiameter70(inputParameters).GetDesignValue();
                    if (diameter70Value < 6.3e-5|| diameter70Value > 0.5e-3)
                    {
                        warnings.Add(string.Format(Resources.PipingCalculationService_GetInputWarnings_Specified_DiameterD70_value_0_not_in_valid_range_of_model, diameter70Value));
                    }
                }
            }

            return warnings;
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
                inputParameters.StochasticSoilProfile.SoilProfile);
        }
    }
}