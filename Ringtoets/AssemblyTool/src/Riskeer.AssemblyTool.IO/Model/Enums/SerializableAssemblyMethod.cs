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
    /// Serializable enum defining the various types of assembly methods.
    /// </summary>
    public enum SerializableAssemblyMethod
    {
        /// <summary>
        /// Represents the assembly method WBI-0E-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0E1)]
        WBI0E1 = 1,

        /// <summary>
        /// Represents the assembly method WBI-0E-3.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0E3)]
        WBI0E3 = 2,

        /// <summary>
        /// Represents the assembly method WBI-0G-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0G1)]
        WBI0G1 = 3,

        /// <summary>
        /// Represents the assembly method WBI-0G-3.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0G3)]
        WBI0G3 = 4,

        /// <summary>
        /// Represents the assembly method WBI-0G-4.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0G4)]
        WBI0G4 = 5,

        /// <summary>
        /// Represents the assembly method WBI-0G-5.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0G5)]
        WBI0G5 = 6,

        /// <summary>
        /// Represents the assembly method WBI-0G-6.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0G6)]
        WBI0G6 = 7,

        /// <summary>
        /// Represents the assembly method WBI-0T-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T1)]
        WBI0T1 = 8,

        /// <summary>
        /// Represents the assembly method WBI-0T-3.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T3)]
        WBI0T3 = 9,

        /// <summary>
        /// Represents the assembly method WBI-0T-4.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T4)]
        WBI0T4 = 10,

        /// <summary>
        /// Represents the assembly method WBI-0T-5.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T5)]
        WBI0T5 = 11,

        /// <summary>
        /// Represents the assembly method WBI-0T-6.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T6)]
        WBI0T6 = 12,

        /// <summary>
        /// Represents the assembly method WBI-0T-7.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T7)]
        WBI0T7 = 13,

        /// <summary>
        /// Represents the assembly method WBI-0A-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0A1)]
        WBI0A1 = 14,

        /// <summary>
        /// Represents the assembly method WBI-1A-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI1A1)]
        WBI1A1 = 15,

        /// <summary>
        /// Represents the assembly method WBI-1B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI1B1)]
        WBI1B1 = 16,

        /// <summary>
        /// Represents the assembly method WBI-2A-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI2A1)]
        WBI2A1 = 17,

        /// <summary>
        /// Represents the assembly method WBI-2B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI2B1)]
        WBI2B1 = 18,

        /// <summary>
        /// Represents the assembly method WBI-2C-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI2C1)]
        WBI2C1 = 19,

        /// <summary>
        /// Represents the assembly method WBI-3A-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI3A1)]
        WBI3A1 = 20,

        /// <summary>
        /// Represents the assembly method WBI-3B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI3B1)]
        WBI3B1 = 21,

        /// <summary>
        /// Represents the assembly method WBI-3C-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI3C1)]
        WBI3C1 = 22
    }
}