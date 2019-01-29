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
using System.Drawing;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TestUtil;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class AssemblyCategoryGroupColorHelperTest
    {
        [Test]
        public void GetFailureMechanismSectionAssemblyCategoryGroupColor_WithInvalidFailureMechanismSectionAssemblyCategoryGroup_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => AssemblyCategoryGroupColorHelper.GetFailureMechanismSectionAssemblyCategoryGroupColor(
                (FailureMechanismSectionAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'assemblyCategoryGroup' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCaseSource(typeof(AssemblyCategoryColorTestHelper), nameof(AssemblyCategoryColorTestHelper.FailureMechanismSectionAssemblyCategoryGroupColorCases))]
        public void GetFailureMechanismSectionAssemblyCategoryGroupColor_WithFailureMechanismSectionAssemblyCategoryGroup_ReturnsExpectedColor(
            FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup, Color expectedColor)
        {
            // Call
            Color color = AssemblyCategoryGroupColorHelper.GetFailureMechanismSectionAssemblyCategoryGroupColor(assemblyCategoryGroup);

            // Assert
            Assert.AreEqual(expectedColor, color);
        }

        [Test]
        public void GetAssessmentSectionAssemblyCategoryGroupColor_WithInvalidAssessmentSectionAssemblyCategoryGroup_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => AssemblyCategoryGroupColorHelper.GetAssessmentSectionAssemblyCategoryGroupColor(
                (AssessmentSectionAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'assemblyCategoryGroup' (99) is invalid for Enum type 'AssessmentSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCaseSource(typeof(AssemblyCategoryColorTestHelper), nameof(AssemblyCategoryColorTestHelper.AssessmentSectionAssemblyCategoryGroupColorCases))]
        public void GetAssessmentSectionAssemblyCategoryGroupColor_WithAssessmentSectionAssemblyCategoryGroup_ReturnsExpectedColor(
            AssessmentSectionAssemblyCategoryGroup assemblyCategoryGroup, Color expectedColor)
        {
            // Call
            Color color = AssemblyCategoryGroupColorHelper.GetAssessmentSectionAssemblyCategoryGroupColor(assemblyCategoryGroup);

            // Assert
            Assert.AreEqual(expectedColor, color);
        }

        [Test]
        public void GetFailureMechanismAssemblyCategoryGroupColor_WithInvalidFailureMechanismSectionAssemblyCategoryGroup_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => AssemblyCategoryGroupColorHelper.GetFailureMechanismAssemblyCategoryGroupColor(
                (FailureMechanismAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'assemblyCategoryGroup' (99) is invalid for Enum type 'FailureMechanismAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCaseSource(typeof(AssemblyCategoryColorTestHelper), nameof(AssemblyCategoryColorTestHelper.FailureMechanismAssemblyCategoryGroupColorCases))]
        public void GetFailureMechanismAssemblyCategoryGroupColor_WithFailureMechanismSectionAssemblyCategoryGroup_ReturnsExpectedColor(
            FailureMechanismAssemblyCategoryGroup assemblyCategoryGroup, Color expectedColor)
        {
            // Call
            Color color = AssemblyCategoryGroupColorHelper.GetFailureMechanismAssemblyCategoryGroupColor(assemblyCategoryGroup);

            // Assert
            Assert.AreEqual(expectedColor, color);
        }
    }
}