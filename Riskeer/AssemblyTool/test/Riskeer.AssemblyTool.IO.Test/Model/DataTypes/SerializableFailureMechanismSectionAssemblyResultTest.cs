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
    public class SerializableFailureMechanismSectionAssemblyResultTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var assemblyResult = new SerializableFailureMechanismSectionAssemblyResult();

            // Assert
            Assert.AreEqual((SerializableAssemblyMethod) 0, assemblyResult.ProbabilityAssemblyMethod);
            Assert.AreEqual((SerializableAssemblyMethod) 0, assemblyResult.AssemblyGroupAssemblyMethod);
            Assert.AreEqual((SerializableFailureMechanismSectionAssemblyGroup) 0, assemblyResult.AssemblyGroup);
            Assert.AreEqual(0, assemblyResult.Probability);
            Assert.AreEqual("VOLLDG", assemblyResult.Status);

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssemblyResult>(
                nameof(SerializableFailureMechanismSectionAssemblyResult.ProbabilityAssemblyMethod), "assemblagemethodeFaalkans");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssemblyResult>(
                nameof(SerializableFailureMechanismSectionAssemblyResult.AssemblyGroupAssemblyMethod), "assemblagemethodeDuidingsklasse");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssemblyResult>(
                nameof(SerializableFailureMechanismSectionAssemblyResult.AssemblyGroup), "duidingsklasse");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssemblyResult>(
                nameof(SerializableFailureMechanismSectionAssemblyResult.Probability), "faalkans");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismAssemblyResult>(
                nameof(SerializableFailureMechanismAssemblyResult.Status), "status");
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var probabilityAssemblyMethod = random.NextEnumValue<SerializableAssemblyMethod>();
            var assemblyGroupAssemblyMethod = random.NextEnumValue<SerializableAssemblyMethod>();
            var assemblyGroup = random.NextEnumValue<SerializableFailureMechanismSectionAssemblyGroup>();
            double probability = random.NextDouble();

            // Call
            var assemblyResult = new SerializableFailureMechanismSectionAssemblyResult(
                probabilityAssemblyMethod, assemblyGroupAssemblyMethod, assemblyGroup, probability);

            // Assert
            Assert.AreEqual(assemblyGroup, assemblyResult.AssemblyGroup);
            Assert.AreEqual(probability, assemblyResult.Probability);
            Assert.AreEqual(probabilityAssemblyMethod, assemblyResult.ProbabilityAssemblyMethod);
            Assert.AreEqual(assemblyGroupAssemblyMethod, assemblyResult.AssemblyGroupAssemblyMethod);
            Assert.AreEqual("VOLLDG", assemblyResult.Status);
        }

        [Test]
        [TestCase(SerializableFailureMechanismSectionAssemblyGroup.III)]
        [TestCase(SerializableFailureMechanismSectionAssemblyGroup.II)]
        [TestCase(SerializableFailureMechanismSectionAssemblyGroup.I)]
        [TestCase(SerializableFailureMechanismSectionAssemblyGroup.Zero)]
        [TestCase(SerializableFailureMechanismSectionAssemblyGroup.IMin)]
        [TestCase(SerializableFailureMechanismSectionAssemblyGroup.IIMin)]
        [TestCase(SerializableFailureMechanismSectionAssemblyGroup.IIIMin)]
        [TestCase(SerializableFailureMechanismSectionAssemblyGroup.NotRelevant)]
        public void ShouldSerializeProbability_WithProbabilityAndAssemblyGroupOtherThanNotDominant_ReturnsTrue(
            SerializableFailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            // Setup
            var random = new Random(39);
            var assemblyResult = new SerializableFailureMechanismSectionAssemblyResult(
                random.NextEnumValue<SerializableAssemblyMethod>(),
                random.NextEnumValue<SerializableAssemblyMethod>(),
                assemblyGroup,
                random.NextDouble());

            // Call
            bool shouldSerialize = assemblyResult.ShouldSerializeProbability();

            // Assert
            Assert.IsTrue(shouldSerialize);
        }

        [Test]
        public void ShouldSerializeProbability_WithProbabilityAndAssemblyGroupNotDominant_ReturnsFalse()
        {
            // Setup
            var random = new Random(39);
            var assemblyResult = new SerializableFailureMechanismSectionAssemblyResult(
                random.NextEnumValue<SerializableAssemblyMethod>(),
                random.NextEnumValue<SerializableAssemblyMethod>(),
                SerializableFailureMechanismSectionAssemblyGroup.NotDominant,
                random.NextDouble());

            // Call
            bool shouldSerialize = assemblyResult.ShouldSerializeProbability();

            // Assert
            Assert.IsFalse(shouldSerialize);
        }
    }
}