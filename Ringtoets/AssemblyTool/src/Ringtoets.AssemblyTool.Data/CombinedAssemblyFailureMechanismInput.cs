// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Collections.Generic;

namespace Ringtoets.AssemblyTool.Data
{
    /// <summary>
    /// Class that contains the data that is necessary to determine
    /// the combined failure mechanism sections assembly. 
    /// </summary>
    public class CombinedAssemblyFailureMechanismInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="CombinedAssemblyFailureMechanismInput"/>.
        /// </summary>
        /// <param name="failureMechanismN">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <param name="failureMechanismContribution">The contribution of the failure mechanism.</param>
        /// <param name="sections">The sections of the failure mechanism.</param>
        public CombinedAssemblyFailureMechanismInput(double failureMechanismN, double failureMechanismContribution,
                                                     IEnumerable<CombinedAssemblyFailureMechanismSection> sections)
        {
            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            N = failureMechanismN;
            FailureMechanismContribution = failureMechanismContribution;
            Sections = sections;
        }


        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        public double N { get; }

        /// <summary>
        /// Gets the contribution of the failure mechanism.
        /// </summary>
        public double FailureMechanismContribution { get; }

        /// <summary>
        /// Gets the sections of the failure mechanism.
        /// </summary>
        public IEnumerable<CombinedAssemblyFailureMechanismSection> Sections { get; }
    }
}