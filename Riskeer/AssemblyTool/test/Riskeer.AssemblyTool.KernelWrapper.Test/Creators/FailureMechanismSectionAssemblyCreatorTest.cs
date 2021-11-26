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
using System.Collections.Generic;
using System.ComponentModel;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Assembly.Kernel.Model.FailurePathSections;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCreatorTest
    {
        [Test]
        public void CreateFailureMechanismSectionAssemblyResult_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyResult(null);

            // Assert
            Assert.That(Call, Throws.TypeOf<ArgumentNullException>()
                                    .With.Property(nameof(ArgumentNullException.ParamName))
                                    .EqualTo("result"));
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyResult_WithInvalidResult_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = random.NextDouble();

            var result = new FailurePathSectionAssemblyResult(new Probability(profileProbability),
                                                              new Probability(sectionProbability),
                                                              (EInterpretationCategory) 99);

            // Call
            void Call() => FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyResult(result);

            // Assert
            var expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(EInterpretationCategory)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(GetValidCategoryConversions))]
        public void CreateFailureMechanismSectionAssemblyResult_WithValidResult_ReturnsExpectedFailureMechanismSectionAssembly(
            EInterpretationCategory category,
            FailureMechanismSectionAssemblyGroup expectedCategory)
        {
            // Setup
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = random.NextDouble();

            var result = new FailurePathSectionAssemblyResult(new Probability(profileProbability),
                                                              new Probability(sectionProbability),
                                                              category);

            // Call
            FailureMechanismSectionAssemblyResult createdAssemblyResult = FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyResult(result);

            // Assert
            Assert.AreEqual(profileProbability, createdAssemblyResult.ProfileProbability);
            Assert.AreEqual(sectionProbability, createdAssemblyResult.SectionProbability);
            Assert.AreEqual(result.NSection, createdAssemblyResult.N);
            Assert.AreEqual(expectedCategory, createdAssemblyResult.AssemblyGroup);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyGroup_InvalidCategory_ThrowsInvalidEnumArgumentException()
        {
            // Call
            void Call() => FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyGroup((EInterpretationCategory) 99);

            // Assert
            var expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(EInterpretationCategory)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(GetValidCategoryConversions))]
        public void CreateFailureMechanismSectionAssemblyGroup_ValidCategory_ReturnsExpectedValue(
            EInterpretationCategory category,
            FailureMechanismSectionAssemblyGroup expectedCategory)
        {
            // Call
            FailureMechanismSectionAssemblyGroup createdCategory = FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyGroup(category);

            // Assert
            Assert.AreEqual(expectedCategory, createdCategory);
        }

        private static IEnumerable<TestCaseData> GetValidCategoryConversions()
        {
            yield return new TestCaseData(EInterpretationCategory.ND, FailureMechanismSectionAssemblyGroup.ND);
            yield return new TestCaseData(EInterpretationCategory.III, FailureMechanismSectionAssemblyGroup.III);
            yield return new TestCaseData(EInterpretationCategory.II, FailureMechanismSectionAssemblyGroup.II);
            yield return new TestCaseData(EInterpretationCategory.I, FailureMechanismSectionAssemblyGroup.I);
            yield return new TestCaseData(EInterpretationCategory.ZeroPlus, FailureMechanismSectionAssemblyGroup.ZeroPlus);
            yield return new TestCaseData(EInterpretationCategory.Zero, FailureMechanismSectionAssemblyGroup.Zero);
            yield return new TestCaseData(EInterpretationCategory.IMin, FailureMechanismSectionAssemblyGroup.IMin);
            yield return new TestCaseData(EInterpretationCategory.IIMin, FailureMechanismSectionAssemblyGroup.IIMin);
            yield return new TestCaseData(EInterpretationCategory.IIIMin, FailureMechanismSectionAssemblyGroup.IIIMin);
            yield return new TestCaseData(EInterpretationCategory.D, FailureMechanismSectionAssemblyGroup.D);
            yield return new TestCaseData(EInterpretationCategory.Gr, FailureMechanismSectionAssemblyGroup.Gr);
        }
    }
}