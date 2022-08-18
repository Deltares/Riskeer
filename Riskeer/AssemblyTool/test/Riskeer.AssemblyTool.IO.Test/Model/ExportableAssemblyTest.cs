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

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class ExportableAssemblyTest
    {
        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Setup
            ExportableAssessmentSection assessmentSection = ExportableAssessmentSectionTestFactory.Create();
            var assessmentProcess = new ExportableAssessmentProcess("id", 0, 0, assessmentSection);

            // Call
            void Call() => new ExportableAssembly(invalidId, assessmentSection, assessmentProcess);

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            ExportableAssessmentSection assessmentSection = ExportableAssessmentSectionTestFactory.Create();
            var assessmentProcess = new ExportableAssessmentProcess("id", 0, 0, assessmentSection);

            // Call
            void Call() => new ExportableAssembly("id", null, assessmentProcess);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentProcessNull_ThrowsArgumentNullException()
        {
            // Setup
            ExportableAssessmentSection assessmentSection = ExportableAssessmentSectionTestFactory.Create();

            // Call
            void Call() => new ExportableAssembly("id", assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentProcess", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string id = "id";
            
            ExportableAssessmentSection assessmentSection = ExportableAssessmentSectionTestFactory.Create();
            var assessmentProcess = new ExportableAssessmentProcess("id2", 0, 0, assessmentSection);

            // Call
            var assembly = new ExportableAssembly(id, assessmentSection, assessmentProcess);

            // Assert
            Assert.AreEqual(id, assembly.Id);
            Assert.AreSame(assessmentSection, assembly.AssessmentSection);
            Assert.AreSame(assessmentProcess, assembly.AssessmentProcess);
        }
    }
}