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

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model.DataTypes
{
    [TestFixture]
    public class SerializableCombinedFailureMechanismSectionAssemblyResultTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var assemblyResult = new SerializableCombinedFailureMechanismSectionAssemblyResult();

            // Assert
            Assert.AreEqual((SerializableAssemblyMethod) 0, assemblyResult.AssemblyMethod);
            Assert.AreEqual((SerializableFailureMechanismType) 0, assemblyResult.FailureMechanismType);
            Assert.IsNull( assemblyResult.GenericFailureMechanismCode);
            Assert.AreEqual((SerializableFailureMechanismSectionAssemblyGroup) 0, assemblyResult.AssemblyGroup);
            Assert.AreEqual("VOLLDG", assemblyResult.Status);

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableCombinedFailureMechanismSectionAssemblyResult>(
                nameof(SerializableCombinedFailureMechanismSectionAssemblyResult.AssemblyMethod), "assemblagemethode");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableCombinedFailureMechanismSectionAssemblyResult>(
                nameof(SerializableCombinedFailureMechanismSectionAssemblyResult.AssemblyGroup), "duidingsklasse");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableCombinedFailureMechanismSectionAssemblyResult>(
                nameof(SerializableCombinedFailureMechanismSectionAssemblyResult.GenericFailureMechanismCode), "generiekFaalmechanisme");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableCombinedFailureMechanismSectionAssemblyResult>(
                nameof(SerializableCombinedFailureMechanismSectionAssemblyResult.FailureMechanismType), "typeFaalmechanisme");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableCombinedFailureMechanismSectionAssemblyResult>(
                nameof(SerializableCombinedFailureMechanismSectionAssemblyResult.Status), "status");
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var assemblyMethod = random.NextEnumValue<SerializableAssemblyMethod>();
            var failureMechanismType = random.NextEnumValue<SerializableFailureMechanismType>();
            const string code = "code";
            var assemblyGroup = random.NextEnumValue<SerializableFailureMechanismSectionAssemblyGroup>();

            // Call
            var assemblyResult = new SerializableCombinedFailureMechanismSectionAssemblyResult(
                assemblyMethod, failureMechanismType, code, assemblyGroup);

            // Assert
            Assert.AreEqual(assemblyMethod, assemblyResult.AssemblyMethod);
            Assert.AreEqual(failureMechanismType, assemblyResult.FailureMechanismType);
            Assert.AreEqual(code, assemblyResult.GenericFailureMechanismCode);
            Assert.AreEqual(assemblyGroup, assemblyResult.AssemblyGroup);
            Assert.AreEqual("VOLLDG", assemblyResult.Status);
        }
    }
}