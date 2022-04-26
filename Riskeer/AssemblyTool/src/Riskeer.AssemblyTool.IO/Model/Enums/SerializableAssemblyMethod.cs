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

namespace Riskeer.AssemblyTool.IO.Model.Enums
{
    /// <summary>
    /// Serializable enum defining the various types of assembly methods.
    /// </summary>
    public enum SerializableAssemblyMethod
    {
        /// <summary>
        /// Represents the assembly method BOI-0A-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI0A1)]
        BOI0A1 = 1,
        
        /// <summary>
        /// Represents the assembly method BOI-0A-2.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI0A2)]
        BOI0A2 = 2,

        /// <summary>
        /// Represents the assembly method BOI-0B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI0B1)]
        BOI0B1 = 3,

        /// <summary>
        /// Represents the assembly method BOI-0C-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI0C1)]
        BOI0C1 = 4,

        /// <summary>
        /// Represents the assembly method BOI-0C-2.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI0C2)]
        BOI0C2 = 5,

        /// <summary>
        /// Represents the assembly method BOI-1A-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI1A1)]
        BOI1A1 = 6,

        /// <summary>
        /// Represents the assembly method BOI-1A-2.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI1A2)]
        BOI1A2 = 7,
        
        /// <summary>
        /// Represents a manual failure mechanism assembly.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodManual)]
        Manual = 8,

        /// <summary>
        /// Represents the assembly method BOI-2A-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI2A1)]
        BOI2A1 = 9,
        
        /// <summary>
        /// Represents the assembly method BOI-2B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI2B1)]
        BOI2B1 = 10,

        /// <summary>
        /// Represents the assembly method BOI-3A-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI3A1)]
        BOI3A1 = 11,

        /// <summary>
        /// Represents the assembly method BOI-3B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI3B1)]
        BOI3B1 = 12,

        /// <summary>
        /// Represents the assembly method BOI-3C-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI3C1)]
        BOI3C1 = 13
    }
}