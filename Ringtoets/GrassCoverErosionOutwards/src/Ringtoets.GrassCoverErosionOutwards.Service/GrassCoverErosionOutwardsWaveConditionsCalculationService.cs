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

using System.Linq;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Service.Properties;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.Revetment.Service;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring wave conditions calculations for the grass cover erosion outwards failure mechanism.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsCalculationService : WaveConditionsCalculationServiceBase
    {
        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/> and <paramref name="hydraulicBoundaryDatabaseFilePath"/>.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> for which to validate the values.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database file which to validate.</param>
        /// <returns><c>True</c> if there were no validation errors; <c>False</c> otherwise.</returns>
        public static bool Validate(GrassCoverErosionOutwardsWaveConditionsCalculation calculation, string hydraulicBoundaryDatabaseFilePath)
        {
            return ValidateWaveConditionsInput(
                calculation.InputParameters,
                calculation.Name,
                hydraulicBoundaryDatabaseFilePath,
                Resources.GrassCoverErosionOutwardsWaveConditionsCalculationService_LogMessage_DesignWaterLevel_name);
        }

        /// <summary>
        /// Performs a wave conditions calculation for the grass cover erosion outwards failure mechanism based on the supplied 
        /// <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>  and sets 
        /// <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation.Output"/> if the calculation was successful. 
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="failureMechanism">The grass cover erosion outwards failure mechanism, which contains general parameters that apply to all 
        /// <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> instances.</param>
        /// <param name="hlcdFilePath">The path of the HLCD file that should be used for performing the calculation.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when an error occurs during parsing of the Hydra-Ring output.</exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs during the calculation.</exception>
        public void Calculate(
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection,
            string hlcdFilePath)
        {
            string calculationName = calculation.Name;

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            var a = failureMechanism.GeneralInput.GeneralWaveConditionsInput.A;
            var b = failureMechanism.GeneralInput.GeneralWaveConditionsInput.B;
            var c = failureMechanism.GeneralInput.GeneralWaveConditionsInput.C;
            var ringId = assessmentSection.Id;
            var mechanismSpecificReturnPeriod = failureMechanism.GetMechanismSpecificReturnPeriod(assessmentSection);
            TotalWaterLevelCalculations = calculation.InputParameters.WaterLevels.Count();

            try
            {
                var outputs = CalculateWaveConditions(calculationName, calculation.InputParameters, a, b, c, mechanismSpecificReturnPeriod, ringId, hlcdFilePath);

                if (!Canceled)
                {
                    calculation.Output = new GrassCoverErosionOutwardsWaveConditionsOutput(outputs);
                }
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEndTime(calculationName);
            }
        }
    }
}