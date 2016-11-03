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
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Service.Properties;
using RingtoetsRevetmentsServicesResources = Ringtoets.Revetment.Service.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring wave conditions calculations for the stability of stone revetment failure mechanism.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculationService : WaveConditionsCalculationServiceBase
    {
        private readonly ILog log = LogManager.GetLogger(typeof(StabilityStoneCoverWaveConditionsCalculationService));

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/> and <paramref name="hydraulicBoundaryDatabaseFilePath"/>.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StabilityStoneCoverWaveConditionsCalculation"/> for which to validate the values.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database file which to validate.</param>
        /// <returns><c>True</c> if there were no validation errors; <c>False</c> otherwise.</returns>
        public static bool Validate(StabilityStoneCoverWaveConditionsCalculation calculation, string hydraulicBoundaryDatabaseFilePath)
        {
            return ValidateWaveConditionsInput(
                calculation.InputParameters,
                calculation.Name,
                hydraulicBoundaryDatabaseFilePath,
                RingtoetsRevetmentsServicesResources.WaveConditionsCalculationService_ValidateInput_default_DesignWaterLevel_name);
        }

        /// <summary>
        /// Performs a wave conditions calculation for the stability of stone revetment failure mechanism based on the supplied 
        /// <see cref="StabilityStoneCoverWaveConditionsCalculation"/>  and sets 
        /// <see cref="StabilityStoneCoverWaveConditionsCalculation.Output"/> if the calculation was successful. 
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StabilityStoneCoverWaveConditionsCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="generalWaveConditionsInput">Calculation input parameters that apply to all <see cref="StabilityStoneCoverWaveConditionsCalculation"/> instances.</param>
        /// <param name="hlcdFilePath">The path of the HLCD file that should be used for performing the calculation.</param>
        public void Calculate(StabilityStoneCoverWaveConditionsCalculation calculation,
                              IAssessmentSection assessmentSection,
                              GeneralStabilityStoneCoverWaveConditionsInput generalWaveConditionsInput,
                              string hlcdFilePath)
        {
            string calculationName = calculation.Name;

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            var aBlocks = generalWaveConditionsInput.GeneralBlocksWaveConditionsInput.A;
            var bBlocks = generalWaveConditionsInput.GeneralBlocksWaveConditionsInput.B;
            var cBlocks = generalWaveConditionsInput.GeneralBlocksWaveConditionsInput.C;

            var aColumns = generalWaveConditionsInput.GeneralColumnsWaveConditionsInput.A;
            var bColumns = generalWaveConditionsInput.GeneralColumnsWaveConditionsInput.B;
            var cColumns = generalWaveConditionsInput.GeneralColumnsWaveConditionsInput.C;

            var ringId = assessmentSection.Id;
            var norm = assessmentSection.FailureMechanismContribution.Norm;
            TotalWaterLevelCalculations = calculation.InputParameters.WaterLevels.Count()*2;

            try
            {
                log.InfoFormat(Resources.StabilityStoneCoverWaveConditionsCalculationService_Calculate_Calculation_0_for_blocks_started, calculationName);
                IEnumerable<WaveConditionsOutput> blocksOutputs = CalculateWaveConditions(calculationName, calculation.InputParameters, aBlocks, bBlocks, cBlocks, norm, ringId, hlcdFilePath);
                log.InfoFormat(Resources.StabilityStoneCoverWaveConditionsCalculationService_Calculate_Calculation_0_for_blocks_finished, calculationName);

                IEnumerable<WaveConditionsOutput> columnsOutputs = null;
                if (!Canceled)
                {
                    log.InfoFormat(Resources.StabilityStoneCoverWaveConditionsCalculationService_Calculate_Calculation_0_for_columns_started, calculationName);
                    columnsOutputs = CalculateWaveConditions(calculationName, calculation.InputParameters, aColumns, bColumns, cColumns, norm, ringId, hlcdFilePath);
                    log.InfoFormat(Resources.StabilityStoneCoverWaveConditionsCalculationService_Calculate_Calculation_0_for_columns_finished, calculationName);
                }

                if (!Canceled)
                {
                    calculation.Output = new StabilityStoneCoverWaveConditionsOutput(columnsOutputs, blocksOutputs);
                }
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEndTime(calculationName);
            }
        }
    }
}