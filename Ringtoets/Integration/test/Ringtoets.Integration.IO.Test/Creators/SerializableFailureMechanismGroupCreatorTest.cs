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
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableFailureMechanismGroupCreatorTest
    {
        [Test]
        public void Create_InvalidFailureMechanismType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const ExportableFailureMechanismGroup failureMechanismGroup = (ExportableFailureMechanismGroup) 999;

            // Call
            TestDelegate call = () => SerializableFailureMechanismGroupCreator.Create(failureMechanismGroup);

            // Assert
            string message = $"The value of argument 'failureMechanismGroup' ({(int) failureMechanismGroup}) is invalid for Enum type '{nameof(ExportableFailureMechanismGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, message);
        }

        [Test]
        [TestCase(ExportableFailureMechanismGroup.Group1, SerializableFailureMechanismGroup.Group1)]
        [TestCase(ExportableFailureMechanismGroup.Group2, SerializableFailureMechanismGroup.Group2)]
        [TestCase(ExportableFailureMechanismGroup.Group3, SerializableFailureMechanismGroup.Group3)]
        [TestCase(ExportableFailureMechanismGroup.Group4, SerializableFailureMechanismGroup.Group4)]
        public void Create_WithFailureMechanismType_ReturnsExpectedValues(ExportableFailureMechanismGroup failureMechanismGroup,
                                                                          SerializableFailureMechanismGroup expectedFailureMechanismType)
        {
            // Call
            SerializableFailureMechanismGroup serializableGroup = SerializableFailureMechanismGroupCreator.Create(failureMechanismGroup);

            // Assert
            Assert.AreEqual(expectedFailureMechanismType, serializableGroup);
        }
    }
}