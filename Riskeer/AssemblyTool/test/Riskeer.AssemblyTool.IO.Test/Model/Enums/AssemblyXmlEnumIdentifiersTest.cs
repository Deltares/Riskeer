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

using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Test.Model.Enums
{
    [TestFixture]
    public class AssemblyXmlEnumIdentifiersTest
    {
        [Test]
        public void AssemblyXmlEnumIdentifiers_ExpectedValues()
        {
            Assert.AreEqual("NVT", AssemblyXmlEnumIdentifiers.NotApplicable);

            Assert.AreEqual("NDo", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupNotDominant);
            Assert.AreEqual("+III", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIII);
            Assert.AreEqual("+II", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupII);
            Assert.AreEqual("+I", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupI);
            Assert.AreEqual("0", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupZero);
            Assert.AreEqual("-I", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIMin);
            Assert.AreEqual("-II", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIIMin);
            Assert.AreEqual("-III", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIIIMin);

            Assert.AreEqual("I-vak", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupIv);
            Assert.AreEqual("II-vak", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupIIv);
            Assert.AreEqual("III-vak", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupIIIv);
            Assert.AreEqual("IV-vak", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupIVv);
            Assert.AreEqual("V-vak", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupVv);
            Assert.AreEqual("VI-vak", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupVIv);
            Assert.AreEqual("VII-vak", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionCategoryGroupVIIv);

            Assert.AreEqual("A+", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupAPlus);
            Assert.AreEqual("A", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupA);
            Assert.AreEqual("B", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupB);
            Assert.AreEqual("C", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupC);
            Assert.AreEqual("D", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupD);

            Assert.AreEqual("WBI-0E-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0E1);
            Assert.AreEqual("WBI-0E-3", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0E3);
            Assert.AreEqual("WBI-0G-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0G1);
            Assert.AreEqual("WBI-0G-3", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0G3);
            Assert.AreEqual("WBI-0G-4", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0G4);
            Assert.AreEqual("WBI-0G-5", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0G5);
            Assert.AreEqual("WBI-0G-6", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0G6);
            Assert.AreEqual("WBI-0T-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T1);
            Assert.AreEqual("WBI-0T-3", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T3);
            Assert.AreEqual("WBI-0T-4", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T4);
            Assert.AreEqual("WBI-0T-5", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T5);
            Assert.AreEqual("WBI-0T-6", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T6);
            Assert.AreEqual("WBI-0T-7", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0T7);
            Assert.AreEqual("WBI-0A-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0A1);
            Assert.AreEqual("WBI-0A-2", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0A2);
            Assert.AreEqual("WBI-1B-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI1B1);
            Assert.AreEqual("WBI-2C-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI2C1);
            Assert.AreEqual("WBI-3A-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI3A1);
            Assert.AreEqual("WBI-3B-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI3B1);
            Assert.AreEqual("WBI-3C-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI3C1);
            Assert.AreEqual("Handmatig", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodManual);

            Assert.AreEqual("STBI", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTBI);
            Assert.AreEqual("STPH", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTPH);
            Assert.AreEqual("STMI", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTMI);
            Assert.AreEqual("AGK", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeAGK);
            Assert.AreEqual("AWO", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeAWO);
            Assert.AreEqual("GEBU", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGEBU);
            Assert.AreEqual("GABU", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGABU);
            Assert.AreEqual("GEKB", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGEKB);
            Assert.AreEqual("GABI", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGABI);
            Assert.AreEqual("ZST", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeZST);
            Assert.AreEqual("DA", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeDA);
            Assert.AreEqual("HTKW", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeHTKW);
            Assert.AreEqual("BSKW", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeBSKW);
            Assert.AreEqual("PKW", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypePKW);
            Assert.AreEqual("STKWp", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSTKWp);

            Assert.AreEqual("EENVDGETS", AssemblyXmlEnumIdentifiers.SerializableAssessmentTypeSimpleAssessment);
            Assert.AreEqual("GEDTETS", AssemblyXmlEnumIdentifiers.SerializableAssessmentTypeDetailedAssessment);
            Assert.AreEqual("TOETSOPMT", AssemblyXmlEnumIdentifiers.SerializableAssessmentTypeTailorMadeAssessment);
            Assert.AreEqual("GECBNTR", AssemblyXmlEnumIdentifiers.SerializableAssessmentTypeCombinedAssessment);
            Assert.AreEqual("GECBNTRDV", AssemblyXmlEnumIdentifiers.SerializableAssessmentTypeCombinedSectionAssessment);

            Assert.AreEqual("TOETSSSTE", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionTypeFailureMechanism);
            Assert.AreEqual("GECBNETSSTE", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionTypeCombined);
        }
    }
}