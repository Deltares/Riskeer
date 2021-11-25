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
        public void CreateFailureMechanismSectionAssembly_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssembly(null);

            // Assert
            Assert.That(Call, Throws.TypeOf<ArgumentNullException>()
                                    .With.Property(nameof(ArgumentNullException.ParamName))
                                    .EqualTo("result"));
        }

        [Test]
        public void CreateFailureMechanismSectionAssembly_WithInvalidResult_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = random.NextDouble();

            var result = new FailurePathSectionAssemblyResult(new Probability(profileProbability),
                                                              new Probability(sectionProbability),
                                                              (EInterpretationCategory) 99);

            // Call
            void Call() => FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssembly(result);

            // Assert
            var expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(EInterpretationCategory)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(GetValidCategoryConversions))]
        public void CreateFailureMechanismSectionAssembly_WithValidResult_ReturnsExpectedFailureMechanismSectionAssembly(
            EInterpretationCategory category,
            FailureMechanismSectionInterpretationCategory expectedCategory)
        {
            // Setup
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = random.NextDouble();

            var result = new FailurePathSectionAssemblyResult(new Probability(profileProbability),
                                                              new Probability(sectionProbability),
                                                              category);

            // Call
            FailureMechanismSectionAssembly createdAssembly = FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssembly(result);

            // Assert
            Assert.AreEqual(profileProbability, createdAssembly.ProfileProbability);
            Assert.AreEqual(sectionProbability, createdAssembly.SectionProbability);
            Assert.AreEqual(result.NSection, createdAssembly.N);
            Assert.AreEqual(expectedCategory, createdAssembly.InterpretationCategory);
        }

        [Test]
        public void CreateFailureMechanismSectionInterpretationCategory_InvalidCategory_ThrowsInvalidEnumArgumentException()
        {
            // Call
            void Call() => FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionInterpretationCategory((EInterpretationCategory) 99);

            // Assert
            var expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(EInterpretationCategory)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(GetValidCategoryConversions))]
        public void CreateFailureMechanismSectionInterpretationCategory_ValidCategory_ReturnsExpectedValue(
            EInterpretationCategory category,
            FailureMechanismSectionInterpretationCategory expectedCategory)
        {
            // Call
            FailureMechanismSectionInterpretationCategory createdCategory = FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionInterpretationCategory(category);

            // Assert
            Assert.AreEqual(expectedCategory, createdCategory);
        }

        private static IEnumerable<TestCaseData> GetValidCategoryConversions()
        {
            yield return new TestCaseData(EInterpretationCategory.ND, FailureMechanismSectionInterpretationCategory.ND);
            yield return new TestCaseData(EInterpretationCategory.III, FailureMechanismSectionInterpretationCategory.III);
            yield return new TestCaseData(EInterpretationCategory.II, FailureMechanismSectionInterpretationCategory.II);
            yield return new TestCaseData(EInterpretationCategory.I, FailureMechanismSectionInterpretationCategory.I);
            yield return new TestCaseData(EInterpretationCategory.ZeroPlus, FailureMechanismSectionInterpretationCategory.ZeroPlus);
            yield return new TestCaseData(EInterpretationCategory.Zero, FailureMechanismSectionInterpretationCategory.Zero);
            yield return new TestCaseData(EInterpretationCategory.IMin, FailureMechanismSectionInterpretationCategory.IMin);
            yield return new TestCaseData(EInterpretationCategory.IIMin, FailureMechanismSectionInterpretationCategory.IIMin);
            yield return new TestCaseData(EInterpretationCategory.IIIMin, FailureMechanismSectionInterpretationCategory.IIIMin);
            yield return new TestCaseData(EInterpretationCategory.D, FailureMechanismSectionInterpretationCategory.D);
            yield return new TestCaseData(EInterpretationCategory.Gr, FailureMechanismSectionInterpretationCategory.Gr);
        }
    }
}