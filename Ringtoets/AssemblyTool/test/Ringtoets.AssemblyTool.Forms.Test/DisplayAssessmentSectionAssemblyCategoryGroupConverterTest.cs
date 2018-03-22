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
    public class DisplayAssessmentSectionAssemblyCategoryGroupConverterTest
    {
        [Test]
        public void Convert_InvalidValue_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => DisplayAssessmentSectionAssemblyCategoryGroupConverter.Convert((AssessmentSectionAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'categoryGroup' (99) is invalid for Enum type 'AssessmentSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.APlus, DisplayAssessmentSectionAssemblyCategoryGroup.APlus)]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.A, DisplayAssessmentSectionAssemblyCategoryGroup.A)]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.B, DisplayAssessmentSectionAssemblyCategoryGroup.B)]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.C, DisplayAssessmentSectionAssemblyCategoryGroup.C)]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.D, DisplayAssessmentSectionAssemblyCategoryGroup.D)]
        public void Convert_ValidValue_ReturnsConvertedValue(AssessmentSectionAssemblyCategoryGroup categoryGroup,
                                                             DisplayAssessmentSectionAssemblyCategoryGroup expectedDisplayCategoryGroup)
        {
            // Call
            DisplayAssessmentSectionAssemblyCategoryGroup displayCategoryGroup = DisplayAssessmentSectionAssemblyCategoryGroupConverter.Convert(categoryGroup);

            // Assert
            Assert.AreEqual(expectedDisplayCategoryGroup, displayCategoryGroup);
        }
    }
}