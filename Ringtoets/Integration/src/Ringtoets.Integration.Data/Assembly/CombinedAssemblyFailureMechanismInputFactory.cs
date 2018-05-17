﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Integration.Data.Assembly
{
    /// <summary>
    /// Factory that creates <see cref="CombinedAssemblyFailureMechanismInput"/> instances.
    /// </summary>
    internal static class CombinedAssemblyFailureMechanismInputFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CombinedAssemblyFailureMechanismInput"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to use.</param>
        /// <param name="failureMechanisms">The failure mechanisms to build input for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="CombinedAssemblyFailureMechanismInput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<CombinedAssemblyFailureMechanismInput> CreateInput(AssessmentSection assessmentSection,
                                                                                     IEnumerable<IFailureMechanism> failureMechanisms)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (failureMechanisms == null)
            {
                throw new ArgumentNullException(nameof(failureMechanisms));
            }

            return new List<CombinedAssemblyFailureMechanismInput>();
        }
    }
}