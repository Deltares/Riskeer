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

namespace Riskeer.AssemblyTool.Forms.Test
{
    [TestFixture]
    public class DisplayFailureMechanismSectionAssemblyGroupConverterTest
    {
         [Test]
        public void Convert_InvalidValue_ThrowsInvalidEnumArgumentException()
        {
            // Call
            void Call() => DisplayFailureMechanismSectionAssemblyGroupConverter.Convert((FailureMechanismSectionAssemblyGroup) 99);

            // Assert
            var expectedMessage = $"The value of argument 'assemblyGroup' (99) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyGroup.NotDominant, DisplayFailureMechanismSectionAssemblyGroup.ND)]
        [TestCase(FailureMechanismSectionAssemblyGroup.III, DisplayFailureMechanismSectionAssemblyGroup.III)]
        [TestCase(FailureMechanismSectionAssemblyGroup.II, DisplayFailureMechanismSectionAssemblyGroup.II)]
        [TestCase(FailureMechanismSectionAssemblyGroup.I, DisplayFailureMechanismSectionAssemblyGroup.I)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Zero, DisplayFailureMechanismSectionAssemblyGroup.Zero)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IMin, DisplayFailureMechanismSectionAssemblyGroup.IMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIMin, DisplayFailureMechanismSectionAssemblyGroup.IIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIIMin, DisplayFailureMechanismSectionAssemblyGroup.IIIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Dominant, DisplayFailureMechanismSectionAssemblyGroup.D)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Gr, DisplayFailureMechanismSectionAssemblyGroup.GR)]
        public void Convert_ValidValue_ReturnsConvertedValue(FailureMechanismSectionAssemblyGroup categoryGroup,
                                                             DisplayFailureMechanismSectionAssemblyGroup expectedDisplayCategoryGroup)
        {
            // Call
            DisplayFailureMechanismSectionAssemblyGroup displayAssemblyGroup = DisplayFailureMechanismSectionAssemblyGroupConverter.Convert(categoryGroup);

            // Assert
            Assert.AreEqual(expectedDisplayCategoryGroup, displayAssemblyGroup);
        }
    }
}