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
using Ringtoets.AssemblyTool.IO.Model;

namespace Ringtoets.Integration.IO
{
    /// <summary>
    /// Class that holds all the information that is related when generating a collection of
    /// <see cref="SerializableCombinedFailureMechanismSectionAssembly"/>.
    /// </summary>
    public class AggregatedSerializableCombinedFailureMechanismSectionAssemblies
    {
        /// <summary>
        /// An instance of <see cref="AggregatedSerializableFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanismSectionCollection">The <see cref="SerializableFailureMechanismSectionCollection"/>
        /// that the <paramref name="failureMechanismSections"/> belong to.</param>
        /// <param name="failureMechanismSections">A collection of <see cref="SerializableFailureMechanismSection"/>
        /// that is associated with <paramref name="combinedFailureMechanismSectionAssemblies"/>.</param>
        /// <param name="combinedFailureMechanismSectionAssemblies">A collection of <see cref="SerializableCombinedFailureMechanismSectionAssembly"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AggregatedSerializableCombinedFailureMechanismSectionAssemblies(SerializableFailureMechanismSectionCollection failureMechanismSectionCollection,
                                                                               IEnumerable<SerializableFailureMechanismSection> failureMechanismSections,
                                                                               IEnumerable<SerializableCombinedFailureMechanismSectionAssembly> combinedFailureMechanismSectionAssemblies)
        {
            if (failureMechanismSectionCollection == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionCollection));
            }

            if (failureMechanismSections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSections));
            }

            if (combinedFailureMechanismSectionAssemblies == null)
            {
                throw new ArgumentNullException(nameof(combinedFailureMechanismSectionAssemblies));
            }

            FailureMechanismSectionCollection = failureMechanismSectionCollection;
            FailureMechanismSections = failureMechanismSections;
            CombinedFailureMechanismSectionAssemblies = combinedFailureMechanismSectionAssemblies;
        }

        /// <summary>
        /// Gets the collection where the serializable failure mechanism sections belong to.
        /// </summary>
        public SerializableFailureMechanismSectionCollection FailureMechanismSectionCollection { get; }

        /// <summary>
        /// Gets the collection of serializable failure mechanism sections.
        /// </summary>
        public IEnumerable<SerializableFailureMechanismSection> FailureMechanismSections { get; }

        /// <summary>
        /// Gets the collection of serializable combined failure mechanism section assemblies.
        /// </summary>
        public IEnumerable<SerializableCombinedFailureMechanismSectionAssembly> CombinedFailureMechanismSectionAssemblies { get; }
    }
}