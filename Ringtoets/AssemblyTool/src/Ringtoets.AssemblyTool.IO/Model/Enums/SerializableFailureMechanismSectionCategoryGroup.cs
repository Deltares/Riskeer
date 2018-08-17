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
    /// Serializable enum defining the assembly categories for a failure mechanism section.
    /// </summary>
    public enum SerializableFailureMechanismSectionCategoryGroup
    {
        /// <summary>
        /// Represents the assembly category NVT (Not applicable) for a failure mechanism section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.NotApplicable)]
        NotApplicable = 1,

        /// <summary>
        /// Represents the assembly category Iv for a failure mechanism section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupIv)]
        Iv = 2,

        /// <summary>
        /// Represents the assembly category IIv for a failure mechanism section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupIIv)]
        IIv = 3,

        /// <summary>
        /// Represents the assembly category IIIv for a failure mechanism section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupIIIv)]
        IIIv = 4,

        /// <summary>
        /// Represents the assembly category IVv for a failure mechanism section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupIVv)]
        IVv = 5,

        /// <summary>
        /// Represents the assembly category Vv for a failure mechanism section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupVv)]
        Vv = 6,

        /// <summary>
        /// Represents the assembly category VIv for a failure mechanism section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupVIv)]
        VIv = 7,

        /// <summary>
        /// Represents the assembly category VIIv for a failure mechanism section.
        /// </summary>
        [XmlEnum(AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupVIIv)]
        VIIv = 8
    }
}