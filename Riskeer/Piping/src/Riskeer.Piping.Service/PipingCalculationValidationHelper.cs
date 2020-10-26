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
using Riskeer.Piping.Data;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service.Properties;

namespace Riskeer.Piping.Service
{
    /// <summary>
    ///  Helper class for validating the input of a <see cref="IPipingCalculation{TPipingInput}"/>.
    /// </summary>
    public static class PipingCalculationValidationHelper
    {
        /// <summary>
        /// Validates the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="PipingInput"/> to validate.</param>
        /// <param name="generalInput">The <see cref="GeneralPipingInput"/> used
        /// in the validation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of validation errors.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<string> GetValidationErrors(PipingInput input, GeneralPipingInput generalInput)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            var validationResults = new List<string>();

            IEnumerable<string> coreValidationError = ValidateCoreSurfaceLineAndSoilProfileProperties(input);
            validationResults.AddRange(coreValidationError);

            if (double.IsNaN(input.EntryPointL))
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_No_value_for_EntryPointL);
            }

            if (!coreValidationError.Any())
            {
                validationResults.AddRange(ValidateSoilLayers(input, generalInput));
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateCoreSurfaceLineAndSoilProfileProperties(PipingInput input)
        {
            var validationResults = new List<string>();
            if (input.SurfaceLine == null)
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_No_SurfaceLine_selected);
            }

            if (input.StochasticSoilProfile == null)
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_No_StochasticSoilProfile_selected);
            }

            if (double.IsNaN(input.ExitPointL))
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_No_value_for_ExitPointL);
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateSoilLayers(PipingInput input, GeneralPipingInput generalInput)
        {
            var validationResults = new List<string>();
            if (double.IsNaN(DerivedPipingInput.GetThicknessAquiferLayer(input).Mean))
            {
                validationResults.Add(Resources.PipingCalculationService_ValidateInput_Cannot_determine_thickness_aquifer_layer);
            }

            PipingSoilProfile pipingSoilProfile = input.StochasticSoilProfile.SoilProfile;
            double surfaceLevel = input.SurfaceLine.GetZAtL(input.ExitPointL);

            validationResults.AddRange(ValidateAquiferLayers(input, pipingSoilProfile, surfaceLevel));
            validationResults.AddRange(ValidateCoverageLayers(input, generalInput, pipingSoilProfile, surfaceLevel));
            return validationResults;
        }

        private static IEnumerable<string> ValidateAquiferLayers(PipingInput input, PipingSoilProfile pipingSoilProfile, double surfaceLevel)
        {
            var validationResult = new List<string>();

            if (!pipingSoilProfile.GetConsecutiveAquiferLayersBelowLevel(surfaceLevel).Any())
            {
                validationResult.Add(Resources.PipingCalculationService_ValidateInput_No_aquifer_layer_at_ExitPointL_under_SurfaceLine);
            }
            else
            {
                if (double.IsNaN(PipingDesignVariableFactory.GetDarcyPermeability(input).GetDesignValue()))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_derive_DarcyPermeability);
                }

                if (double.IsNaN(PipingDesignVariableFactory.GetDiameter70(input).GetDesignValue()))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_derive_Diameter70);
                }
            }

            return validationResult;
        }

        private static IEnumerable<string> ValidateCoverageLayers(PipingInput input, GeneralPipingInput generalInput,
                                                                  PipingSoilProfile pipingSoilProfile, double surfaceLevel)
        {
            var validationResult = new List<string>();

            if (pipingSoilProfile.GetConsecutiveCoverageLayersBelowLevel(surfaceLevel).Any())
            {
                RoundedDouble saturatedVolumicWeightOfCoverageLayer =
                    PipingDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(input).GetDesignValue();

                if (double.IsNaN(saturatedVolumicWeightOfCoverageLayer))
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_Cannot_derive_SaturatedVolumicWeight);
                }
                else if (saturatedVolumicWeightOfCoverageLayer < generalInput.WaterVolumetricWeight)
                {
                    validationResult.Add(Resources.PipingCalculationService_ValidateInput_SaturatedVolumicWeightCoverageLayer_must_be_larger_than_WaterVolumetricWeight);
                }
            }

            return validationResult;
        }
    }
}