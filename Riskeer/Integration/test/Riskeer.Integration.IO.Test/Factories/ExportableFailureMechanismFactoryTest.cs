// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.AssemblyTool.Data.TestUtil;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.IO.Converters;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableFailureMechanismFactoryTest
    {
        [Test]
        public void CreateExportableGenericFailureMechanism_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableGenericFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                null, new ExportableModelRegistry(), new TestFailureMechanism(), assessmentSection, (fm, section) => null, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableGenericFailureMechanism_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableGenericFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                new IdentifierGenerator(), null, new TestFailureMechanism(), assessmentSection, (fm, section) => null, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableGenericFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableGenericFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                new IdentifierGenerator(), new ExportableModelRegistry(), null, assessmentSection, (fm, section) => null, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableGenericFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableGenericFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                new IdentifierGenerator(), new ExportableModelRegistry(), new TestFailureMechanism(), null, (fm, section) => null, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableGenericFailureMechanism_AssembleFailureMechanismFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableGenericFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                new IdentifierGenerator(), new ExportableModelRegistry(), new TestFailureMechanism(), assessmentSection, null, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assembleFailureMechanismFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableGenericFailureMechanism_AssembleFailureMechanismSectionFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableGenericFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                new IdentifierGenerator(), new ExportableModelRegistry(), new TestFailureMechanism(), assessmentSection, (fm, section) => null, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assembleFailureMechanismSectionFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableGenericFailureMechanism_WithValidData_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));
            var assessmentSection = new AssessmentSectionStub();

            double probability = random.NextDouble();
            var assemblyMethod = random.NextEnumValue<AssemblyMethod>();
            FailureMechanismSectionAssemblyResultWrapper expectedSectionOutput = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create();

            var idGenerator = new IdentifierGenerator();

            var registry = new ExportableModelRegistry();
            RegisterFailureMechanismSections(registry, failureMechanism.Sections);

            // Call
            ExportableGenericFailureMechanism exportableFailureMechanism =
                ExportableFailureMechanismFactory.CreateExportableGenericFailureMechanism<TestFailureMechanism, TestFailureMechanismSectionResult>(
                    idGenerator, registry, failureMechanism, assessmentSection, (fm, section) => new FailureMechanismAssemblyResultWrapper(probability, assemblyMethod),
                    (sr, fm, section) => expectedSectionOutput);

            // Assert
            Assert.AreEqual("Fm.0", exportableFailureMechanism.Id);
            Assert.AreEqual(failureMechanism.Code, exportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssembly = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(probability, exportableFailureMechanismAssembly.Probability);
            Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(assemblyMethod), exportableFailureMechanismAssembly.AssemblyMethod);

            IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections = exportableFailureMechanism.SectionAssemblyResults
                                                                                                                          .Select(sar => sar.FailureMechanismSection);
            ExportableFailureMechanismSectionAssertHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, exportableFailureMechanismSections);

            ExportableFailureMechanismSectionAssemblyResultAssertHelper.AssertExportableFailureMechanismSectionResults(
                expectedSectionOutput, exportableFailureMechanismSections, exportableFailureMechanism.SectionAssemblyResults);
        }

        [Test]
        public void CreateExportableSpecificFailureMechanism_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableSpecificFailureMechanism(
                null, new ExportableModelRegistry(), new SpecificFailureMechanism(), assessmentSection,
                (fm, section) => null, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableSpecificFailureMechanism_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableSpecificFailureMechanism(
                new IdentifierGenerator(), null, new SpecificFailureMechanism(), assessmentSection,
                (fm, section) => null, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableSpecificFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableSpecificFailureMechanism(
                new IdentifierGenerator(), new ExportableModelRegistry(), null, assessmentSection,
                (fm, section) => null, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableSpecificFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableSpecificFailureMechanism(
                new IdentifierGenerator(), new ExportableModelRegistry(), new SpecificFailureMechanism(),
                null, (fm, section) => null, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableSpecificFailureMechanism_AssembleFailureMechanismFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableSpecificFailureMechanism(
                new IdentifierGenerator(), new ExportableModelRegistry(), new SpecificFailureMechanism(),
                assessmentSection, null, (sr, fm, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assembleFailureMechanismFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableSpecificFailureMechanism_AssembleFailureMechanismSectionFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateExportableSpecificFailureMechanism(
                new IdentifierGenerator(), new ExportableModelRegistry(), new SpecificFailureMechanism(),
                assessmentSection, (fm, section) => null, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assembleFailureMechanismSectionFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableSpecificFailureMechanism_WithValidData_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new SpecificFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));
            var assessmentSection = new AssessmentSectionStub();

            double probability = random.NextDouble();
            var assemblyMethod = random.NextEnumValue<AssemblyMethod>();
            FailureMechanismSectionAssemblyResultWrapper expectedSectionOutput = FailureMechanismSectionAssemblyResultWrapperTestFactory.Create();

            var idGenerator = new IdentifierGenerator();
            var registry = new ExportableModelRegistry();
            RegisterFailureMechanismSections(registry, failureMechanism.Sections);

            // Call
            ExportableSpecificFailureMechanism exportableFailureMechanism =
                ExportableFailureMechanismFactory.CreateExportableSpecificFailureMechanism(
                    idGenerator, registry, failureMechanism, assessmentSection,
                    (fm, section) => new FailureMechanismAssemblyResultWrapper(probability, assemblyMethod),
                    (sr, fm, section) => expectedSectionOutput);

            // Assert
            Assert.AreEqual("Fm.0", exportableFailureMechanism.Id);
            Assert.AreEqual(failureMechanism.Name, exportableFailureMechanism.Name);

            ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssembly = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(probability, exportableFailureMechanismAssembly.Probability);
            Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(assemblyMethod), exportableFailureMechanismAssembly.AssemblyMethod);

            IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections = exportableFailureMechanism.SectionAssemblyResults
                                                                                                                          .Select(sar => sar.FailureMechanismSection);
            ExportableFailureMechanismSectionAssertHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, exportableFailureMechanismSections);

            ExportableFailureMechanismSectionAssemblyResultAssertHelper.AssertExportableFailureMechanismSectionResults(
                expectedSectionOutput, exportableFailureMechanismSections, exportableFailureMechanism.SectionAssemblyResults);
        }

        private static void RegisterFailureMechanismSections(ExportableModelRegistry registry, IEnumerable<FailureMechanismSection> failureMechanismSections)
        {
            ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                new IdentifierGenerator(), registry, failureMechanismSections);
        }
    }
}