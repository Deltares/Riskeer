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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.TestUtil;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableCombinedSectionAssemblyFactoryTest
    {
        [Test]
        public void CreateExportableCombinedSectionAssemblyCollection_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(
                null, new ExportableModelRegistry(), Enumerable.Empty<CombinedFailureMechanismSectionAssemblyResult>(),
                new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateExportableCombinedSectionAssemblyCollection_RegistryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(
                new IdentifierGenerator(), null, Enumerable.Empty<CombinedFailureMechanismSectionAssemblyResult>(),
                new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void CreateExportableCombinedSectionAssemblyCollection_CombinedSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(
                new IdentifierGenerator(), new ExportableModelRegistry(), null, new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("combinedSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void CreateExportableCombinedSectionAssemblyCollection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(
                new IdentifierGenerator(), new ExportableModelRegistry(), Enumerable.Empty<CombinedFailureMechanismSectionAssemblyResult>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyGroup.NoResult)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Dominant)]
        public void CreateExportableCombinedSectionAssemblyCollection_WithInvalidAssemblyResults_ThrowsAssemblyFactoryException(
            FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var random = new Random(21);
            CombinedFailureMechanismSectionAssemblyResult[] assemblyResults =
            {
                new CombinedFailureMechanismSectionAssemblyResult(
                    random.NextDouble(), random.NextDouble(), assemblyGroup,
                    random.NextEnumValue<AssemblyMethod>(), random.NextEnumValue<AssemblyMethod>(),
                    random.NextEnumValue<AssemblyMethod>(), new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties())
            };

            // Call
            void Call() => ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(
                new IdentifierGenerator(), new ExportableModelRegistry(), assemblyResults, assessmentSection);

            // Assert
            var exception = Assert.Throws<AssemblyFactoryException>(Call);
            Assert.AreEqual("The assembly result is invalid and cannot be created.", exception.Message);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateExportableCombinedSectionAssemblyCollection_WithAssemblyResults_ReturnsExportableCombinedSectionAssemblyCollection(bool hasAssemblyGroupResults)
        {
            // Setup
            CombinedFailureMechanismSectionAssemblyResult[] assemblyResults =
            {
                CombinedFailureMechanismSectionAssemblyResultTestFactory.Create(21, hasAssemblyGroupResults),
                CombinedFailureMechanismSectionAssemblyResultTestFactory.Create(22, hasAssemblyGroupResults)
            };

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(0, 0),
                new Point2D(2, 2)
            });

            assessmentSection.SpecificFailureMechanisms.AddRange(new[]
            {
                new SpecificFailureMechanism
                {
                    Code = "Nieuw1"
                },
                new SpecificFailureMechanism
                {
                    Code = "Nieuw2"
                }
            });
            SetFailureMechanismSections(assessmentSection, assemblyResults.Length);

            var idGenerator = new IdentifierGenerator();
            var registry = new ExportableModelRegistry();
            RegisterFailureMechanismSections(registry, assessmentSection.ReferenceLine, assemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection, assemblyResults);

            // Call
            IEnumerable<ExportableCombinedSectionAssembly> exportableCombinedSectionAssemblies =
                ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(
                    idGenerator, registry, assemblyResults, assessmentSection);

            // Assert
            AssertCombinedFailureMechanismSectionAssemblyResults(registry, assessmentSection, assemblyResults, 
                                                                 exportableCombinedSectionAssemblies, hasAssemblyGroupResults);
        }

        private static void SetFailureMechanismSections(IAssessmentSection assessmentSection, int nrOfCombinedAssemblyResults)
        {
            IEnumerable<IFailureMechanism> failureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                .Concat(assessmentSection.SpecificFailureMechanisms);
            foreach (IFailureMechanism failureMechanism in failureMechanisms)
            {
                FailureMechanismTestHelper.AddSections(failureMechanism, nrOfCombinedAssemblyResults);
            }
        }

        private static void RegisterFailureMechanismSections(ExportableModelRegistry registry, ReferenceLine referenceLine,
                                                             IEnumerable<CombinedFailureMechanismSectionAssemblyResult> assemblyResults)
        {
            ExportableFailureMechanismSectionCollectionFactory.CreateExportableFailureMechanismSectionCollection(
                new IdentifierGenerator(), registry, referenceLine, assemblyResults);
        }

        private static void RegisterFailureMechanismSectionResults(ExportableModelRegistry registry, AssessmentSection assessmentSection,
                                                                   IEnumerable<CombinedFailureMechanismSectionAssemblyResult> combinedSectionAssemblyResults)
        {
            RegisterFailureMechanismSectionResults(registry, assessmentSection.Piping.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.GrassCoverErosionInwards.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.MacroStabilityInwards.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.Microstability.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.StabilityStoneCover.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.WaveImpactAsphaltCover.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.WaterPressureAsphaltCover.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.GrassCoverErosionOutwards.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.GrassCoverSlipOffOutwards.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.GrassCoverSlipOffInwards.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.HeightStructures.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.ClosingStructures.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.PipingStructure.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.StabilityPointStructures.SectionResults, combinedSectionAssemblyResults);
            RegisterFailureMechanismSectionResults(registry, assessmentSection.DuneErosion.SectionResults, combinedSectionAssemblyResults);

            foreach (SpecificFailureMechanism specificFailureMechanism in assessmentSection.SpecificFailureMechanisms)
            {
                RegisterFailureMechanismSectionResults(registry, specificFailureMechanism.SectionResults, combinedSectionAssemblyResults);
            }
        }

        private static void RegisterFailureMechanismSectionResults(ExportableModelRegistry registry,
                                                                   IEnumerable<FailureMechanismSectionResult> sectionAssemblyResults,
                                                                   IEnumerable<CombinedFailureMechanismSectionAssemblyResult> combinedSectionAssemblyResults)
        {
            for (var i = 0; i < combinedSectionAssemblyResults.Count(); i++)
            {
                CombinedFailureMechanismSectionAssemblyResult assemblyResult = combinedSectionAssemblyResults.ElementAt(i);
                ExportableFailureMechanismSection exportableSection = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(
                    assemblyResult.SectionStart, assemblyResult.SectionEnd);
                registry.Register(sectionAssemblyResults.ElementAt(i), ExportableFailureMechanismSectionAssemblyResultTestFactory.Create(exportableSection, i));
            }
        }

        private static void AssertCombinedFailureMechanismSectionAssemblyResults(
            ExportableModelRegistry registry, AssessmentSection assessmentSection, IEnumerable<CombinedFailureMechanismSectionAssemblyResult> assemblyResults,
            IEnumerable<ExportableCombinedSectionAssembly> exportableCombinedSectionAssemblies, bool hasAssemblyGroupResults)
        {
            int expectedNrOfSections = assemblyResults.Count();
            Assert.AreEqual(expectedNrOfSections, exportableCombinedSectionAssemblies.Count());

            for (var i = 0; i < expectedNrOfSections; i++)
            {
                CombinedFailureMechanismSectionAssemblyResult combinedFailureMechanismSectionAssemblyResult = assemblyResults.ElementAt(i);
                ExportableCombinedSectionAssembly exportableCombinedSectionAssembly = exportableCombinedSectionAssemblies.ElementAt(i);

                AssertExportableCombinedFailureMechanismSectionResult(i, registry, assessmentSection, combinedFailureMechanismSectionAssemblyResult,
                                                                      registry.Get(combinedFailureMechanismSectionAssemblyResult),
                                                                      exportableCombinedSectionAssembly, hasAssemblyGroupResults);
            }
        }

        private static void AssertExportableCombinedFailureMechanismSectionResult(
            int index, ExportableModelRegistry registry, AssessmentSection assessmentSection,
            CombinedFailureMechanismSectionAssemblyResult expectedSection, ExportableCombinedFailureMechanismSection actualSection,
            ExportableCombinedSectionAssembly actualSectionResult, bool hasAssemblyGroupResults)
        {
            Assert.AreEqual($"Gf.{index}", actualSectionResult.Id);

            Assert.AreSame(actualSection, actualSectionResult.Section);
            Assert.AreEqual(ExportableFailureMechanismSectionAssemblyGroupConverter.ConvertTo(expectedSection.TotalResult), actualSectionResult.AssemblyGroup);
            Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(expectedSection.CombinedSectionResultAssemblyMethod), actualSectionResult.AssemblyGroupAssemblyMethod);

            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismCombinedSectionResults = actualSectionResult.FailureMechanismResults;

            if (!hasAssemblyGroupResults)
            {
                CollectionAssert.IsEmpty(failureMechanismCombinedSectionResults);
                return;
            }

            Assert.AreEqual(17, failureMechanismCombinedSectionResults.Count());
            Assert.IsTrue(failureMechanismCombinedSectionResults.All(result => result.AssemblyMethod == ExportableAssemblyMethodConverter.ConvertTo(
                                                                                   expectedSection.FailureMechanismResultsAssemblyMethod)));

            AssertSubSection(index, registry, assessmentSection.Piping,
                             expectedSection.Piping, failureMechanismCombinedSectionResults.ElementAt(0));
            AssertSubSection(index, registry, assessmentSection.GrassCoverErosionInwards,
                             expectedSection.GrassCoverErosionInwards, failureMechanismCombinedSectionResults.ElementAt(1));
            AssertSubSection(index, registry, assessmentSection.MacroStabilityInwards,
                             expectedSection.MacroStabilityInwards, failureMechanismCombinedSectionResults.ElementAt(2));
            AssertSubSection(index, registry, assessmentSection.Microstability,
                             expectedSection.Microstability, failureMechanismCombinedSectionResults.ElementAt(3));
            AssertSubSection(index, registry, assessmentSection.StabilityStoneCover,
                             expectedSection.StabilityStoneCover, failureMechanismCombinedSectionResults.ElementAt(4));
            AssertSubSection(index, registry, assessmentSection.WaveImpactAsphaltCover,
                             expectedSection.WaveImpactAsphaltCover, failureMechanismCombinedSectionResults.ElementAt(5));
            AssertSubSection(index, registry, assessmentSection.WaterPressureAsphaltCover,
                             expectedSection.WaterPressureAsphaltCover, failureMechanismCombinedSectionResults.ElementAt(6));
            AssertSubSection(index, registry, assessmentSection.GrassCoverErosionOutwards,
                             expectedSection.GrassCoverErosionOutwards, failureMechanismCombinedSectionResults.ElementAt(7));
            AssertSubSection(index, registry, assessmentSection.GrassCoverSlipOffOutwards,
                             expectedSection.GrassCoverSlipOffOutwards, failureMechanismCombinedSectionResults.ElementAt(8));
            AssertSubSection(index, registry, assessmentSection.GrassCoverSlipOffInwards,
                             expectedSection.GrassCoverSlipOffInwards, failureMechanismCombinedSectionResults.ElementAt(9));
            AssertSubSection(index, registry, assessmentSection.HeightStructures,
                             expectedSection.HeightStructures, failureMechanismCombinedSectionResults.ElementAt(10));
            AssertSubSection(index, registry, assessmentSection.ClosingStructures,
                             expectedSection.ClosingStructures, failureMechanismCombinedSectionResults.ElementAt(11));
            AssertSubSection(index, registry, assessmentSection.PipingStructure,
                             expectedSection.PipingStructure, failureMechanismCombinedSectionResults.ElementAt(12));
            AssertSubSection(index, registry, assessmentSection.StabilityPointStructures,
                             expectedSection.StabilityPointStructures, failureMechanismCombinedSectionResults.ElementAt(13));
            AssertSubSection(index, registry, assessmentSection.DuneErosion,
                             expectedSection.DuneErosion, failureMechanismCombinedSectionResults.ElementAt(14));
            AssertSubSection(index, registry, assessmentSection.SpecificFailureMechanisms[0],
                             expectedSection.SpecificFailureMechanisms[0], failureMechanismCombinedSectionResults.ElementAt(15));
            AssertSubSection(index, registry, assessmentSection.SpecificFailureMechanisms[1],
                             expectedSection.SpecificFailureMechanisms[1], failureMechanismCombinedSectionResults.ElementAt(16));
        }

        private static void AssertSubSection<T>(int index, ExportableModelRegistry registry, IFailureMechanism<T> failureMechanism,
                                                FailureMechanismSectionAssemblyGroup? subSectionGroup,
                                                ExportableFailureMechanismCombinedSectionAssemblyResult actualResult)
            where T : FailureMechanismSectionResult
        {
            Assert.AreEqual(ExportableFailureMechanismSectionAssemblyGroupConverter.ConvertTo(subSectionGroup.Value), actualResult.AssemblyGroup);

            T sectionResult = failureMechanism.SectionResults.ElementAt(index);
            ExportableFailureMechanismSectionAssemblyResult expectedExportableSectionResult = registry.Get(sectionResult);
            Assert.AreSame(expectedExportableSectionResult, actualResult.FailureMechanismSectionResult);
        }
    }
}