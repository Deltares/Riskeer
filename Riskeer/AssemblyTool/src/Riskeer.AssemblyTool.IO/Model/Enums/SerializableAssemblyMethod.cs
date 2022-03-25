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
    /// Serializable enum defining the various types of assembly methods.
    /// </summary>
    public enum SerializableAssemblyMethod
    {
        /// <summary>
        /// Represents the assembly method WBI-0A-2.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0A2)]
        WBI0A2 = 1,

        /// <summary>
        /// Represents the assembly method WBI-1B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI1B1)]
        WBI1B1 = 2,

        /// <summary>
        /// Represents the assembly method WBI-2B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI2B1)]
        WBI2B1 = 3,

        /// <summary>
        /// Represents the assembly method WBI-3A-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI3A1)]
        WBI3A1 = 4,

        /// <summary>
        /// Represents the assembly method WBI-3B-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI3B1)]
        WBI3B1 = 5,

        /// <summary>
        /// Represents the assembly method WBI-3C-1.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI3C1)]
        WBI3C1 = 6,

        /// <summary>
        /// Represents a manual assembly.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodManual)]
        Manual = 7
    }
}