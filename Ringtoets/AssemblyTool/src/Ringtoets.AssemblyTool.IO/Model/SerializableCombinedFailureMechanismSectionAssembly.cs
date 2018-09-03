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
using Ringtoets.AssemblyTool.IO.Model.Helpers;

namespace Ringtoets.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class describing a serializable combined failure mechanism section assembly.
    /// </summary>
    [XmlType(AssemblyXmlIdentifiers.CombinedFailureMechanismSectionAssembly)]
    public class SerializableCombinedFailureMechanismSectionAssembly : SerializableFeatureMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableCombinedFailureMechanismSectionAssembly"/>.
        /// </summary>
        public SerializableCombinedFailureMechanismSectionAssembly() {}

        /// <summary>
        /// Creates a new instance of <see cref="SerializableCombinedFailureMechanismSectionAssembly"/>.
        /// </summary>
        /// <param name="id">The unique ID of the assembly.</param>
        /// <param name="totalAssemblyResult">The total assembly result this assembly belongs to.</param>
        /// <param name="section">The section this assembly belongs to.</param>
        /// <param name="failureMechanismResults">The collection of assembly results for this assembly per failure mechanism.</param>
        /// <param name="combinedSectionResult">The combined assembly result for this assembly.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public SerializableCombinedFailureMechanismSectionAssembly(string id,
                                                                   SerializableTotalAssemblyResult totalAssemblyResult,
                                                                   SerializableFailureMechanismSection section,
                                                                   SerializableCombinedFailureMechanismSectionAssemblyResult[] failureMechanismResults,
                                                                   SerializableFailureMechanismSectionAssemblyResult combinedSectionResult) : this()
        {
            if (!IdValidator.Validate(id))
            {
                throw new ArgumentException($@"'{nameof(id)}' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.");
            }

            if (totalAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(totalAssemblyResult));
            }

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (failureMechanismResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismResults));
            }

            if (combinedSectionResult == null)
            {
                throw new ArgumentNullException(nameof(combinedSectionResult));
            }

            Id = id;
            TotalAssemblyResultId = totalAssemblyResult.Id;
            FailureMechanismSectionId = section.Id;
            FailureMechanismResults = failureMechanismResults;
            CombinedSectionResult = combinedSectionResult;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.CombinedFailureMechanismSectionAssemblyId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the parent total assembly result ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.TotalAssemblyResultIdRef)]
        public string TotalAssemblyResultId { get; set; }

        /// <summary>
        /// Gets or sets the parent failure mechanism section ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.FailureMechanismSectionIdRef)]
        public string FailureMechanismSectionId { get; set; }

        /// <summary>
        /// Gets or sets the combined assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.CombinedCombinedSectionResult)]
        public SerializableFailureMechanismSectionAssemblyResult CombinedSectionResult { get; set; }

        /// <summary>
        /// Gets or sets the array of results for this section per failure mechanism.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.CombinedSectionFailureMechanismResult)]
        public SerializableCombinedFailureMechanismSectionAssemblyResult[] FailureMechanismResults { get; set; }
    }
}