// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
    /// Class for Truncated Normal Hydra-Ring variable related data.
    /// </summary>
    public class TruncatedNormalHydraRingVariable : RandomHydraRingVariable
    {
        /// <summary>
        /// Creates a new instance of <see cref="TruncatedNormalHydraRingVariable"/>.
        /// </summary>
        /// <param name="variableId">The Hydra-Ring id corresponding to the variable that is considered.</param>
        /// <param name="deviationType">The deviation type of the variable.</param>
        /// <param name="mean">The mean value of the variable.</param>
        /// <param name="variance">The variance value of the variable.</param>
        /// <param name="lowerBoundary">The lower boundary value of the variable.</param>
        /// <param name="upperBoundary">The upper boundary value of the variable.</param>
        public TruncatedNormalHydraRingVariable(int variableId, HydraRingDeviationType deviationType,
                                                double mean, double variance, double lowerBoundary, double upperBoundary)
            : base(variableId, deviationType, mean, variance)
        {
            Parameter3 = lowerBoundary;
            Parameter4 = upperBoundary;
        }

        public override double Parameter3 { get; }

        public override double Parameter4 { get; }

        public override HydraRingDistributionType DistributionType
        {
            get
            {
                return HydraRingDistributionType.TruncatedNormal;
            }
        }
    }
}