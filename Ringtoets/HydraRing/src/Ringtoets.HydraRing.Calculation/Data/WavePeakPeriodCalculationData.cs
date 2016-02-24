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

using Ringtoets.HydraRing.Data;

namespace Ringtoets.HydraRing.Calculation.Data
{
    /// <summary>
    /// Container of all data necessary for performing a wave peak period calculation via Hydra-Ring.
    /// </summary>
    public class WavePeakPeriodCalculationData : HydraulicCalculationData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WavePeakPeriodCalculationData"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to perform the calculation for.</param>
        /// <param name="beta">The beta value to use during the calculation.</param>
        public WavePeakPeriodCalculationData(HydraulicBoundaryLocation hydraulicBoundaryLocation, double beta) : base(hydraulicBoundaryLocation, beta) {}

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.WavePeakPeriod;
            }
        }
    }
}