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

using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model.Enums;

namespace Ringtoets.AssemblyTool.IO.Test.Model.Enums
{
    [TestFixture]
    public class AssemblyXmlEnumIdentifiersTest
    {
        [Test]
        public void AssemblyXmlEnumIdentifiers_ExpectedValues()
        {
            Assert.AreEqual("NVT", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupNotApplicable);
            Assert.AreEqual("I-traject", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupIt);
            Assert.AreEqual("II-traject", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupIIt);
            Assert.AreEqual("III-traject", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupIIIt);
            Assert.AreEqual("IV-traject", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupIVt);
            Assert.AreEqual("V-traject", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupVt);
            Assert.AreEqual("VI-traject", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupVIt);
            Assert.AreEqual("VII-traject", AssemblyXmlEnumIdentifiers.SerializableFailureMechanismCategoryGroupVIIt);

            Assert.AreEqual("WBI-0E-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0E1);
            Assert.AreEqual("WBI-0E-3", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0E3);
            Assert.AreEqual("WBI-0G-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0G1);
            Assert.AreEqual("WBI-0G-3", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0G3);
            Assert.AreEqual("WBI-0G-4", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0G4);
            Assert.AreEqual("WBI-0G-5", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0G5);
            Assert.AreEqual("WBI-0G-6", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0G6);
            Assert.AreEqual("WBI-0T-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0T1);
            Assert.AreEqual("WBI-0T-3", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0T3);
            Assert.AreEqual("WBI-0T-4", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0T4);
            Assert.AreEqual("WBI-0T-5", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0T5);
            Assert.AreEqual("WBI-0T-6", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0T6);
            Assert.AreEqual("WBI-0T-7", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0T7);
            Assert.AreEqual("WBI-0A-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI0A1);
            Assert.AreEqual("WBI-1A-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI1A1);
            Assert.AreEqual("WBI-1B-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI1B1);
            Assert.AreEqual("WBI-2A-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI2A1);
            Assert.AreEqual("WBI-2B-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI2B1);
            Assert.AreEqual("WBI-2C-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI2C1);
            Assert.AreEqual("WBI-3A-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI3A1);
            Assert.AreEqual("WBI-3B-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI3B1);
            Assert.AreEqual("WBI-3C-1", AssemblyXmlEnumIdentifiers.AssemblyMethodWBI3C1);
        }
    }
}