// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Integration.IO.AggregatedSerializable;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="SerializableAssembly"/>.
    /// </summary>
    public static class SerializableAssemblyCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="SerializableAssembly"/> based
        /// on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="ExportableAssessmentSection"/>
        /// to create a <see cref="SerializableAssembly"/> for.</param>
        /// <returns>A <see cref="SerializableAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyCreatorException">Thrown when the <paramref name="assessmentSection"/> is invalid
        /// to create a serializable counterpart for.</exception>
        public static SerializableAssembly Create(ExportableAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var idGenerator = new IdentifierGenerator();
            string serializableAssemblyId = idGenerator.GetNewId(Resources.SerializableAssembly_IdPrefix);

            SerializableAssessmentSection serializableAssessmentSection = SerializableAssessmentSectionCreator.Create(assessmentSection);
            SerializableAssessmentProcess serializableAssessmentProcess = SerializableAssessmentProcessCreator.Create(idGenerator, serializableAssessmentSection);
            SerializableTotalAssemblyResult serializableTotalAssemblyResult =
                SerializableTotalAssemblyResultCreator.Create(
                    idGenerator, serializableAssessmentProcess,
                    SerializableAssessmentSectionAssemblyResultCreator.Create(assessmentSection.AssessmentSectionAssembly));

            AggregatedSerializableFailureMechanism[] aggregatedFailureMechanismsWithProbability = assessmentSection.FailureMechanismsWithoutProbability
                                                                                                                   .Select(fm => CreateFailureMechanismsWithoutProbability(idGenerator, serializableTotalAssemblyResult, fm))
                                                                                                                   .ToArray();
            AggregatedSerializableFailureMechanism[] aggregatedFailureMechanismsWithoutProbability = assessmentSection.FailureMechanismsWithoutProbability
                                                                                                                      .Select(fm => CreateFailureMechanismsWithoutProbability(idGenerator, serializableTotalAssemblyResult, fm))
                                                                                                                      .ToArray();

            AggregatedSerializableCombinedFailureMechanismSectionAssemblies aggregatedSerializableCombinedFailureMechanismSectionAssemblies =
                AggregatedSerializableCombinedFailureMechanismSectionAssembliesCreator.Create(idGenerator,
                                                                                              serializableTotalAssemblyResult,
                                                                                              assessmentSection.CombinedSectionAssemblies);

            return new SerializableAssembly(serializableAssemblyId,
                                            GetLowerCorner(assessmentSection.Geometry),
                                            GetUpperCorner(assessmentSection.Geometry),
                                            serializableAssessmentSection,
                                            serializableAssessmentProcess,
                                            serializableTotalAssemblyResult,
                                            GetAllSerializableFailureMechanisms(aggregatedFailureMechanismsWithProbability,
                                                                                aggregatedFailureMechanismsWithoutProbability),
                                            GetAllSerializableFailureMechanismSectionAssemblies(aggregatedFailureMechanismsWithProbability,
                                                                                                aggregatedFailureMechanismsWithoutProbability),
                                            aggregatedSerializableCombinedFailureMechanismSectionAssemblies.CombinedFailureMechanismSectionAssemblies,
                                            GetAllSerializableFailureMechanismSectionCollections(aggregatedFailureMechanismsWithProbability,
                                                                                                 aggregatedFailureMechanismsWithoutProbability,
                                                                                                 aggregatedSerializableCombinedFailureMechanismSectionAssemblies),
                                            GetAllSerializableFailureMechanismSections(aggregatedFailureMechanismsWithProbability,
                                                                                       aggregatedFailureMechanismsWithoutProbability,
                                                                                       aggregatedSerializableCombinedFailureMechanismSectionAssemblies));
        }

        private static AggregatedSerializableFailureMechanism CreateFailureMechanismsWithoutProbability(IdentifierGenerator idGenerator,
                                                                                                        SerializableTotalAssemblyResult serializableTotalAssemblyResult,
                                                                                                        ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> failureMechanism)
        {
            return AggregatedSerializableFailureMechanismCreator.Create(idGenerator, serializableTotalAssemblyResult, failureMechanism);
        }

        private static IEnumerable<SerializableFailureMechanismSectionAssembly> GetAllSerializableFailureMechanismSectionAssemblies(
            IEnumerable<AggregatedSerializableFailureMechanism> aggregatedFailureMechanismsWithProbability,
            IEnumerable<AggregatedSerializableFailureMechanism> aggregatedFailureMechanismsWithoutProbability)
        {
            var serializableFailureMechanismSectionAssemblies = new List<SerializableFailureMechanismSectionAssembly>();
            serializableFailureMechanismSectionAssemblies.AddRange(aggregatedFailureMechanismsWithProbability.SelectMany(afm => afm.FailureMechanismSectionAssemblyResults));
            serializableFailureMechanismSectionAssemblies.AddRange(aggregatedFailureMechanismsWithoutProbability.SelectMany(afm => afm.FailureMechanismSectionAssemblyResults));
            return serializableFailureMechanismSectionAssemblies;
        }

        private static IEnumerable<SerializableFailureMechanismSection> GetAllSerializableFailureMechanismSections(
            IEnumerable<AggregatedSerializableFailureMechanism> aggregatedFailureMechanismsWithProbability,
            IEnumerable<AggregatedSerializableFailureMechanism> aggregatedFailureMechanismsWithoutProbability,
            AggregatedSerializableCombinedFailureMechanismSectionAssemblies aggregatedSerializableCombinedFailureMechanismSectionAssemblies)
        {
            var serializableFailureMechanismSections = new List<SerializableFailureMechanismSection>();
            serializableFailureMechanismSections.AddRange(aggregatedFailureMechanismsWithProbability.SelectMany(afm => afm.FailureMechanismSections));
            serializableFailureMechanismSections.AddRange(aggregatedFailureMechanismsWithoutProbability.SelectMany(afm => afm.FailureMechanismSections));
            serializableFailureMechanismSections.AddRange(aggregatedSerializableCombinedFailureMechanismSectionAssemblies.FailureMechanismSections);
            return serializableFailureMechanismSections;
        }

        private static IEnumerable<SerializableFailureMechanismSectionCollection> GetAllSerializableFailureMechanismSectionCollections(
            IEnumerable<AggregatedSerializableFailureMechanism> aggregatedFailureMechanismsWithProbability,
            IEnumerable<AggregatedSerializableFailureMechanism> aggregatedFailureMechanismsWithoutProbability,
            AggregatedSerializableCombinedFailureMechanismSectionAssemblies aggregatedSerializableCombinedFailureMechanismSectionAssemblies)
        {
            var serializableFailureMechanismSectionCollection = new List<SerializableFailureMechanismSectionCollection>();
            serializableFailureMechanismSectionCollection.AddRange(aggregatedFailureMechanismsWithProbability.Select(afm => afm.FailureMechanismSectionCollection));
            serializableFailureMechanismSectionCollection.AddRange(aggregatedFailureMechanismsWithoutProbability.Select(afm => afm.FailureMechanismSectionCollection));
            serializableFailureMechanismSectionCollection.Add(aggregatedSerializableCombinedFailureMechanismSectionAssemblies.FailureMechanismSectionCollection);
            return serializableFailureMechanismSectionCollection;
        }

        private static IEnumerable<SerializableFailureMechanism> GetAllSerializableFailureMechanisms(
            IEnumerable<AggregatedSerializableFailureMechanism> aggregatedFailureMechanismsWithProbability,
            IEnumerable<AggregatedSerializableFailureMechanism> aggregatedFailureMechanismsWithoutProbability)
        {
            var serializableFailureMechanisms = new List<SerializableFailureMechanism>();
            serializableFailureMechanisms.AddRange(aggregatedFailureMechanismsWithProbability.Select(afm => afm.FailureMechanism));
            serializableFailureMechanisms.AddRange(aggregatedFailureMechanismsWithoutProbability.Select(afm => afm.FailureMechanism));
            return serializableFailureMechanisms;
        }

        private static Point2D GetLowerCorner(IEnumerable<Point2D> geometry)
        {
            return new Point2D(geometry.Select(p => p.X).Min(),
                               geometry.Select(p => p.Y).Min());
        }

        private static Point2D GetUpperCorner(IEnumerable<Point2D> geometry)
        {
            return new Point2D(geometry.Select(p => p.X).Max(),
                               geometry.Select(p => p.Y).Max());
        }
    }
}