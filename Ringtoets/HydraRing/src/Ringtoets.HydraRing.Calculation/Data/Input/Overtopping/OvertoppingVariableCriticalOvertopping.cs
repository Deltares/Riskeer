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

namespace Ringtoets.HydraRing.Calculation.Data.Input.Overtopping
{
    /// <summary>
    /// Hydra-Ring variable related data for critical overtopping.
    /// </summary>
    internal class OvertoppingVariableCriticalOvertopping : HydraRingVariable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="OvertoppingVariableCriticalOvertopping"/> class.
        /// </summary>
        /// <param name="mean">The mean value in case the variable is random.</param>
        /// <param name="variability">The variability in case the variable is random.</param>
        public OvertoppingVariableCriticalOvertopping(double mean, double variability)
            : base(17, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard,
                   !double.IsNaN(mean) ? mean : 0.004,
                   !double.IsNaN(variability) ? variability : 0.0006,
                   double.NaN) {}
    }
}