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
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.TestUtil;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.Util;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableFailureMechanismSectionFactoryTest
    {
        [Test]
        public void CreateExportableFailureMechanismSection_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSection(
                null, new ExportableModelRegistry(), FailureMechanismSectionTestFactory.CreateFailureMechanismSection(), random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanism_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSection(
                new IdentifierGenerator(), null, FailureMechanismSectionTestFactory.CreateFailureMechanismSection(), random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanismSection_SectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSection(
                new IdentifierGenerator(), new ExportableModelRegistry(), null, random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanismSection_WithValidArguments_ReturnsExpectedExportableFailureMechanismSection()
        {
            // Setup
            var random = new Random(21);
            double startDistance = random.NextDouble();

            var idGenerator = new IdentifierGenerator();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            var registry = new ExportableModelRegistry();

            // Call
            ExportableFailureMechanismSection exportableSection =
                ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSection(
                    idGenerator, registry, section, startDistance);

            // Assert
            Assert.AreEqual("Bv.0", exportableSection.Id);
            Assert.AreSame(section.Points, exportableSection.Geometry);
            Assert.AreEqual(startDistance, exportableSection.StartDistance);

            double expectedEndDistance = startDistance + section.Length;
            Assert.AreEqual(expectedEndDistance, exportableSection.EndDistance);
        }

        [Test]
        public void CreateExportableFailureMechanismSection_SectionAlreadyRegistered_ReturnsRegisteredExportableFailureMechanismSection()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            var idGenerator = new IdentifierGenerator();
            var registry = new ExportableModelRegistry();
            ExportableFailureMechanismSection exportableSection1 =
                ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSection(
                    idGenerator, registry, section, random.NextDouble());

            // Precondition
            Assert.True(registry.Contains(section));

            // Call
            ExportableFailureMechanismSection exportableSection2 =
                ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSection(
                    idGenerator, registry, section, random.NextDouble());

            // Assert
            Assert.AreSame(exportableSection1, exportableSection2);
        }

        [Test]
        public void CreateExportableCombinedFailureMechanismSection_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismSectionFactory.CreateExportableCombinedFailureMechanismSection(
                null, new ExportableModelRegistry(), ReferenceLineTestFactory.CreateReferenceLineWithGeometry(),
                CombinedFailureMechanismSectionAssemblyResultTestFactory.Create());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateExportableCombinedFailureMechanismSection_RegistryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismSectionFactory.CreateExportableCombinedFailureMechanismSection(
                new IdentifierGenerator(), null, ReferenceLineTestFactory.CreateReferenceLineWithGeometry(),
                CombinedFailureMechanismSectionAssemblyResultTestFactory.Create());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void CreateExportableCombinedFailureMechanismSection_ReferenceLineNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismSectionFactory.CreateExportableCombinedFailureMechanismSection(
                new IdentifierGenerator(), new ExportableModelRegistry(), null,
                CombinedFailureMechanismSectionAssemblyResultTestFactory.Create());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void CreateExportableCombinedFailureMechanismSection_AssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismSectionFactory.CreateExportableCombinedFailureMechanismSection(
                new IdentifierGenerator(), new ExportableModelRegistry(), ReferenceLineTestFactory.CreateReferenceLineWithGeometry(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assemblyResult", exception.ParamName);
        }

        [Test]
        public void CreateExportableCombinedFailureMechanismSection_WithValidArguments_ReturnsExpectedExportableCombinedFailureMechanismSection()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            ReferenceLine referenceLine = ReferenceLineTestFactory.CreateReferenceLineWithGeometry();
            CombinedFailureMechanismSectionAssemblyResult assemblyResult = CombinedFailureMechanismSectionAssemblyResultTestFactory.Create();

            var registry = new ExportableModelRegistry();

            // Call
            ExportableCombinedFailureMechanismSection exportableSection =
                ExportableFailureMechanismSectionFactory.CreateExportableCombinedFailureMechanismSection(
                    idGenerator, registry, referenceLine, assemblyResult);

            // Assert
            Assert.AreEqual("Bv.0", exportableSection.Id);

            Assert.AreEqual(assemblyResult.SectionStart, exportableSection.StartDistance);
            Assert.AreEqual(assemblyResult.SectionEnd, exportableSection.EndDistance);
            CollectionAssert.AreEqual(FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(
                                          referenceLine, exportableSection.StartDistance, exportableSection.EndDistance),
                                      exportableSection.Geometry);
            Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(assemblyResult.CommonSectionAssemblyMethod),
                            exportableSection.AssemblyMethod);
        }

        [Test]
        public void CreateExportableCombinedFailureMechanismSection_AssemblyResultAlreadyRegistered_ReturnsRegisteredExportableCombinedFailureMechanismSection()
        {
            // Setup
            CombinedFailureMechanismSectionAssemblyResult assemblyResult = CombinedFailureMechanismSectionAssemblyResultTestFactory.Create();

            var idGenerator = new IdentifierGenerator();
            var registry = new ExportableModelRegistry();
            ExportableCombinedFailureMechanismSection exportableSection1 =
                ExportableFailureMechanismSectionFactory.CreateExportableCombinedFailureMechanismSection(
                    idGenerator, registry, ReferenceLineTestFactory.CreateReferenceLineWithGeometry(), assemblyResult);

            // Precondition
            Assert.True(registry.Contains(assemblyResult));

            // Call
            ExportableCombinedFailureMechanismSection exportableSection2 =
                ExportableFailureMechanismSectionFactory.CreateExportableCombinedFailureMechanismSection(
                    idGenerator, registry, ReferenceLineTestFactory.CreateReferenceLineWithGeometry(), assemblyResult);

            // Assert
            Assert.AreSame(exportableSection1, exportableSection2);
        }
    }
}