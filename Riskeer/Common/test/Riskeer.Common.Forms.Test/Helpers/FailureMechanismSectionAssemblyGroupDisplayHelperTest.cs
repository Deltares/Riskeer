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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyGroupDisplayHelperTest
    {
        [Test]
        public void GetAssemblyGroupDisplayName_InvalidValue_ThrowsInvalidEnumArgumentException()
        {
            // Call
            void Call() => FailureMechanismSectionAssemblyGroupDisplayHelper.GetAssemblyGroupDisplayName((FailureMechanismSectionAssemblyGroup) 99);

            // Assert
            var expectedMessage = $"The value of argument 'assemblyGroup' (99) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyGroup.NotDominant, "ND")]
        [TestCase(FailureMechanismSectionAssemblyGroup.III, "+III")]
        [TestCase(FailureMechanismSectionAssemblyGroup.II, "+II")]
        [TestCase(FailureMechanismSectionAssemblyGroup.I, "+I")]
        [TestCase(FailureMechanismSectionAssemblyGroup.Zero, "0")]
        [TestCase(FailureMechanismSectionAssemblyGroup.IMin, "-I")]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIMin, "-II")]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIIMin, "-III")]
        [TestCase(FailureMechanismSectionAssemblyGroup.Dominant, "D")]
        [TestCase(FailureMechanismSectionAssemblyGroup.Gr, "")]
        public void GetAssemblyGroupDisplayName_ValidValue_ReturnsDisplayName(FailureMechanismSectionAssemblyGroup categoryGroup,
                                                                              string expectedDisplayName)
        {
            // Call
            string displayName = FailureMechanismSectionAssemblyGroupDisplayHelper.GetAssemblyGroupDisplayName(categoryGroup);

            // Assert
            Assert.AreEqual(expectedDisplayName, displayName);
        }
    }
}