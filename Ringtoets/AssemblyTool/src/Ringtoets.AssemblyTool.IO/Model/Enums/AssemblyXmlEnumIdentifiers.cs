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

namespace Ringtoets.AssemblyTool.IO.Model.Enums
{
    /// <summary>
    /// Class containing definitions for XML enum identifiers for
    /// the serializable assembly model.
    /// </summary>
    public static class AssemblyXmlEnumIdentifiers
    {
        #region SerializableFailureMechanismCategoryGroup

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismCategoryGroup.NotApplicable"/>
        /// </summary>
        public const string SerializableFailureMechanismCategoryGroupNotApplicable = "NVT";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismCategoryGroup.It"/>
        /// </summary>
        public const string SerializableFailureMechanismCategoryGroupIt = "I-traject";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismCategoryGroup.IIt"/>
        /// </summary>
        public const string SerializableFailureMechanismCategoryGroupIIt = "II-traject";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismCategoryGroup.IIIt"/>
        /// </summary>
        public const string SerializableFailureMechanismCategoryGroupIIIt = "III-traject";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismCategoryGroup.IVt"/>
        /// </summary>
        public const string SerializableFailureMechanismCategoryGroupIVt = "IV-traject";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismCategoryGroup.Vt"/>
        /// </summary>
        public const string SerializableFailureMechanismCategoryGroupVt = "V-traject";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismCategoryGroup.VIt"/>
        /// </summary>
        public const string SerializableFailureMechanismCategoryGroupVIt = "VI-traject";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismCategoryGroup.VIIt"/>
        /// </summary>
        public const string SerializableFailureMechanismCategoryGroupVIIt = "VII-traject";

        #endregion
    }
}