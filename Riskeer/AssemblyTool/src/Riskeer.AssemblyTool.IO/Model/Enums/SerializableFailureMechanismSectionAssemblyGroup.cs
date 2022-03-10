// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.AssemblyTool.IO.Model.Enums
{
    /// <summary>
    /// Serializable enum defining the assembly groups for a failure mechanism section.
    /// </summary>
    public enum SerializableFailureMechanismSectionAssemblyGroup
    {
        /// <summary>
        /// Represents group Not Dominant.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupNotDominant)]
        NotDominant = 1,

        /// <summary>
        /// Represents group +III.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIII)]
        III = 2,

        /// <summary>
        /// Represents group +II.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupII)]
        II = 3,

        /// <summary>
        /// Represents group +I.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupI)]
        I = 4,

        /// <summary>
        /// Represents group 0.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupZero)]
        Zero = 5,

        /// <summary>
        /// Represents group -I.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIMin)]
        IMin = 6,

        /// <summary>
        /// Represents group -II.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIIMin)]
        IIMin = 7,

        /// <summary>
        /// Represents group -III.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIIIMin)]
        IIIMin = 8
    }
}