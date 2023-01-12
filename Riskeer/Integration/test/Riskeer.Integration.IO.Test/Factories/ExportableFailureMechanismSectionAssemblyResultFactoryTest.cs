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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Data.TestUtil;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;
using Riskeer.Common.Data.FailureMechanism;
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
                null, new ExportableModelRegistry(), failureMechanism.SectionResults.First(),
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
                new IdentifierGenerator(), new ExportableModelRegistry(), null,
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
                new IdentifierGenerator(), new ExportableModelRegistry(), failureMechanism.SectionResults.First(),
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
                new IdentifierGenerator(), new ExportableModelRegistry(), failureMechanism.SectionResults.First(),
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
                new IdentifierGenerator(), new ExportableModelRegistry(), failureMechanism.SectionResults.First(),
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
            var assessmentSection = new AssessmentSectionStub();

            // Call
            void Call() => ExportableFailureMechanismSectionAssemblyResultFactory.Create(
                new IdentifierGenerator(), new ExportableModelRegistry(),
                new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                failureMechanism, assessmentSection, (sr, fm, section) => new FailureMechanismSectionAssemblyResultWrapper(
                    new FailureMechanismSectionAssemblyResult(random.NextDouble(), random.NextDouble(), random.NextDouble(), assemblyGroup),
                    random.NextEnumValue<AssemblyMethod>(), random.NextEnumValue<AssemblyMethod>()));

            // Assert
            var exception = Assert.Throws<AssemblyFactoryException>(Call);
            Assert.AreEqual("The assembly result is invalid and cannot be created.", exception.Message);
        }

        [Test]
        public void Create_ValidData_ReturnsExportableFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var registry = new ExportableModelRegistry();
            ExportableFailureMechanismSection exportableSection =
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();
            registry.Register(failureMechanismSection, exportableSection);

            var idGenerator = new IdentifierGenerator();
            var sectionResult = new TestFailureMechanismSectionResult(failureMechanismSection);
            FailureMechanismSectionAssemblyResultWrapper expectedSectionOutput = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create();

            // Call
            ExportableFailureMechanismSectionAssemblyResult assemblyResult = ExportableFailureMechanismSectionAssemblyResultFactory.Create(
                idGenerator, registry, sectionResult, failureMechanism, assessmentSection,
                (sr, fm, section) => expectedSectionOutput);

            // Assert
            ExportableFailureMechanismSectionAssemblyResultAssertHelper.AssertExportableFailureMechanismSectionResult(
                expectedSectionOutput, assemblyResult, exportableSection);
        }

        [Test]
        public void Create_SectionResultAlreadyRegistered_ReturnsRegisteredExportableFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));
            var assessmentSection = new AssessmentSectionStub();

            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var registry = new ExportableModelRegistry();
            ExportableFailureMechanismSection exportableSection =
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();
            registry.Register(failureMechanismSection, exportableSection);

            var idGenerator = new IdentifierGenerator();
            var sectionResult = new TestFailureMechanismSectionResult(failureMechanismSection);
            FailureMechanismSectionAssemblyResultWrapper expectedSectionOutput = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create();

            ExportableFailureMechanismSectionAssemblyResult assemblyResult1 = ExportableFailureMechanismSectionAssemblyResultFactory.Create(
                idGenerator, registry, sectionResult, failureMechanism, assessmentSection,
                (sr, fm, section) => expectedSectionOutput);

            // Precondition
            Assert.True(registry.Contains(sectionResult));

            // Call
            ExportableFailureMechanismSectionAssemblyResult assemblyResult2 = ExportableFailureMechanismSectionAssemblyResultFactory.Create(
                idGenerator, registry, sectionResult, failureMechanism, assessmentSection,
                (sr, fm, section) => expectedSectionOutput);

            // Assert
            Assert.AreSame(assemblyResult1, assemblyResult2);
        }
    }
}