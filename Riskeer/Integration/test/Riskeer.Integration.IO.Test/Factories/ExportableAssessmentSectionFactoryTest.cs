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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableAssessmentSectionFactoryTest
    {
        private const int pipingIndex = 0;
        private const int grassCoverErosionInwardsIndex = 1;
        private const int macroStabilityInwardsIndex = 2;
        private const int microstabilityIndex = 3;
        private const int stabilityStoneCoverIndex = 4;
        private const int waveImpactAsphaltCoverIndex = 5;
        private const int waterPressureAsphaltCoverIndex = 6;
        private const int grassCoverErosionOutwardsIndex = 7;
        private const int grassCoverSlipOffOutwardsIndex = 8;
        private const int grassCoverSlipOffInwardsIndex = 9;
        private const int heightStructuresIndex = 10;
        private const int closingStructuresIndex = 11;
        private const int pipingStructureIndex = 12;
        private const int stabilityPointStructuresIndex = 13;
        private const int duneErosionIndex = 14;
        private const int firstSpecificFailureMechanismIndex = 15;
        private const int secondSpecificFailureMechanismIndex = 16;

        [Test]
        public void CreateExportableAssessmentSection_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(
                null, new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateExportableAssessmentSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(new IdentifierGenerator(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableAssessmentSection_WithAssessmentSectionWithReferenceLine_ReturnsExpectedValues()
        {
            // Setup
            const string name = "assessmentSectionName";
            const string id = "assessmentSectionId";

            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                Name = name,
                Id = id
            };
            ReferenceLineTestFactory.SetReferenceLineGeometry(assessmentSection.ReferenceLine);

            assessmentSection.SpecificFailureMechanisms.Add(new SpecificFailureMechanism());
            assessmentSection.SpecificFailureMechanisms.Add(new SpecificFailureMechanism());

            AddFailureMechanismSections(assessmentSection);

            var idGenerator = new IdentifierGenerator();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(
                    idGenerator, assessmentSection);

                // Assert
                Assert.AreEqual(name, exportableAssessmentSection.Name);
                Assert.AreEqual($"Wks.{assessmentSection.Id}", exportableAssessmentSection.Id);
                CollectionAssert.AreEqual(assessmentSection.ReferenceLine.Points, exportableAssessmentSection.Geometry);

                ExportableAssessmentSectionAssemblyResult exportableAssessmentSectionAssemblyResult = exportableAssessmentSection.AssessmentSectionAssembly;
                Assert.AreEqual(ExportableAssemblyMethod.BOI2A1, exportableAssessmentSectionAssemblyResult.ProbabilityAssemblyMethod);
                Assert.AreEqual(ExportableAssemblyMethod.BOI2B1, exportableAssessmentSectionAssemblyResult.AssemblyGroupAssemblyMethod);
                Assert.AreEqual(ExportableAssessmentSectionAssemblyGroup.APlus, exportableAssessmentSectionAssemblyResult.AssemblyGroup);
                Assert.AreEqual(0.14, exportableAssessmentSectionAssemblyResult.Probability);

                AssertExportableFailureMechanisms(exportableAssessmentSection.FailureMechanisms, assessmentSection);
                AssertExportableFailureMechanismSectionCollections(assessmentSection, exportableAssessmentSection.FailureMechanismSectionCollections);

                Assert.AreEqual(1, exportableAssessmentSection.CombinedSectionAssemblies.Count());
                ExportableCombinedSectionAssembly exportableCombinedSectionAssembly = exportableAssessmentSection.CombinedSectionAssemblies.ElementAt(0);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Zero, exportableCombinedSectionAssembly.AssemblyGroup);
                Assert.AreEqual(ExportableAssemblyMethod.BOI3C1, exportableCombinedSectionAssembly.AssemblyGroupAssemblyMethod);

                ExportableCombinedFailureMechanismSection exportableCombinedFailureMechanismSection = exportableCombinedSectionAssembly.Section;
                AssertExportableFailureMechanismCombinedSectionAssemblyResults(exportableCombinedFailureMechanismSection,
                                                                               exportableAssessmentSection.FailureMechanisms,
                                                                               exportableCombinedSectionAssembly.FailureMechanismResults);

                Assert.AreEqual(ExportableAssemblyMethod.BOI3A1, exportableCombinedFailureMechanismSection.AssemblyMethod);
                Assert.AreEqual(0.0, exportableCombinedFailureMechanismSection.StartDistance);
                Assert.AreEqual(1.0, exportableCombinedFailureMechanismSection.EndDistance);
            }
        }

        [Test]
        public void CreateExportableAssessmentSection_AllFailureMechanismNotInAssembly_ReturnsExpectedValues()
        {
            // Setup
            const string name = "assessmentSectionName";
            const string id = "assessmentSectionId";

            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                Name = name,
                Id = id
            };
            ReferenceLineTestFactory.SetReferenceLineGeometry(assessmentSection.ReferenceLine);

            assessmentSection.SpecificFailureMechanisms.Add(new SpecificFailureMechanism());
            assessmentSection.SpecificFailureMechanisms.Add(new SpecificFailureMechanism());

            AddFailureMechanismSections(assessmentSection);

            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms()
                                                                            .Concat(assessmentSection.SpecificFailureMechanisms))
            {
                failureMechanism.InAssembly = false;
            }

            var idGenerator = new IdentifierGenerator();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(idGenerator, assessmentSection);

                // Assert
                Assert.AreEqual(name, exportableAssessmentSection.Name);
                Assert.AreEqual($"Wks.{assessmentSection.Id}", exportableAssessmentSection.Id);
                CollectionAssert.AreEqual(assessmentSection.ReferenceLine.Points, exportableAssessmentSection.Geometry);

                ExportableAssessmentSectionAssemblyResult exportableAssessmentSectionAssemblyResult = exportableAssessmentSection.AssessmentSectionAssembly;
                Assert.AreEqual(ExportableAssemblyMethod.BOI2A1, exportableAssessmentSectionAssemblyResult.ProbabilityAssemblyMethod);
                Assert.AreEqual(ExportableAssemblyMethod.BOI2B1, exportableAssessmentSectionAssemblyResult.AssemblyGroupAssemblyMethod);
                Assert.AreEqual(ExportableAssessmentSectionAssemblyGroup.APlus, exportableAssessmentSectionAssemblyResult.AssemblyGroup);
                Assert.AreEqual(0.14, exportableAssessmentSectionAssemblyResult.Probability);

                CollectionAssert.IsEmpty(exportableAssessmentSection.FailureMechanisms);
                AssertExportableFailureMechanismSectionCollections(assessmentSection, exportableAssessmentSection.FailureMechanismSectionCollections);

                Assert.AreEqual(1, exportableAssessmentSection.CombinedSectionAssemblies.Count());
                ExportableCombinedSectionAssembly exportableCombinedSectionAssembly = exportableAssessmentSection.CombinedSectionAssemblies.ElementAt(0);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Zero, exportableCombinedSectionAssembly.AssemblyGroup);
                Assert.AreEqual(ExportableAssemblyMethod.BOI3C1, exportableCombinedSectionAssembly.AssemblyGroupAssemblyMethod);

                CollectionAssert.IsEmpty(exportableCombinedSectionAssembly.FailureMechanismResults);

                ExportableCombinedFailureMechanismSection exportableCombinedFailureMechanismSection = exportableCombinedSectionAssembly.Section;
                Assert.AreEqual(ExportableAssemblyMethod.BOI3A1, exportableCombinedFailureMechanismSection.AssemblyMethod);
                Assert.AreEqual(0.0, exportableCombinedFailureMechanismSection.StartDistance);
                Assert.AreEqual(1.0, exportableCombinedFailureMechanismSection.EndDistance);
            }
        }

        private static void AddFailureMechanismSections(AssessmentSection assessmentSection)
        {
            var random = new Random(21);
            FailureMechanismTestHelper.AddSections(assessmentSection.Piping, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.MacroStabilityInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.HeightStructures, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.ClosingStructures, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityPointStructures, random.Next(1, 10));

            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityStoneCover, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.WaveImpactAsphaltCover, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionOutwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.DuneErosion, random.Next(1, 10));

            FailureMechanismTestHelper.AddSections(assessmentSection.Microstability, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffOutwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.PipingStructure, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.WaterPressureAsphaltCover, random.Next(1, 10));

            foreach (SpecificFailureMechanism specificFailureMechanism in assessmentSection.SpecificFailureMechanisms)
            {
                FailureMechanismTestHelper.AddSections(specificFailureMechanism, random.Next(1, 10));
            }
        }

        private static void AssertExportableFailureMechanismSectionCollections(
            IAssessmentSection assessmentSection, IEnumerable<ExportableFailureMechanismSectionCollection> failureMechanismSectionCollections)
        {
            IEnumerable<IFailureMechanism> failureMechanismsInAssembly = assessmentSection.GetFailureMechanisms()
                                                                                          .Concat(assessmentSection.SpecificFailureMechanisms)
                                                                                          .Where(fm => fm.InAssembly);

            int nrOfFailureMechanismsInAssembly = failureMechanismsInAssembly.Count();
            int nrOfExpectedCollections = nrOfFailureMechanismsInAssembly + 1;
            Assert.AreEqual(nrOfExpectedCollections, failureMechanismSectionCollections.Count());

            for (var i = 0; i < nrOfFailureMechanismsInAssembly; i++)
            {
                int nrOfExpectedSections = failureMechanismsInAssembly.ElementAt(i).Sections.Count();
                Assert.AreEqual(nrOfExpectedSections, failureMechanismSectionCollections.ElementAt(i).Sections.Count());
            }

            ExportableFailureMechanismSectionCollection combinedFailureMechanismSectionCollection = failureMechanismSectionCollections.Last();
            IEnumerable<ExportableFailureMechanismSection> exportableCombinedFailureMechanismSections = combinedFailureMechanismSectionCollection.Sections;
            CollectionAssert.AllItemsAreInstancesOfType(exportableCombinedFailureMechanismSections, typeof(ExportableCombinedFailureMechanismSection));
            Assert.AreEqual(1, exportableCombinedFailureMechanismSections.Count());
        }

        private static void AssertExportableFailureMechanisms(
            IEnumerable<ExportableFailureMechanism> exportableFailureMechanisms,
            AssessmentSection assessmentSection)
        {
            Assert.AreEqual(17, exportableFailureMechanisms.Count());

            AssertExportableGenericFailureMechanism(assessmentSection.Piping,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(pipingIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.GrassCoverErosionInwards,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(grassCoverErosionInwardsIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.MacroStabilityInwards,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(macroStabilityInwardsIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.Microstability,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(microstabilityIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.StabilityStoneCover,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(stabilityStoneCoverIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.WaveImpactAsphaltCover,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(waveImpactAsphaltCoverIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.WaterPressureAsphaltCover,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(waterPressureAsphaltCoverIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.GrassCoverErosionOutwards,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(grassCoverErosionOutwardsIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(grassCoverSlipOffOutwardsIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.GrassCoverSlipOffInwards,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(grassCoverSlipOffInwardsIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.HeightStructures,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(heightStructuresIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.ClosingStructures,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(closingStructuresIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.PipingStructure,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(pipingStructureIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.StabilityPointStructures,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(stabilityPointStructuresIndex));

            AssertExportableGenericFailureMechanism(assessmentSection.DuneErosion,
                                                    (ExportableGenericFailureMechanism) exportableFailureMechanisms.ElementAt(duneErosionIndex));

            AssertExportableSpecificFailureMechanism(assessmentSection.SpecificFailureMechanisms.First(),
                                                     (ExportableSpecificFailureMechanism) exportableFailureMechanisms.ElementAt(firstSpecificFailureMechanismIndex));

            AssertExportableSpecificFailureMechanism(assessmentSection.SpecificFailureMechanisms.Last(),
                                                     (ExportableSpecificFailureMechanism) exportableFailureMechanisms.ElementAt(secondSpecificFailureMechanismIndex));
        }

        private static void AssertExportableGenericFailureMechanism(IFailureMechanism<FailureMechanismSectionResult> failureMechanism,
                                                                    ExportableGenericFailureMechanism actualExportableFailureMechanism)
        {
            Assert.AreEqual(failureMechanism.Code, actualExportableFailureMechanism.Code);

            AssertExportableFailureMechanism(failureMechanism, actualExportableFailureMechanism);
        }

        private static void AssertExportableSpecificFailureMechanism(IFailureMechanism<FailureMechanismSectionResult> failureMechanism,
                                                                     ExportableSpecificFailureMechanism actualExportableFailureMechanism)
        {
            Assert.AreEqual(failureMechanism.Name, actualExportableFailureMechanism.Name);

            AssertExportableFailureMechanism(failureMechanism, actualExportableFailureMechanism);
        }

        private static void AssertExportableFailureMechanism(IFailureMechanism<FailureMechanismSectionResult> failureMechanism,
                                                             ExportableFailureMechanism actualExportableFailureMechanism)
        {
            ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssemblyResult = actualExportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(0.1, exportableFailureMechanismAssemblyResult.Probability);
            Assert.AreEqual(ExportableAssemblyMethod.BOI1A1, exportableFailureMechanismAssemblyResult.AssemblyMethod);

            Assert.AreEqual(failureMechanism.SectionResults.Count(), actualExportableFailureMechanism.SectionAssemblyResults.Count());
        }

        private static void AssertExportableFailureMechanismCombinedSectionAssemblyResults(
            ExportableFailureMechanismSection combinedFailureMechanismSection, IEnumerable<ExportableFailureMechanism> exportableFailureMechanisms,
            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> exportableFailureMechanismCombinedSectionAssemblyResults)
        {
            Assert.AreEqual(17, exportableFailureMechanismCombinedSectionAssemblyResults.Count());

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(pipingIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(pipingIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(grassCoverErosionInwardsIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(grassCoverErosionInwardsIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(macroStabilityInwardsIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(macroStabilityInwardsIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(microstabilityIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(microstabilityIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(stabilityStoneCoverIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(stabilityStoneCoverIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(waveImpactAsphaltCoverIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(waveImpactAsphaltCoverIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(waterPressureAsphaltCoverIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(waterPressureAsphaltCoverIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(grassCoverErosionOutwardsIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(grassCoverErosionOutwardsIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(grassCoverSlipOffOutwardsIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(grassCoverSlipOffOutwardsIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(grassCoverSlipOffInwardsIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(grassCoverSlipOffInwardsIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(heightStructuresIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(heightStructuresIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(closingStructuresIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(closingStructuresIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(pipingStructureIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(pipingStructureIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(stabilityPointStructuresIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(stabilityPointStructuresIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(duneErosionIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(duneErosionIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(firstSpecificFailureMechanismIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(firstSpecificFailureMechanismIndex));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(secondSpecificFailureMechanismIndex).SectionAssemblyResults,
                exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(secondSpecificFailureMechanismIndex));
        }

        private static void AssertExportableFailureMechanismCombinedSectionAssemblyResult(
            ExportableFailureMechanismSection combinedFailureMechanismSectionAssembly,
            IEnumerable<ExportableFailureMechanismSectionAssemblyResult> failureMechanismSectionAssemblyResults,
            ExportableFailureMechanismCombinedSectionAssemblyResult actualExportableFailureMechanismCombinedSectionAssemblyResult)
        {
            ExportableFailureMechanismSectionAssemblyResult associatedAssemblyResult = GetCorrespondingSectionAssemblyResultResult(
                combinedFailureMechanismSectionAssembly, failureMechanismSectionAssemblyResults);
            Assert.AreSame(associatedAssemblyResult, actualExportableFailureMechanismCombinedSectionAssemblyResult.FailureMechanismSectionResult);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Dominant, actualExportableFailureMechanismCombinedSectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableAssemblyMethod.BOI3B1, actualExportableFailureMechanismCombinedSectionAssemblyResult.AssemblyMethod);
        }

        private static ExportableFailureMechanismSectionAssemblyResult GetCorrespondingSectionAssemblyResultResult(
            ExportableFailureMechanismSection combinedFailureMechanismSection,
            IEnumerable<ExportableFailureMechanismSectionAssemblyResult> sectionAssemblyResults)
        {
            return sectionAssemblyResults.FirstOrDefault(assemblyResult =>
            {
                ExportableFailureMechanismSection exportableFailureMechanismSection = assemblyResult.FailureMechanismSection;

                return combinedFailureMechanismSection.StartDistance >= exportableFailureMechanismSection.StartDistance
                       && combinedFailureMechanismSection.EndDistance <= exportableFailureMechanismSection.EndDistance;
            });
        }
    }
}