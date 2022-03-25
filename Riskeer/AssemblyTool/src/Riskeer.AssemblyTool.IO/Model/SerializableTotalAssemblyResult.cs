﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Helpers;

namespace Riskeer.AssemblyTool.IO.Model
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
        /// <param name="id">The unique assembly ID.</param>
        /// <param name="assessmentProcess">The assessment process this result belongs to.</param>
        /// <param name="assessmentSectionAssemblyResult">The assembly result for the assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter except <paramref name="id"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public SerializableTotalAssemblyResult(string id,
                                               SerializableAssessmentProcess assessmentProcess,
                                               SerializableAssessmentSectionAssemblyResult assessmentSectionAssemblyResult) : this()
        {
            SerializableIdValidator.ThrowIfInvalid(id);

            if (assessmentProcess == null)
            {
                throw new ArgumentNullException(nameof(assessmentProcess));
            }

            if (assessmentSectionAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(assessmentSectionAssemblyResult));
            }

            Id = id;
            AssessmentProcessId = assessmentProcess.Id;
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
    }
}