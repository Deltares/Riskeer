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
using Assembly.Kernel.Model.Categories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;
using KernelFailureMechanismSectionAssemblyResult = Assembly.Kernel.Model.FailureMechanismSections.FailureMechanismSectionAssemblyResult;
using RiskeerFailureMechanismSectionAssemblyResult = Riskeer.AssemblyTool.Data.FailureMechanismSectionAssemblyResult;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismAssemblyCalculatorInputCreatorTest
    {
        [Test]
        public void CreateFailureMechanismSectionAssemblyResult_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyResult(null);

            // Assert
            Assert.That(Call, Throws.TypeOf<ArgumentNullException>()
                                    .With.Property(nameof(ArgumentNullException.ParamName))
                                    .EqualTo("result"));
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyResult_InvalidAssemblyGroup_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            const FailureMechanismSectionAssemblyGroup assemblyGroup = (FailureMechanismSectionAssemblyGroup) 99;

            var result = new RiskeerFailureMechanismSectionAssemblyResult(
                probability, probability, random.NextDouble(), assemblyGroup);

            // Call
            void Call() => FailureMechanismAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyResult(result);

            // Assert
            var expectedMessage = $"The value of argument 'assemblyGroup' ({assemblyGroup}) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(ResultCases))]
        public void CreateFailureMechanismSectionAssemblyResult_WithValidResult_ReturnsExpectedFailureMechanismSectionAssemblyResult(
            FailureMechanismSectionAssemblyGroup assemblyGroup, EInterpretationCategory expectedInterpretationCategory,
            double profileProbability, double sectionProbability)
        {
            // Setup
            var random = new Random(21);
            var result = new RiskeerFailureMechanismSectionAssemblyResult(profileProbability, sectionProbability,
                                                                          random.NextDouble(),
                                                                          assemblyGroup);
            // Call
            KernelFailureMechanismSectionAssemblyResult createdResult = FailureMechanismAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyResult(result);

            // Assert
            ProbabilityAssert.AreEqual(profileProbability, createdResult.ProbabilityProfile);
            ProbabilityAssert.AreEqual(sectionProbability, createdResult.ProbabilitySection);
            Assert.AreEqual(expectedInterpretationCategory, createdResult.InterpretationCategory);
        }

        private static IEnumerable<TestCaseData> ResultCases()
        {
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = profileProbability + 0.001;

            yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.NotDominant, EInterpretationCategory.NotDominant, double.NaN, double.NaN);
            yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.III, EInterpretationCategory.III, profileProbability, sectionProbability);
            yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.II, EInterpretationCategory.II, profileProbability, sectionProbability);
            yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.I, EInterpretationCategory.I, profileProbability, sectionProbability);
            yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.Zero, EInterpretationCategory.Zero, profileProbability, sectionProbability);
            yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.IMin, EInterpretationCategory.IMin, profileProbability, sectionProbability);
            yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.IIMin, EInterpretationCategory.IIMin, profileProbability, sectionProbability);
            yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.IIIMin, EInterpretationCategory.IIIMin, profileProbability, sectionProbability);
            yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.Dominant, EInterpretationCategory.Dominant, double.NaN, double.NaN);
            yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.Gr, EInterpretationCategory.Gr, double.NaN, double.NaN);
            yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.NotRelevant, EInterpretationCategory.NotRelevant, double.NaN, double.NaN);
        }
    }
}