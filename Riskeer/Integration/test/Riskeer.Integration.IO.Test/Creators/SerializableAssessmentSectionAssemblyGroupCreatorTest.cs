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
    public class SerializableAssessmentSectionAssemblyGroupCreatorTest
    {
        [Test]
        public void Create_InvalidAssessmentSectionAssemblyGroup_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const AssessmentSectionAssemblyGroup group = (AssessmentSectionAssemblyGroup) 999;

            // Call
            void Call() => SerializableAssessmentSectionAssemblyGroupCreator.Create(group);

            // Assert
            var message = $"The value of argument 'group' ({group}) is invalid for Enum type '{nameof(AssessmentSectionAssemblyGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [TestCase(AssessmentSectionAssemblyGroup.None)]
        [TestCase(AssessmentSectionAssemblyGroup.NotApplicable)]
        public void Create_WithNotSupportedAssessmentSectionAssemblyGroup_ThrowsNotSupportedException(AssessmentSectionAssemblyGroup notSupportedGroup)
        {
            // Call
            void Call() => SerializableAssessmentSectionAssemblyGroupCreator.Create(notSupportedGroup);

            // Assert
            Assert.Throws<NotSupportedException>(Call);
        }

        [Test]
        [TestCase(AssessmentSectionAssemblyGroup.APlus, SerializableAssessmentSectionAssemblyGroup.APlus)]
        [TestCase(AssessmentSectionAssemblyGroup.A, SerializableAssessmentSectionAssemblyGroup.A)]
        [TestCase(AssessmentSectionAssemblyGroup.B, SerializableAssessmentSectionAssemblyGroup.B)]
        [TestCase(AssessmentSectionAssemblyGroup.C, SerializableAssessmentSectionAssemblyGroup.C)]
        [TestCase(AssessmentSectionAssemblyGroup.D, SerializableAssessmentSectionAssemblyGroup.D)]
        public void Create_WithValidAssessmentSectionAssemblyGroup_ReturnExpectedValues(AssessmentSectionAssemblyGroup group,
                                                                                        SerializableAssessmentSectionAssemblyGroup expectedAssemblyGroup)
        {
            // Call
            SerializableAssessmentSectionAssemblyGroup serializableAssemblyGroup = SerializableAssessmentSectionAssemblyGroupCreator.Create(group);

            // Assert
            Assert.AreEqual(expectedAssemblyGroup, serializableAssemblyGroup);
        }
    }
}