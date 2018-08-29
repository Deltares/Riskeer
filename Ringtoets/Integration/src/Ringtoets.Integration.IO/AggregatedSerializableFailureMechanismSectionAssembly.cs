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
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;

namespace Ringtoets.Integration.IO
{
    /// <summary>
    /// Class that holds all the information related to creating a
    /// <see cref="SerializableFailureMechanismSectionAssemblyResult"/>.
    /// </summary>
    public class AggregatedSerializableFailureMechanismSectionAssembly
    {
        /// <summary>
        /// Instantiates a <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/>.
        /// </summary>
        /// <param name="failureMechanismSection">The <see cref="SerializableFailureMechanismSection"/></param>
        /// <param name="failureMechanismSectionAssemblyResult">The <see cref="SerializableFailureMechanismSectionAssemblyResult"/>
        /// that is associated with <paramref name="failureMechanismSection"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AggregatedSerializableFailureMechanismSectionAssembly(SerializableFailureMechanismSection failureMechanismSection,
                                                                     SerializableFailureMechanismSectionAssemblyResult failureMechanismSectionAssemblyResult)
        {
            if (failureMechanismSection == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSection));
            }

            if (failureMechanismSectionAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionAssemblyResult));
            }

            FailureMechanismSection = failureMechanismSection;
            FailureMechanismSectionAssemblyResult = failureMechanismSectionAssemblyResult;
        }

        /// <summary>
        /// Gets the failure mechanism section.
        /// </summary>
        public SerializableFailureMechanismSection FailureMechanismSection { get; }

        /// <summary>
        /// Gets the failure mechanism section assembly result.
        /// </summary>
        public SerializableFailureMechanismSectionAssemblyResult FailureMechanismSectionAssemblyResult { get; }
    }
}