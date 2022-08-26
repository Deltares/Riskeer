// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Integration.IO.Factories;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableFailureMechanismSectionAssemblyGroupConverterTest
    {
        [Test]
        public void ConvertTo_InvalidAssemblyGroup_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const FailureMechanismSectionAssemblyGroup failureMechanismSectionAssemblyGroup = (FailureMechanismSectionAssemblyGroup) 999;

            // ExportableAssemblyMethodFactory
            void Call() => ExportableFailureMechanismSectionAssemblyGroupConverter.ConvertTo(failureMechanismSectionAssemblyGroup);

            // Assert
            var message = $"The value of argument 'failureMechanismSectionAssemblyGroup' ({failureMechanismSectionAssemblyGroup}) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyGroup.NotDominant, ExportableFailureMechanismSectionAssemblyGroup.NotDominant)]
        [TestCase(FailureMechanismSectionAssemblyGroup.III, ExportableFailureMechanismSectionAssemblyGroup.III)]
        [TestCase(FailureMechanismSectionAssemblyGroup.II, ExportableFailureMechanismSectionAssemblyGroup.II)]
        [TestCase(FailureMechanismSectionAssemblyGroup.I, ExportableFailureMechanismSectionAssemblyGroup.I)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Zero, ExportableFailureMechanismSectionAssemblyGroup.Zero)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IMin, ExportableFailureMechanismSectionAssemblyGroup.IMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIMin, ExportableFailureMechanismSectionAssemblyGroup.IIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIIMin, ExportableFailureMechanismSectionAssemblyGroup.IIIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Dominant, ExportableFailureMechanismSectionAssemblyGroup.Dominant)]
        [TestCase(FailureMechanismSectionAssemblyGroup.NoResult, ExportableFailureMechanismSectionAssemblyGroup.NoResult)]
        [TestCase(FailureMechanismSectionAssemblyGroup.NotRelevant, ExportableFailureMechanismSectionAssemblyGroup.NotRelevant)]
        public void ConvertTo_WithAssemblyGroup_ReturnsExpectedValue(FailureMechanismSectionAssemblyGroup failureMechanismSectionAssemblyGroup,
                                                                     ExportableFailureMechanismSectionAssemblyGroup expectedExportableSectionAssemblyGroup)
        {
            // Call
            ExportableFailureMechanismSectionAssemblyGroup convertedAssemblyGroup =
                ExportableFailureMechanismSectionAssemblyGroupConverter.ConvertTo(failureMechanismSectionAssemblyGroup);

            // Assert
            Assert.AreEqual(expectedExportableSectionAssemblyGroup, convertedAssemblyGroup);
        }
    }
}