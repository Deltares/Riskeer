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
    public class FailureMechanismAssemblyCreatorTest
    {
        [Test]
        public void Create_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismAssemblyCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_InvalidGroup_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var result = new FailureMechanismAssemblyResult((EFailureMechanismCategory) 99, new Random(39).NextDouble());

            // Call
            TestDelegate test = () => FailureMechanismAssemblyCreator.Create(result);

            // Assert
            string expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(EFailureMechanismCategory)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(FailureMechanismAssemblyCategoryGroupCases))]
        public void Create_ValidResult_ReturnFailureMechanismAssembly(EFailureMechanismCategory originalGroup,
                                                                      FailureMechanismAssemblyCategoryGroup expectedGroup)
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();

            var result = new FailureMechanismAssemblyResult(originalGroup, probability);

            // Call
            FailureMechanismAssembly assembly = FailureMechanismAssemblyCreator.Create(result);

            // Assert
            Assert.AreEqual(expectedGroup, assembly.Group);
            Assert.AreEqual(probability, assembly.Probability);
        }

        [Test]
        public void CreateFailureMechanismAssemblyCategoryGroup_InvalidGroup_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismAssemblyCreator.CreateFailureMechanismAssemblyCategoryGroup((EFailureMechanismCategory) 99);

            // Assert
            string expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(EFailureMechanismCategory)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(FailureMechanismAssemblyCategoryGroupCases))]
        public void CreateFailureMechanismAssemblyCategoryGroup_ValidGroup_ReturnFailureMechanismAssemblyCategoryGroup(
            EFailureMechanismCategory originalGroup,
            FailureMechanismAssemblyCategoryGroup expectedGroup)
        {
            // Call
            FailureMechanismAssemblyCategoryGroup actualGroup = FailureMechanismAssemblyCreator.CreateFailureMechanismAssemblyCategoryGroup(originalGroup);

            // Assert
            Assert.AreEqual(expectedGroup, actualGroup);
        }

        private static IEnumerable<TestCaseData> FailureMechanismAssemblyCategoryGroupCases()
        {
            yield return new TestCaseData(EFailureMechanismCategory.Gr, FailureMechanismAssemblyCategoryGroup.None);
            yield return new TestCaseData(EFailureMechanismCategory.Nvt, FailureMechanismAssemblyCategoryGroup.NotApplicable);
            yield return new TestCaseData(EFailureMechanismCategory.It, FailureMechanismAssemblyCategoryGroup.It);
            yield return new TestCaseData(EFailureMechanismCategory.IIt, FailureMechanismAssemblyCategoryGroup.IIt);
            yield return new TestCaseData(EFailureMechanismCategory.IIIt, FailureMechanismAssemblyCategoryGroup.IIIt);
            yield return new TestCaseData(EFailureMechanismCategory.IVt, FailureMechanismAssemblyCategoryGroup.IVt);
            yield return new TestCaseData(EFailureMechanismCategory.Vt, FailureMechanismAssemblyCategoryGroup.Vt);
            yield return new TestCaseData(EFailureMechanismCategory.VIt, FailureMechanismAssemblyCategoryGroup.VIt);
            yield return new TestCaseData(EFailureMechanismCategory.VIIt, FailureMechanismAssemblyCategoryGroup.VIIt);
        }
    }
}