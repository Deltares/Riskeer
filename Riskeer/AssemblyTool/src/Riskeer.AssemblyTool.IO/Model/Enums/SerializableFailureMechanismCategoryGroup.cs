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
    /// Serializable enum defining the assembly categories for a failure mechanism.
    /// </summary>
    public enum SerializableFailureMechanismCategoryGroup
    {
        /// <summary>
        /// Represents the assembly category NVT (Not applicable) for a failure mechanism.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.NotApplicable)]
        NotApplicable = 1,

        /// <summary>
        /// Represents the assembly category It for a failure mechanism.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupIt)]
        It = 2,

        /// <summary>
        /// Represents the assembly category IIt for a failure mechanism.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupIIt)]
        IIt = 3,

        /// <summary>
        /// Represents the assembly category IIIt for a failure mechanism.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupIIIt)]
        IIIt = 4,

        /// <summary>
        /// Represents the assembly category IVt for a failure mechanism.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupIVt)]
        IVt = 5,

        /// <summary>
        /// Represents the assembly category Vt for a failure mechanism.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupVt)]
        Vt = 6,

        /// <summary>
        /// Represents the assembly category VIt for a failure mechanism.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupVIt)]
        VIt = 7,

        /// <summary>
        /// Represents the assembly category VIIt for a failure mechanism.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupVIIt)]
        VIIt = 8
    }
}