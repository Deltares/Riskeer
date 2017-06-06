// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Service.Properties;
using Ringtoets.HydraRing.Calculation.Activities;

namespace Ringtoets.DuneErosion.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a dune erosion boundary calculation.
    /// </summary>
    public class DuneErosionBoundaryCalculationActivity : HydraRingActivityBase
    {
        private readonly DuneLocation duneLocation;
        private readonly DuneErosionFailureMechanism failureMechanism;
        private readonly string hydraulicBoundaryDatabaseFilePath;
        private readonly double norm;
        private readonly DuneErosionBoundaryCalculationService calculationService;

        /// <summary>
        /// Creates a new instance of <see cref="DuneErosionBoundaryCalculationActivity"/>.
        /// </summary>
        /// <param name="duneLocation">The <see cref="DuneLocation"/> to perform the calculation for.</param>
        /// <param name="failureMechanism">The <see cref="DuneErosionFailureMechanism"/> that holds information about the contribution and
        /// the general inputs used in the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The hydraulic boundary database file that should be used for performing the calculation.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DuneErosionBoundaryCalculationActivity(DuneLocation duneLocation,
                                                      DuneErosionFailureMechanism failureMechanism,
                                                      string hydraulicBoundaryDatabaseFilePath,
                                                      double norm)
        {
            if (duneLocation == null)
            {
                throw new ArgumentNullException(nameof(duneLocation));
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            this.duneLocation = duneLocation;
            this.failureMechanism = failureMechanism;
            this.hydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            this.norm = norm;

            Description = string.Format(Resources.DuneErosionBoundaryCalculationActivity_Calculate_hydraulic_boundary_conditions_for_location_with_name_0_, duneLocation.Name);

            calculationService = new DuneErosionBoundaryCalculationService();
        }

        protected override void PerformCalculation()
        {
            if (duneLocation.Output != null)
            {
                State = ActivityState.Skipped;
                return;
            }

            calculationService.Calculate(duneLocation,
                                         failureMechanism,
                                         norm,
                                         hydraulicBoundaryDatabaseFilePath);
        }

        protected override void OnCancel()
        {
            calculationService.Cancel();
        }

        protected override void OnFinish() {}
    }
}