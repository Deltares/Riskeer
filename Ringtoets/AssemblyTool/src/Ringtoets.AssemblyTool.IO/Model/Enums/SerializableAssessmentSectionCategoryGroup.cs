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

using System.Xml.Serialization;

namespace Ringtoets.AssemblyTool.IO.Model.Enums
{
    /// <summary>
    /// Enum defining the serializable assembly categories for an assessment section.
    /// </summary>
    public enum SerializableAssessmentSectionCategoryGroup
    {
        /// <summary>
        /// Represents the assembly category A+ for an assessment section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionCategoryGroupAPlus)]
        APlus = 1,

        /// <summary>
        /// Represents the assembly category A for an assessment section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionCategoryGroupA)]
        A = 2,

        /// <summary>
        /// Represents the assembly category IIt for an assessment section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionCategoryGroupB)]
        B = 3,

        /// <summary>
        /// Represents the assembly category IIIt for an assessment section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionCategoryGroupC)]
        C = 4,

        /// <summary>
        /// Represents the assembly category IVt for an assessment section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionCategoryGroupD)]
        D = 5
    }
}