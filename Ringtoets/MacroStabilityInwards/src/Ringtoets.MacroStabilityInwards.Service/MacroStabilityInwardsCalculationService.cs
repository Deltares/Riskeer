// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper;
using Ringtoets.MacroStabilityInwards.KernelWrapper.SubCalculator;
using Ringtoets.MacroStabilityInwards.Service.Properties;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Service
{
    /// <summary>
    /// This class is responsible for invoking operations on the <see cref="MacroStabilityInwardsCalculator"/>. Error and status information is 
    /// logged during the execution of the operation.
    /// </summary>
    public static class MacroStabilityInwardsCalculationService
    {
        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="MacroStabilityInwardsCalculation"/> for which to validate the values.</param>
        /// <returns><c>False</c> if <paramref name="calculation"/> contains validation errors; <c>True</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static bool Validate(MacroStabilityInwardsCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            CalculationServiceHelper.LogValidationBegin(calculation.Name);

            string[] inputValidationResults = ValidateInput(calculation.InputParameters).ToArray();

            if (inputValidationResults.Length > 0)
            {
                CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, inputValidationResults);
                CalculationServiceHelper.LogValidationEnd(calculation.Name);
                return false;
            }

            List<string> validationResults = new MacroStabilityInwardsCalculator(CreateInputFromData(calculation.InputParameters), MacroStabilityInwardsSubCalculatorFactory.Instance).Validate();
            CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, validationResults.ToArray());

            CalculationServiceHelper.LogValidationEnd(calculation.Name);

            return validationResults.Count == 0;
        }

        /// <summary>
        /// Performs a macro stability inwards calculation based on the supplied <see cref="MacroStabilityInwardsCalculation"/> and sets <see cref="MacroStabilityInwardsCalculation.Output"/>
        /// based on the result if the calculation was successful. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="MacroStabilityInwardsCalculation"/> to base the input for the calculation upon.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        /// <remarks>Consider calling <see cref="Validate"/> first to see if calculation is possible.</remarks>
        public static void Calculate(MacroStabilityInwardsCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            CalculationServiceHelper.LogCalculationBegin(calculation.Name);

            try
            {
                MacroStabilityInwardsCalculatorResult macroStabilityInwardsResult = new MacroStabilityInwardsCalculator(CreateInputFromData(calculation.InputParameters), MacroStabilityInwardsSubCalculatorFactory.Instance).Calculate();

                calculation.Output = new MacroStabilityInwardsOutput();
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEnd(calculation.Name);
            }
        }

        private static List<string> ValidateInput(MacroStabilityInwardsInput inputParameters)
        {
            var validationResults = new List<string>();

            validationResults.AddRange(ValidateHydraulics(inputParameters));

            IEnumerable<string> coreValidationError = ValidateCoreSurfaceLineAndSoilProfileProperties(inputParameters);
            validationResults.AddRange(coreValidationError);

            return validationResults;
        }

        private static IEnumerable<string> ValidateHydraulics(MacroStabilityInwardsInput inputParameters)
        {
            var validationResults = new List<string>();
            if (!inputParameters.UseAssessmentLevelManualInput && inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(Resources.MacroStabilityInwardsCalculationService_ValidateInput_No_HydraulicBoundaryLocation_selected);
            }
            else
            {
                validationResults.AddRange(ValidateAssessmentLevel(inputParameters));
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateAssessmentLevel(MacroStabilityInwardsInput inputParameters)
        {
            var validationResult = new List<string>();

            if (inputParameters.UseAssessmentLevelManualInput)
            {
                validationResult.AddRange(new NumericInputRule(inputParameters.AssessmentLevel, ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.AssessmentLevel_DisplayName)).Validate());
            }
            else
            {
                if (double.IsNaN(inputParameters.AssessmentLevel))
                {
                    validationResult.Add(Resources.MacroStabilityInwardsCalculationService_ValidateInput_Cannot_determine_AssessmentLevel);
                }
            }

            return validationResult;
        }

        private static IEnumerable<string> ValidateCoreSurfaceLineAndSoilProfileProperties(MacroStabilityInwardsInput inputParameters)
        {
            var validationResults = new List<string>();
            if (inputParameters.SurfaceLine == null)
            {
                validationResults.Add(Resources.MacroStabilityInwardsCalculationService_ValidateInput_No_SurfaceLine_selected);
            }
            if (inputParameters.StochasticSoilProfile == null)
            {
                validationResults.Add(Resources.MacroStabilityInwardsCalculationService_ValidateInput_No_StochasticSoilProfile_selected);
            }
            return validationResults;
        }

        private static MacroStabilityInwardsCalculatorInput CreateInputFromData(MacroStabilityInwardsInput inputParameters)
        {
            return new MacroStabilityInwardsCalculatorInput(
                new MacroStabilityInwardsCalculatorInput.ConstructionProperties
                {
                    AssessmentLevel = inputParameters.AssessmentLevel,
                    SurfaceLine = inputParameters.SurfaceLine,
                    SoilProfile = inputParameters.StochasticSoilProfile?.SoilProfile
                });
        }
    }
}