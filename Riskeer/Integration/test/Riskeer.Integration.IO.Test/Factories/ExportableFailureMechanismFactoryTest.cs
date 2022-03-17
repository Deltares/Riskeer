﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.TestUtil;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableFailureMechanismFactoryTest
    {
        [Test]
        public void CreateExportableFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var random = new Random(21);

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                null, assessmentSection, (fm, section) => double.NaN, (sr, fm, section) => null, random.NextEnumValue<ExportableFailureMechanismType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                new TestFailureMechanism(), null, (fm, section) => double.NaN, (sr, fm, section) => null, random.NextEnumValue<ExportableFailureMechanismType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanism_AssembleFailureMechanismFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var random = new Random(21);

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                new TestFailureMechanism(), assessmentSection, null, (sr, fm, section) => null, random.NextEnumValue<ExportableFailureMechanismType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assembleFailureMechanismFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableFailureMechanism_AssembleFailureMechanismSectionFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var random = new Random(21);
            
            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                new TestFailureMechanism(), assessmentSection, (fm, section) => double.NaN, null, random.NextEnumValue<ExportableFailureMechanismType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assembleFailureMechanismSectionFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableFailureMechanism_WithValidData_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));
            var assessmentSection = new AssessmentSectionStub();

            double probability = random.NextDouble();
            var expectedSectionOutput = new FailureMechanismSectionAssemblyResult(
                random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

            var failureMechanismType = random.NextEnumValue<ExportableFailureMechanismType>();

            // Call
            ExportableFailureMechanism exportableFailureMechanism =
                ExportableFailureMechanismFactory.CreateExportableFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                    failureMechanism, assessmentSection, (fm, section) => probability,
                    (sr, fm, section) => expectedSectionOutput, failureMechanismType);

            // Assert
            Assert.AreEqual(failureMechanismType, exportableFailureMechanism.FailureMechanismType);
            Assert.AreEqual(failureMechanism.Code, exportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssembly = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(probability, exportableFailureMechanismAssembly.Probability);
            Assert.IsFalse(exportableFailureMechanismAssembly.IsManual);

            IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections = exportableFailureMechanism.SectionAssemblyResults
                                                                                                                          .Select(sar => sar.FailureMechanismSection);
            ExportableFailureMechanismSectionTestHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, exportableFailureMechanismSections);

            ExportableFailureMechanismSectionAssemblyResultTestHelper.AssertExportableFailureMechanismSectionResults(
                expectedSectionOutput, exportableFailureMechanismSections, exportableFailureMechanism.SectionAssemblyResults);
        }
    }
}