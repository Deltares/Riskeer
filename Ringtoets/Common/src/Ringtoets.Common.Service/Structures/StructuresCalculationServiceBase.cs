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
using System.ComponentModel;
using System.Linq;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service.Properties;
using Ringtoets.Common.Service.ValidationRules;

namespace Ringtoets.Common.Service.Structures
{
    /// <summary>
    /// Service that provides generic logic for performing Hydra-Ring calculations for structures.
    /// </summary>
    /// <typeparam name="TStructureValidationRules">The type of the validation rules.</typeparam>
    /// <typeparam name="TStructureInput">The input type.</typeparam>
    /// <typeparam name="TStructure">The structure type.</typeparam>
    public abstract class StructuresCalculationServiceBase<TStructureValidationRules, TStructureInput, TStructure>
        where TStructureValidationRules : IStructuresValidationRulesRegistry<TStructureInput, TStructure>, new()
        where TStructureInput : StructuresInputBase<TStructure>, new()
        where TStructure : StructureBase
    {
        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>true</c> if <paramref name="calculation"/> has no validation errors; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when an unexpected
        /// enum value is encountered.</exception>
        public static bool Validate(StructuresCalculation<TStructureInput> calculation, IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            CalculationServiceHelper.LogValidationBegin();
            string[] messages = ValidateInput(calculation.InputParameters, assessmentSection);
            CalculationServiceHelper.LogMessagesAsError(Resources.Error_in_validation_0, messages);
            CalculationServiceHelper.LogValidationEnd();

            return !messages.Any();
        }

        private static string[] ValidateInput(TStructureInput input, IAssessmentSection assessmentSection)
        {
            var validationResults = new List<string>();

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResults.Add(validationProblem);
                return validationResults.ToArray();
            }

            if (input.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(Resources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }

            if (input.Structure == null)
            {
                validationResults.Add(Resources.StructuresCalculationService_ValidateInput_No_Structure_selected);
            }
            else
            {
                IEnumerable<ValidationRule> validationRules = new TStructureValidationRules().GetValidationRules(input);

                foreach (ValidationRule validationRule in validationRules)
                {
                    validationResults.AddRange(validationRule.Validate());
                }
            }
            return validationResults.ToArray();
        }
    }
}