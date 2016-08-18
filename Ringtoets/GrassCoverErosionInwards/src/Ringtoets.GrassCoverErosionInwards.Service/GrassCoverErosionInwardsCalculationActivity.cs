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

using System;
using Core.Common.Base.Service;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service.Properties;
using Ringtoets.GrassCoverErosionInwards.Utils;
using Ringtoets.HydraRing.Calculation.Activities;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a grass cover erosion inwards calculation.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationActivity : HydraRingActivity<GrassCoverErosionInwardsCalculationServiceOutput>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GrassCoverErosionInwardsCalculationActivity));

        private readonly GrassCoverErosionInwardsCalculation calculation;
        private readonly string hlcdDirectory;
        private readonly GrassCoverErosionInwardsFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The height structures data used for the calculation.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public GrassCoverErosionInwardsCalculationActivity(GrassCoverErosionInwardsCalculation calculation, string hlcdDirectory,
                                                           GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
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
            FailureMechanismSection failureMechanismSection =
                GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(failureMechanism.SectionResults, calculation);

            PerformRun(() => GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection),
                       () => GrassCoverErosionInwardsDataSynchronizationService.ClearCalculationOutput(calculation),
                       () =>
                       {
                           log.Info(string.Format(RingtoetsCommonServiceResources.Calculation_Subject_0_started_Time_1_,
                                                  calculation.Name,
                                                  DateTimeService.CurrentTimeAsString));

                           ProgressText = Resources.GrassCoverErosionInwardsCalculationActivity_OnRun_Calculate_probability;
                           GrassCoverErosionInwardsCalculationServiceOutput output =
                               GrassCoverErosionInwardsCalculationService.CalculateProbability(calculation,
                                                                                               hlcdDirectory,
                                                                                               failureMechanismSection,
                                                                                               assessmentSection.Id,
                                                                                               failureMechanism.GeneralInput);

                           if (output != null && calculation.InputParameters.CalculateDikeHeight)
                           {
                               ProgressText = Resources.GrassCoverErosionInwardsCalculationActivity_OnRun_Calculate_dikeHeight;
                               output.DikeHeight =
                                   GrassCoverErosionInwardsCalculationService.CalculateDikeHeight(calculation,
                                                                                                  assessmentSection,
                                                                                                  hlcdDirectory,
                                                                                                  failureMechanismSection,
                                                                                                  assessmentSection.Id,
                                                                                                  failureMechanism.GeneralInput);
                           }

                           log.Info(string.Format(RingtoetsCommonServiceResources.Calculation_Subject_0_ended_Time_1_,
                                                  calculation.Name,
                                                  DateTimeService.CurrentTimeAsString));

                           return output;
                       });
        }

        protected override void OnFinish()
        {
            PerformFinish(() =>
            {
                calculation.Output = new GrassCoverErosionInwardsOutput(
                    Output.WaveHeight,
                    Output.IsOvertoppingDominant,
                    ProbabilityAssessmentService.Calculate(
                        assessmentSection.FailureMechanismContribution.Norm,
                        failureMechanism.Contribution,
                        failureMechanism.GeneralInput.N,
                        Output.Beta),
                    Output.DikeHeight);
            });
            calculation.NotifyObservers();
        }
    }
}