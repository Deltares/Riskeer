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
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;

namespace Ringtoets.Integration.IO.Test.Creators
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
            TestDelegate call = () => SerializableFailureMechanismTypeCreator.Create(failureMechanismType);

            // Assert
            string message = $"The value of argument 'failureMechanismType' ({(int) failureMechanismType}) is invalid for Enum type '{nameof(ExportableFailureMechanismType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, message);
        }

        [Test]
        [TestCase(ExportableFailureMechanismType.STBI, SerializableFailureMechanismType.STBI)]
        [TestCase(ExportableFailureMechanismType.STBU, SerializableFailureMechanismType.STBU)]
        [TestCase(ExportableFailureMechanismType.STPH, SerializableFailureMechanismType.STPH)]
        [TestCase(ExportableFailureMechanismType.STMI, SerializableFailureMechanismType.STMI)]
        [TestCase(ExportableFailureMechanismType.AGK, SerializableFailureMechanismType.AGK)]
        [TestCase(ExportableFailureMechanismType.AWO, SerializableFailureMechanismType.AWO)]
        [TestCase(ExportableFailureMechanismType.GEBU, SerializableFailureMechanismType.GEBU)]
        [TestCase(ExportableFailureMechanismType.GABU, SerializableFailureMechanismType.GABU)]
        [TestCase(ExportableFailureMechanismType.GEKB, SerializableFailureMechanismType.GEKB)]
        [TestCase(ExportableFailureMechanismType.GABI, SerializableFailureMechanismType.GABI)]
        [TestCase(ExportableFailureMechanismType.ZST, SerializableFailureMechanismType.ZST)]
        [TestCase(ExportableFailureMechanismType.DA, SerializableFailureMechanismType.DA)]
        [TestCase(ExportableFailureMechanismType.HTKW, SerializableFailureMechanismType.HTKW)]
        [TestCase(ExportableFailureMechanismType.BSKW, SerializableFailureMechanismType.BSKW)]
        [TestCase(ExportableFailureMechanismType.PKW, SerializableFailureMechanismType.PKW)]
        [TestCase(ExportableFailureMechanismType.STKWp, SerializableFailureMechanismType.STKWp)]
        [TestCase(ExportableFailureMechanismType.STKWl, SerializableFailureMechanismType.STKWl)]
        [TestCase(ExportableFailureMechanismType.INN, SerializableFailureMechanismType.INN)]
        public void Create_WithFailureMechanismType_ReturnsExpectedValues(ExportableFailureMechanismType failureMechanismType,
                                                                          SerializableFailureMechanismType expectedFailureMechanismType)
        {
            // Call
            SerializableFailureMechanismType serializableFailureMechanismType = SerializableFailureMechanismTypeCreator.Create(failureMechanismType);

            // Assert
            Assert.AreEqual(expectedFailureMechanismType, serializableFailureMechanismType);
        }
    }
}