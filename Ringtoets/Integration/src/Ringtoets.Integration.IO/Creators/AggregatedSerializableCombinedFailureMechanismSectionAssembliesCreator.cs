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
using System.Linq;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.Integration.IO.AggregatedSerializable;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.Integration.IO.Properties;

namespace Ringtoets.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="AggregatedSerializableCombinedFailureMechanismSectionAssemblies"/>.
    /// </summary>
    internal static class AggregatedSerializableCombinedFailureMechanismSectionAssembliesCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="AggregatedSerializableCombinedFailureMechanismSectionAssemblies"/>
        /// based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the serializable components.</param>
        /// <param name="totalAssemblyResult">The <see cref="SerializableTotalAssemblyResult"/> the serializable components belong to.</param>
        /// <param name="combinedSectionAssemblies">The collection of <see cref="ExportableCombinedSectionAssembly"/>
        /// to create an <see cref="AggregatedSerializableCombinedFailureMechanismSectionAssemblies"/> for.</param>
        /// <returns>An <see cref="AggregatedSerializableCombinedFailureMechanismSectionAssemblies"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static AggregatedSerializableCombinedFailureMechanismSectionAssemblies Create(IdentifierGenerator idGenerator,
                                                                                             SerializableTotalAssemblyResult totalAssemblyResult,
                                                                                             IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblies)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (totalAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(totalAssemblyResult));
            }

            if (combinedSectionAssemblies == null)
            {
                throw new ArgumentNullException(nameof(combinedSectionAssemblies));
            }

            var failureMechanismSectionCollection = new SerializableFailureMechanismSectionCollection(
                idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionCollection_IdPrefix));

            AggregatedSerializableCombinedFailureMechanismSectionAssembly[] aggregatedAssemblies =
                combinedSectionAssemblies.Select(assembly => AggregatedSerializableCombinedFailureMechanismSectionAssemblyCreator.Create(
                                                     idGenerator, totalAssemblyResult, failureMechanismSectionCollection, assembly))
                                         .ToArray();

            return new AggregatedSerializableCombinedFailureMechanismSectionAssemblies(failureMechanismSectionCollection,
                                                                                       aggregatedAssemblies.Select(ag => ag.FailureMechanismSection),
                                                                                       aggregatedAssemblies.Select(ag => ag.CombinedFailureMechanismSectionAssembly));
        }
    }
}