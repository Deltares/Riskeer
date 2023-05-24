// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base.Geometry;
using Riskeer.AssemblyTool.IO.Helpers;

namespace Riskeer.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class that holds all the information to export the assembly results
    /// of an assessment section.
    /// </summary>
    public class ExportableAssessmentSection
    {
        /// <summary>
        /// Creates an instance of <see cref="ExportableAssessmentSection"/>.
        /// </summary>
        /// <param name="id">The id of the assessment section.</param>
        /// <param name="name">The name of the assessment section.</param>
        /// <param name="geometry">The geometry of the assessment section.</param>
        /// <param name="failureMechanismSectionCollections">The collection of failure mechanism section collections.</param>
        /// <param name="assessmentSectionAssembly">The assembly result of the assessment section.</param>
        /// <param name="failureMechanisms">The assembly results of failure mechanisms belonging
        /// to this assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter except <paramref name="id"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public ExportableAssessmentSection(string id,
                                           string name,
                                           IEnumerable<Point2D> geometry,
                                           IEnumerable<ExportableFailureMechanismSectionCollection> failureMechanismSectionCollections,
                                           ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly,
                                           IEnumerable<ExportableFailureMechanism> failureMechanisms)
        {
            IdValidationHelper.ThrowIfInvalid(id);

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            if (failureMechanismSectionCollections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionCollections));
            }

            if (assessmentSectionAssembly == null)
            {
                throw new ArgumentNullException(nameof(assessmentSectionAssembly));
            }

            if (failureMechanisms == null)
            {
                throw new ArgumentNullException(nameof(failureMechanisms));
            }

            Id = id;
            Name = name;
            Geometry = geometry;
            FailureMechanismSectionCollections = failureMechanismSectionCollections;
            AssessmentSectionAssembly = assessmentSectionAssembly;
            FailureMechanisms = failureMechanisms;
        }

        /// <summary>
        /// Gets the id of the assessment section.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the name of the assessment section.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the geometry of the assessment section.
        /// </summary>
        public IEnumerable<Point2D> Geometry { get; }

        /// <summary>
        /// Gets the collection of failure mechanism section collections.
        /// </summary>
        public IEnumerable<ExportableFailureMechanismSectionCollection> FailureMechanismSectionCollections { get; }

        /// <summary>
        /// Gets the assembly result of the assessment section.
        /// </summary>
        public ExportableAssessmentSectionAssemblyResult AssessmentSectionAssembly { get; }

        /// <summary>
        /// Gets the collection of assembly results of failure mechanisms belonging to this assessment section.
        /// </summary>
        public IEnumerable<ExportableFailureMechanism> FailureMechanisms { get; }
    }
}