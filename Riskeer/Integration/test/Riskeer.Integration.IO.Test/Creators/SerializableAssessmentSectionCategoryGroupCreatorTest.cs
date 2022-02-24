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
    public class SerializableAssessmentSectionCategoryGroupCreatorTest
    {
        [Test]
        public void Create_InvalidAssessmentSectionAssemblyCategoryGroup_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const AssessmentSectionAssemblyGroup groupInput = (AssessmentSectionAssemblyGroup) 999;

            // Call
            TestDelegate call = () => SerializableAssessmentSectionCategoryGroupCreator.Create(groupInput);

            // Assert
            string message = $"The value of argument 'categoryGroup' ({groupInput}) is invalid for Enum type '{nameof(AssessmentSectionAssemblyGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, message);
        }

        [Test]
        [TestCase(AssessmentSectionAssemblyGroup.None)]
        [TestCase(AssessmentSectionAssemblyGroup.NotApplicable)]
        public void Create_WithNotSupportedInvalidCategoryGroup_ThrowsNotSupportedException(AssessmentSectionAssemblyGroup notSupportedCategoryGroup)
        {
            // Call
            TestDelegate call = () => SerializableAssessmentSectionCategoryGroupCreator.Create(notSupportedCategoryGroup);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(AssessmentSectionAssemblyGroup.APlus, SerializableAssessmentSectionCategoryGroup.APlus)]
        [TestCase(AssessmentSectionAssemblyGroup.A, SerializableAssessmentSectionCategoryGroup.A)]
        [TestCase(AssessmentSectionAssemblyGroup.B, SerializableAssessmentSectionCategoryGroup.B)]
        [TestCase(AssessmentSectionAssemblyGroup.C, SerializableAssessmentSectionCategoryGroup.C)]
        [TestCase(AssessmentSectionAssemblyGroup.D, SerializableAssessmentSectionCategoryGroup.D)]
        [TestCase(AssessmentSectionAssemblyGroup.NotAssessed, SerializableAssessmentSectionCategoryGroup.NotAssessed)]
        public void Create_WithValidAssessmentSectionAssemblyCategoryGroup_ReturnExpectedValues(AssessmentSectionAssemblyGroup categoryGroup,
                                                                                                SerializableAssessmentSectionCategoryGroup expectedGroup)
        {
            // Call
            SerializableAssessmentSectionCategoryGroup serializableGroup = SerializableAssessmentSectionCategoryGroupCreator.Create(categoryGroup);

            // Assert
            Assert.AreEqual(expectedGroup, serializableGroup);
        }
    }
}