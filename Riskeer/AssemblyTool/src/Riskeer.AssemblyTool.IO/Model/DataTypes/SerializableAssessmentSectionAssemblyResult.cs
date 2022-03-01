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

using System.Xml.Serialization;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.IO.Properties;

namespace Riskeer.AssemblyTool.IO.Model.DataTypes
{
    /// <summary>
    /// Class describing a serializable assessment section assembly result.
    /// </summary>
    public class SerializableAssessmentSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableAssessmentSectionAssemblyResult"/>.
        /// </summary>
        public SerializableAssessmentSectionAssemblyResult()
        {
            Status = Resources.FullAssembly;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableAssessmentSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="assemblyMethod">The method used to assemble this result.</param>
        /// <param name="group">The group of this assembly result.</param>
        public SerializableAssessmentSectionAssemblyResult(SerializableAssemblyMethod assemblyMethod,
                                                           SerializableAssessmentSectionGroup group) : this()
        {
            Group = group;
            AssemblyMethod = assemblyMethod;
        }

        /// <summary>
        /// Gets or sets the name of the method used to assemble this result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssemblyMethod)]
        public SerializableAssemblyMethod AssemblyMethod { get; set; }

        /// <summary>
        /// Gets or sets the group of this assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssessmentSectionAssemblyGroup)]
        public SerializableAssessmentSectionGroup Group { get; set; }

        /// <summary>
        /// Gets or sets the status of this assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Status)]
        public string Status { get; set; }
    }
}