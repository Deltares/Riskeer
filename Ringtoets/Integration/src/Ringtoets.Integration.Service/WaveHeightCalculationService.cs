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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Service;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Services;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;
using Ringtoets.Integration.Service.Properties;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for marginal wave statistics.
    /// </summary>
    public class WaveHeightCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WaveHeightCalculationService));

        /// <summary>
        /// Performs validation over the values on the given <paramref name="hydraulicBoundaryDatabase"/>. Error information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> for which to validate the values.</param>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> for which to validate the values.</param>
        /// <returns><c>False</c> if <paramref name="hydraulicBoundaryDatabase"/> contains validation errors; <c>True</c> otherwise.</returns>
        internal static bool Validate(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            CalculationServiceHelper.LogValidationBeginTime(hydraulicBoundaryLocation.Name);

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabase.FilePath);
            var hasErrors = string.IsNullOrEmpty(validationProblem);

            if (!hasErrors)
            {
                CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonFormsResources.Hydraulic_boundary_database_connection_failed_0_,
                                                            validationProblem);
            }

            CalculationServiceHelper.LogValidationEndTime(hydraulicBoundaryLocation.Name);

            return hasErrors;
        }

        internal static TargetProbabilityCalculationOutput Calculate(IAssessmentSection assessmentSection, HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                                                     HydraulicBoundaryLocation hydraulicBoundaryLocation, string ringId)
        {
            var hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabase.FilePath);
            var input = CreateInput(assessmentSection, hydraulicBoundaryLocation);
            var targetProbabilityCalculationParser = new TargetProbabilityCalculationParser();

            CalculationServiceHelper.PerformCalculation(
                hydraulicBoundaryLocation.Name,
                () =>
                {
                    HydraRingCalculationService.PerformCalculation(
                        hlcdDirectory,
                        ringId,
                        HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta,
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

        private static void VerifyOutput(TargetProbabilityCalculationOutput output, string name)
        {
            if (output == null)
            {
                log.ErrorFormat(Resources.WaveHeightCalculationService_Calculate_Error_in_wave_height_0_calculation, name);
            }
        }

        private static WaveHeightCalculationInput CreateInput(IAssessmentSection assessmentSection, HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new WaveHeightCalculationInput(1, hydraulicBoundaryLocation.Id, assessmentSection.FailureMechanismContribution.Norm);
        }
    }
}