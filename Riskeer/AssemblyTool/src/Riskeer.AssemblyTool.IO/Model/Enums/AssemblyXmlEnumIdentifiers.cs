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
        /// Identifier for <see cref="SerializableAssemblyMethod.WBI0A2"/>.
        /// </summary>
        public const string SerializableAssemblyMethodWBI0A2 = "WBI-0A-2";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.WBI1B1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodWBI1B1 = "WBI-1B-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.WBI2C1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodWBI2C1 = "WBI-2C-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.WBI3A1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodWBI3A1 = "WBI-3A-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.WBI3B1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodWBI3B1 = "WBI-3B-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.WBI3C1"/>.
        /// </summary>
        public const string SerializableAssemblyMethodWBI3C1 = "WBI-3C-1";

        /// <summary>
        /// Identifier for <see cref="SerializableAssemblyMethod.Manual"/>.
        /// </summary>
        public const string SerializableAssemblyMethodManual = "Handmatig";

        #endregion

        #region SerializableFailureMechanismType

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.STBI"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeSTBI = "STBI";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.STPH"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeSTPH = "STPH";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.STMI"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeSTMI = "STMI";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.AGK"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeAGK = "AGK";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.AWO"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeAWO = "AWO";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.GEBU"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeGEBU = "GEBU";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.GABU"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeGABU = "GABU";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.GEKB"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeGEKB = "GEKB";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.GABI"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeGABI = "GABI";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.ZST"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeZST = "ZST";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.DA"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeDA = "DA";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.HTKW"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeHTKW = "HTKW";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.BSKW"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeBSKW = "BSKW";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.PKW"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypePKW = "PKW";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismType.STKWp"/>.
        /// </summary>
        public const string SerializableFailureMechanismTypeSTKWp = "STKWp";

        #endregion

        #region SerializableAssessmentType

        /// <summary>
        /// Identifier for <see cref="SerializableAssessmentType.SimpleAssessment"/>.
        /// </summary>
        public const string SerializableAssessmentTypeSimpleAssessment = "EENVDGETS";

        /// <summary>
        /// Identifier for <see cref="SerializableAssessmentType.DetailedAssessment"/>.
        /// </summary>
        public const string SerializableAssessmentTypeDetailedAssessment = "GEDTETS";

        /// <summary>
        /// Identifier for <see cref="SerializableAssessmentType.TailorMadeAssessment"/>.
        /// </summary>
        public const string SerializableAssessmentTypeTailorMadeAssessment = "TOETSOPMT";

        /// <summary>
        /// Identifier for <see cref="SerializableAssessmentType.CombinedAssessment"/>.
        /// </summary>
        public const string SerializableAssessmentTypeCombinedAssessment = "GECBNTR";

        /// <summary>
        /// Identifier for <see cref="SerializableAssessmentType.CombinedSectionAssessment"/>.
        /// </summary>
        public const string SerializableAssessmentTypeCombinedSectionAssessment = "GECBNTRDV";

        #endregion

        #region SerializableFailureMechanismSectionType

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionType.FailureMechanism"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionTypeFailureMechanism = "TOETSSSTE";

        /// <summary>
        /// Identifier for <see cref="SerializableFailureMechanismSectionType.Combined"/>.
        /// </summary>
        public const string SerializableFailureMechanismSectionTypeCombined = "GECBNETSSTE";

        #endregion
    }
}