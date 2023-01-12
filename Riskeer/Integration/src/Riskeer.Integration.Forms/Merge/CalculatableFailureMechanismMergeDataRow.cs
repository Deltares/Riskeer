// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System;
using System.Linq;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Integration.Forms.Merge
{
    /// <summary>
    /// Row representing the information of a <see cref="ICalculatableFailureMechanism"/> to be
    /// used for merging.
    /// </summary>
    internal class CalculatableFailureMechanismMergeDataRow : FailureMechanismMergeDataRow
    {
        private readonly ICalculatableFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="CalculatableFailureMechanismMergeDataRow"/>.
        /// </summary>
        /// <param name="failureMechanism">The wrapped <see cref="ICalculatableFailureMechanism"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public CalculatableFailureMechanismMergeDataRow(ICalculatableFailureMechanism failureMechanism) : base(failureMechanism)
        {
            this.failureMechanism = failureMechanism;
        }

        public override int NumberOfCalculations => failureMechanism.Calculations.Count();
    }
}