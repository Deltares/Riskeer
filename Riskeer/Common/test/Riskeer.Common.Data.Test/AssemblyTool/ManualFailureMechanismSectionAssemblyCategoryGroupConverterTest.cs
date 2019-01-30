// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Primitives;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class ManualFailureMechanismSectionAssemblyCategoryGroupConverterTest
    {
        [Test]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.None)]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.NotApplicable, FailureMechanismSectionAssemblyCategoryGroup.NotApplicable)]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.Iv, FailureMechanismSectionAssemblyCategoryGroup.Iv)]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionAssemblyCategoryGroup.IIv)]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.Vv, FailureMechanismSectionAssemblyCategoryGroup.Vv)]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.VIIv, FailureMechanismSectionAssemblyCategoryGroup.VIIv)]
        public void Convert_WithValidManualFailureMechanismSectionAssemblyCategoryGroup_ReturnsExpectedFailureMechanismSectionAssemblyCategoryGroup(
            ManualFailureMechanismSectionAssemblyCategoryGroup originalCategoryGroup,
            FailureMechanismSectionAssemblyCategoryGroup expectedCategoryGroup)
        {
            // Call
            FailureMechanismSectionAssemblyCategoryGroup result =
                ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(originalCategoryGroup);

            // Assert
            Assert.AreEqual(expectedCategoryGroup, result);
        }

        [Test]
        public void Convert_WithInvalidManualFailureMechanismSectionAssemblyCategoryGroup_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const ManualFailureMechanismSectionAssemblyCategoryGroup invalidCategoryGroup = (ManualFailureMechanismSectionAssemblyCategoryGroup) 99;

            // Call
            TestDelegate test = () => ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(invalidCategoryGroup);

            // Assert
            string expectedMessage = $"The value of argument 'categoryGroup' (99) is invalid for Enum type '{nameof(ManualFailureMechanismSectionAssemblyCategoryGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }
    }
}