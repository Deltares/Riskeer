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
using System.Xml.Serialization;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Helpers;

namespace Ringtoets.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class describing a serializable total assembly result.
    /// </summary>
    [XmlType(AssemblyXmlIdentifiers.TotalAssemblyResult)]
    public class SerializableTotalAssemblyResult : SerializableFeatureMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableTotalAssemblyResult"/>.
        /// </summary>
        public SerializableTotalAssemblyResult() {}

        /// <summary>
        /// Creates a new instance of <see cref="SerializableTotalAssemblyResult"/>.
        /// </summary>
        /// <param name="id">The unique ID of the assembly result.</param>
        /// <param name="assessmentProcess">The assessment process this result belongs to.</param>
        /// <param name="assemblyResultWithoutProbability">The assembly result for failure mechanisms with a probability.</param>
        /// <param name="assemblyResultWithProbability">The assembly result for failure mechanisms without a probability.</param>
        /// <param name="assessmentSectionAssemblyResult">The assembly result for the assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter except <paramref name="id"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public SerializableTotalAssemblyResult(string id,
                                               SerializableAssessmentProcess assessmentProcess,
                                               SerializableFailureMechanismAssemblyResult assemblyResultWithoutProbability,
                                               SerializableFailureMechanismAssemblyResult assemblyResultWithProbability,
                                               SerializableAssessmentSectionAssemblyResult assessmentSectionAssemblyResult) : this()
        {
            if (!SerializableIdValidator.Validate(id))
            {
                throw new ArgumentException($@"'{nameof(id)}' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.");
            }

            if (assessmentProcess == null)
            {
                throw new ArgumentNullException(nameof(assessmentProcess));
            }

            if (assemblyResultWithoutProbability == null)
            {
                throw new ArgumentNullException(nameof(assemblyResultWithoutProbability));
            }

            if (assemblyResultWithProbability == null)
            {
                throw new ArgumentNullException(nameof(assemblyResultWithProbability));
            }

            if (assessmentSectionAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(assessmentSectionAssemblyResult));
            }

            Id = id;
            AssessmentProcessId = assessmentProcess.Id;
            AssemblyResultWithoutProbability = assemblyResultWithoutProbability;
            AssemblyResultWithProbability = assemblyResultWithProbability;
            AssessmentSectionAssemblyResult = assessmentSectionAssemblyResult;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.TotalAssemblyResultId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the parent assessment process ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.AssessmentProcessIdRef)]
        public string AssessmentProcessId { get; set; }

        /// <summary>
        /// Gets or sets the assessment section assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssessmentSectionAssemblyResult)]
        public SerializableAssessmentSectionAssemblyResult AssessmentSectionAssemblyResult { get; set; }

        /// <summary>
        /// Gets or sets the assembly result with probability.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssemblyResultWithProbability)]
        public SerializableFailureMechanismAssemblyResult AssemblyResultWithProbability { get; set; }

        /// <summary>
        /// Gets or sets the assembly result without probability.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssemblyResultWithoutProbability)]
        public SerializableFailureMechanismAssemblyResult AssemblyResultWithoutProbability { get; set; }
    }
}