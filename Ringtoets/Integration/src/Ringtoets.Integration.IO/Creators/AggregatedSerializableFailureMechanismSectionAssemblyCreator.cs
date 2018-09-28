// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.Integration.IO.AggregatedSerializable;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Exceptions;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.Integration.IO.Properties;

namespace Ringtoets.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/>.
    /// </summary>
    internal static class AggregatedSerializableFailureMechanismSectionAssemblyCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the serializable components.</param>
        /// <param name="serializableCollection">The <see cref="SerializableFailureMechanismSectionCollection"/> the result belongs to.</param>
        /// <param name="serializableFailureMechanism">The <see cref="SerializableFailureMechanism"/> the result belongs to.</param>
        /// <param name="sectionResult">The <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResult"/>
        /// to create an <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> for.</param>
        /// <returns>An <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="sectionResult"/> is invalid to create a serializable counterpart for.</exception>
        public static AggregatedSerializableFailureMechanismSectionAssembly Create(IdentifierGenerator idGenerator,
                                                                                   SerializableFailureMechanismSectionCollection serializableCollection,
                                                                                   SerializableFailureMechanism serializableFailureMechanism,
                                                                                   ExportableAggregatedFailureMechanismSectionAssemblyResult sectionResult)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (serializableCollection == null)
            {
                throw new ArgumentNullException(nameof(serializableCollection));
            }

            if (serializableFailureMechanism == null)
            {
                throw new ArgumentNullException(nameof(serializableFailureMechanism));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            SerializableFailureMechanismSection failureMechanismSection = SerializableFailureMechanismSectionCreator.Create(idGenerator,
                                                                                                                            serializableCollection,
                                                                                                                            sectionResult.FailureMechanismSection);

            var failureMechanismSectionAssembly = new SerializableFailureMechanismSectionAssembly(idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionAssembly_IdPrefix),
                                                                                                  serializableFailureMechanism,
                                                                                                  failureMechanismSection,
                                                                                                  CreateAssemblySectionResults(sectionResult),
                                                                                                  SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.CombinedAssessment,
                                                                                                                                                                  sectionResult.CombinedAssembly));
            return new AggregatedSerializableFailureMechanismSectionAssembly(failureMechanismSection,
                                                                             failureMechanismSectionAssembly);
        }

        /// <summary>
        /// Creates an instance of <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The id generator to generate an id for <see cref="SerializableFailureMechanismSectionAssembly"/>.</param>
        /// <param name="serializableCollection">The <see cref="SerializableFailureMechanismSectionCollection"/> the result belongs to.</param>
        /// <param name="serializableFailureMechanism">The <see cref="SerializableFailureMechanism"/> the result belongs to.</param>
        /// <param name="sectionResult">The <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/>
        /// to create an <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> for.</param>
        /// <returns>An <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="sectionResult"/> is invalid to create a serializable counterpart for.</exception>
        public static AggregatedSerializableFailureMechanismSectionAssembly Create(IdentifierGenerator idGenerator,
                                                                                   SerializableFailureMechanismSectionCollection serializableCollection,
                                                                                   SerializableFailureMechanism serializableFailureMechanism,
                                                                                   ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability sectionResult)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (serializableCollection == null)
            {
                throw new ArgumentNullException(nameof(serializableCollection));
            }

            if (serializableFailureMechanism == null)
            {
                throw new ArgumentNullException(nameof(serializableFailureMechanism));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            SerializableFailureMechanismSection failureMechanismSection = SerializableFailureMechanismSectionCreator.Create(idGenerator,
                                                                                                                            serializableCollection,
                                                                                                                            sectionResult.FailureMechanismSection);

            var failureMechanismSectionAssembly = new SerializableFailureMechanismSectionAssembly(idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionAssembly_IdPrefix),
                                                                                                  serializableFailureMechanism,
                                                                                                  failureMechanismSection,
                                                                                                  CreateAssemblySectionResults(sectionResult),
                                                                                                  SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.CombinedAssessment,
                                                                                                                                                                  sectionResult.CombinedAssembly));
            return new AggregatedSerializableFailureMechanismSectionAssembly(failureMechanismSection,
                                                                             failureMechanismSectionAssembly);
        }

        /// <summary>
        /// Creates an instance of <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The id generator to generate an id for <see cref="SerializableFailureMechanismSectionAssembly"/>.</param>
        /// <param name="serializableCollection">The <see cref="SerializableFailureMechanismSectionCollection"/> the result belongs to.</param>
        /// <param name="serializableFailureMechanism">The <see cref="SerializableFailureMechanism"/> the result belongs to.</param>
        /// <param name="sectionResult">The <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly"/>
        /// to create an <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> for.</param>
        /// <returns>An <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="sectionResult"/> is invalid to create a serializable counterpart for.</exception>
        public static AggregatedSerializableFailureMechanismSectionAssembly Create(IdentifierGenerator idGenerator,
                                                                                   SerializableFailureMechanismSectionCollection serializableCollection,
                                                                                   SerializableFailureMechanism serializableFailureMechanism,
                                                                                   ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly sectionResult)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (serializableCollection == null)
            {
                throw new ArgumentNullException(nameof(serializableCollection));
            }

            if (serializableFailureMechanism == null)
            {
                throw new ArgumentNullException(nameof(serializableFailureMechanism));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            SerializableFailureMechanismSection failureMechanismSection = SerializableFailureMechanismSectionCreator.Create(idGenerator,
                                                                                                                            serializableCollection,
                                                                                                                            sectionResult.FailureMechanismSection);

            var failureMechanismSectionAssembly = new SerializableFailureMechanismSectionAssembly(idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionAssembly_IdPrefix),
                                                                                                  serializableFailureMechanism,
                                                                                                  failureMechanismSection,
                                                                                                  CreateAssemblySectionResults(sectionResult),
                                                                                                  SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.CombinedAssessment,
                                                                                                                                                                  sectionResult.CombinedAssembly));
            return new AggregatedSerializableFailureMechanismSectionAssembly(failureMechanismSection,
                                                                             failureMechanismSectionAssembly);
        }

        /// <summary>
        /// Creates an instance of <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The id generator to generate an id for <see cref="SerializableFailureMechanismSectionAssembly"/>.</param>
        /// <param name="serializableCollection">The <see cref="SerializableFailureMechanismSectionCollection"/> the result belongs to.</param>
        /// <param name="serializableFailureMechanism">The <see cref="SerializableFailureMechanism"/> the result belongs to.</param>
        /// <param name="sectionResult">The <see cref="ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult"/>
        /// to create an <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> for.</param>
        /// <returns>An <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="sectionResult"/> is invalid to create a serializable counterpart for.</exception>
        public static AggregatedSerializableFailureMechanismSectionAssembly Create(IdentifierGenerator idGenerator,
                                                                                   SerializableFailureMechanismSectionCollection serializableCollection,
                                                                                   SerializableFailureMechanism serializableFailureMechanism,
                                                                                   ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult sectionResult)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (serializableCollection == null)
            {
                throw new ArgumentNullException(nameof(serializableCollection));
            }

            if (serializableFailureMechanism == null)
            {
                throw new ArgumentNullException(nameof(serializableFailureMechanism));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            SerializableFailureMechanismSection failureMechanismSection = SerializableFailureMechanismSectionCreator.Create(idGenerator,
                                                                                                                            serializableCollection,
                                                                                                                            sectionResult.FailureMechanismSection);

            var failureMechanismSectionAssembly = new SerializableFailureMechanismSectionAssembly(idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionAssembly_IdPrefix),
                                                                                                  serializableFailureMechanism,
                                                                                                  failureMechanismSection,
                                                                                                  new SerializableFailureMechanismSectionAssemblyResult[0],
                                                                                                  SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.CombinedAssessment,
                                                                                                                                                                  sectionResult.CombinedAssembly));
            return new AggregatedSerializableFailureMechanismSectionAssembly(failureMechanismSection,
                                                                             failureMechanismSectionAssembly);
        }

        /// <summary>
        /// Creates an instance of <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The id generator to generate an id for <see cref="SerializableFailureMechanismSectionAssembly"/>.</param>
        /// <param name="serializableCollection">The <see cref="SerializableFailureMechanismSectionCollection"/> the result belongs to.</param>
        /// <param name="serializableFailureMechanism">The <see cref="SerializableFailureMechanism"/> the result belongs to.</param>
        /// <param name="sectionResult">The <see cref="ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult"/> to create an
        /// <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/> for.</param>
        /// <returns>An <see cref="AggregatedSerializableFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="sectionResult"/> is invalid to create a serializable counterpart for.</exception>
        public static AggregatedSerializableFailureMechanismSectionAssembly Create(IdentifierGenerator idGenerator,
                                                                                   SerializableFailureMechanismSectionCollection serializableCollection,
                                                                                   SerializableFailureMechanism serializableFailureMechanism,
                                                                                   ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult sectionResult)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (serializableCollection == null)
            {
                throw new ArgumentNullException(nameof(serializableCollection));
            }

            if (serializableFailureMechanism == null)
            {
                throw new ArgumentNullException(nameof(serializableFailureMechanism));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            SerializableFailureMechanismSection failureMechanismSection = SerializableFailureMechanismSectionCreator.Create(idGenerator,
                                                                                                                            serializableCollection,
                                                                                                                            sectionResult.FailureMechanismSection);

            var failureMechanismSectionAssembly = new SerializableFailureMechanismSectionAssembly(idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionAssembly_IdPrefix),
                                                                                                  serializableFailureMechanism,
                                                                                                  failureMechanismSection,
                                                                                                  new SerializableFailureMechanismSectionAssemblyResult[0],
                                                                                                  SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.CombinedAssessment,
                                                                                                                                                                  sectionResult.CombinedAssembly));
            return new AggregatedSerializableFailureMechanismSectionAssembly(failureMechanismSection,
                                                                             failureMechanismSectionAssembly);
        }

        /// <summary>
        /// Creates a collection of <see cref="SerializableFailureMechanismSectionAssemblyResult"/> based on <paramref name="sectionResult"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResult"/> to create a collection of
        /// <see cref="SerializableFailureMechanismSectionAssemblyResult"/> for.</param>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="sectionResult"/> is invalid to create a serializable counterpart for.</exception>
        private static SerializableFailureMechanismSectionAssemblyResult[] CreateAssemblySectionResults(ExportableAggregatedFailureMechanismSectionAssemblyResult sectionResult)
        {
            var serializableSectionAssemblyResults = new List<SerializableFailureMechanismSectionAssemblyResult>();

            if (sectionResult.SimpleAssembly.AssemblyCategory != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                serializableSectionAssemblyResults.Add(SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.SimpleAssessment,
                                                                                                                       sectionResult.SimpleAssembly));
            }

            if (sectionResult.DetailedAssembly.AssemblyCategory != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                serializableSectionAssemblyResults.Add(SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.DetailedAssessment,
                                                                                                                       sectionResult.DetailedAssembly));
            }

            if (sectionResult.TailorMadeAssembly.AssemblyCategory != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                serializableSectionAssemblyResults.Add(SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.TailorMadeAssessment,
                                                                                                                       sectionResult.TailorMadeAssembly));
            }

            return serializableSectionAssemblyResults.ToArray();
        }

        /// <summary>
        /// Creates a collection of <see cref="SerializableFailureMechanismSectionAssemblyResult"/> based on <paramref name="sectionResult"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/> to create a collection of
        /// <see cref="SerializableFailureMechanismSectionAssemblyResult"/> for.</param>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="sectionResult"/> is invalid to create a serializable counterpart for.</exception>
        private static SerializableFailureMechanismSectionAssemblyResult[] CreateAssemblySectionResults(
            ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability sectionResult)
        {
            var serializableSectionAssemblyResults = new List<SerializableFailureMechanismSectionAssemblyResult>();

            if (sectionResult.SimpleAssembly.AssemblyCategory != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                serializableSectionAssemblyResults.Add(SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.SimpleAssessment,
                                                                                                                       sectionResult.SimpleAssembly));
            }

            if (sectionResult.DetailedAssembly.AssemblyCategory != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                serializableSectionAssemblyResults.Add(SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.DetailedAssessment,
                                                                                                                       sectionResult.DetailedAssembly));
            }

            if (sectionResult.TailorMadeAssembly.AssemblyCategory != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                serializableSectionAssemblyResults.Add(SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.TailorMadeAssessment,
                                                                                                                       sectionResult.TailorMadeAssembly));
            }

            return serializableSectionAssemblyResults.ToArray();
        }

        /// <summary>
        /// Creates a collection of <see cref="SerializableFailureMechanismSectionAssemblyResult"/> based on <paramref name="sectionResult"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly"/> to create a collection of
        /// <see cref="SerializableFailureMechanismSectionAssemblyResult"/> for.</param>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="sectionResult"/> is invalid to create a serializable counterpart for.</exception>
        private static SerializableFailureMechanismSectionAssemblyResult[] CreateAssemblySectionResults(
            ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly sectionResult)
        {
            var serializableSectionAssemblyResults = new List<SerializableFailureMechanismSectionAssemblyResult>();

            if (sectionResult.SimpleAssembly.AssemblyCategory != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                serializableSectionAssemblyResults.Add(SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.SimpleAssessment,
                                                                                                                       sectionResult.SimpleAssembly));
            }

            if (sectionResult.TailorMadeAssembly.AssemblyCategory != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                serializableSectionAssemblyResults.Add(SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.TailorMadeAssessment,
                                                                                                                       sectionResult.TailorMadeAssembly));
            }

            return serializableSectionAssemblyResults.ToArray();
        }
    }
}