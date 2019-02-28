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
        /// Performs validation on the given input parameters. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> with the
        /// hydraulic boundary calculation settings.</param>
        /// <param name="targetProbability">The target probability to validate.</param>
        /// <returns><c>true</c> if there were no validation errors; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/> is <c>null</c>.</exception>
        public bool Validate(HydraulicBoundaryCalculationSettings calculationSettings,
                             double targetProbability)
        {
            if (calculationSettings == null)
            {
                throw new ArgumentNullException(nameof(calculationSettings));
            }

            var isValid = true;

            CalculationServiceHelper.LogValidationBegin();

            string preprocessorDirectory = calculationSettings.PreprocessorDirectory;
            string databaseFilePathValidationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(
                calculationSettings.HydraulicBoundaryDatabaseFilePath,
                calculationSettings.HlcdFilePath,
                preprocessorDirectory,
                calculationSettings.UsePreprocessorClosure);

            if (!string.IsNullOrEmpty(databaseFilePathValidationProblem))
            {
                CalculationServiceHelper.LogMessagesAsError(Resources.Hydraulic_boundary_database_connection_failed_0_,
                                                            new[]
                                                            {
                                                                databaseFilePathValidationProblem
                                                            });

                isValid = false;
            }

            string preprocessorDirectoryValidationProblem = HydraulicBoundaryDatabaseHelper.ValidatePreprocessorDirectory(preprocessorDirectory);
            if (!string.IsNullOrEmpty(preprocessorDirectoryValidationProblem))
            {
                CalculationServiceHelper.LogMessagesAsError(new[]
                {
                    preprocessorDirectoryValidationProblem
                });

                isValid = false;
            }

            if (!TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(targetProbability,
                                                                                     message => CalculationServiceHelper.LogMessagesAsError(new[]
                                                                                     {
                                                                                         message
                                                                                     })))
            {
                isValid = false;
            }

            CalculationServiceHelper.LogValidationEnd();

            return isValid;
        }
    }
}