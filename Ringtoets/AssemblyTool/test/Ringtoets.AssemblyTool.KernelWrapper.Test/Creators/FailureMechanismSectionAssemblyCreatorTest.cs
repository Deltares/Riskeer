// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using AssemblyTool.Kernel.Data;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.Common.Data.AssemblyTool;

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
            var result = new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN);

            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCreator.Create(result);

            // Assert
            const string expectedMessage = "The value of argument 'originalGroup' (99) is invalid for Enum type 'FailureMechanismSectionCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.None)]
        [TestCase(FailureMechanismSectionCategoryGroup.Iv, FailureMechanismSectionAssemblyCategoryGroup.Iv)]
        [TestCase(FailureMechanismSectionCategoryGroup.IIv, FailureMechanismSectionAssemblyCategoryGroup.IIv)]
        [TestCase(FailureMechanismSectionCategoryGroup.IIIv, FailureMechanismSectionAssemblyCategoryGroup.IIIv)]
        [TestCase(FailureMechanismSectionCategoryGroup.IVv, FailureMechanismSectionAssemblyCategoryGroup.IVv)]
        [TestCase(FailureMechanismSectionCategoryGroup.Vv, FailureMechanismSectionAssemblyCategoryGroup.Vv)]
        [TestCase(FailureMechanismSectionCategoryGroup.VIv, FailureMechanismSectionAssemblyCategoryGroup.VIv)]
        [TestCase(FailureMechanismSectionCategoryGroup.VIIv, FailureMechanismSectionAssemblyCategoryGroup.VIIv)]
        public void Create_ValidResult_ReturnFailureMechanismSectionAssembly(FailureMechanismSectionCategoryGroup originalGroup,
                                                                             FailureMechanismSectionAssemblyCategoryGroup expectedGroup)
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();

            var result = new FailureMechanismSectionAssemblyCategoryResult(originalGroup, new Probability(probability));

            // Call
            FailureMechanismSectionAssembly assembly = FailureMechanismSectionAssemblyCreator.Create(result);

            // Assert
            Assert.AreEqual(expectedGroup, assembly.Group);
            Assert.AreEqual(probability, assembly.Probability);
        }
    }
}