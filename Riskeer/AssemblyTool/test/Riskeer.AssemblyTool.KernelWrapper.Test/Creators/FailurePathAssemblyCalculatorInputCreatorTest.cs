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
using Assembly.Kernel.Model.FailurePathSections;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailurePathAssemblyCalculatorInputCreatorTest
    {
        [Test]
        public void CreateFailurePathSectionAssemblyResult_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailurePathAssemblyCalculatorInputCreator.CreateFailurePathSectionAssemblyResult(null);

            // Assert
            Assert.That(Call, Throws.TypeOf<ArgumentNullException>()
                                    .With.Property(nameof(ArgumentNullException.ParamName))
                                    .EqualTo("result"));
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyGroup.ND, EInterpretationCategory.ND)]
        [TestCase(FailureMechanismSectionAssemblyGroup.III, EInterpretationCategory.III)]
        [TestCase(FailureMechanismSectionAssemblyGroup.II, EInterpretationCategory.II)]
        [TestCase(FailureMechanismSectionAssemblyGroup.I, EInterpretationCategory.I)]
        [TestCase(FailureMechanismSectionAssemblyGroup.ZeroPlus, EInterpretationCategory.ZeroPlus)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Zero, EInterpretationCategory.Zero)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IMin, EInterpretationCategory.IMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIMin, EInterpretationCategory.IIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIIMin, EInterpretationCategory.IIIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.D, EInterpretationCategory.D)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Gr, EInterpretationCategory.Gr)]
        public void CreateFailurePathSectionAssemblyResult_WithValidResult_ReturnsExpectedFailurePathSectionAssemblyResult(
            FailureMechanismSectionAssemblyGroup assemblyGroup, EInterpretationCategory expectedCategory)
        {
            // Setup
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = profileProbability + 0.001;

            var result = new FailureMechanismSectionAssemblyResult(profileProbability, sectionProbability,
                                                                   random.NextDouble(),
                                                                   assemblyGroup);
            // Call
            FailurePathSectionAssemblyResult createdResult = FailurePathAssemblyCalculatorInputCreator.CreateFailurePathSectionAssemblyResult(result);

            // Assert
            ProbabilityAssert.AreEqual(profileProbability, createdResult.ProbabilityProfile);
            ProbabilityAssert.AreEqual(sectionProbability, createdResult.ProbabilitySection);
            Assert.AreEqual(expectedCategory, createdResult.InterpretationCategory);
        }

        [Test]
        public void CreateFailurePathSectionAssemblyResult_InvalidAssemblyGroup_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            var result = new FailureMechanismSectionAssemblyResult(probability, probability,
                                                                   random.NextDouble(),
                                                                   (FailureMechanismSectionAssemblyGroup) 99);

            // Call
            void Call() => FailurePathAssemblyCalculatorInputCreator.CreateFailurePathSectionAssemblyResult(result);

            // Assert
            var expectedMessage = $"The value of argument 'assemblyGroup' (99) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }
    }
}