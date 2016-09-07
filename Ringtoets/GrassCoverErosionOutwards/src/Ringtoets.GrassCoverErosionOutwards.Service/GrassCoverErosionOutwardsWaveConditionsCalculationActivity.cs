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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Service;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Service.Properties;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a grass cover erosion outwards wave conditions calculation.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsCalculationActivity : HydraRingActivity<List<WaveConditionsOutput>>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GrassCoverErosionOutwardsWaveConditionsCalculationActivity));

        private readonly GrassCoverErosionOutwardsWaveConditionsCalculation calculation;
        private readonly string hlcdDirectory;
        private readonly GrassCoverErosionOutwardsFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The grass cover erosion outwards wave conditions data used for the calculation.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveConditionsCalculationActivity(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                                          string hlcdDirectory,
                                                                          GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                          IAssessmentSection assessmentSection)
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
                       () => GrassCoverErosionOutwardsDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation),
                       () =>
                       {
                           log.Info(string.Format(RingtoetsCommonServiceResources.Calculation_Subject_0_started_Time_1_,
                                                  calculation.Name,
                                                  DateTimeService.CurrentTimeAsString));

                           List<WaveConditionsOutput> outputs = new List<WaveConditionsOutput>();

                           var a = failureMechanism.GeneralInput.GeneralWaveConditionsInput.A;
                           var b = failureMechanism.GeneralInput.GeneralWaveConditionsInput.B;
                           var c = failureMechanism.GeneralInput.GeneralWaveConditionsInput.C;
                           var norm = assessmentSection.FailureMechanismContribution.Norm;

                           foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                           {
                               log.Info(string.Format(Resources.GrassCoverErosionOutwardsWaveConditionsCalculationActivity_OnRun_Subject_0_for_waterlevel_1_started_time_2_,
                                                      calculation.Name,
                                                      waterLevel,
                                                      DateTimeService.CurrentTimeAsString));

                               ProgressText = string.Format(Resources.GrassCoverErosionOutwardsWaveConditionsCalculationActivity_OnRun_Calculate_waterlevel_0_, waterLevel);

                               var output = WaveConditionsCalculationService.Instance.Calculate(waterLevel,
                                                                                                a,
                                                                                                b,
                                                                                                c,
                                                                                                norm,
                                                                                                calculation.InputParameters,
                                                                                                hlcdDirectory,
                                                                                                assessmentSection.Id,
                                                                                                calculation.Name);

                               if (output != null)
                               {
                                   outputs.Add(output);
                               }

                               log.Info(string.Format(Resources.GrassCoverErosionOutwardsWaveConditionsCalculationActivity_OnRun_Subject_0_for_waterlevel_1_ended_time_2_,
                                                      calculation.Name,
                                                      waterLevel,
                                                      DateTimeService.CurrentTimeAsString));
                           }

                           log.Info(string.Format(RingtoetsCommonServiceResources.Calculation_Subject_0_ended_Time_1_,
                                                  calculation.Name,
                                                  DateTimeService.CurrentTimeAsString));

                           return outputs.Any() ? outputs : null;
                       });
        }

        protected override void OnFinish()
        {
            PerformFinish(() => { calculation.Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Output); });
        }
    }
}