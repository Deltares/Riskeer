﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    public class AssemblyGroupColorHelperTest
    {
        [Test]
        public void GetFailureMechanismSectionAssemblyGroupColor_InvalidFailureMechanismSectionAssemblyGroup_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const FailureMechanismSectionAssemblyGroup assemblyGroup = (FailureMechanismSectionAssemblyGroup) 99;

            // Call
            void Call() => AssemblyGroupColorHelper.GetFailureMechanismSectionAssemblyGroupColor(assemblyGroup);

            // Assert
            var expectedMessage = $"The value of argument 'assemblyGroup' ({assemblyGroup}) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCaseSource(typeof(AssemblyGroupColorTestHelper), nameof(AssemblyGroupColorTestHelper.FailureMechanismSectionAssemblyGroupColorCases))]
        public void GetFailureMechanismSectionAssemblyGroupColor_ValidFailureMechanismSectionAssemblyGroup_ReturnsExpectedColor(
            FailureMechanismSectionAssemblyGroup assemblyGroup, Color expectedColor)
        {
            // Call
            Color color = AssemblyGroupColorHelper.GetFailureMechanismSectionAssemblyGroupColor(assemblyGroup);

            // Assert
            Assert.AreEqual(expectedColor, color);
        }
    }
}