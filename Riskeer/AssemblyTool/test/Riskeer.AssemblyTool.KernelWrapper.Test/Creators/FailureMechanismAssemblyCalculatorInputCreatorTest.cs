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
        [TestCase(FailureMechanismSectionAssemblyGroup.NotDominant, EInterpretationCategory.NotDominant)]
        [TestCase(FailureMechanismSectionAssemblyGroup.III, EInterpretationCategory.III)]
        [TestCase(FailureMechanismSectionAssemblyGroup.II, EInterpretationCategory.II)]
        [TestCase(FailureMechanismSectionAssemblyGroup.I, EInterpretationCategory.I)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Zero, EInterpretationCategory.Zero)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IMin, EInterpretationCategory.IMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIMin, EInterpretationCategory.IIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIIMin, EInterpretationCategory.IIIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Dominant, EInterpretationCategory.Dominant)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Gr, EInterpretationCategory.Gr)]
        public void CreateFailureMechanismSectionAssemblyResult_WithValidResult_ReturnsExpectedFailureMechanismSectionAssemblyResult(
            FailureMechanismSectionAssemblyGroup assemblyGroup, EInterpretationCategory expectedInterpretationCategory)
        {
            // Setup
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = profileProbability + 0.001;

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
    }
}