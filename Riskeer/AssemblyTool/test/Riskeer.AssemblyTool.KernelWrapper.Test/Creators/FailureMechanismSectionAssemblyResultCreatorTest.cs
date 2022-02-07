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
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using AssemblyFailureMechanismSectionAssemblyResult = Assembly.Kernel.Model.FailureMechanismSections.FailureMechanismSectionAssemblyResult;
using RiskeerFailureMechanismSectionAssemblyResult = Riskeer.AssemblyTool.Data.FailureMechanismSectionAssemblyResult;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyResultCreatorTest
    {
        [Test]
        public void CreateFailureMechanismSectionAssemblyResult_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionAssemblyResultCreator.CreateFailureMechanismSectionAssemblyResult(null);

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

            var result = new AssemblyFailureMechanismSectionAssemblyResult(
                new Probability(profileProbability), new Probability(sectionProbability), (EInterpretationCategory) 99);

            // Call
            void Call() => FailureMechanismSectionAssemblyResultCreator.CreateFailureMechanismSectionAssemblyResult(result);

            // Assert
            var expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(EInterpretationCategory)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyResult_WithValidResult_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = random.NextDouble();
            var category = random.NextEnumValue<EInterpretationCategory>();

            var result = new AssemblyFailureMechanismSectionAssemblyResult(
                new Probability(profileProbability), new Probability(sectionProbability), category);

            // Call
            RiskeerFailureMechanismSectionAssemblyResult createdAssemblyResult = FailureMechanismSectionAssemblyResultCreator.CreateFailureMechanismSectionAssemblyResult(result);

            // Assert
            Assert.AreEqual(profileProbability, createdAssemblyResult.ProfileProbability);
            Assert.AreEqual(sectionProbability, createdAssemblyResult.SectionProbability);
            Assert.AreEqual(result.NSection, createdAssemblyResult.N);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroupConverter.ConvertTo(category),
                            createdAssemblyResult.AssemblyGroup);
        }
    }
}