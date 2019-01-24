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

using System.Xml.Serialization;

namespace Riskeer.AssemblyTool.IO.Model.Enums
{
    /// <summary>
    /// Serializable enum defining the assessment types.
    /// </summary>
    public enum SerializableAssessmentType
    {
        /// <summary>
        /// Represents the simple assessment type.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentTypeSimpleAssessment)]
        SimpleAssessment = 1,

        /// <summary>
        /// Represents the detailed assessment type.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentTypeDetailedAssessment)]
        DetailedAssessment = 2,

        /// <summary>
        /// Represents the tailor made assessment type.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentTypeTailorMadeAssessment)]
        TailorMadeAssessment = 3,

        /// <summary>
        /// Represents the combined assessment type.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentTypeCombinedAssessment)]
        CombinedAssessment = 4,

        /// <summary>
        /// Represents the cmbined section assessment type.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentTypeCombinedSectionAssessment)]
        CombinedSectionAssessment = 5
    }
}