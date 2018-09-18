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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.Integration.IO.AggregatedSerializable;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Exceptions;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.Integration.IO.Properties;

namespace Ringtoets.Integration.IO.Creators
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
        /// <param name="serializableTotalAssemblyResult">The <see cref="SerializableTotalAssemblyResult"/> the serializable components belong to.</param>
        /// <param name="failureMechanism">The <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> without a probability to
        /// create an <see cref="AggregatedSerializableFailureMechanism"/> for.</param>
        /// <returns>An <see cref="AggregatedSerializableFailureMechanism"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyCreatorException">Thrown when the assembly result cannot be created.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// contains unsupported items in the failure mechanism (section) assembly results.</exception>
        public static AggregatedSerializableFailureMechanism Create(IdentifierGenerator idGenerator,
                                                                    SerializableTotalAssemblyResult serializableTotalAssemblyResult,
                                                                    ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> failureMechanism)
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

            ValidateFailureMechanismAssemblyResult(failureMechanism.FailureMechanismAssembly);

            SerializableFailureMechanism serializableFailureMechanism = SerializableFailureMechanismCreator.Create(idGenerator, serializableTotalAssemblyResult, failureMechanism);
            var serializableCollection = new SerializableFailureMechanismSectionCollection(idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionCollection_IdPrefix));

            AggregatedSerializableFailureMechanismSectionAssembly[] serializableFailureMechanismSectionAssemblyResults =
                failureMechanism.SectionAssemblyResults
                                .Select(sectionAssemblyResult => CreateFailureMechanismSectionAssembly(idGenerator,
                                                                                                       serializableFailureMechanism,
                                                                                                       serializableCollection,
                                                                                                       sectionAssemblyResult))
                                .ToArray();

            return new AggregatedSerializableFailureMechanism(serializableFailureMechanism,
                                                              serializableCollection,
                                                              serializableFailureMechanismSectionAssemblyResults.Select(fmr => fmr.FailureMechanismSection),
                                                              serializableFailureMechanismSectionAssemblyResults.Select(fmr => fmr.FailureMechanismSectionAssembly));
        }

        /// <summary>
        /// Creates an instance of <see cref="AggregatedSerializableFailureMechanism"/> based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The id generator to generate an id for the serializable components.</param>
        /// <param name="serializableTotalAssemblyResult">The <see cref="SerializableTotalAssemblyResult"/> the serializable components belong to.</param>
        /// <param name="failureMechanism">The <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with a probability to
        /// create an <see cref="AggregatedSerializableFailureMechanism"/> for.</param>
        /// <returns>An <see cref="AggregatedSerializableFailureMechanism"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyCreatorException">Thrown when the assembly result cannot be created.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// contains unsupported items in the failure mechanism (section) assembly results.</exception>
        public static AggregatedSerializableFailureMechanism Create(IdentifierGenerator idGenerator,
                                                                    SerializableTotalAssemblyResult serializableTotalAssemblyResult,
                                                                    ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> failureMechanism)
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

            ValidateFailureMechanismAssemblyResult(failureMechanism.FailureMechanismAssembly);

            SerializableFailureMechanism serializableFailureMechanism = SerializableFailureMechanismCreator.Create(idGenerator, serializableTotalAssemblyResult, failureMechanism);
            var serializableCollection = new SerializableFailureMechanismSectionCollection(idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionCollection_IdPrefix));

            AggregatedSerializableFailureMechanismSectionAssembly[] serializableFailureMechanismSectionAssemblyResults =
                failureMechanism.SectionAssemblyResults
                                .Select(sectionAssemblyResult => CreateFailureMechanismSectionAssembly(idGenerator,
                                                                                                       serializableFailureMechanism,
                                                                                                       serializableCollection,
                                                                                                       sectionAssemblyResult))
                                .ToArray();

            return new AggregatedSerializableFailureMechanism(serializableFailureMechanism,
                                                              serializableCollection,
                                                              serializableFailureMechanismSectionAssemblyResults.Select(fmr => fmr.FailureMechanismSection),
                                                              serializableFailureMechanismSectionAssemblyResults.Select(fmr => fmr.FailureMechanismSectionAssembly));
        }


        /// <summary>
        /// Validates whether an <see cref="ExportableFailureMechanismAssemblyResult"/> is valid to be created.
        /// </summary>
        /// <param name="assemblyResult">The <see cref="ExportableFailureMechanismAssemblyResult"/> to validate.</param>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="assemblyResult"/>
        /// is invalid to create.</exception>
        private static void ValidateFailureMechanismAssemblyResult(ExportableFailureMechanismAssemblyResult assemblyResult)
        {
            if (assemblyResult.AssemblyCategory == FailureMechanismAssemblyCategoryGroup.None)
            {
                throw new AssemblyCreatorException(@"The assembly result is invalid and cannot be created.");
            }
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
        /// <exception cref="AssemblyCreatorException">Thrown when the assembly result cannot be created.</exception>
        /// <exception cref="NotSupportedException">Thrown when an <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/>
        /// cannot be created for <paramref name="failureMechanismSectionAssemblyResult"/>.</exception>
        private static AggregatedSerializableFailureMechanismSectionAssembly CreateFailureMechanismSectionAssembly(
            IdentifierGenerator idGenerator,
            SerializableFailureMechanism serializableFailureMechanism,
            SerializableFailureMechanismSectionCollection serializableCollection,
            ExportableAggregatedFailureMechanismSectionAssemblyResultBase failureMechanismSectionAssemblyResult)
        {
            var resultWithProbability = failureMechanismSectionAssemblyResult as ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability;
            if (resultWithProbability != null)
            {
                return AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, resultWithProbability);
            }

            var resultWithoutProbability = failureMechanismSectionAssemblyResult as ExportableAggregatedFailureMechanismSectionAssemblyResult;
            if (resultWithoutProbability != null)
            {
                return AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, resultWithoutProbability);
            }

            var resultWithoutDetailedAssembly = failureMechanismSectionAssemblyResult as ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly;
            if (resultWithoutDetailedAssembly != null)
            {
                return AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, resultWithoutDetailedAssembly);
            }

            throw new NotSupportedException($"{failureMechanismSectionAssemblyResult.GetType().Name} is not supported.");
        }
    }
}