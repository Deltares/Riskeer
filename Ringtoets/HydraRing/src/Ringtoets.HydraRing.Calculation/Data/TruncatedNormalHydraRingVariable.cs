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

namespace Ringtoets.HydraRing.Calculation.Data
{
    public class TruncatedNormalHydraRingVariable : HydraRingVariable2
    {
        private readonly double mean;
        private readonly double variance;
        private readonly double lowerBoundary;
        private readonly double upperBoundary;

        /// <summary>
        /// Creates a new instance of <see cref="TruncatedNormalHydraRingVariable"/>.
        /// </summary>
        /// <param name="variableId">The Hydra-Ring id corresponding to the variable that is considered.</param>
        /// <param name="deviationType">The deviation type in case the variable is random.</param>
        /// <param name="mean">The mean value of the variable.</param>
        /// <param name="variance">The variance value of the variable.</param>
        /// <param name="lowerBoundary">The lowerBoundary value of the variable.</param>
        /// <param name="upperBoundary">The upperBoundary value of the variable.</param>
        public TruncatedNormalHydraRingVariable(int variableId, HydraRingDeviationType deviationType,
                                                double mean, double variance, double lowerBoundary, double upperBoundary)
            : base(variableId, deviationType)
        {
            this.mean = mean;
            this.variance = variance;
            this.lowerBoundary = lowerBoundary;
            this.upperBoundary = upperBoundary;
        }

        public override double Parameter1
        {
            get
            {
                return mean;
            }
        }

        public override double? Parameter2
        {
            get
            {
                return DeviationType == HydraRingDeviationType.Standard
                           ? variance
                           : base.Parameter2;
            }
        }

        public override double? Parameter3
        {
            get
            {
                return lowerBoundary;
            }
        }

        public override double? Parameter4
        {
            get
            {
                return upperBoundary;
            }
        }

        public override double CoefficientOfVariation
        {
            get
            {
                return DeviationType == HydraRingDeviationType.Variation
                           ? variance
                           : base.CoefficientOfVariation;
            }
        }

        public override HydraRingDistributionType DistributionType
        {
            get
            {
                return HydraRingDistributionType.TruncatedNormal;
            }
        }
    }
}