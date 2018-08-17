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
using System.Xml.Serialization;
using Core.Common.Base.Geometry;
using Ringtoets.AssemblyTool.IO.Model.Gml;

namespace Ringtoets.AssemblyTool.IO.Model
{
    /// <summary>
    /// Serializable class containing all data for an assembly result export.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = AssemblyXmlIdentifiers.Assembly, Namespace = AssemblyXmlIdentifiers.AssemblyNamespace)]
    public class SerializableAssembly
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableAssembly"/>.
        /// </summary>
        public SerializableAssembly() {}

        /// <summary>
        /// Creates a new instance of <see cref="SerializableAssembly"/>.
        /// </summary>
        /// <param name="id">The unique ID of the assembly.</param>
        /// <param name="lowerCorner">The lower corner of the assembly map boundary.</param>
        /// <param name="upperCorner">The upper corner of the assembly map boundary.</param>
        /// <param name="assessmentSections">The collection of <see cref="SerializableAssessmentSection"/> that
        /// belong to the assembly.</param>
        /// <param name="assessmentProcesses">The collection of <see cref="SerializableAssessmentProcess"/> that
        /// belong to the assembly.</param>
        /// <param name="totalAssemblyResults">The collection of <see cref="SerializableTotalAssemblyResult"/> that
        /// belong to the assembly.</param>
        /// <param name="failureMechanisms">The collection of <see cref="SerializableFailureMechanism"/> that
        /// belong to the assembly.</param>
        /// <param name="failureMechanismSectionAssemblies">The collection of <see cref="SerializableFailureMechanismSectionAssembly"/> that
        /// belong to the assembly.</param>
        /// <param name="combinedFailureMechanismSectionAssemblies">The collection of <see cref="SerializableCombinedFailureMechanismSectionAssembly"/> that
        /// belong to the assembly.</param>
        /// <param name="failureMechanismSectionCollections">The collection of <see cref="SerializableFailureMechanismSectionCollection"/> that
        /// belong to the assembly.</param>
        /// <param name="failureMechanismSections">The collection of <see cref="SerializableFailureMechanismSection"/> that
        /// belong to the assembly.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SerializableAssembly(string id,
                                    Point2D lowerCorner,
                                    Point2D upperCorner,
                                    IEnumerable<SerializableAssessmentSection> assessmentSections,
                                    IEnumerable<SerializableAssessmentProcess> assessmentProcesses,
                                    IEnumerable<SerializableTotalAssemblyResult> totalAssemblyResults,
                                    IEnumerable<SerializableFailureMechanism> failureMechanisms,
                                    IEnumerable<SerializableFailureMechanismSectionAssembly> failureMechanismSectionAssemblies,
                                    IEnumerable<SerializableCombinedFailureMechanismSectionAssembly> combinedFailureMechanismSectionAssemblies,
                                    IEnumerable<SerializableFailureMechanismSectionCollection> failureMechanismSectionCollections,
                                    IEnumerable<SerializableFailureMechanismSection> failureMechanismSections)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (lowerCorner == null)
            {
                throw new ArgumentNullException(nameof(lowerCorner));
            }

            if (upperCorner == null)
            {
                throw new ArgumentNullException(nameof(upperCorner));
            }

            if (assessmentSections == null)
            {
                throw new ArgumentNullException(nameof(assessmentSections));
            }

            if (assessmentProcesses == null)
            {
                throw new ArgumentNullException(nameof(assessmentProcesses));
            }

            if (totalAssemblyResults == null)
            {
                throw new ArgumentNullException(nameof(totalAssemblyResults));
            }

            if (failureMechanisms == null)
            {
                throw new ArgumentNullException(nameof(failureMechanisms));
            }

            if (failureMechanismSectionAssemblies == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionAssemblies));
            }

            if (combinedFailureMechanismSectionAssemblies == null)
            {
                throw new ArgumentNullException(nameof(combinedFailureMechanismSectionAssemblies));
            }

            if (failureMechanismSectionCollections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionCollections));
            }

            if (failureMechanismSections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSections));
            }

            Id = id;
            Boundary = new SerializableBoundary(lowerCorner, upperCorner);

            var featureMembers = new List<SerializableFeatureMember>();
            featureMembers.AddRange(assessmentSections);
            featureMembers.AddRange(assessmentProcesses);
            featureMembers.AddRange(totalAssemblyResults);
            featureMembers.AddRange(failureMechanisms);
            featureMembers.AddRange(failureMechanismSectionAssemblies);
            featureMembers.AddRange(combinedFailureMechanismSectionAssemblies);
            featureMembers.AddRange(failureMechanismSectionCollections);
            featureMembers.AddRange(failureMechanismSections);
            FeatureMembers = featureMembers.ToArray();
        }

        /// <summary>
        /// Gets or sets the ID of the assembly.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.Id, Namespace = AssemblyXmlIdentifiers.GmlNamespace)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the map boundary of the assembly.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.BoundedBy, Namespace = AssemblyXmlIdentifiers.GmlNamespace)]
        public SerializableBoundary Boundary { get; set; }

        /// <summary>
        /// Gets or sets the collection of feature members for the assembly.
        /// </summary>
        [XmlArray(AssemblyXmlIdentifiers.FeatureMember)]
        [XmlArrayItem(typeof(SerializableAssessmentSection))]
        [XmlArrayItem(typeof(SerializableAssessmentProcess))]
        [XmlArrayItem(typeof(SerializableTotalAssemblyResult))]
        [XmlArrayItem(typeof(SerializableFailureMechanism))]
        [XmlArrayItem(typeof(SerializableFailureMechanismSectionAssembly))]
        [XmlArrayItem(typeof(SerializableCombinedFailureMechanismSectionAssembly))]
        [XmlArrayItem(typeof(SerializableFailureMechanismSectionCollection))]
        [XmlArrayItem(typeof(SerializableFailureMechanismSection))]
        public SerializableFeatureMember[] FeatureMembers { get; set; }
    }
}