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

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model.DataTypes
{
    [TestFixture]
    public class SerializableFailureMechanismSubSectionAssemblyResultTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var assemblyResult = new SerializableFailureMechanismSubSectionAssemblyResult();

            // Assert
            Assert.AreEqual((SerializableAssemblyMethod) 0, assemblyResult.AssemblyMethod);
            Assert.AreEqual((SerializableFailureMechanismSectionAssemblyGroup) 0, assemblyResult.AssemblyGroup);
            Assert.AreEqual("VOLLDG", assemblyResult.Status);

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSubSectionAssemblyResult>(
                nameof(SerializableFailureMechanismSubSectionAssemblyResult.AssemblyMethod), "assemblagemethode");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSubSectionAssemblyResult>(
                nameof(SerializableFailureMechanismSubSectionAssemblyResult.AssemblyGroup), "duidingsklasse");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSubSectionAssemblyResult>(
                nameof(SerializableFailureMechanismSubSectionAssemblyResult.Status), "status");
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assemblyMethod = random.NextEnumValue<SerializableAssemblyMethod>();
            var assemblyGroup = random.NextEnumValue<SerializableFailureMechanismSectionAssemblyGroup>();

            // Call
            var assemblyResult = new SerializableFailureMechanismSubSectionAssemblyResult(assemblyMethod, assemblyGroup);

            // Assert
            Assert.AreEqual(assemblyGroup, assemblyResult.AssemblyGroup);
            Assert.AreEqual(assemblyMethod, assemblyResult.AssemblyMethod);
            Assert.AreEqual("VOLLDG", assemblyResult.Status);
        }
    }
}