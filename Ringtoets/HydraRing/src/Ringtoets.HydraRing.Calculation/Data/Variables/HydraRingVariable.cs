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
    /// Abstract base class for Hydra-Ring variable related data.
    /// </summary>
    public abstract class HydraRingVariable
    {
        private const double defaultHydraRingValue = 0.0;
        private readonly double? defaultHydraRingNullValue = null;

        private readonly int variableId;
        private readonly HydraRingDeviationType deviationType;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingVariable"/> class.
        /// </summary>
        /// <param name="variableId">The Hydra-Ring id corresponding to the variable that is considered.</param>
        /// <param name="deviationType">The deviation type in case the variable is random.</param>
        protected HydraRingVariable(int variableId, HydraRingDeviationType deviationType)
        {
            this.variableId = variableId;
            this.deviationType = deviationType;
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
        /// Gets the value in case the variable is deterministic.
        /// </summary>
        public virtual double Value
        {
            get
            {
                return defaultHydraRingValue;
            }
        }

        /// <summary>
        /// Gets the probability distribution of the variable.
        /// </summary>
        public abstract HydraRingDistributionType DistributionType { get; }

        /// <summary>
        /// Gets the deviation type in case the variable is random.
        /// </summary>
        public HydraRingDeviationType DeviationType
        {
            get
            {
                return deviationType;
            }
        }

        /// <summary>
        /// Gets the parameter 1 value in case the variable is random.
        /// </summary>
        public virtual double Parameter1
        {
            get
            {
                return defaultHydraRingValue;
            }
        }

        /// <summary>
        /// Gets the parameter 2 value in case the variable is random.
        /// </summary>
        public virtual double? Parameter2
        {
            get
            {
                return defaultHydraRingNullValue;
            }
        }

        /// <summary>
        /// Gets the parameter 3 value in case the variable is random.
        /// </summary>
        public virtual double? Parameter3
        {
            get
            {
                return defaultHydraRingNullValue;
            }
        }

        /// <summary>
        /// Gets the parameter 4 value in case the variable is random.
        /// </summary>
        public virtual double? Parameter4
        {
            get
            {
                return defaultHydraRingNullValue;
            }
        }

        /// <summary>
        /// Gets the coefficient of variation in case the variable is random.
        /// </summary>
        public virtual double CoefficientOfVariation
        {
            get
            {
                return defaultHydraRingValue;
            }
        }
    }
}