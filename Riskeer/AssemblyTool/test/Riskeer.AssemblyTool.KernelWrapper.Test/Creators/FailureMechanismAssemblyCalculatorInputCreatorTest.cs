// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Assembly.Kernel.Model.FailureMechanismSections;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;
using FailureMechanismSectionAssemblyResult = Riskeer.AssemblyTool.Data.FailureMechanismSectionAssemblyResult;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismAssemblyCalculatorInputCreatorTest
    {
        [Test]
        public void CreateResultWithProfileAndSectionProbabilities_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismAssemblyCalculatorInputCreator.CreateResultWithProfileAndSectionProbabilities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void CreateResultWithProfileAndSectionProbabilities_WithValidResult_ReturnsExpectedFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var result = new FailureMechanismSectionAssemblyResult(
                random.NextDouble(0.001, 0.01), random.NextDouble(0.01, 0.1), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

            // Call
            ResultWithProfileAndSectionProbabilities createdResult = FailureMechanismAssemblyCalculatorInputCreator.CreateResultWithProfileAndSectionProbabilities(result);

            // Assert
            ProbabilityAssert.AreEqual(result.ProfileProbability, createdResult.ProbabilityProfile);
            ProbabilityAssert.AreEqual(result.SectionProbability, createdResult.ProbabilitySection);
        }
    }
}