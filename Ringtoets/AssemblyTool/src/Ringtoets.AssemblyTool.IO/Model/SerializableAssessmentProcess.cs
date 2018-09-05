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
using Ringtoets.AssemblyTool.IO.Model.Helpers;

namespace Ringtoets.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class describing a serializable assessment process.
    /// </summary>
    [XmlType(AssemblyXmlIdentifiers.AssessmentProcess)]
    public class SerializableAssessmentProcess : SerializableFeatureMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableAssessmentProcess"/>.
        /// </summary>
        public SerializableAssessmentProcess()
        {
            StartYear = 2017;
            EndYear = 2023;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableAssessmentProcess"/>.
        /// </summary>
        /// <param name="id">The unique ID of the assessment process.</param>
        /// <param name="assessmentSection">The assessment section this process belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public SerializableAssessmentProcess(string id,
                                             SerializableAssessmentSection assessmentSection) : this()
        {
            if (!SerializableIdValidator.Validate(id))
            {
                throw new ArgumentException($@"'{nameof(id)}' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.");
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Id = id;
            AssessmentSectionId = assessmentSection.Id;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.AssessmentProcessId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the parent assessment section.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.AssessmentSectionIdRef)]
        public string AssessmentSectionId { get; set; }

        /// <summary>
        /// Gets or sets the starting year.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.StartYear)]
        public int StartYear { get; set; }

        /// <summary>
        /// Gets or sets the end year.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.EndYear)]
        public int EndYear { get; set; }
    }
}