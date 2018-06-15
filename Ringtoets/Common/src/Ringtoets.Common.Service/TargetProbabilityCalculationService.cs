﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service.Properties;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Abstract service that provides methods for performing Hydra-Ring calculations with a target probability.
    /// </summary>
    public abstract class TargetProbabilityCalculationService
    {
        /// <summary>
        /// Performs validation on the given input parameters. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database to validate.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory to validate.</param>
        /// <param name="norm">The norm to validate.</param>
        /// <returns><c>true</c> if there were no validation errors; <c>false</c> otherwise.</returns>
        public bool Validate(string hydraulicBoundaryDatabaseFilePath,
                             string preprocessorDirectory,
                             double norm)
        {
            var isValid = true;

            CalculationServiceHelper.LogValidationBegin();

            string databaseFilePathValidationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(hydraulicBoundaryDatabaseFilePath,
                                                                                                                   preprocessorDirectory);
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

            CalculationServiceHelper.LogValidationEnd();

            return isValid;
        }
    }
}