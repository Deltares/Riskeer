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
    /// Enum defining the serializable failure mechanism groups.
    /// </summary>
    public enum SerializableFailureMechanismGroup
    {
        /// <summary>
        /// Represents the failure mechanism group 1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismGroup1)]
        Group1 = 1,

        /// <summary>
        /// Represents the failure mechanism group 2.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismGroup2)]
        Group2 = 2,

        /// <summary>
        /// Represents the failure mechanism group 3.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismGroup3)]
        Group3 = 3,

        /// <summary>
        /// Represents the failure mechanism group 4.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismGroup4)]
        Group4 = 4
    }
}