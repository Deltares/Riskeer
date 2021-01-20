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
using Riskeer.Piping.Data;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service.Properties;

namespace Riskeer.Piping.Service
{
    /// <summary>
    /// Helper class for validating the input of a <see cref="IPipingCalculation{TPipingInput}"/>.
    /// </summary>
    public static class PipingCalculationValidationHelper
    {
        /// <summary>
        /// Gets validation warnings for the given <paramref name="input"/>. 
        /// </summary>
        /// <param name="input">The <see cref="PipingInput"/> to get the warnings for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of validation warnings.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        public static IEnumerable<string> GetValidationWarnings(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var warnings = new List<string>();

            if (IsSurfaceLineProfileDefinitionComplete(input))
            {
                double surfaceLineLevel = input.SurfaceLine.GetZAtL(input.ExitPointL);

                warnings.AddRange(GetMultipleAquiferLayersWarning(input, surfaceLineLevel));
                warnings.AddRange(GetMultipleCoverageLayersWarning(input, surfaceLineLevel));
                warnings.AddRange(GetThicknessCoverageLayerWarning(input));
            }

            return warnings;
        }

        /// <summary>
        /// Gets validation errors for the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="PipingInput"/> to validate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of validation errors.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        public static IEnumerable<string> GetValidationErrors(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var errors = new List<string>(ValidateCoreSurfaceLineAndSoilProfileProperties(input));
            if (!errors.Any())
            {
                errors.AddRange(ValidateSoilLayers(input));
            }

            return errors;
        }

        private static IEnumerable<string> GetThicknessCoverageLayerWarning(PipingInput input)
        {
            if (double.IsNaN(DerivedPipingInput.GetThicknessCoverageLayer(input).Mean))
            {
                yield return Resources.PipingCalculationService_ValidateInput_No_coverage_layer_at_ExitPointL_under_SurfaceLine;
            }
        }

        private static IEnumerable<string> GetMultipleCoverageLayersWarning(PipingInput input, double surfaceLineLevel)
        {
            bool hasMoreThanOneCoverageLayer = input.StochasticSoilProfile.SoilProfile.GetConsecutiveCoverageLayersBelowLevel(surfaceLineLevel).Count() > 1;
            if (hasMoreThanOneCoverageLayer)
            {
                yield return Resources.PipingCalculationService_GetInputWarnings_Multiple_coverage_layers_Attempt_to_determine_value_from_combination;
            }
        }

        private static IEnumerable<string> GetMultipleAquiferLayersWarning(PipingInput input, double surfaceLineLevel)
        {
            bool hasMoreThanOneAquiferLayer = input.StochasticSoilProfile.SoilProfile.GetConsecutiveAquiferLayersBelowLevel(surfaceLineLevel).Count() > 1;
            if (hasMoreThanOneAquiferLayer)
            {
                yield return Resources.PipingCalculationService_GetInputWarnings_Multiple_aquifer_layers_Attempt_to_determine_values_for_DiameterD70_and_DarcyPermeability_from_top_layer;
            }
        }

        private static bool IsSurfaceLineProfileDefinitionComplete(PipingInput input)
        {
            return input.SurfaceLine != null
                   && input.StochasticSoilProfile != null
                   && !double.IsNaN(input.ExitPointL);
        }

        private static IEnumerable<string> ValidateCoreSurfaceLineAndSoilProfileProperties(PipingInput input)
        {
            if (input.SurfaceLine == null)
            {
                yield return Resources.PipingCalculationService_ValidateInput_No_SurfaceLine_selected;
            }

            if (input.StochasticSoilProfile == null)
            {
                yield return Resources.PipingCalculationService_ValidateInput_No_StochasticSoilProfile_selected;
            }

            if (double.IsNaN(input.ExitPointL))
            {
                yield return Resources.PipingCalculationService_ValidateInput_No_value_for_ExitPointL;
            }

            if (double.IsNaN(input.EntryPointL))
            {
                yield return Resources.PipingCalculationService_ValidateInput_No_value_for_EntryPointL;
            }
        }

        private static IEnumerable<string> ValidateSoilLayers(PipingInput input)
        {
            var validationResults = new List<string>();
            if (double.IsNaN(DerivedPipingInput.GetThicknessAquiferLayer(input).Mean))
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_thickness_aquifer_layer);
            }

            validationResults.AddRange(ValidateAquiferLayers(input));
            validationResults.AddRange(ValidateCoverageLayers(input));
            return validationResults;
        }

        private static IEnumerable<string> ValidateAquiferLayers(PipingInput input)
        {
            double surfaceLevel = input.SurfaceLine.GetZAtL(input.ExitPointL);
            PipingSoilProfile pipingSoilProfile = input.StochasticSoilProfile.SoilProfile;

            if (!pipingSoilProfile.GetConsecutiveAquiferLayersBelowLevel(surfaceLevel).Any())
            {
                yield return Resources.PipingCalculationService_ValidateInput_No_aquifer_layer_at_ExitPointL_under_SurfaceLine;
            }
            else
            {
                VariationCoefficientLogNormalDistribution darcyPermeability = DerivedPipingInput.GetDarcyPermeability(input);
                if (IsInvalidDistributionValue(darcyPermeability.Mean) || IsInvalidDistributionValue(darcyPermeability.CoefficientOfVariation))
                {
                    yield return Resources.PipingCalculationService_ValidateInput_Cannot_derive_DarcyPermeability;
                }

                VariationCoefficientLogNormalDistribution diameter70 = DerivedPipingInput.GetDiameterD70(input);
                if (IsInvalidDistributionValue(diameter70.Mean) || IsInvalidDistributionValue(diameter70.CoefficientOfVariation))
                {
                    yield return Resources.PipingCalculationService_ValidateInput_Cannot_derive_Diameter70;
                }
            }
        }

        private static IEnumerable<string> ValidateCoverageLayers(PipingInput input)
        {
            if (!double.IsNaN(DerivedPipingInput.GetThicknessCoverageLayer(input).Mean))
            {
                LogNormalDistribution saturatedVolumicWeightOfCoverageLayer = DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(input);
                if (IsInvalidDistributionValue(saturatedVolumicWeightOfCoverageLayer.Mean) || IsInvalidDistributionValue(saturatedVolumicWeightOfCoverageLayer.StandardDeviation))
                {
                    yield return Resources.PipingCalculationService_ValidateInput_Cannot_derive_SaturatedVolumicWeight;
                }
            }
        }

        private static bool IsInvalidDistributionValue(RoundedDouble value)
        {
            return double.IsNaN(value) || double.IsInfinity(value);
        }
    }
}