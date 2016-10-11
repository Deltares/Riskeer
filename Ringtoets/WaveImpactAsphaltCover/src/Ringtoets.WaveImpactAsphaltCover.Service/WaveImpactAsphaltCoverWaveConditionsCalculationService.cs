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
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service;
using Ringtoets.Revetment.Service.Properties;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring wave conditions calculations for the wave impact on asphalt failure mechanism.
    /// </summary>
    public class WaveImpactAsphaltCoverWaveConditionsCalculationService : WaveConditionsCalculationServiceBase
    {
        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/> and <paramref name="hydraulicBoundaryDatabaseFilePath"/>.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/> for which to validate the values.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database file which to validate.</param>
        /// <returns><c>True</c>c> if there were no validation errors; <c>False</c>c> otherwise.</returns>
        public bool Validate(WaveImpactAsphaltCoverWaveConditionsCalculation calculation, string hydraulicBoundaryDatabaseFilePath)
        {
            return ValidateWaveConditionsInput(
                calculation.InputParameters,
                calculation.Name,
                hydraulicBoundaryDatabaseFilePath,
                Resources.WaveConditionsCalculationService_ValidateInput_default_DesignWaterLevel_name);
        }

        /// <summary>
        /// Performs a wave conditions calculation for the wave impact on asphalt failure mechanism based on the supplied 
        /// <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/>  and sets 
        /// <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation.Output"/> if the calculation was successful. 
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="generalWaveConditionsInput">Calculation input parameters that apply to all <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/> instances.</param>
        /// <param name="hlcdFilePath">The path of the HLCD file that should be used for performing the calculation.</param>
        public void Calculate(WaveImpactAsphaltCoverWaveConditionsCalculation calculation,
                              IAssessmentSection assessmentSection,
                              GeneralWaveConditionsInput generalWaveConditionsInput,
                              string hlcdFilePath)
        {
            string calculationName = calculation.Name;

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            var a = generalWaveConditionsInput.A;
            var b = generalWaveConditionsInput.B;
            var c = generalWaveConditionsInput.C;

            var ringId = assessmentSection.Id;
            var norm = assessmentSection.FailureMechanismContribution.Norm;
            TotalWaterLevelCalculations = calculation.InputParameters.WaterLevels.Count();

            var outputs = CalculateWaveConditions(calculationName, calculation.InputParameters, a, b, c, norm, ringId, hlcdFilePath);

            if (!Canceled)
            {
                calculation.Output = new WaveImpactAsphaltCoverWaveConditionsOutput(outputs);
            }

            CalculationServiceHelper.LogCalculationEndTime(calculationName);
        }
    }
}