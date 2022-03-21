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

using System;
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Integration.IO.Creators;

namespace Riskeer.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableFailureMechanismSectionAssemblyGroupCreatorTest
    {
        [Test]
        public void Create_InvalidFailureMechanismSectionAssemblyGroup_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const FailureMechanismSectionAssemblyGroup assemblyGroup = (FailureMechanismSectionAssemblyGroup) 999;

            // Call
            void Call() => SerializableFailureMechanismSectionAssemblyGroupCreator.Create(assemblyGroup);

            // Assert
            var message = $"The value of argument 'assemblyGroup' ({assemblyGroup}) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyGroup.Gr)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Dominant)]
        public void Create_NotSupportedFailureMechanismSectionAssemblyGroup_ThrowsNotSupportedException(
            FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            // Call
            void Call() => SerializableFailureMechanismSectionAssemblyGroupCreator.Create(assemblyGroup);

            // Assert
            Assert.Throws<NotSupportedException>(Call);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyGroup.NotDominant, SerializableFailureMechanismSectionAssemblyGroup.NotDominant)]
        [TestCase(FailureMechanismSectionAssemblyGroup.III, SerializableFailureMechanismSectionAssemblyGroup.III)]
        [TestCase(FailureMechanismSectionAssemblyGroup.II, SerializableFailureMechanismSectionAssemblyGroup.II)]
        [TestCase(FailureMechanismSectionAssemblyGroup.I, SerializableFailureMechanismSectionAssemblyGroup.I)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Zero, SerializableFailureMechanismSectionAssemblyGroup.Zero)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IMin, SerializableFailureMechanismSectionAssemblyGroup.IMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIMin, SerializableFailureMechanismSectionAssemblyGroup.IIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIIMin, SerializableFailureMechanismSectionAssemblyGroup.IIIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.NotRelevant, SerializableFailureMechanismSectionAssemblyGroup.NotRelevant)]
        public void Create_WithFailureMechanismSectionCategoryGroup_ReturnsExpectedValues(
            FailureMechanismSectionAssemblyGroup assemblyGroup, SerializableFailureMechanismSectionAssemblyGroup expectedSerializableAssemblyGroup)
        {
            // Call
            SerializableFailureMechanismSectionAssemblyGroup serializableGroup = SerializableFailureMechanismSectionAssemblyGroupCreator.Create(assemblyGroup);

            // Assert
            Assert.AreEqual(expectedSerializableAssemblyGroup, serializableGroup);
        }
    }
}