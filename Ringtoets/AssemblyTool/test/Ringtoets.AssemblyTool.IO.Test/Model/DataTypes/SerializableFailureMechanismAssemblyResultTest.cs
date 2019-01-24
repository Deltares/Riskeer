﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.TestUtil;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Ringtoets.AssemblyTool.IO.Test.Model.DataTypes
{
    [TestFixture]
    public class SerializableFailureMechanismAssemblyResultTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var assemblyResult = new SerializableFailureMechanismAssemblyResult();

            // Assert
            Assert.AreEqual((SerializableAssemblyMethod) 0, assemblyResult.AssemblyMethod);
            Assert.AreEqual((SerializableFailureMechanismCategoryGroup) 0, assemblyResult.CategoryGroup);
            Assert.IsNull(assemblyResult.Probability);
            Assert.AreEqual("VOLLDG", assemblyResult.Status);

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismAssemblyResult>(
                nameof(SerializableFailureMechanismAssemblyResult.AssemblyMethod), "assemblagemethode");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismAssemblyResult>(
                nameof(SerializableFailureMechanismAssemblyResult.CategoryGroup), "categorieTraject");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismAssemblyResult>(
                nameof(SerializableFailureMechanismAssemblyResult.Probability), "faalkans");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismAssemblyResult>(
                nameof(SerializableFailureMechanismAssemblyResult.Status), "status");
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var category = random.NextEnumValue<SerializableFailureMechanismCategoryGroup>();
            var assemblyMethod = random.NextEnumValue<SerializableAssemblyMethod>();
            double probability = random.NextDouble();

            // Call
            var assemblyResult = new SerializableFailureMechanismAssemblyResult(assemblyMethod, category, probability);

            // Assert
            Assert.AreEqual(category, assemblyResult.CategoryGroup);
            Assert.AreEqual(probability, assemblyResult.Probability);
            Assert.AreEqual(assemblyMethod, assemblyResult.AssemblyMethod);
            Assert.AreEqual("VOLLDG", assemblyResult.Status);
        }

        [Test]
        [TestCase(0.5)]
        [TestCase(double.NaN)]
        public void ShouldSerializeProbability_WithProbabilityValues_ReturnsTrue(double probability)
        {
            // Setup
            var random = new Random(39);
            var assemblyResult = new SerializableFailureMechanismAssemblyResult(
                random.NextEnumValue<SerializableAssemblyMethod>(),
                random.NextEnumValue<SerializableFailureMechanismCategoryGroup>(),
                probability);

            // Call
            bool shouldSerialize = assemblyResult.ShouldSerializeProbability();

            // Assert
            Assert.IsTrue(shouldSerialize);
        }

        [Test]
        public void ShouldSerializeProbability_WithoutProbability_ReturnsFalse()
        {
            // Setup
            var random = new Random(39);
            var assemblyResult = new SerializableFailureMechanismAssemblyResult(
                random.NextEnumValue<SerializableAssemblyMethod>(),
                random.NextEnumValue<SerializableFailureMechanismCategoryGroup>());

            // Call
            bool shouldSerialize = assemblyResult.ShouldSerializeProbability();

            // Assert
            Assert.IsFalse(shouldSerialize);
        }
    }
}