﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
            Assert.AreEqual("NR", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionAssemblyGroupNotRelevant);

            Assert.AreEqual("A+", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupAPlus);
            Assert.AreEqual("A", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupA);
            Assert.AreEqual("B", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupB);
            Assert.AreEqual("C", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupC);
            Assert.AreEqual("D", AssemblyXmlEnumIdentifiers.SerializableAssessmentSectionAssemblyGroupD);

            Assert.AreEqual("BOI-0A-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI0A1);
            Assert.AreEqual("BOI-0A-2", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI0A2);
            Assert.AreEqual("BOI-0B-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI0B1);
            Assert.AreEqual("BOI-0C-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI0C1);
            Assert.AreEqual("BOI-0C-2", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI0C2);
            Assert.AreEqual("BOI-1A-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI1A1);
            Assert.AreEqual("BOI-1A-2", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI1A2);
            Assert.AreEqual("BOI-1B-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI1B1);
            Assert.AreEqual("BOI-2A-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI2A1);
            Assert.AreEqual("BOI-2A-2", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI2A2);
            Assert.AreEqual("BOI-2B-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI2B1);
            Assert.AreEqual("BOI-3A-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI3A1);
            Assert.AreEqual("BOI-3B-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI3B1);
            Assert.AreEqual("BOI-3C-1", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodBOI3C1);
            Assert.AreEqual("HANDMTG", AssemblyXmlEnumIdentifiers.SerializableAssemblyMethodManual);

            Assert.AreEqual("GENRK", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeGeneric);
            Assert.AreEqual("SPECFK", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismTypeSpecific);

            Assert.AreEqual("FAALMVK", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionTypeFailureMechanism);
            Assert.AreEqual("DEELVK", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismSectionTypeCombined);
        }
    }
}