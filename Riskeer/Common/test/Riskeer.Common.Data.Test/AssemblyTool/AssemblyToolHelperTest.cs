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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class AssemblyToolHelperTest
    {
        [Test]
        public void AssembleFailureMechanismSection_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssemblyToolHelper.AssembleFailureMechanismSection<FailureMechanismSectionResult>(null, sr => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanismSection_PerformSectionAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => AssemblyToolHelper.AssembleFailureMechanismSection(sectionResult, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performSectionAssemblyFunc", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanismSection_WithValidData_ReturnsFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var sectionResult = new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            var random = new Random(21);
            var expectedAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

            // Call
            FailureMechanismSectionAssemblyResult assemblyResult = AssemblyToolHelper.AssembleFailureMechanismSection(
                sectionResult, sr => new FailureMechanismSectionAssemblyResultWrapper(expectedAssemblyResult, random.NextEnumValue<AssemblyMethod>(),
                                                                                      random.NextEnumValue<AssemblyMethod>()));

            // Assert
            Assert.AreSame(expectedAssemblyResult, assemblyResult);
        }

        [Test]
        public void AssembleFailureMechanismSection_PerformSectionAssemblyFuncThrowsAssemblyException_ReturnsDefaultFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var sectionResult = new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            FailureMechanismSectionAssemblyResult assemblyResult = AssemblyToolHelper.AssembleFailureMechanismSection(
                sectionResult, sr => throw new AssemblyException());

            // Assert
            Assert.IsInstanceOf<DefaultFailureMechanismSectionAssemblyResult>(assemblyResult);
        }
    }
}