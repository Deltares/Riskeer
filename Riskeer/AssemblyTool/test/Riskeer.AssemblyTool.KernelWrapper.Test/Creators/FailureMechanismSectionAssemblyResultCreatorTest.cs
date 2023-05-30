// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyResultCreatorTest
    {
        [Test]
        public void CreateForProbabilityAndCategory_WithValidData_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            var random = new Random(21);
            double sectionProbability = random.NextDouble();
            var category = random.NextEnumValue<EInterpretationCategory>();

            // Call
            FailureMechanismSectionAssemblyResult result = FailureMechanismSectionAssemblyResultCreator.Create(
                new Probability(sectionProbability), category);

            // Assert
            Assert.AreEqual(sectionProbability, result.ProfileProbability);
            Assert.AreEqual(sectionProbability, result.SectionProbability);
            Assert.AreEqual(1.0, result.N);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroupConverter.ConvertTo(category),
                            result.FailureMechanismSectionAssemblyGroup);
        }

        [Test]
        public void CreateForResultWithProfileAndSectionProbabilities_ResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => FailureMechanismSectionAssemblyResultCreator.Create(null, random.NextEnumValue<EInterpretationCategory>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void CreateForResultWithProfileAndSectionProbabilities_WithValidResult_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            var random = new Random(21);
            double profileProbability = random.NextDouble(0.0001, 0.001);
            double sectionProbability = random.NextDouble(0.0, 0.01);
            EInterpretationCategory category = random.NextEnumValue(new[]
            {
                EInterpretationCategory.III,
                EInterpretationCategory.II,
                EInterpretationCategory.I,
                EInterpretationCategory.Zero,
                EInterpretationCategory.IMin,
                EInterpretationCategory.IIMin,
                EInterpretationCategory.IIIMin
            });

            var result = new ResultWithProfileAndSectionProbabilities(
                new Probability(profileProbability), new Probability(sectionProbability));

            // Call
            FailureMechanismSectionAssemblyResult createdAssemblyResult = FailureMechanismSectionAssemblyResultCreator.Create(result, category);

            // Assert
            Assert.AreEqual(profileProbability, createdAssemblyResult.ProfileProbability);
            Assert.AreEqual(sectionProbability, createdAssemblyResult.SectionProbability);
            Assert.AreEqual(result.LengthEffectFactor, createdAssemblyResult.N);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroupConverter.ConvertTo(category),
                            createdAssemblyResult.FailureMechanismSectionAssemblyGroup);
        }
    }
}