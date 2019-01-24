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

using System.ComponentModel;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class AssemblyCalculatorInputCreatorTest
    {
        [Test]
        public void CreateFailureMechanismSectionCategory_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => AssemblyCalculatorInputCreator.CreateFailureMechanismSectionCategory(
                (FailureMechanismSectionAssemblyCategoryGroup) 99);

            // Assert
            string expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyCategoryGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, EFmSectionCategory.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, EFmSectionCategory.Gr)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, EFmSectionCategory.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, EFmSectionCategory.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, EFmSectionCategory.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, EFmSectionCategory.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, EFmSectionCategory.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, EFmSectionCategory.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, EFmSectionCategory.VIIv)]
        public void CreateFailureMechanismSectionCategory_WithValidGroup_ReturnEFmSectionCategory(
            FailureMechanismSectionAssemblyCategoryGroup originalGroup,
            EFmSectionCategory expectedGroup)
        {
            // Call
            EFmSectionCategory actualGroup = AssemblyCalculatorInputCreator.CreateFailureMechanismSectionCategory(
                originalGroup);

            // Assert
            Assert.AreEqual(expectedGroup, actualGroup);
        }
    }
}