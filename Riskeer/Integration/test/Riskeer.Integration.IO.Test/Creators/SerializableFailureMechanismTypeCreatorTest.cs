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
using Riskeer.AssemblyTool.IO.Assembly;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Integration.IO.Creators;

namespace Riskeer.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableFailureMechanismTypeCreatorTest
    {
        [Test]
        public void Create_InvalidFailureMechanismType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const ExportableFailureMechanismType failureMechanismType = (ExportableFailureMechanismType) 999;

            // Call
            void Call() => SerializableFailureMechanismTypeCreator.Create(failureMechanismType);

            // Assert
            var message = $"The value of argument 'failureMechanismType' ({failureMechanismType}) is invalid for Enum type '{nameof(ExportableFailureMechanismType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [TestCase(ExportableFailureMechanismType.Generic, SerializableFailureMechanismType.Generic)]
        [TestCase(ExportableFailureMechanismType.Specific, SerializableFailureMechanismType.Specific)]
        public void Create_WithFailureMechanismType_ReturnsExpectedValues(
            ExportableFailureMechanismType failureMechanismType, SerializableFailureMechanismType expectedFailureMechanismType)
        {
            // Call
            SerializableFailureMechanismType serializableFailureMechanismType = SerializableFailureMechanismTypeCreator.Create(
                failureMechanismType);

            // Assert
            Assert.AreEqual(expectedFailureMechanismType, serializableFailureMechanismType);
        }
    }
}