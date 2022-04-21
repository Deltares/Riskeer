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
        /// Represents the assembly method BOI-0A-2.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI0A2)]
        BOI0A2 = 1,

        /// <summary>
        /// Represents the assembly method BOI-1B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI1B1)]
        BOI1B1 = 2,

        /// <summary>
        /// Represents the assembly method BOI-2B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI2B1)]
        BOI2B1 = 3,

        /// <summary>
        /// Represents the assembly method BOI-3A-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI3A1)]
        BOI3A1 = 4,

        /// <summary>
        /// Represents the assembly method BOI-3B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI3B1)]
        BOI3B1 = 5,

        /// <summary>
        /// Represents the assembly method BOI-3C-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI3C1)]
        BOI3C1 = 6,

        /// <summary>
        /// Represents a manual assembly.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodManual)]
        Manual = 7
    }
}