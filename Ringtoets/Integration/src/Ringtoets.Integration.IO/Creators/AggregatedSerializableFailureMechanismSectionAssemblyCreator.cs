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
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.Integration.IO.Properties;

namespace Ringtoets.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instance of <see cref="SerializableFailureMechanismSectionAssembly"/>
    /// </summary>
    public static class AggregatedSerializableFailureMechanismSectionAssemblyCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="SerializableFailureMechanismSectionAssembly"/>
        /// based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The id generator to generate an id for <see cref="SerializableFailureMechanismSectionAssembly"/>.</param>
        /// <param name="serializableCollection">The <see cref="SerializableFailureMechanismSectionCollection"/> the result belongs to.</param>
        /// <param name="serializableFailureMechanism">The <see cref="SerializableFailureMechanism"/> the result belongs to.</param>
        /// <param name="sectionResult">The <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResult"/> to create a
        /// <see cref="SerializableFailureMechanismSectionAssembly"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SerializableFailureMechanismSectionAssembly Create(UniqueIdentifierGenerator idGenerator,
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

            return new SerializableFailureMechanismSectionAssembly(idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionAssembly_IdPrefix),
                                                                   serializableFailureMechanism,
                                                                   SerializableFailureMechanismSectionCreator.Create(idGenerator,
                                                                                                                     serializableCollection,
                                                                                                                     sectionResult.FailureMechanismSection),
                                                                   CreateAssemblySectionResults(sectionResult),
                                                                   SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.CombinedAssessment,
                                                                                                                                   sectionResult.CombinedAssembly));
        }

        /// <summary>
        /// Creates an instance of <see cref="SerializableFailureMechanismSectionAssembly"/>
        /// based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The id generator to generate an id for <see cref="SerializableFailureMechanismSectionAssembly"/>.</param>
        /// <param name="serializableCollection">The <see cref="SerializableFailureMechanismSectionCollection"/> the result belongs to.</param>
        /// <param name="serializableFailureMechanism">The <see cref="SerializableFailureMechanism"/> the result belongs to.</param>
        /// <param name="sectionResult">The <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/> to create a
        /// <see cref="SerializableFailureMechanismSectionAssembly"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SerializableFailureMechanismSectionAssembly Create(UniqueIdentifierGenerator idGenerator,
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

            return new SerializableFailureMechanismSectionAssembly(idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionAssembly_IdPrefix),
                                                                   serializableFailureMechanism,
                                                                   SerializableFailureMechanismSectionCreator.Create(idGenerator,
                                                                                                                     serializableCollection,
                                                                                                                     sectionResult.FailureMechanismSection),
                                                                   CreateAssemblySectionResults(sectionResult),
                                                                   SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.CombinedAssessment,
                                                                                                                                   sectionResult.CombinedAssembly));
        }

        /// <summary>
        /// Creates an instance of <see cref="SerializableFailureMechanismSectionAssembly"/>
        /// based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The id generator to generate an id for <see cref="SerializableFailureMechanismSectionAssembly"/>.</param>
        /// <param name="serializableCollection">The <see cref="SerializableFailureMechanismSectionCollection"/> the result belongs to.</param>
        /// <param name="serializableFailureMechanism">The <see cref="SerializableFailureMechanism"/> the result belongs to.</param>
        /// <param name="sectionResult">The <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly"/> to create a
        /// <see cref="SerializableFailureMechanismSectionAssembly"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SerializableFailureMechanismSectionAssembly Create(UniqueIdentifierGenerator idGenerator,
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

            return new SerializableFailureMechanismSectionAssembly(idGenerator.GetNewId(Resources.SerializableFailureMechanismSectionAssembly_IdPrefix),
                                                                   serializableFailureMechanism,
                                                                   SerializableFailureMechanismSectionCreator.Create(idGenerator,
                                                                                                                     serializableCollection,
                                                                                                                     sectionResult.FailureMechanismSection),
                                                                   CreateAssemblySectionResults(sectionResult),
                                                                   SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.CombinedAssessment,
                                                                                                                                   sectionResult.CombinedAssembly));
        }

        private static SerializableFailureMechanismSectionAssemblyResult[] CreateAssemblySectionResults(ExportableAggregatedFailureMechanismSectionAssemblyResult sectionResult)
        {
            return new[]
            {
                SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.SimpleAssessment, sectionResult.SimpleAssembly),
                SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.DetailedAssessment, sectionResult.DetailedAssembly),
                SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.TailorMadeAssessment, sectionResult.TailorMadeAssembly)
            };
        }

        private static SerializableFailureMechanismSectionAssemblyResult[] CreateAssemblySectionResults(
            ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability sectionResult)
        {
            return new[]
            {
                SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.SimpleAssessment, sectionResult.SimpleAssembly),
                SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.DetailedAssessment, sectionResult.DetailedAssembly),
                SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.TailorMadeAssessment, sectionResult.TailorMadeAssembly)
            };
        }

        private static SerializableFailureMechanismSectionAssemblyResult[] CreateAssemblySectionResults(
            ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly sectionResult)
        {
            return new[]
            {
                SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.SimpleAssessment, sectionResult.SimpleAssembly),
                SerializableFailureMechanismSectionAssemblyResultCreator.Create(SerializableAssessmentType.TailorMadeAssessment, sectionResult.TailorMadeAssembly)
            };
        }
    }
}