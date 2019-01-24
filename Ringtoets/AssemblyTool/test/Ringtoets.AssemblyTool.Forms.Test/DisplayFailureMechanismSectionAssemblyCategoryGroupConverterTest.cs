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
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.Forms.Test
{
    [TestFixture]
    public class DisplayFailureMechanismSectionAssemblyCategoryGroupConverterTest
    {
        [Test]
        public void Convert_InvalidValue_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => DisplayFailureMechanismSectionAssemblyCategoryGroupConverter.Convert((FailureMechanismSectionAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'categoryGroup' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, DisplayFailureMechanismSectionAssemblyCategoryGroup.None)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, DisplayFailureMechanismSectionAssemblyCategoryGroup.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, DisplayFailureMechanismSectionAssemblyCategoryGroup.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, DisplayFailureMechanismSectionAssemblyCategoryGroup.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, DisplayFailureMechanismSectionAssemblyCategoryGroup.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, DisplayFailureMechanismSectionAssemblyCategoryGroup.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, DisplayFailureMechanismSectionAssemblyCategoryGroup.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, DisplayFailureMechanismSectionAssemblyCategoryGroup.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, DisplayFailureMechanismSectionAssemblyCategoryGroup.VIIv)]
        public void Convert_ValidValue_ReturnsConvertedValue(FailureMechanismSectionAssemblyCategoryGroup categoryGroup,
                                                             DisplayFailureMechanismSectionAssemblyCategoryGroup expectedDisplayCategoryGroup)
        {
            // Call
            DisplayFailureMechanismSectionAssemblyCategoryGroup displayCategoryGroup = DisplayFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(categoryGroup);

            // Assert
            Assert.AreEqual(expectedDisplayCategoryGroup, displayCategoryGroup);
        }
    }
}