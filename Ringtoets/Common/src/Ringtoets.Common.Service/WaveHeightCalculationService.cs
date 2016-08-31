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

using System.IO;
using log4net;
using Ringtoets.Common.Service.Properties;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Services;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for marginal wave statistics.
    /// </summary>
    internal static class WaveHeightCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WaveHeightCalculationService));

        /// <summary>
        /// Performs validation of the values in the given <paramref name="hydraulicBoundaryDatabaseFilePath"/>. Error information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> for which to validate the values.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The HLCD file that should be used for performing the calculation.</param>
        /// <returns><c>False</c> if the connection to <paramref name="hydraulicBoundaryDatabaseFilePath"/> contains validation errors; <c>True</c> otherwise.</returns>
        internal static bool Validate(HydraulicBoundaryLocation hydraulicBoundaryLocation, string hydraulicBoundaryDatabaseFilePath)
        {
            var calculationName = string.Format(Resources.WaveHeightCalculationService_Name_Wave_height_for_location_0_, hydraulicBoundaryLocation.Name);
            CalculationServiceHelper.LogValidationBeginTime(calculationName);

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabaseFilePath);
            var isValid = string.IsNullOrEmpty(validationProblem);

            if (!isValid)
            {
                CalculationServiceHelper.LogMessagesAsError(Resources.Hydraulic_boundary_database_connection_failed_0_,
                                                            validationProblem);
            }

            CalculationServiceHelper.LogValidationEndTime(calculationName);

            return isValid;
        }

        /// <summary>
        /// Performs a wave height calculation based on the supplied <see cref="HydraulicBoundaryLocation"/> and returns the result
        /// if the calculation was successful. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> to perform the calculation for.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <returns>A <see cref="ReliabilityIndexCalculationOutput"/> on a successful calculation, <c>null</c> otherwise.</returns>
        internal static ReliabilityIndexCalculationOutput Calculate(HydraulicBoundaryLocation hydraulicBoundaryLocation, string hydraulicBoundaryDatabaseFilePath,
                                                                    string ringId, int norm)
        {
            var hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            var input = CreateInput(hydraulicBoundaryLocation, norm);
            var targetProbabilityCalculationParser = new ReliabilityIndexCalculationParser();
            var calculationName = string.Format(Resources.WaveHeightCalculationService_Name_Wave_height_for_location_0_, hydraulicBoundaryLocation.Name);

            CalculationServiceHelper.PerformCalculation(
                calculationName,
                () =>
                {
                    HydraRingCalculationService.Instance.PerformCalculation(
                        hlcdDirectory,
                        ringId,
                        HydraRingUncertaintiesType.All,
                        input,
                        new[]
                        {
                            targetProbabilityCalculationParser
                        });

                    VerifyOutput(targetProbabilityCalculationParser.Output, hydraulicBoundaryLocation.Name);
                });

            return targetProbabilityCalculationParser.Output;
        }

        private static void VerifyOutput(ReliabilityIndexCalculationOutput output, string name)
        {
            if (output == null)
            {
                log.ErrorFormat(Resources.WaveHeightCalculationService_Calculate_Error_in_wave_height_0_calculation, name);
            }
        }

        private static WaveHeightCalculationInput CreateInput(HydraulicBoundaryLocation hydraulicBoundaryLocation, int norm)
        {
            return new WaveHeightCalculationInput(1, hydraulicBoundaryLocation.Id, norm);
        }
    }
}