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
    /// Serializable enum defining the failure mechanism types.
    /// </summary>
    public enum SerializableFailureMechanismType
    {
        /// <summary>
        /// Represents the failure mechanism macro stability inwards.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTBI)]
        STBI = 1,

        /// <summary>
        /// Represents the failure mechanism piping.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTPH)]
        STPH = 2,

        /// <summary>
        /// Represents the failure mechanism microstability.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTMI)]
        STMI = 3,

        /// <summary>
        /// Represents the failure mechanism wave impact asphalt cover.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeAGK)]
        AGK = 4,

        /// <summary>
        /// Represents the failure mechanism water pressure asphalt cover.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeAWO)]
        AWO = 5,

        /// <summary>
        /// Represents the failure mechanism grass cover erosion outwards.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGEBU)]
        GEBU = 6,

        /// <summary>
        /// Represents the failure mechanism grass cover slipoff outwards.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGABU)]
        GABU = 7,

        /// <summary>
        /// Represents the failure mechanism grass cover erosion inwards.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGEKB)]
        GEKB = 8,

        /// <summary>
        /// Represents the failure mechanism grass cover slipoff inwards.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGABI)]
        GABI = 9,

        /// <summary>
        /// Represents the failure mechanism stability stone cover.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeZST)]
        ZST = 10,

        /// <summary>
        /// Represents the failure mechanism dune erosion.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeDA)]
        DA = 11,

        /// <summary>
        /// Represents the failure mechanism height structures.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeHTKW)]
        HTKW = 12,

        /// <summary>
        /// Represents the failure mechanism closing structures.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeBSKW)]
        BSKW = 13,

        /// <summary>
        /// Represents the failure mechanism piping structures.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypePKW)]
        PKW = 14,

        /// <summary>
        /// Represents the failure mechanism stability point structures.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTKWp)]
        STKWp = 15
    }
}