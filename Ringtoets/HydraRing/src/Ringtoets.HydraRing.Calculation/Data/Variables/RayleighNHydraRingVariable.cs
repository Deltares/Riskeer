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

namespace Ringtoets.HydraRing.Calculation.Data.Variables
{
    /// <summary>
    /// Class for Rayleigh N Hydra-Ring variable related data.
    /// </summary>
    public class RayleighNHydraRingVariable : RandomHydraRingVariable
    {
        /// <summary>
        /// Creates a new instance of <see cref="RayleighNHydraRingVariable"/>.
        /// </summary>
        /// <param name="variableId">The Hydra-Ring id corresponding to the variable that is considered.</param>
        /// <param name="deviationType">The deviation type of the variable.</param>
        /// <param name="n">The n value of the variable.</param>
        /// <param name="standardDeviation">The standardDeviation value of the variable.</param>
        public RayleighNHydraRingVariable(int variableId, HydraRingDeviationType deviationType,
                                          double n, double standardDeviation)
            : base(variableId, deviationType, standardDeviation, n) {}

        public override HydraRingDistributionType DistributionType
        {
            get
            {
                return HydraRingDistributionType.RayleighN;
            }
        }
    }
}