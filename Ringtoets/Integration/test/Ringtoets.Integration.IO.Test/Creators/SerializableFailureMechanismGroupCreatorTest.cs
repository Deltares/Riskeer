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

using System;
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.Integration.IO.Creators;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableFailureMechanismGroupCreatorTest
    {
        [Test]
        public void Create_InvalidFailureMechanismAssemblyCategoryGroup_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const FailureMechanismAssemblyCategoryGroup groupInput = (FailureMechanismAssemblyCategoryGroup) 999;

            // Call
            TestDelegate call = () => SerializableFailureMechanismCategoryGroupCreator.Create(groupInput);

            // Assert
            string message = $"The value of argument 'categoryGroup' ({(int) groupInput}) is invalid for Enum type '{nameof(FailureMechanismAssemblyCategoryGroup)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, message);
            Assert.AreEqual("categoryGroup", exception.ParamName);
        }

        [Test]
        public void Create_WithFailureMechanismCategoryGroupNone_ThrowsNotSupportedException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismCategoryGroupCreator.Create(FailureMechanismAssemblyCategoryGroup.None);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(FailureMechanismAssemblyCategoryGroup.NotApplicable, SerializableFailureMechanismCategoryGroup.NotApplicable)]
        [TestCase(FailureMechanismAssemblyCategoryGroup.It, SerializableFailureMechanismCategoryGroup.It)]
        [TestCase(FailureMechanismAssemblyCategoryGroup.IIt, SerializableFailureMechanismCategoryGroup.IIt)]
        [TestCase(FailureMechanismAssemblyCategoryGroup.IIIt, SerializableFailureMechanismCategoryGroup.IIIt)]
        [TestCase(FailureMechanismAssemblyCategoryGroup.IVt, SerializableFailureMechanismCategoryGroup.IVt)]
        [TestCase(FailureMechanismAssemblyCategoryGroup.Vt, SerializableFailureMechanismCategoryGroup.Vt)]
        [TestCase(FailureMechanismAssemblyCategoryGroup.VIt, SerializableFailureMechanismCategoryGroup.VIt)]
        [TestCase(FailureMechanismAssemblyCategoryGroup.VIIt, SerializableFailureMechanismCategoryGroup.VIIt)]
        public void Create_WithFailureMechanismCategoryGroup_ReturnsExpectedValues(FailureMechanismAssemblyCategoryGroup categoryGroup,
                                                                                   SerializableFailureMechanismCategoryGroup expectedGroup)
        {
            // Call
            SerializableFailureMechanismCategoryGroup serializableGroup = SerializableFailureMechanismCategoryGroupCreator.Create(categoryGroup);

            // Assert
            Assert.AreEqual(expectedGroup, serializableGroup);
        }
    }
}