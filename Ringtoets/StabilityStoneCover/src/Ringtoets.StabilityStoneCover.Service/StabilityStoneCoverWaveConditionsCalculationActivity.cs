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

using System;
using System.Linq;
using Core.Common.Base.Service;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Service;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.Revetment.Service;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Service.Properties;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a stability stone cover wave conditions calculation.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculationActivity : HydraRingActivity<StabilityStoneCoverWaveConditionsCalculationActivityOutput>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StabilityStoneCoverWaveConditionsCalculationActivity));

        private readonly StabilityStoneCoverWaveConditionsCalculation calculation;
        private readonly string hlcdDirectory;
        private readonly StabilityStoneCoverFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The stability stone cover wave conditions data used for the calculation.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public StabilityStoneCoverWaveConditionsCalculationActivity(StabilityStoneCoverWaveConditionsCalculation calculation, string hlcdDirectory,
                                                                    StabilityStoneCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }
            if (hlcdDirectory == null)
            {
                throw new ArgumentNullException("hlcdDirectory");
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            this.calculation = calculation;
            this.hlcdDirectory = hlcdDirectory;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            Name = calculation.Name;
        }

        protected override void OnRun()
        {
            PerformRun(() => true,
                       () => StabilityStoneCoverDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation),
                       () =>
                       {
                           log.Info(string.Format(RingtoetsCommonServiceResources.Calculation_Subject_0_started_Time_1_,
                                                  calculation.Name,
                                                  DateTimeService.CurrentTimeAsString));

                           Output = new StabilityStoneCoverWaveConditionsCalculationActivityOutput();

                           var generalInput = failureMechanism.GeneralInput;
                           var aBlocks = generalInput.ABlocks;
                           var bBlocks = generalInput.BBlocks;
                           var cBlocks = generalInput.CBlocks;
                           var aColumns = generalInput.AColumns;
                           var bColumns = generalInput.BColumns;
                           var cColumns = generalInput.CColumns;
                           var norm = assessmentSection.FailureMechanismContribution.Norm;

                           foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                           {
                               log.Info(string.Format(Resources.StabilityStoneCoverWaveConditionsCalculationActivity_OnRun_Subject_0_blocks_for_waterlevel_1_started_time_1_,
                                                      calculation.Name,
                                                      waterLevel,
                                                      DateTimeService.CurrentTimeAsString));

                               var blocksOuput = WaveConditionsCalculationService.Instance.Calculate(waterLevel,
                                                                                                     aBlocks,
                                                                                                     bBlocks,
                                                                                                     cBlocks,
                                                                                                     norm,
                                                                                                     calculation.InputParameters,
                                                                                                     hlcdDirectory,
                                                                                                     assessmentSection.Id,
                                                                                                     calculation.Name);

                               if (blocksOuput != null)
                               {
                                   Output.AddBlocksOutput(blocksOuput);
                               }

                               log.Info(string.Format(Resources.StabilityStoneCoverWaveConditionsCalculationActivity_OnRun_Subject_0_blocks_for_waterlevel_1_ended_time_1_,
                                                      calculation.Name,
                                                      waterLevel,
                                                      DateTimeService.CurrentTimeAsString));

                               log.Info(string.Format(Resources.StabilityStoneCoverWaveConditionsCalculationActivity_OnRun_Subject_0_columns_for_waterlevel_1_started_time_1_,
                                                      calculation.Name,
                                                      waterLevel,
                                                      DateTimeService.CurrentTimeAsString));

                               var columnsOuput = WaveConditionsCalculationService.Instance.Calculate(waterLevel,
                                                                                                      aColumns,
                                                                                                      bColumns,
                                                                                                      cColumns,
                                                                                                      norm,
                                                                                                      calculation.InputParameters,
                                                                                                      hlcdDirectory,
                                                                                                      assessmentSection.Id,
                                                                                                      calculation.Name);

                               if (columnsOuput != null)
                               {
                                   Output.AddColumnsOutput(columnsOuput);
                               }

                               log.Info(string.Format(Resources.StabilityStoneCoverWaveConditionsCalculationActivity_OnRun_Subject_0_columns_for_waterlevel_1_ended_time_1_,
                                                      calculation.Name,
                                                      waterLevel,
                                                      DateTimeService.CurrentTimeAsString));
                           }

                           log.Info(string.Format(RingtoetsCommonServiceResources.Calculation_Subject_0_ended_Time_1_,
                                                  calculation.Name,
                                                  DateTimeService.CurrentTimeAsString));

                           return !Output.ColumnsOutput.Any() && !Output.BlocksOutput.Any() ? null : Output;
                       });
        }

        protected override void OnFinish()
        {
        }
    }
}