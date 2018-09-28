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
using System.ComponentModel;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCreatorTest
    {
        [Test]
        public void Create_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionAssemblyCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Create_InvalidGroup_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var result = new FmSectionAssemblyDirectResult((EFmSectionCategory) 99);

            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCreator.Create(result);

            // Assert
            const string expectedMessage = "The value of argument 'category' (99) is invalid for Enum type 'EFmSectionCategory'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(EFmSectionCategory.Gr, FailureMechanismSectionAssemblyCategoryGroup.None)]
        [TestCase(EFmSectionCategory.NotApplicable, FailureMechanismSectionAssemblyCategoryGroup.NotApplicable)]
        [TestCase(EFmSectionCategory.Iv, FailureMechanismSectionAssemblyCategoryGroup.Iv)]
        [TestCase(EFmSectionCategory.IIv, FailureMechanismSectionAssemblyCategoryGroup.IIv)]
        [TestCase(EFmSectionCategory.IIIv, FailureMechanismSectionAssemblyCategoryGroup.IIIv)]
        [TestCase(EFmSectionCategory.IVv, FailureMechanismSectionAssemblyCategoryGroup.IVv)]
        [TestCase(EFmSectionCategory.Vv, FailureMechanismSectionAssemblyCategoryGroup.Vv)]
        [TestCase(EFmSectionCategory.VIv, FailureMechanismSectionAssemblyCategoryGroup.VIv)]
        [TestCase(EFmSectionCategory.VIIv, FailureMechanismSectionAssemblyCategoryGroup.VIIv)]
        public void Create_ValidResult_ReturnExpectedFailureMechanismSectionAssembly(
            EFmSectionCategory originalGroup,
            FailureMechanismSectionAssemblyCategoryGroup expectedGroup)
        {
            // Call
            FailureMechanismSectionAssembly assembly = FailureMechanismSectionAssemblyCreator.Create(
                new FmSectionAssemblyDirectResult(originalGroup));

            // Assert
            Assert.AreEqual(expectedGroup, assembly.Group);
            Assert.IsNaN(assembly.Probability);
        }

        [Test]
        public void CreateWithFmSectionAssemblyDirectResultWithProbability_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionAssemblyCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void CreateWithFmSectionAssemblyDirectResultWithProbability_InvalidGroup_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var result = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99, new Random(39).NextDouble());

            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCreator.Create(result);

            // Assert
            const string expectedMessage = "The value of argument 'category' (99) is invalid for Enum type 'EFmSectionCategory'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(EFmSectionCategory.Gr, FailureMechanismSectionAssemblyCategoryGroup.None)]
        [TestCase(EFmSectionCategory.NotApplicable, FailureMechanismSectionAssemblyCategoryGroup.NotApplicable)]
        [TestCase(EFmSectionCategory.Iv, FailureMechanismSectionAssemblyCategoryGroup.Iv)]
        [TestCase(EFmSectionCategory.IIv, FailureMechanismSectionAssemblyCategoryGroup.IIv)]
        [TestCase(EFmSectionCategory.IIIv, FailureMechanismSectionAssemblyCategoryGroup.IIIv)]
        [TestCase(EFmSectionCategory.IVv, FailureMechanismSectionAssemblyCategoryGroup.IVv)]
        [TestCase(EFmSectionCategory.Vv, FailureMechanismSectionAssemblyCategoryGroup.Vv)]
        [TestCase(EFmSectionCategory.VIv, FailureMechanismSectionAssemblyCategoryGroup.VIv)]
        [TestCase(EFmSectionCategory.VIIv, FailureMechanismSectionAssemblyCategoryGroup.VIIv)]
        public void CreateWithFmSectionAssemblyDirectResultWithProbability_ValidResult_ReturnFailureMechanismSectionAssembly(
            EFmSectionCategory originalGroup,
            FailureMechanismSectionAssemblyCategoryGroup expectedGroup)
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();

            var result = new FmSectionAssemblyDirectResultWithProbability(originalGroup, probability);

            // Call
            FailureMechanismSectionAssembly assembly = FailureMechanismSectionAssemblyCreator.Create(result);

            // Assert
            Assert.AreEqual(expectedGroup, assembly.Group);
            Assert.AreEqual(probability, assembly.Probability);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCategoryGroup_InvalidGroup_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup((EFmSectionCategory) 99);

            // Assert
            const string expectedMessage = "The value of argument 'category' (99) is invalid for Enum type 'EFmSectionCategory'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(EFmSectionCategory.Gr, FailureMechanismSectionAssemblyCategoryGroup.None)]
        [TestCase(EFmSectionCategory.NotApplicable, FailureMechanismSectionAssemblyCategoryGroup.NotApplicable)]
        [TestCase(EFmSectionCategory.Iv, FailureMechanismSectionAssemblyCategoryGroup.Iv)]
        [TestCase(EFmSectionCategory.IIv, FailureMechanismSectionAssemblyCategoryGroup.IIv)]
        [TestCase(EFmSectionCategory.IIIv, FailureMechanismSectionAssemblyCategoryGroup.IIIv)]
        [TestCase(EFmSectionCategory.IVv, FailureMechanismSectionAssemblyCategoryGroup.IVv)]
        [TestCase(EFmSectionCategory.Vv, FailureMechanismSectionAssemblyCategoryGroup.Vv)]
        [TestCase(EFmSectionCategory.VIv, FailureMechanismSectionAssemblyCategoryGroup.VIv)]
        [TestCase(EFmSectionCategory.VIIv, FailureMechanismSectionAssemblyCategoryGroup.VIIv)]
        public void CreateFailureMechanismSectionAssemblyCategoryGroup_ValidGroup_ReturnFailureMechanismSectionAssemblyCategoryGroup(
            EFmSectionCategory originalGroup,
            FailureMechanismSectionAssemblyCategoryGroup expectedGroup)
        {
            // Call
            FailureMechanismSectionAssemblyCategoryGroup actualGroup = FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup(originalGroup);

            // Assert
            Assert.AreEqual(expectedGroup, actualGroup);
        }
    }
}