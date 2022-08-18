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

using System.Xml.Serialization;

namespace Riskeer.AssemblyTool.IO.ModelOld.Enums
{
    /// <summary>
    /// Serializable enum defining the assembly groups for an assessment section.
    /// </summary>
    public enum SerializableAssessmentSectionAssemblyGroup
    {
        /// <summary>
        /// Represents the assembly group A+.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupAPlus)]
        APlus = 1,

        /// <summary>
        /// Represents the assembly group A.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupA)]
        A = 2,

        /// <summary>
        /// Represents the assembly group B.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupB)]
        B = 3,

        /// <summary>
        /// Represents the assembly group C.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupC)]
        C = 4,

        /// <summary>
        /// Represents the assembly group D.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupD)]
        D = 5
    }
}