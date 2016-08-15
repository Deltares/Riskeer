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
using Core.Common.Base.Service;
using Core.Common.Utils;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Service.Properties;

namespace Ringtoets.Integration.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a design water level calculation.
    /// </summary>
    public class DesignWaterLevelCalculationActivity : HydraRingActivity<TargetProbabilityCalculationOutput>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DesignWaterLevelCalculationActivity));

        private readonly IAssessmentSection assessmentSection;
        private readonly HydraulicBoundaryLocation hydraulicBoundaryLocation;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationActivity"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> data which is used for the calculation.</param>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryDatabase"/> to perform the calculation for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public DesignWaterLevelCalculationActivity(IAssessmentSection assessmentSection, HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocation");
            }

            this.assessmentSection = assessmentSection;
            this.hydraulicBoundaryLocation = hydraulicBoundaryLocation;

            Name = string.Format(Resources.DesignWaterLevelCalculationService_Name_Calculate_assessment_level_for_location_0_,
                                     hydraulicBoundaryLocation.Name);
        }

        protected override void OnRun()
        {
            if (!double.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel))
            {
                State = ActivityState.Skipped;
                return;
            }

            PerformRun(() => DesignWaterLevelCalculationService.Validate(assessmentSection.HydraulicBoundaryDatabase, hydraulicBoundaryLocation),
                       () => hydraulicBoundaryLocation.DesignWaterLevel = double.NaN,
                       () => DesignWaterLevelCalculationService.Calculate(assessmentSection,
                                                                          assessmentSection.HydraulicBoundaryDatabase,
                                                                          hydraulicBoundaryLocation,
                                                                          assessmentSection.Id));
        }

        protected override void OnFinish()
        {
            PerformFinish(() =>
            {
                hydraulicBoundaryLocation.DesignWaterLevel = Output.Result;
                bool designWaterLevelCalculationConvergence = 
                    Math.Abs(Output.ActualTargetProbability - StatisticsConverter.NormToBeta(assessmentSection.FailureMechanismContribution.Norm)) <= 10e-5;
                if (!designWaterLevelCalculationConvergence)
                {
                    log.WarnFormat(Resources.DesignWaterLevelCalculationActivity_DesignWaterLevel_calculation_for_location_0_not_converged, hydraulicBoundaryLocation.Name);
                }
                hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence = designWaterLevelCalculationConvergence;
            });
        }
    }
}