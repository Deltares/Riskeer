// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
    public class SerializableAssessmentSectionCategoryGroupCreatorTest
    {
        [Test]
        public void Create_InvalidAssessmentSectionAssemblyCategoryGroup_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const AssessmentSectionAssemblyCategoryGroup groupInput = (AssessmentSectionAssemblyCategoryGroup) 999;

            // Call
            TestDelegate call = () => SerializableAssessmentSectionCategoryGroupCreator.Create(groupInput);

            // Assert
            string message = $"The value of argument 'categoryGroup' ({(int) groupInput}) is invalid for Enum type '{nameof(AssessmentSectionAssemblyCategoryGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, message);
        }

        [Test]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.None)]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.NotApplicable)]
        public void Create_WithNotSupportedInvalidCategoryGroup_ThrowsNotSupportedException(AssessmentSectionAssemblyCategoryGroup notSupportedCategoryGroup)
        {
            // Call
            TestDelegate call = () => SerializableAssessmentSectionCategoryGroupCreator.Create(notSupportedCategoryGroup);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.APlus, SerializableAssessmentSectionCategoryGroup.APlus)]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.A, SerializableAssessmentSectionCategoryGroup.A)]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.B, SerializableAssessmentSectionCategoryGroup.B)]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.C, SerializableAssessmentSectionCategoryGroup.C)]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.D, SerializableAssessmentSectionCategoryGroup.D)]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.NotAssessed, SerializableAssessmentSectionCategoryGroup.NotAssessed)]
        public void Create_WithValidAssessmentSectionAssemblyCategoryGroup_ReturnExpectedValues(AssessmentSectionAssemblyCategoryGroup categoryGroup,
                                                                                                SerializableAssessmentSectionCategoryGroup expectedGroup)
        {
            // Call
            SerializableAssessmentSectionCategoryGroup serializableGroup = SerializableAssessmentSectionCategoryGroupCreator.Create(categoryGroup);

            // Assert
            Assert.AreEqual(expectedGroup, serializableGroup);
        }
    }
}