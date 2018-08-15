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
using System.Xml.Serialization;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;

namespace Ringtoets.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class describing a serializable failure mechanism section assembly result.
    /// </summary>
    [XmlType(AssemblyXmlIdentifiers.FailureMechanismSectionAssemblyResult)]
    public class SerializableFailureMechanismSectionAssembly : SerializableFeatureMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanismSectionAssembly"/>.
        /// </summary>
        public SerializableFailureMechanismSectionAssembly() {}

        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanismSectionAssembly"/>.
        /// </summary>
        /// <param name="id">The unique ID of the assembly result.</param>
        /// <param name="failureMechanism">The failure mechanism this assembly belongs to.</param>
        /// <param name="sectionResults">The collection of assembly results for this section assembly.</param>
        /// <param name="combinedSectionResult">The combined assembly result for this section assembly.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SerializableFailureMechanismSectionAssembly(string id,
                                                           SerializableFailureMechanism failureMechanism,
                                                           SerializableFailureMechanismSectionAssemblyResult[] sectionResults,
                                                           SerializableFailureMechanismSectionAssemblyResult combinedSectionResult) : this()
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (sectionResults == null)
            {
                throw new ArgumentNullException(nameof(sectionResults));
            }

            if (combinedSectionResult == null)
            {
                throw new ArgumentNullException(nameof(combinedSectionResult));
            }

            Id = id;
            FailureMechanismId = failureMechanism.Id;
            SectionResults = sectionResults;
            CombinedSectionResult = combinedSectionResult;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.FailureMechanismSectionAssemblyResultId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the parent failure mechanism ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.FailureMechanismIdRef)]
        public string FailureMechanismId { get; set; }

        /// <summary>
        /// Gets or sets the combined assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.CombinedSectionResult)]
        public SerializableFailureMechanismSectionAssemblyResult CombinedSectionResult { get; set; }

        /// <summary>
        /// Gets or sets the array of results for this section.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.SectionResults)]
        public SerializableFailureMechanismSectionAssemblyResult[] SectionResults { get; set; }
    }
}