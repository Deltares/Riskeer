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
            Assert.AreEqual("NDo", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupNotDominant);
            Assert.AreEqual("+III", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIII);
            Assert.AreEqual("+II", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupII);
            Assert.AreEqual("+I", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupI);
            Assert.AreEqual("0", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupZero);
            Assert.AreEqual("-I", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIMin);
            Assert.AreEqual("-II", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIIMin);
            Assert.AreEqual("-III", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupIIIMin);

            Assert.AreEqual("A+", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupAPlus);
            Assert.AreEqual("A", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupA);
            Assert.AreEqual("B", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupB);
            Assert.AreEqual("C", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupC);
            Assert.AreEqual("D", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupD);

            Assert.AreEqual("WBI-0A-2", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI0A2);
            Assert.AreEqual("WBI-1B-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI1B1);
            Assert.AreEqual("WBI-2B-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodWBI2B1);
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

            Assert.AreEqual("TOETSSSTE", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionTypeFailureMechanism);
            Assert.AreEqual("GECBNETSSTE", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionTypeCombined);
        }
    }
}