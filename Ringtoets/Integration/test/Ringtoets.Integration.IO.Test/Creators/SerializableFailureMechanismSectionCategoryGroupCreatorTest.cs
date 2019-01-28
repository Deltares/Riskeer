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

using System;
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Integration.IO.Creators;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableFailureMechanismSectionCategoryGroupCreatorTest
    {
        [Test]
        public void Create_InvalidFailureMechanismSectionAssemblyCategoryGroup_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const FailureMechanismSectionAssemblyCategoryGroup groupInput = (FailureMechanismSectionAssemblyCategoryGroup) 999;

            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionCategoryGroupCreator.Create(groupInput);

            // Assert
            string message = $"The value of argument 'categoryGroup' ({(int) groupInput}) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyCategoryGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, message);
        }

        [Test]
        public void Create_WithFailureMechanismSectionAssemblyCategoryGroupNone_ThrowsNotSupportedException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionCategoryGroupCreator.Create(FailureMechanismSectionAssemblyCategoryGroup.None);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, SerializableFailureMechanismSectionCategoryGroup.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, SerializableFailureMechanismSectionCategoryGroup.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, SerializableFailureMechanismSectionCategoryGroup.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, SerializableFailureMechanismSectionCategoryGroup.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, SerializableFailureMechanismSectionCategoryGroup.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, SerializableFailureMechanismSectionCategoryGroup.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, SerializableFailureMechanismSectionCategoryGroup.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, SerializableFailureMechanismSectionCategoryGroup.VIIv)]
        public void Create_WithFailureMechanismSectionCategoryGroup_ReturnsExpectedValues(FailureMechanismSectionAssemblyCategoryGroup categoryGroup,
                                                                                          SerializableFailureMechanismSectionCategoryGroup expectedGroup)
        {
            // Call
            SerializableFailureMechanismSectionCategoryGroup serializableGroup = SerializableFailureMechanismSectionCategoryGroupCreator.Create(categoryGroup);

            // Assert
            Assert.AreEqual(expectedGroup, serializableGroup);
        }
    }
}