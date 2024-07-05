// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.Service.Properties;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// Abstract service that provides methods for performing Hydra-Ring calculations with a target probability.
    /// </summary>
    public abstract class TargetProbabilityCalculationService
    {
        /// <summary>
        /// Performs validation on the given input parameters. Error and status information is logged during the execution of
        /// the operation.
        /// </summary>
        /// <param name="calculationSettings">The hydraulic boundary calculation settings.</param>
        /// <returns><c>true</c> if there were no validation errors; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/> is <c>null</c>.</exception>
        public bool Validate(HydraulicBoundaryCalculationSettings calculationSettings)
        {
            if (calculationSettings == null)
            {
                throw new ArgumentNullException(nameof(calculationSettings));
            }

            var isValid = true;

            CalculationServiceHelper.LogValidationBegin();

            string filesForCalculationValidationProblem = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(
                calculationSettings.HlcdFilePath,
                calculationSettings.HrdFilePath,
                calculationSettings.UsePreprocessorClosure);

            if (!string.IsNullOrEmpty(filesForCalculationValidationProblem))
            {
                CalculationServiceHelper.LogMessagesAsError(Resources.Hydraulic_boundary_database_connection_failed_0_,
                                                            new[]
                                                            {
                                                                filesForCalculationValidationProblem
                                                            });

                isValid = false;
            }

            if (isValid && !HydraulicBoundaryDataHelper.IsCorrectVersion(calculationSettings.HrdFileVersion, calculationSettings.HrdFilePath))
            {
                CalculationServiceHelper.LogMessagesAsError(Resources.Hydraulic_boundary_database_mismatching_version_in_file_0_,
                                                            new[]
                                                            {
                                                                calculationSettings.HrdFilePath
                                                            });

                isValid = false;
            }

            if (isValid && !HydraulicBoundaryDataHelper.IsCorrectHrdFile(calculationSettings.HlcdFilePath, calculationSettings.HrdFilePath))
            {
                CalculationServiceHelper.LogMessagesAsError(Resources.Hydraulic_location_configuration_database_refers_to_hydraulic_boundary_database_that_does_not_correspond_with_0,
                                                            new[]
                                                            {
                                                                calculationSettings.HrdFilePath
                                                            });

                isValid = false;
            }

            CalculationServiceHelper.LogValidationEnd();

            return isValid;
        }
    }
}