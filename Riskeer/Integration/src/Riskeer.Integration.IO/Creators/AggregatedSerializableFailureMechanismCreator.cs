﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Integration.IO.AggregatedSerializable;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="AggregatedSerializableFailureMechanism"/>.
    /// </summary>
    internal static class AggregatedSerializableFailureMechanismCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="AggregatedSerializableFailureMechanism"/> based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the serializable components.</param>
        /// <param name="serializableTotalAssemblyResult">The <see cref="SerializableTotalAssemblyResult"/>
        /// the serializable components belong to.</param>
        /// <param name="failureMechanism">The <see cref="ExportableFailureMechanism"/>
        /// to create an <see cref="AggregatedSerializableFailureMechanism"/> for.</param>
        /// <returns>An <see cref="AggregatedSerializableFailureMechanism"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="failureMechanism"/>
        /// is invalid to create a serializable counterpart for.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="ExportableFailureMechanism"/>
        /// contains unsupported items in the failure mechanism section assembly results.</exception>
        public static AggregatedSerializableFailureMechanism Create(IdentifierGenerator idGenerator,
                                                                    SerializableTotalAssemblyResult serializableTotalAssemblyResult,
                                                                    ExportableFailureMechanism failureMechanism)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (serializableTotalAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(serializableTotalAssemblyResult));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            SerializableFailureMechanism serializableFailureMechanism = SerializableFailureMechanismCreator.Create(idGenerator, serializableTotalAssemblyResult, failureMechanism);
            var serializableCollection = new SerializableFailureMechanismSectionCollection(idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionCollection_IdPrefix));

            AggregatedSerializableFailureMechanismSectionAssembly[] serializableFailureMechanismSectionAssemblyResults =
                failureMechanism.SectionAssemblyResults
                                .Select(sectionAssemblyResult => CreateFailureMechanismSectionAssembly(
                                            idGenerator, serializableFailureMechanism, serializableCollection, sectionAssemblyResult))
                                .ToArray();

            return new AggregatedSerializableFailureMechanism(serializableFailureMechanism,
                                                              serializableCollection,
                                                              serializableFailureMechanismSectionAssemblyResults.Select(fmr => fmr.FailureMechanismSection),
                                                              serializableFailureMechanismSectionAssemblyResults.Select(fmr => fmr.FailureMechanismSectionAssembly));
        }

        /// <summary>
        /// Creates an instance of <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The id generator to generate the id for the serializable components.</param>
        /// <param name="serializableFailureMechanism">The <see cref="SerializableFailureMechanism"/> the section assembly belongs to.</param>
        /// <param name="serializableCollection">The <see cref="SerializableFailureMechanismSectionCollection"/> the section assembly belongs to.</param>
        /// <param name="failureMechanismSectionAssemblyResult">An aggregated failure mechanism section assembly result to
        /// create an <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> for.</param>
        /// <returns>An <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="failureMechanismSectionAssemblyResult"/> is invalid to create a serializable counterpart for.</exception>
        /// <exception cref="NotSupportedException">Thrown when an <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/>
        /// cannot be created for <paramref name="failureMechanismSectionAssemblyResult"/>.</exception>
        private static AggregatedSerializableFailureMechanismSectionAssembly CreateFailureMechanismSectionAssembly(
            IdentifierGenerator idGenerator,
            SerializableFailureMechanism serializableFailureMechanism,
            SerializableFailureMechanismSectionCollection serializableCollection,
            ExportableFailureMechanismSectionAssemblyWithProbabilityResult failureMechanismSectionAssemblyResult)
        {
            SerializableFailureMechanismSection failureMechanismSection = SerializableFailureMechanismSectionCreator.Create(
                idGenerator, serializableCollection, failureMechanismSectionAssemblyResult.FailureMechanismSection);

            var failureMechanismSectionAssembly = new SerializableFailureMechanismSectionAssembly(
                idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionAssembly_IdPrefix),
                serializableFailureMechanism, failureMechanismSection,
                SerializableFailureMechanismSectionAssemblyResultCreator.Create(failureMechanismSectionAssemblyResult));

            return new AggregatedSerializableFailureMechanismSectionAssembly(failureMechanismSection, failureMechanismSectionAssembly);
        }
    }
}