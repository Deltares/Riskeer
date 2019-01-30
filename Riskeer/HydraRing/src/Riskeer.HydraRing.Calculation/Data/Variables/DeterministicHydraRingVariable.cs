// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Riskeer.HydraRing.Calculation.Data.Variables
{
    /// <summary>
    /// Class for Deterministic Hydra-Ring variable related data.
    /// </summary>
    public class DeterministicHydraRingVariable : HydraRingVariable
    {
        /// <summary>
        /// Creates a new instance of <see cref="DeterministicHydraRingVariable"/>.
        /// </summary>
        /// <param name="variableId">The Hydra-Ring id corresponding to the variable that is considered.</param>
        /// <param name="value">The value of the variable.</param>
        public DeterministicHydraRingVariable(int variableId, double value)
            : base(variableId)
        {
            Value = value;
        }

        public override double Value { get; }

        public override HydraRingDistributionType DistributionType
        {
            get
            {
                return HydraRingDistributionType.Deterministic;
            }
        }

        public override HydraRingDeviationType DeviationType
        {
            get
            {
                return HydraRingDeviationType.Standard;
            }
        }
    }
}