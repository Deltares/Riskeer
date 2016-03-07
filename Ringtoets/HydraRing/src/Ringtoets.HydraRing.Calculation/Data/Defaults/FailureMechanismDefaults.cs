﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.HydraRing.Calculation.Data.Defaults
{
    /// <summary>
    /// Container for failure mechanism defaults.
    /// </summary>
    internal class FailureMechanismDefaults
    {
        private readonly int variableId;
        private readonly int mechanismId;
        private readonly IEnumerable<int> subMechanismIds;

        /// <summary>
        /// Creates a new instance of the <see cref="FailureMechanismDefaults"/> class.
        /// </summary>
        /// <param name="mechanismId">The mechanism id.</param>
        /// <param name="variableId">The id of the variable that is relevant.</param>
        /// <param name="subMechanismIds">The sub mechanism ids that are applicable.</param>
        public FailureMechanismDefaults(int mechanismId, int variableId, IEnumerable<int> subMechanismIds)
        {
            this.mechanismId = mechanismId;
            this.variableId = variableId;
            this.subMechanismIds = subMechanismIds;
        }

        /// <summary>
        /// Gets the mechanism id.
        /// </summary>
        public int MechanismId
        {
            get
            {
                return mechanismId;
            }
        }

        /// <summary>
        /// Gets the id of the variable that is relevant.
        /// </summary>
        public int VariableId
        {
            get
            {
                return variableId;
            }
        }

        /// <summary>
        /// Gets the sub mechanism ids that are applicable.
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