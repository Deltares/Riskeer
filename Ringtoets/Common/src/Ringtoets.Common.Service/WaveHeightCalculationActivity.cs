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
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.Utils;
using log4net;
using Ringtoets.Common.Service.Properties;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a water height calculation.
    /// </summary>
    public class WaveHeightCalculationActivity : HydraRingActivity<ReliabilityIndexCalculationOutput>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WaveHeightCalculationActivity));
        private readonly HydraulicBoundaryLocation hydraulicBoundaryLocation;
        private readonly int norm;
        private readonly string hydraulicBoundaryDatabaseFilePath;
        private readonly string ringId;

        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightCalculationActivity"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> to perform the calculation for.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        public WaveHeightCalculationActivity(HydraulicBoundaryLocation hydraulicBoundaryLocation, string hydraulicBoundaryDatabaseFilePath, string ringId, int norm)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocation");
            }

            this.hydraulicBoundaryLocation = hydraulicBoundaryLocation;
            this.hydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            this.ringId = ringId;
            this.norm = norm;

            Name = string.Format(Resources.WaveHeightCalculationService_Name_Calculate_wave_height_for_location_0_,
                                 hydraulicBoundaryLocation.Name);
        }

        protected override void OnRun()
        {
            if (!double.IsNaN(hydraulicBoundaryLocation.WaveHeight))
            {
                State = ActivityState.Skipped;
                return;
            }

            PerformRun(() => WaveHeightCalculationService.Validate(hydraulicBoundaryLocation, hydraulicBoundaryDatabaseFilePath),
                       () => hydraulicBoundaryLocation.WaveHeight = (RoundedDouble) double.NaN,
                       () => WaveHeightCalculationService.Calculate(hydraulicBoundaryLocation, hydraulicBoundaryDatabaseFilePath,
                                                                    ringId, norm));
        }

        protected override void OnFinish()
        {
            PerformFinish(() =>
            {
                hydraulicBoundaryLocation.WaveHeight = (RoundedDouble) Output.Result;
                bool waveHeightCalculationConvergence =
                    Math.Abs(Output.CalculatedReliabilityIndex - StatisticsConverter.NormToBeta(norm)) <= 1.0e-3;
                if (!waveHeightCalculationConvergence)
                {
                    log.WarnFormat(Resources.WaveHeightCalculationActivity_WaveHeight_calculation_for_location_0_not_converged, hydraulicBoundaryLocation.Name);
                }
                hydraulicBoundaryLocation.WaveHeightCalculationConvergence = waveHeightCalculationConvergence
                                                                                 ? CalculationConvergence.CalculatedConverged
                                                                                 : CalculationConvergence.CalculatedNotConverged;
            });
        }
    }
}