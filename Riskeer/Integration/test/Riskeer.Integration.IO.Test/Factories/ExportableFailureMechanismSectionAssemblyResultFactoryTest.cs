﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Data.TestUtil;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableFailureMechanismSectionAssemblyResultFactoryTest
    {
        [Test]
        public void Create_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));
            var assessmentSection = new AssessmentSectionStub();

            // Call
            void Call() => ExportableFailureMechanismSectionAssemblyResultFactory.Create(
                null, new ExportableFailureMechanismSectionRegistry(), failureMechanism.SectionResults.First(),
                failureMechanism, assessmentSection, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));
            var assessmentSection = new AssessmentSectionStub();

            // Call
            void Call() => ExportableFailureMechanismSectionAssemblyResultFactory.Create(
                new IdentifierGenerator(), null, failureMechanism.SectionResults.First(),
                failureMechanism, assessmentSection, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));
            var assessmentSection = new AssessmentSectionStub();

            // Call
            void Call() => ExportableFailureMechanismSectionAssemblyResultFactory.Create<TestFailureMechanismSectionResult, TestFailureMechanism>(
                new IdentifierGenerator(), new ExportableFailureMechanismSectionRegistry(), null,
                failureMechanism, assessmentSection, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void Create_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));
            var assessmentSection = new AssessmentSectionStub();

            // Call
            void Call() => ExportableFailureMechanismSectionAssemblyResultFactory.Create<TestFailureMechanismSectionResult, TestFailureMechanism>(
                new IdentifierGenerator(), new ExportableFailureMechanismSectionRegistry(), failureMechanism.SectionResults.First(),
                null, assessmentSection, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            // Call
            void Call() => ExportableFailureMechanismSectionAssemblyResultFactory.Create(
                new IdentifierGenerator(), new ExportableFailureMechanismSectionRegistry(), failureMechanism.SectionResults.First(),
                failureMechanism, null, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Create_AssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));
            var assessmentSection = new AssessmentSectionStub();

            // Call
            void Call() => ExportableFailureMechanismSectionAssemblyResultFactory.Create(
                new IdentifierGenerator(), new ExportableFailureMechanismSectionRegistry(), failureMechanism.SectionResults.First(),
                failureMechanism, assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assemblyFunc", exception.ParamName);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyGroup.NoResult)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Dominant)]
        public void Create_InvalidFailureMechanismSectionAssemblyResult_ThrowsAssemblyFactoryException(
            FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));
            var assessmentSection = new AssessmentSectionStub();

            // Call
            void Call() => ExportableFailureMechanismSectionAssemblyResultFactory.Create(
                new IdentifierGenerator(), new ExportableFailureMechanismSectionRegistry(), failureMechanism.SectionResults.First(),
                failureMechanism, assessmentSection, (sr, fm, section) => new FailureMechanismSectionAssemblyResultWrapper(
                    new FailureMechanismSectionAssemblyResult(
                        random.NextDouble(), random.NextDouble(), random.NextDouble(), assemblyGroup),
                    random.NextEnumValue<AssemblyMethod>(), random.NextEnumValue<AssemblyMethod>()));

            // Assert
            var exception = Assert.Throws<AssemblyFactoryException>(Call);
            Assert.AreEqual("The assembly result is invalid and cannot be created.", exception.Message);
        }

        [Test]
        public void Create_ValidData_ReturnsExportableFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));
            var assessmentSection = new AssessmentSectionStub();

            FailureMechanismSectionAssemblyResultWrapper expectedSectionOutput = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create();

            var idGenerator = new IdentifierGenerator();

            var registry = new ExportableFailureMechanismSectionRegistry();
            ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                new IdentifierGenerator(), registry, failureMechanism.Sections);

            // Call
            ExportableFailureMechanismSectionAssemblyResult assemblyResult = ExportableFailureMechanismSectionAssemblyResultFactory.Create(
                idGenerator, registry, failureMechanism.SectionResults.First(), failureMechanism, assessmentSection,
                (sr, fm, section) => expectedSectionOutput);

            // Assert
            ExportableFailureMechanismSectionAssemblyResultTestHelper.AssertExportableFailureMechanismSectionResult(
                expectedSectionOutput, assemblyResult, registry.Get(failureMechanism.Sections.First()));
        }
    }
}