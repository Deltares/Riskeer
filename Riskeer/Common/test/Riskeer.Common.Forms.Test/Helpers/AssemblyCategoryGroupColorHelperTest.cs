// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Drawing;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TestUtil;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class AssemblyCategoryGroupColorHelperTest
    {
        [Test]
        public void GetAssessmentSectionAssemblyCategoryGroupColor_WithInvalidAssessmentSectionAssemblyCategoryGroup_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => AssemblyCategoryGroupColorHelper.GetAssessmentSectionAssemblyCategoryGroupColor(
                (AssessmentSectionAssemblyGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'assemblyCategoryGroup' (99) is invalid for Enum type 'AssessmentSectionAssemblyGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCaseSource(typeof(AssemblyCategoryColorTestHelper), nameof(AssemblyCategoryColorTestHelper.AssessmentSectionAssemblyCategoryGroupColorCases))]
        public void GetAssessmentSectionAssemblyCategoryGroupColor_WithAssessmentSectionAssemblyCategoryGroup_ReturnsExpectedColor(
            AssessmentSectionAssemblyGroup assemblyCategoryGroup, Color expectedColor)
        {
            // Call
            Color color = AssemblyCategoryGroupColorHelper.GetAssessmentSectionAssemblyCategoryGroupColor(assemblyCategoryGroup);

            // Assert
            Assert.AreEqual(expectedColor, color);
        }
    }
}