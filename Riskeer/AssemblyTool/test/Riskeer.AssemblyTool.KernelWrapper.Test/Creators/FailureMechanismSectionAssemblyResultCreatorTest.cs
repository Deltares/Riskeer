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
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Assembly.Kernel.Model.FailureMechanismSections;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using KernelFailureMechanismSectionAssemblyResult = Assembly.Kernel.Model.FailureMechanismSections.FailureMechanismSectionAssemblyResult;
using RiskeerFailureMechanismSectionAssemblyResult = Riskeer.AssemblyTool.Data.FailureMechanismSectionAssemblyResult;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyResultCreatorTest
    {
        [Test]
        public void CreateForFailureMechanismSectionAssemblyResult_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionAssemblyResultCreator.Create((KernelFailureMechanismSectionAssemblyResult) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void CreateForFailureMechanismSectionAssemblyResult_WithValidResult_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            var random = new Random(21);
            double sectionProbability = random.NextDouble();
            var interpretationCategory = random.NextEnumValue<EInterpretationCategory>();

            var result = new KernelFailureMechanismSectionAssemblyResult(
                new Probability(sectionProbability), interpretationCategory);

            // Call
            RiskeerFailureMechanismSectionAssemblyResult createdAssemblyResult = FailureMechanismSectionAssemblyResultCreator.Create(result);

            // Assert
            Assert.AreEqual(sectionProbability, createdAssemblyResult.ProfileProbability);
            Assert.AreEqual(sectionProbability, createdAssemblyResult.SectionProbability);
            Assert.AreEqual(1.0, createdAssemblyResult.N);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroupConverter.ConvertTo(interpretationCategory),
                            createdAssemblyResult.FailureMechanismSectionAssemblyGroup);
        }

        [Test]
        public void CreateForFailureMechanismSectionAssemblyResultWithLengthEffect_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionAssemblyResultCreator.Create((FailureMechanismSectionAssemblyResultWithLengthEffect) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void CreateForFailureMechanismSectionAssemblyResultWithLengthEffect_WithValidResult_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            var random = new Random(21);
            double profileProbability = random.NextDouble();
            double sectionProbability = random.NextDouble();
            var interpretationCategory = random.NextEnumValue<EInterpretationCategory>();

            var result = new FailureMechanismSectionAssemblyResultWithLengthEffect(
                new Probability(profileProbability), new Probability(sectionProbability), interpretationCategory);

            // Call
            RiskeerFailureMechanismSectionAssemblyResult createdAssemblyResult = FailureMechanismSectionAssemblyResultCreator.Create(result);

            // Assert
            Assert.AreEqual(profileProbability, createdAssemblyResult.ProfileProbability);
            Assert.AreEqual(sectionProbability, createdAssemblyResult.SectionProbability);
            Assert.AreEqual(result.LengthEffectFactor, createdAssemblyResult.N);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroupConverter.ConvertTo(interpretationCategory),
                            createdAssemblyResult.FailureMechanismSectionAssemblyGroup);
        }
    }
}