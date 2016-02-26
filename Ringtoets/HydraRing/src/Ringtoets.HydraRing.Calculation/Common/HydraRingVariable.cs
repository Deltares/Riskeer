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

using Ringtoets.HydraRing.Calculation.Types;

namespace Ringtoets.HydraRing.Calculation.Common
{
    /// <summary>
    /// Container for Hydra-Ring variable related data.
    /// </summary>
    public abstract class HydraRingVariable
    {
        private readonly int variableId;
        private readonly HydraRingDistributionType distributionType;
        private readonly double value;
        private readonly HydraRingDeviationType deviationType;
        private readonly double mean;
        private readonly double variability;
        private readonly double shift;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingVariable"/> class.
        /// </summary>
        /// <param name="variableId">The Hydra-Ring id corresponding to the variable that is considered.</param>
        /// <param name="distributionType">The probability distribution of the variable.</param>
        /// <param name="value">The value in case the variable is deterministic.</param>
        /// <param name="deviationType">The deviation type in case the variable is random.</param>
        /// <param name="mean">The mean value in case the variable is random.</param>
        /// <param name="variability">The variability in case the variable is random.</param>
        /// <param name="shift">The shift in case the variable is random.</param>
        protected HydraRingVariable(int variableId, HydraRingDistributionType distributionType, double value, HydraRingDeviationType deviationType, double mean, double variability, double shift)
        {
            this.variableId = variableId;
            this.distributionType = distributionType;
            this.value = value;
            this.deviationType = deviationType;
            this.mean = mean;
            this.variability = variability;
            this.shift = shift;
        }

        /// <summary>
        /// Gets the Hydra-Ring id corresponding to the variable that is considered.
        /// </summary>
        public int VariableId
        {
            get
            {
                return variableId;
            }
        }

        /// <summary>
        /// Gets the probability distribution of the variable.
        /// </summary>
        public HydraRingDistributionType DistributionType
        {
            get
            {
                return distributionType;
            }
        }

        /// <summary>
        /// Gets the value in case the variable is deterministic.
        /// </summary>
        /// <remarks>
        /// This property is only relevant when <see cref="DistributionType"/> equals <see cref="HydraRingDistributionType.Deterministic"/>.
        /// </remarks>
        public double Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// Gets the deviation type in case the variable is random.
        /// </summary>
        /// <remarks>
        /// This property is only relevant when <see cref="DistributionType"/> is not equal to <see cref="HydraRingDistributionType.Deterministic"/>.
        /// </remarks>
        public HydraRingDeviationType DeviationType
        {
            get
            {
                return deviationType;
            }
        }

        /// <summary>
        /// Gets the mean value in case the variable is random.
        /// </summary>
        /// <remarks>
        /// This property is only relevant when <see cref="DistributionType"/> is not equal to <see cref="HydraRingDistributionType.Deterministic"/>.
        /// </remarks>
        public double Mean
        {
            get
            {
                return mean;
            }
        }

        /// <summary>
        /// Gets the variablity in case the variable is random.
        /// </summary>
        /// <remarks>
        /// The value represents a standard deviation in case the <see cref="DeviationType"/> equals <see cref="HydraRingDeviationType.Standard"/>.
        /// The value represents a variation coefficient in case the <see cref="DeviationType"/> equals <see cref="HydraRingDeviationType.Variation"/>.
        /// </remarks>
        public double Variability
        {
            get
            {
                return variability;
            }
        }

        /// <summary>
        /// Gets the shift in case the variable is random.
        /// </summary>
        /// <remarks>
        /// This property is only relevant when <see cref="DistributionType"/> equals <see cref="HydraRingDistributionType.LogNormal"/>.
        /// </remarks>
        public double Shift
        {
            get
            {
                return shift;
            }
        }
    }
}
