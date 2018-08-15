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
    /// Serializable enum defining the assessment levels.
    /// </summary>
    public enum SerializableAssessmentLevel
    {
        /// <summary>
        /// Represents the simple assessment level.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentLevelSimpleAssessment)]
        SimpleAssessment = 1,

        /// <summary>
        /// Represents the detailed assessment level.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentLevelDetailedAssessment)]
        DetailedAssessment = 2,

        /// <summary>
        /// Represents the tailor made assessment level.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentLevelTailorMadeAssessment)]
        TailorMadeAssessment = 3,

        /// <summary>
        /// Represents the combined assessment level.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentLevelCombinedAssessment)]
        CombinedAssessment = 4,

        /// <summary>
        /// Represents the cmbined section assessment level.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentLevelCombinedSectionAssessment)]
        CombinedSectionAssessment = 5,

        /// <summary>
        /// Represents the combined section per failure mechanism assessment level.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssessmentLevelCombinedSectionFailureMechanismAssessment)]
        CombinedSectionFailureMechanismAssessment = 6,
    }
}