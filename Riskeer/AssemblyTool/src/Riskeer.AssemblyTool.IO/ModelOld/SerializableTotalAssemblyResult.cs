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
using System.Xml.Serialization;
using Riskeer.AssemblyTool.IO.ModelOld.Enums;
using Riskeer.AssemblyTool.IO.ModelOld.Helpers;
using Riskeer.AssemblyTool.IO.Properties;

namespace Riskeer.AssemblyTool.IO.ModelOld
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
        public SerializableTotalAssemblyResult()
        {
            Status = Resources.FullAssembly;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableTotalAssemblyResult"/>.
        /// </summary>
        /// <param name="id">The unique assembly ID.</param>
        /// <param name="assessmentProcess">The assessment process this result belongs to.</param>
        /// <param name="probabilityAssemblyMethod">The method used to assemble the probability of this result.</param>
        /// <param name="assemblyGroupAssemblyMethod">The method used to assemble the assembly group of this result.</param>
        /// <param name="assemblyGroup">The group of this assembly result.</param>
        /// <param name="probability">The probability of this assembly result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentProcess"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public SerializableTotalAssemblyResult(string id,
                                               SerializableAssessmentProcess assessmentProcess,
                                               SerializableAssemblyMethod probabilityAssemblyMethod,
                                               SerializableAssemblyMethod assemblyGroupAssemblyMethod,
                                               SerializableAssessmentSectionAssemblyGroup assemblyGroup,
                                               double probability) : this()
        {
            SerializableIdValidator.ThrowIfInvalid(id);

            if (assessmentProcess == null)
            {
                throw new ArgumentNullException(nameof(assessmentProcess));
            }

            Id = id;
            AssessmentProcessId = assessmentProcess.Id;
            ProbabilityAssemblyMethod = probabilityAssemblyMethod;
            AssemblyGroupAssemblyMethod = assemblyGroupAssemblyMethod;
            AssemblyGroup = assemblyGroup;
            Probability = probability;
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
        /// Gets or sets the method used to assemble the probability of this result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.ProbabilityAssemblyMethod)]
        public SerializableAssemblyMethod ProbabilityAssemblyMethod { get; set; }

        /// <summary>
        /// Gets or sets the method used to assemble the assembly group of this result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.TotalAssemblyResultAssemblyMethod)]
        public SerializableAssemblyMethod AssemblyGroupAssemblyMethod { get; set; }

        /// <summary>
        /// Gets or sets the group of this assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssessmentSectionAssemblyGroup)]
        public SerializableAssessmentSectionAssemblyGroup AssemblyGroup { get; set; }

        /// <summary>
        /// Gets or sets the probability of this assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Probability)]
        public double Probability { get; set; }

        /// <summary>
        /// Gets or sets the status of this assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Status)]
        public string Status { get; set; }
    }
}