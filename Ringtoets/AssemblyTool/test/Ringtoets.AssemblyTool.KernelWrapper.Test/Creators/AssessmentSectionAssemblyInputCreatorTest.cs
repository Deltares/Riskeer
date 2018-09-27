// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class AssessmentSectionAssemblyInputCreatorTest
    {
        private static IEnumerable<TestCaseData> GetFailureMechanismAssemblyCategoryGroupConversions
        {
            get
            {
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.NotApplicable, EFailureMechanismCategory.Nvt);
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.None, EFailureMechanismCategory.Gr);
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.It, EFailureMechanismCategory.It);
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.IIt, EFailureMechanismCategory.IIt);
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.IIIt, EFailureMechanismCategory.IIIt);
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.IVt, EFailureMechanismCategory.IVt);
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.Vt, EFailureMechanismCategory.Vt);
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.VIt, EFailureMechanismCategory.VIt);
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.VIIt, EFailureMechanismCategory.VIIt);
            }
        }

        [Test]
        public void CreateFailureMechanismAssemblyResult_FailureMechanismAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssessmentSectionAssemblyInputCreator.CreateFailureMechanismAssemblyResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismAssemblyResult_WithFailureMechanismAssemblyAndInvalidEnumInput_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(),
                                                                        (FailureMechanismAssemblyCategoryGroup) 99);

            // Call
            TestDelegate test = () => AssessmentSectionAssemblyInputCreator.CreateFailureMechanismAssemblyResult(failureMechanismAssembly);

            // Assert
            const string expectedMessage = "The value of argument 'input' (99) is invalid for Enum type 'FailureMechanismAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismAssemblyCategoryGroupConversions))]
        public void CreateFailureMechanismAssemblyResult_WithValidFailureMechanismAssembly_ReturnsFailureMechanismAssemblyResult(
            FailureMechanismAssemblyCategoryGroup originalGroup,
            EFailureMechanismCategory expectedGroup)
        {
            // Setup
            var random = new Random(21);
            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(), originalGroup);

            // Call
            FailureMechanismAssemblyResult result = AssessmentSectionAssemblyInputCreator.CreateFailureMechanismAssemblyResult(failureMechanismAssembly);

            // Assert
            Assert.AreEqual(expectedGroup, result.Category);
            Assert.AreEqual(failureMechanismAssembly.Probability, result.FailureProbability);
        }

        [Test]
        public void CreateFailureMechanismAssemblyResult_WithInvalidEnumInput_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => AssessmentSectionAssemblyInputCreator.CreateFailureMechanismAssemblyResult((FailureMechanismAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'input' (99) is invalid for Enum type 'FailureMechanismAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismAssemblyCategoryGroupConversions))]
        public void CreateFailureMechanismAssemblyResult_WithValidEnumInput_ReturnsFailureMechanismAssemblyResult(
            FailureMechanismAssemblyCategoryGroup originalGroup,
            EFailureMechanismCategory expectedGroup)
        {
            // Call
            FailureMechanismAssemblyResult result = AssessmentSectionAssemblyInputCreator.CreateFailureMechanismAssemblyResult(originalGroup);

            // Assert
            Assert.AreEqual(expectedGroup, result.Category);
            Assert.IsNaN(result.FailureProbability);
        }

        [Test]
        public void CreateFailureMechanismCategory_WithInvalidEnumInput_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => AssessmentSectionAssemblyInputCreator.CreateFailureMechanismCategory((FailureMechanismAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'input' (99) is invalid for Enum type 'FailureMechanismAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismAssemblyCategoryGroupConversions))]
        public void CreateFailureMechanismCategory_WithValidEnumInput_ReturnsFailureMechanismAssemblyResult(
            FailureMechanismAssemblyCategoryGroup originalGroup,
            EFailureMechanismCategory expectedGroup)
        {
            // Call
            EFailureMechanismCategory result = AssessmentSectionAssemblyInputCreator.CreateFailureMechanismCategory(originalGroup);

            // Assert
            Assert.AreEqual(expectedGroup, result);
        }
    }
}