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
using Ringtoets.Revetment.Service;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring wave conditions calculations.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsCalculationService : WaveConditionsCalculationServiceBase
    {
        public bool Validate(GrassCoverErosionOutwardsWaveConditionsCalculation calculation, string hydraulicBoundaryDatabaseFilePath)
        {
            return ValidateWaveConditionsInput(
                calculation.InputParameters,
                calculation.Name,
                hydraulicBoundaryDatabaseFilePath,
                Resources.GrassCoverErosionOutwardsWaveConditionsCalculationService_LogMessage_DesignWaterLevel_name);
        }

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
            var norm = failureMechanism.CalculationBeta(assessmentSection);
            TotalWaterLevelCalculations = calculation.InputParameters.WaterLevels.Count();

            var outputs = CalculateWaveConditions(calculationName, calculation.InputParameters, a, b, c, norm, ringId, hlcdFilePath);

            if (!Canceled)
            {
                calculation.Output = new GrassCoverErosionOutwardsWaveConditionsOutput(outputs);
            }

            CalculationServiceHelper.LogCalculationEndTime(calculationName);
        }
    }
}