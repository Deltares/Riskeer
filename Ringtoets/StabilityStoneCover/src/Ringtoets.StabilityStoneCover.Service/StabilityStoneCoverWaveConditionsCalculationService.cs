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
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Service;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service;
using Ringtoets.Revetment.Service.Properties;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring wave conditions calculations.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculationService : WaveConditionsCalculationServiceBase
    {
        private readonly ILog log = LogManager.GetLogger(typeof(StabilityStoneCoverWaveConditionsCalculationService));

        public bool Validate(StabilityStoneCoverWaveConditionsCalculation calculation, string hydraulicBoundaryDatabaseFilePath)
        {
            return ValidateWaveConditionsInput(
                calculation.InputParameters, 
                calculation.Name, 
                hydraulicBoundaryDatabaseFilePath, 
                Resources.WaveConditionsCalculationService_ValidateInput_default_DesignWaterLevel_name);
        }

        public void Calculate(
            StabilityStoneCoverWaveConditionsCalculation calculation,
            StabilityStoneCoverFailureMechanism failureMechanism, 
            IAssessmentSection assessmentSection, 
            string hlcdFilePath)
        {
            string calculationName = calculation.Name;

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            var aBlocks = failureMechanism.GeneralInput.GeneralBlocksWaveConditionsInput.A;
            var bBlocks = failureMechanism.GeneralInput.GeneralBlocksWaveConditionsInput.B;
            var cBlocks = failureMechanism.GeneralInput.GeneralBlocksWaveConditionsInput.C;

            var aColumns = failureMechanism.GeneralInput.GeneralColumnsWaveConditionsInput.A;
            var bColumns = failureMechanism.GeneralInput.GeneralColumnsWaveConditionsInput.B;
            var cColumns = failureMechanism.GeneralInput.GeneralColumnsWaveConditionsInput.C;

            var ringId = assessmentSection.Id;
            var norm = assessmentSection.FailureMechanismContribution.Norm;
            TotalWaterLevelCalculations = calculation.InputParameters.WaterLevels.Count() * 2;

            log.InfoFormat("Berekening '{0}' voor blokken gestart.", calculationName);
            IEnumerable<WaveConditionsOutput> blocksOutputs = CalculateWaveConditions(calculationName, calculation.InputParameters, aBlocks, bBlocks, cBlocks, norm, ringId, hlcdFilePath);
            log.InfoFormat("Berekening '{0}' voor blokken beëindigd.", calculationName);

            IEnumerable<WaveConditionsOutput> columnsOutputs = null;
            if (!Canceled)
            {
                log.InfoFormat("Berekening '{0}' voor zuilen gestart.", calculationName);
                columnsOutputs = CalculateWaveConditions(calculationName, calculation.InputParameters, aColumns, bColumns, cColumns, norm, ringId, hlcdFilePath);
                log.InfoFormat("Berekening '{0}' voor zuilen beëindigd.", calculationName);
            }

            if (!Canceled)
            {
                calculation.Output = new StabilityStoneCoverWaveConditionsOutput(columnsOutputs, blocksOutputs);
            }

            CalculationServiceHelper.LogCalculationEndTime(calculationName);
        }
    }
}