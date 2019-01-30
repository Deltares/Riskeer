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

using System;
using System.Collections.Generic;
using Riskeer.AssemblyTool.IO.Model;

namespace Riskeer.Integration.IO.AggregatedSerializable
{
    /// <summary>
    /// Class that holds all the information that is related when creating a <see cref="SerializableFailureMechanism"/>.
    /// </summary>
    internal class AggregatedSerializableFailureMechanism
    {
        /// <summary>
        /// Creates a new instance of <see cref="AggregatedSerializableFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="SerializableFailureMechanism"/>.</param>
        /// <param name="failureMechanismSectionCollection">The <see cref="SerializableFailureMechanismSectionCollection"/>
        /// that belongs to the failure mechanism.</param>
        /// <param name="failureMechanismSections">A collection of <see cref="SerializableFailureMechanismSection"/>
        /// that belongs to the failure mechanism.</param>
        /// <param name="failureMechanismSectionAssemblyResults">A collection of <see cref="SerializableFailureMechanismSectionAssembly"/>
        /// that belongs to the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AggregatedSerializableFailureMechanism(SerializableFailureMechanism failureMechanism,
                                                      SerializableFailureMechanismSectionCollection failureMechanismSectionCollection,
                                                      IEnumerable<SerializableFailureMechanismSection> failureMechanismSections,
                                                      IEnumerable<SerializableFailureMechanismSectionAssembly> failureMechanismSectionAssemblyResults)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (failureMechanismSectionCollection == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionCollection));
            }

            if (failureMechanismSections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSections));
            }

            if (failureMechanismSectionAssemblyResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionAssemblyResults));
            }

            FailureMechanism = failureMechanism;
            FailureMechanismSectionCollection = failureMechanismSectionCollection;
            FailureMechanismSections = failureMechanismSections;
            FailureMechanismSectionAssemblyResults = failureMechanismSectionAssemblyResults;
        }

        /// <summary>
        /// Gets the serializable failure mechanism.
        /// </summary>
        public SerializableFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the collection where the serializable failure mechanism sections belongs to.
        /// </summary>
        public SerializableFailureMechanismSectionCollection FailureMechanismSectionCollection { get; }

        /// <summary>
        /// Gets the collection of serializable failure mechanism sections.
        /// </summary>
        public IEnumerable<SerializableFailureMechanismSection> FailureMechanismSections { get; }

        /// <summary>
        /// Gets the collection of serializable failure mechanism section assembly results.
        /// </summary>
        public IEnumerable<SerializableFailureMechanismSectionAssembly> FailureMechanismSectionAssemblyResults { get; }
    }
}