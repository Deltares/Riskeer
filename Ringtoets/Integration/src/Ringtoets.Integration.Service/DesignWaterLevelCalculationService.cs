﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Services;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;
using Ringtoets.Integration.Service.Properties;

using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for design water level.
    /// </summary>
    internal static class DesignWaterLevelCalculationService 
    {
        private static readonly ILog designWaterLevelCalculationLogger = LogManager.GetLogger(typeof(DesignWaterLevelCalculationService));

        /// <summary>
        /// Performs validation over the values on the given <paramref name="hydraulicBoundaryDatabase"/>. Error information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> for which to validate the values.</param>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> for which to validate the values.</param>
        /// <returns><c>False</c> if <paramref name="hydraulicBoundaryDatabase"/> contains validation errors; <c>True</c> otherwise.</returns>
        internal static bool Validate(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            designWaterLevelCalculationLogger.Info(string.Format(RingtoetsCommonServiceResources.Validation_Subject_0_started_Time_1_,
                                                                 hydraulicBoundaryLocation.Id, DateTimeService.CurrentTimeAsString));

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabase.FilePath);
            var hasErrors = string.IsNullOrEmpty(validationProblem);

            if (!hasErrors)
            {
                LogMessagesAsError(RingtoetsCommonFormsResources.Hydraulic_boundary_database_connection_failed_0_,
                                   validationProblem);
            }

            LogValidationEndTime(hydraulicBoundaryLocation);

            return hasErrors;
        }

        /// <summary>
        /// Performs a design water level calculation based on the supplied <see cref="HydraulicBoundaryLocation"/> and sets <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>
        /// if the calculation was successful. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to base the input for the calculation upon.</param>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> to base the input for the calculation upon.</param>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> to perform the calculation for.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <returns>A <see cref="TargetProbabilityCalculationOutput"/> on a successful calculation, <c>null</c> otherwise.</returns>
        internal static TargetProbabilityCalculationOutput Calculate(IAssessmentSection assessmentSection, HydraulicBoundaryDatabase hydraulicBoundaryDatabase, 
                                                                     HydraulicBoundaryLocation hydraulicBoundaryLocation, string ringId)
        {
            designWaterLevelCalculationLogger.Info(string.Format(RingtoetsCommonServiceResources.Calculation_Subject_0_started_Time_1_,
                                                                 hydraulicBoundaryLocation.Id, DateTimeService.CurrentTimeAsString));

            try
            {
                var hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabase.FilePath);
                var input = CreateInput(assessmentSection, hydraulicBoundaryLocation);

                var output = HydraRingCalculationService.PerformCalculation(hlcdDirectory, ringId, HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, input);

                if (output == null)
                {
                    LogMessagesAsError(Resources.DesignWaterLevelCalculationService_Calculate_Error_in_design_water_level_0_calculation, hydraulicBoundaryLocation.Id.ToString());
                }

                return output;
            }
            finally
            {
                designWaterLevelCalculationLogger.Info(string.Format(RingtoetsCommonServiceResources.Calculation_Subject_0_ended_Time_1_,
                                                                     hydraulicBoundaryLocation.Id, DateTimeService.CurrentTimeAsString));
            }
        }

        private static AssessmentLevelCalculationInput CreateInput(IAssessmentSection assessmentSection, HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new AssessmentLevelCalculationInput((int) hydraulicBoundaryLocation.Id, assessmentSection.FailureMechanismContribution.Norm);
        }

        private static void LogMessagesAsError(string format, params string[] errorMessages)
        {
            foreach (var errorMessage in errorMessages)
            {
                designWaterLevelCalculationLogger.ErrorFormat(format, errorMessage);
            }
        }

        private static void LogValidationEndTime(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            designWaterLevelCalculationLogger.Info(string.Format(RingtoetsCommonServiceResources.Validation_Subject_0_ended_Time_1_,
                                                                 hydraulicBoundaryLocation.Id, DateTimeService.CurrentTimeAsString));
        }
    }
}