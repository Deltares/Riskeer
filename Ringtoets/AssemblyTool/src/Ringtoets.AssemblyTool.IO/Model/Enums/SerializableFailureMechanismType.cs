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
    /// Enum defining the serializable failure mechanism types.
    /// </summary>
    public enum SerializableFailureMechanismType
    {
        /// <summary>
        /// Represents the failure mechanism macro stability inwards.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTBI)]
        STBI = 1,

        /// <summary>
        /// Represents the failure mechanism macro stability outwards.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTBU)]
        STBU = 2,

        /// <summary>
        /// Represents the failure mechanism piping.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTPH)]
        STPH = 3,

        /// <summary>
        /// Represents the failure mechanism microstability.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTMI)]
        STMI = 4,

        /// <summary>
        /// Represents the failure mechanism wave impact asphalt cover.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeAGK)]
        AGK = 5,

        /// <summary>
        /// Represents the failure mechanism water pressure asphalt cover.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeAWO)]
        AWO = 6,

        /// <summary>
        /// Represents the failure mechanism grass cover erosion outwards.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGEBU)]
        GEBU = 7,

        /// <summary>
        /// Represents the failure mechanism grass cover slipoff outwards.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGABU)]
        GABU = 8,

        /// <summary>
        /// Represents the failure mechanism grass cover erosion inwards.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGEKB)]
        GEKB = 9,

        /// <summary>
        /// Represents the failure mechanism grass cover slipoff inwards.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGABI)]
        GABI = 10,

        /// <summary>
        /// Represents the failure mechanism stability stone cover.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeZST)]
        ZST = 11,

        /// <summary>
        /// Represents the failure mechanism dune erosion.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeDA)]
        DA = 12,

        /// <summary>
        /// Represents the failure mechanism height structurews.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeHTKW)]
        HTKW = 13,

        /// <summary>
        /// Represents the failure mechanism closing structures.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeBSKW)]
        BSKW = 14,

        /// <summary>
        /// Represents the failure mechanism piping structures.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypePKW)]
        PKW = 15,

        /// <summary>
        /// Represents the failure mechanism stability point structures.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTKWp)]
        STKWp = 16,

        /// <summary>
        /// Represents the failure mechanism strength stability lengthwise construction.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTKWl)]
        STKWl = 17,

        /// <summary>
        /// Represents the failure mechanism technical innovation.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeINN)]
        INN = 18
    }
}