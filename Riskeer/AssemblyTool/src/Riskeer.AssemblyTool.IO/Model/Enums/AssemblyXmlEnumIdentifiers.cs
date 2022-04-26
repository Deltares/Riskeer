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

namespace Riskeer.AssemblyTool.IO.Model.Enums
{
    /// <summary>
    /// Class containing definitions for XML enum identifiers for
    /// the serializable assembly model.
    /// </summary>
    public static class AssemblyXmlEnumIdentifiers
    {
        #region SerializableFailureMechanismSectionAssemblyGroup

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionAssemblyGroup.NotDominant"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionAssemblyGroupNotDominant = "NDo";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionAssemblyGroup.III"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionAssemblyGroupIII = "+III";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionAssemblyGroup.II"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionAssemblyGroupII = "+II";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionAssemblyGroup.I"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionAssemblyGroupI = "+I";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionAssemblyGroup.Zero"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionAssemblyGroupZero = "0";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionAssemblyGroup.IMin"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionAssemblyGroupIMin = "-I";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionAssemblyGroup.IIMin"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionAssemblyGroupIIMin = "-II";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionAssemblyGroup.IIIMin"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionAssemblyGroupIIIMin = "-III";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionAssemblyGroup.NotRelevant"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionAssemblyGroupNotRelevant = "NR";

        #endregion

        #region SerializableAssessmentSectionAssemblyGroup

        /// <summary>
        /// Identifier for <see cref="SerializableAssessmentSectionAssemblyGroup.APlus"/>.
        /// </summary>
        public const string SerializableAssessmentSectionAssemblyGroupAPlus = "A+";

        /// <summary>
        /// Identifier for <see cref="SerializableAssessmentSectionAssemblyGroup.A"/>.
        /// </summary>
        public const string SerializableAssessmentSectionAssemblyGroupA = "A";

        /// <summary>
        /// Identifier for <see cref="SerializableAssessmentSectionAssemblyGroup.B"/>.
        /// </summary>
        public const string SerializableAssessmentSectionAssemblyGroupB = "B";

        /// <summary>
        /// Identifier for <see cref="SerializableAssessmentSectionAssemblyGroup.C"/>.
        /// </summary>
        public const string SerializableAssessmentSectionAssemblyGroupC = "C";

        /// <summary>
        /// Identifier for <see cref="SerializableAssessmentSectionAssemblyGroup.D"/>.
        /// </summary>
        public const string SerializableAssessmentSectionAssemblyGroupD = "D";

        #endregion

        #region SerializableAssemblyMethod

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI0A1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI0A1 = "BOI-0A-1";
        
        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI0A2"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI0A2 = "BOI-0A-2";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI0B1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI0B1 = "BOI-0B-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI0C1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI0C1 = "BOI-0C-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI0C2"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI0C2 = "BOI-0C-2";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI1A1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI1A1 = "BOI-1A-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI1A2"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI1A2 = "BOI-1A-2";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI2A1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI2A1 = "BOI-2A-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI2B1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI2B1 = "BOI-2B-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI3A1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI3A1 = "BOI-3A-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI3B1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI3B1 = "BOI-3B-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.BOI3C1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodBOI3C1 = "BOI-3C-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.Manual"/>.
        /// </summary>
        public const string SerializableAssemblyMethodManual = "HANDMTG";

        #endregion

        #region SerializableFailureMechanismType

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.Generic"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeGeneric = "GENRK";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.Specific"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeSpecific = "SPECFK";

        #endregion

        #region SerializableFailureMechanismSectionType

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionType.FailureMechanism"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionTypeFailureMechanism = "FAALMVK";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionType.Combined"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionTypeCombined = "DEELVK";

        #endregion
    }
}