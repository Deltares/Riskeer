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

using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.Forms.Test
{
    [TestFixture]
    public class SelectableFailureMechanismSectionAssemblyCategoryGroupConverterTest
    {
        [Test]
        public void ConvertTo_InvalidValue_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertTo((FailureMechanismSectionAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'categoryGroup' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, SelectableFailureMechanismSectionAssemblyCategoryGroup.None)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, SelectableFailureMechanismSectionAssemblyCategoryGroup.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, SelectableFailureMechanismSectionAssemblyCategoryGroup.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, SelectableFailureMechanismSectionAssemblyCategoryGroup.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, SelectableFailureMechanismSectionAssemblyCategoryGroup.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, SelectableFailureMechanismSectionAssemblyCategoryGroup.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, SelectableFailureMechanismSectionAssemblyCategoryGroup.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, SelectableFailureMechanismSectionAssemblyCategoryGroup.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, SelectableFailureMechanismSectionAssemblyCategoryGroup.VIIv)]
        public void ConvertTo_ValidValue_ReturnsConvertedValue(FailureMechanismSectionAssemblyCategoryGroup categoryGroup,
                                                               SelectableFailureMechanismSectionAssemblyCategoryGroup expectedDisplayCategoryGroup)
        {
            // Call
            SelectableFailureMechanismSectionAssemblyCategoryGroup displayCategoryGroup = SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertTo(
                categoryGroup);

            // Assert
            Assert.AreEqual(expectedDisplayCategoryGroup, displayCategoryGroup);
        }
    }
}