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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Forms.Helpers;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCategoryGroupHelperTest
    {
        [Test]
        public void GetCategoryGroupDisplayName_InvalidValue_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName((FailureMechanismSectionAssemblyCategoryGroup) 99);

            // Assert
            string expectedMessage = $"The value of argument 'assemblyCategoryGroup' (99) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyCategoryGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, "")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, "-")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, "Iv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, "IIv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, "IIIv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, "IVv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, "Vv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, "VIv")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, "VIIv")]
        public void GetCategoryGroupDisplayName_ValidValue_ReturnsDisplayName(FailureMechanismSectionAssemblyCategoryGroup categoryGroup,
                                                                              string expectedDisplayName)
        {
            // Call
            string displayName = FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(categoryGroup);

            // Assert
            Assert.AreEqual(expectedDisplayName, displayName);
        }
    }
}