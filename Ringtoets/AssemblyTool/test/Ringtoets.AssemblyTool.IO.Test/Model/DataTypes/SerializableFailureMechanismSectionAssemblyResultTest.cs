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

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model.DataTypes
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
            Assert.AreEqual((SerializableAssemblyMethod) 0, assemblyResult.AssemblyMethod);
            Assert.AreEqual((SerializableAssessmentType) 0, assemblyResult.AssessmentType);
            Assert.AreEqual((SerializableFailureMechanismSectionCategoryGroup) 0, assemblyResult.CategoryGroup);
            Assert.IsNull(assemblyResult.Probability);

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssemblyResult>(
                nameof(SerializableFailureMechanismSectionAssemblyResult.AssemblyMethod), "assemblagemethode");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssemblyResult>(
                nameof(SerializableFailureMechanismSectionAssemblyResult.CategoryGroup), "categorieVak");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssemblyResult>(
                nameof(SerializableFailureMechanismSectionAssemblyResult.Probability), "faalkans");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssemblyResult>(
                nameof(SerializableFailureMechanismSectionAssemblyResult.AssessmentType), "toets");
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var category = random.NextEnumValue<SerializableFailureMechanismSectionCategoryGroup>();
            var assemblyMethod = random.NextEnumValue<SerializableAssemblyMethod>();
            var assessmentLevel = random.NextEnumValue<SerializableAssessmentType>();
            double probability = random.NextDouble();

            // Call
            var assemblyResult = new SerializableFailureMechanismSectionAssemblyResult(assemblyMethod, assessmentLevel, category, probability);

            // Assert
            Assert.AreEqual(category, assemblyResult.CategoryGroup);
            Assert.AreEqual(assessmentLevel, assemblyResult.AssessmentType);
            Assert.AreEqual(probability, assemblyResult.Probability);
            Assert.AreEqual(assemblyMethod, assemblyResult.AssemblyMethod);
        }

        [Test]
        [TestCase(0.5)]
        [TestCase(double.NaN)]
        public void ShouldSerializeProbability_WithProbabilityValues_ReturnsTrue(double probability)
        {
            // Setup
            var random = new Random(39);
            var assemblyResult = new SerializableFailureMechanismSectionAssemblyResult(
                random.NextEnumValue<SerializableAssemblyMethod>(),
                random.NextEnumValue<SerializableAssessmentType>(),
                random.NextEnumValue<SerializableFailureMechanismSectionCategoryGroup>(),
                probability);

            // Call
            bool shouldSerialize = assemblyResult.ShouldSerializeProbability();

            // Assert
            Assert.IsTrue(shouldSerialize);
        }

        [Test]
        public void ShouldSerializeProbability_WithoutProbabilityValues_ReturnsFalse()
        {
            // Setup
            var random = new Random(39);
            var assemblyResult = new SerializableFailureMechanismSectionAssemblyResult(
                random.NextEnumValue<SerializableAssemblyMethod>(),
                random.NextEnumValue<SerializableAssessmentType>(),
                random.NextEnumValue<SerializableFailureMechanismSectionCategoryGroup>());

            // Call
            bool shouldSerialize = assemblyResult.ShouldSerializeProbability();

            // Assert
            Assert.IsFalse(shouldSerialize);
        }
    }
}