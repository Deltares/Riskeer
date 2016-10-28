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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.Properties;
using Ringtoets.HydraRing.IO;
using Ringtoets.StabilityPointStructures.Data;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-ring calculations for stability point structures calculations.
    /// </summary>
    public class StabilityPointStructuresCalculationService
    {
        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>true</c>c> if <paramref name="calculation"/> has no validation errors; <c>false</c>c> otherwise.</returns>
        public static bool Validate(StructuresCalculation<StabilityPointStructuresInput> calculation, IAssessmentSection assessmentSection)
        {
            CalculationServiceHelper.LogValidationBeginTime(calculation.Name);
            var messages = ValidateInput(calculation.InputParameters, assessmentSection);
            CalculationServiceHelper.LogMessagesAsError(Resources.Error_in_validation_0, messages);
            CalculationServiceHelper.LogValidationEndTime(calculation.Name);

            return !messages.Any();
        }

        private static string[] ValidateInput(StabilityPointStructuresInput inputParameters, IAssessmentSection assessmentSection)
        {
            var validationResults = new List<string>();

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResults.Add(validationProblem);
                return validationResults.ToArray();
            }

            if (inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }

            return validationResults.ToArray();
        }
    }
}