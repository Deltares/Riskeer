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
using System.Linq;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.Integration.IO.AggregatedSerializable;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.Integration.IO.Properties;

namespace Ringtoets.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="AggregatedSerializableCombinedFailureMechanismSectionAssembly"/>.
    /// </summary>
    internal static class AggregatedSerializableCombinedFailureMechanismSectionAssemblyCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="AggregatedSerializableCombinedFailureMechanismSectionAssembly"/>
        /// based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the serializable components.</param>
        /// <param name="serializableTotalAssemblyResult">The <see cref="SerializableTotalAssemblyResult"/> the serializable
        /// components belongs to.</param>
        /// <param name="serializableFailureMechanismSectionCollection">The <see cref="SerializableFailureMechanismSectionCollection"/>
        /// the serializable failure mechanism sections belong to.</param>
        /// <param name="combinedSectionAssembly">The <see cref="ExportableCombinedSectionAssembly"/> to create an
        /// <see cref="AggregatedSerializableCombinedFailureMechanismSectionAssembly"/> for.</param>
        /// <returns>An <see cref="AggregatedSerializableCombinedFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static AggregatedSerializableCombinedFailureMechanismSectionAssembly Create(IdentifierGenerator idGenerator,
                                                                                           SerializableTotalAssemblyResult serializableTotalAssemblyResult,
                                                                                           SerializableFailureMechanismSectionCollection serializableFailureMechanismSectionCollection,
                                                                                           ExportableCombinedSectionAssembly combinedSectionAssembly)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (serializableTotalAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(serializableTotalAssemblyResult));
            }

            if (serializableFailureMechanismSectionCollection == null)
            {
                throw new ArgumentNullException(nameof(serializableFailureMechanismSectionCollection));
            }

            if (combinedSectionAssembly == null)
            {
                throw new ArgumentNullException(nameof(combinedSectionAssembly));
            }

            SerializableFailureMechanismSection failureMechanismSection = SerializableFailureMechanismSectionCreator.Create(idGenerator,
                                                                                                                            serializableFailureMechanismSectionCollection,
                                                                                                                            combinedSectionAssembly.Section);

            return new AggregatedSerializableCombinedFailureMechanismSectionAssembly(
                failureMechanismSection,
                new SerializableCombinedFailureMechanismSectionAssembly(
                    idGenerator.GetNewId(Resources.SerializableCombinedFailureMechanismSectionAssembly_IdPrefix),
                    serializableTotalAssemblyResult,
                    failureMechanismSection,
                    combinedSectionAssembly.FailureMechanismResults
                                           .Select(SerializableCombinedFailureMechanismSectionAssemblyResultCreator.Create)
                                           .ToArray(),
                    SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.CombinedAssessment,
                                                                                    combinedSectionAssembly.CombinedSectionAssemblyResult)));
        }
    }
}