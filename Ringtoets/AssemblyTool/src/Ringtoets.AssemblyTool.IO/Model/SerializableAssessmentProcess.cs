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
        public SerializableAssessmentProcess() {}

        /// <summary>
        /// Creates a new instance of <see cref="SerializableAssessmentProcess"/>.
        /// </summary>
        /// <param name="id">The unique ID of the assessment process.</param>
        /// <param name="assessmentSection">The assessment section this process belongs to.</param>
        /// <param name="startYear">The starting year of the assessment process.</param>
        /// <param name="endYear">The end year of the assessment process.</param>
        /// <param name="description">The description of the assessment process.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/>
        /// or <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public SerializableAssessmentProcess(string id,
                                             SerializableAssessmentSection assessmentSection,
                                             int startYear,
                                             int endYear,
                                             string description = null)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Id = id;
            AssessmentSectionId = assessmentSection.Id;
            StartYear = startYear;
            EndYear = endYear;
            Description = description;
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

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Description)]
        public string Description { get; set; }
    }
}