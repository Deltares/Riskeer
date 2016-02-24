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

using System.Collections.Generic;
using Ringtoets.HydraRing.Calculation.Types;

namespace Ringtoets.HydraRing.Calculation.Settings
{
    /// <summary>
    /// Container for failure mechanims defaults.
    /// </summary>
    public class FailureMechanismDefaults
    {
        private readonly int variableId;
        private readonly int mechanismId;
        private readonly int calculationTypeId;
        private readonly IEnumerable<int> subMechanismIds;

        /// <summary>
        /// Creates a new instance of the <see cref="FailureMechanismDefaults"/> class.
        /// </summary>
        /// <param name="mechanismId">The mechanism id that corresponds to a specific <see cref="HydraRingFailureMechanismType"/>.</param>
        /// <param name="calculationTypeId">The calculation type id that is applicable for a specific <see cref="HydraRingFailureMechanismType"/>.</param>
        /// <param name="variableId">The id of the variable that is relevant for a specific <see cref="HydraRingFailureMechanismType"/>.</param>
        /// <param name="subMechanismIds">The sub mechanism ids that are applicable for a specific <see cref="HydraRingFailureMechanismType"/>.</param>
        public FailureMechanismDefaults(int mechanismId, int calculationTypeId, int variableId, IEnumerable<int> subMechanismIds)
        {
            this.mechanismId = mechanismId;
            this.variableId = variableId;
            this.calculationTypeId = calculationTypeId;
            this.subMechanismIds = subMechanismIds;
        }

        /// <summary>
        /// Gets the mechanism id that corresponds to a specific <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        public int MechanismId
        {
            get
            {
                return mechanismId;
            }
        }

        /// <summary>
        /// Gets the calculation type id that is applicable for a specific <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        public int CalculationTypeId
        {
            get
            {
                return calculationTypeId;
            }
        }

        /// <summary>
        /// Gets the id of the variable that is relevant for a specific <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        public int VariableId
        {
            get
            {
                return variableId;
            }
        }

        /// <summary>
        /// Gets the sub mechanism ids that are applicable for a specific <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        public IEnumerable<int> SubMechanismIds
        {
            get
            {
                return subMechanismIds;
            }
        }
    }
}
