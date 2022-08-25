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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
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
    public class ExportableFailureMechanismSectionCollectionFactoryTest
    {
        [Test]
        public void CreateExportableFailureMechanismSectionCollection_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                null, new ExportableModelRegistry(), Enumerable.Empty<FailureMechanismSection>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanismCollection_RegistryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                new IdentifierGenerator(), null, Enumerable.Empty<FailureMechanismSection>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanismSectionCollection_SectionsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                new IdentifierGenerator(), new ExportableModelRegistry(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanismSectionCollection_WithValidArguments_ReturnsExpectedExportableFailureMechanismSectionCollection()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            FailureMechanismSection[] sections =
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(0, 10)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 10),
                    new Point2D(0, 20)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 20),
                    new Point2D(0, 40)
                })
            };

            var registry = new ExportableModelRegistry();

            // Call
            ExportableFailureMechanismSectionCollection collection =
                ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(idGenerator, registry, sections);

            // Assert
            Assert.AreEqual("Vi.0", collection.Id);
            IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections = collection.Sections;
            Assert.AreEqual(sections.Length, exportableFailureMechanismSections.Count());

            ExportableFailureMechanismSection firstExportableSection = exportableFailureMechanismSections.ElementAt(0);
            Assert.AreSame(sections[0].Points, firstExportableSection.Geometry);
            Assert.AreEqual("Bv.0", firstExportableSection.Id);
            Assert.AreEqual(0, firstExportableSection.StartDistance);
            Assert.AreEqual(10, firstExportableSection.EndDistance);

            ExportableFailureMechanismSection secondExportableSection = exportableFailureMechanismSections.ElementAt(1);
            Assert.AreSame(sections[1].Points, secondExportableSection.Geometry);
            Assert.AreEqual("Bv.1", secondExportableSection.Id);
            Assert.AreEqual(10, secondExportableSection.StartDistance);
            Assert.AreEqual(20, secondExportableSection.EndDistance);

            ExportableFailureMechanismSection thirdExportableSection = exportableFailureMechanismSections.ElementAt(2);
            Assert.AreEqual("Bv.2", thirdExportableSection.Id);
            Assert.AreSame(sections[2].Points, thirdExportableSection.Geometry);
            Assert.AreEqual(20, thirdExportableSection.StartDistance);
            Assert.AreEqual(40, thirdExportableSection.EndDistance);
        }

        [Test]
        public void CreateExportableFailureMechanismSectionCollectionWithCombinedAssemblyResults_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                null, new ExportableModelRegistry(), ReferenceLineTestFactory.CreateReferenceLineWithGeometry(),
                Enumerable.Empty<CombinedFailureMechanismSectionAssemblyResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanismSectionCollectionWithCombinedAssemblyResults_RegistryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                new IdentifierGenerator(), null, ReferenceLineTestFactory.CreateReferenceLineWithGeometry(),
                Enumerable.Empty<CombinedFailureMechanismSectionAssemblyResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanismSectionCollectionWithCombinedAssemblyResults_ReferenceLineNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                new IdentifierGenerator(), new ExportableModelRegistry(), null,
                Enumerable.Empty<CombinedFailureMechanismSectionAssemblyResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanismSectionCollectionWithCombinedAssemblyResults_AssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                new IdentifierGenerator(), new ExportableModelRegistry(), ReferenceLineTestFactory.CreateReferenceLineWithGeometry(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assemblyResults", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanismSectionCollectionWithCombinedAssemblyResults_WithValidArguments_ReturnsExpectedExportableFailureMechanismSectionCollection()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            var registry = new ExportableModelRegistry();

            CombinedFailureMechanismSectionAssemblyResult[] assemblyResults =
            {
                CombinedFailureMechanismSectionAssemblyResultTestFactory.Create(0, 5),
                CombinedFailureMechanismSectionAssemblyResultTestFactory.Create(5, 10),
                CombinedFailureMechanismSectionAssemblyResultTestFactory.Create(10, 15)
            };

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0, 0),
                new Point2D(10, 10)
            });

            // Call
            ExportableFailureMechanismSectionCollection collection =
                ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                    idGenerator, registry, referenceLine, assemblyResults);

            // Assert
            Assert.AreEqual("Vi.0", collection.Id);
            Assert.AreEqual(assemblyResults.Length, collection.Sections.Count());
            CollectionAssert.AllItemsAreInstancesOfType(collection.Sections, typeof(ExportableCombinedFailureMechanismSection));

            IEnumerable<ExportableCombinedFailureMechanismSection> exportableSections =
                collection.Sections.Cast<ExportableCombinedFailureMechanismSection>();

            for (var i = 0; i < assemblyResults.Length; i++)
            {
                AssertExportableCombinedFailureMechanismSection(i, referenceLine, assemblyResults[i], exportableSections.ElementAt(i));
            }
        }

        private static void AssertExportableCombinedFailureMechanismSection(int index, ReferenceLine referenceLine,
                                                                            CombinedFailureMechanismSectionAssemblyResult assemblyResult,
                                                                            ExportableCombinedFailureMechanismSection exportableCombinedFailureMechanismSection)
        {
            IEnumerable<Point2D> expectedGeometry = FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(
                referenceLine,
                assemblyResult.SectionStart,
                assemblyResult.SectionEnd).ToArray();
            CollectionAssert.IsNotEmpty(expectedGeometry);

            Assert.AreEqual($"Bv.{index}", exportableCombinedFailureMechanismSection.Id);
            Assert.AreEqual(assemblyResult.SectionStart, exportableCombinedFailureMechanismSection.StartDistance);
            Assert.AreEqual(assemblyResult.SectionEnd, exportableCombinedFailureMechanismSection.EndDistance);
            CollectionAssert.AreEqual(expectedGeometry, exportableCombinedFailureMechanismSection.Geometry);
            Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(assemblyResult.CommonSectionAssemblyMethod), exportableCombinedFailureMechanismSection.AssemblyMethod);
        }
    }
}