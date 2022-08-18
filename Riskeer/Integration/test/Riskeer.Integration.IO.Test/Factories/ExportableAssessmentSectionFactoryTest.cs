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
using Riskeer.AssemblyTool.IO.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Factories;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableAssessmentSectionFactoryTest
    {
        [Test]
        public void CreateExportableAssessmentSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(null);

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
            FailureMechanismTestHelper.AddSections(assessmentSection.SpecificFailureMechanisms.First(), random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.SpecificFailureMechanisms.Last(), random.Next(1, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);

                // Assert
                Assert.AreEqual(name, exportableAssessmentSection.Name);
                Assert.AreEqual(id, exportableAssessmentSection.Id);
                CollectionAssert.AreEqual(assessmentSection.ReferenceLine.Points, exportableAssessmentSection.Geometry);

                ExportableAssessmentSectionAssemblyResult exportableAssessmentSectionAssemblyResult = exportableAssessmentSection.AssessmentSectionAssembly;
                Assert.AreEqual(ExportableAssemblyMethod.BOI2A1, exportableAssessmentSectionAssemblyResult.ProbabilityAssemblyMethod);
                Assert.AreEqual(ExportableAssemblyMethod.BOI2B1, exportableAssessmentSectionAssemblyResult.AssemblyGroupAssemblyMethod);
                Assert.AreEqual(AssessmentSectionAssemblyGroup.APlus, exportableAssessmentSectionAssemblyResult.AssemblyGroup);
                Assert.AreEqual(0.14, exportableAssessmentSectionAssemblyResult.Probability);

                AssertExportableFailureMechanisms(exportableAssessmentSection.FailureMechanisms, assessmentSection);

                Assert.AreEqual(1, exportableAssessmentSection.CombinedSectionAssemblies.Count());
                ExportableCombinedSectionAssembly exportableCombinedSectionAssembly = exportableAssessmentSection.CombinedSectionAssemblies.ElementAt(0);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Zero, exportableCombinedSectionAssembly.CombinedSectionAssemblyResult.AssemblyGroup);
                Assert.AreEqual(ExportableAssemblyMethod.BOI3C1, exportableCombinedSectionAssembly.CombinedSectionAssemblyResult.AssemblyGroupAssemblyMethod);
                Assert.AreEqual(0.0, exportableCombinedSectionAssembly.CombinedSectionAssemblyResult.FailureMechanismSection.StartDistance);
                Assert.AreEqual(1.0, exportableCombinedSectionAssembly.CombinedSectionAssemblyResult.FailureMechanismSection.EndDistance);
                AssertExportableFailureMechanismCombinedSectionAssemblyResults(exportableCombinedSectionAssembly.FailureMechanismResults, assessmentSection);
                Assert.AreEqual(ExportableAssemblyMethod.BOI3A1, exportableCombinedSectionAssembly.Section.AssemblyMethod);
                Assert.AreEqual(0.0, exportableCombinedSectionAssembly.Section.StartDistance);
                Assert.AreEqual(1.0, exportableCombinedSectionAssembly.Section.EndDistance);
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

            FailureMechanismTestHelper.AddSections(assessmentSection.SpecificFailureMechanisms.First(), random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.SpecificFailureMechanisms.Last(), random.Next(1, 10));

            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms()
                                                                            .Concat(assessmentSection.SpecificFailureMechanisms))
            {
                failureMechanism.InAssembly = false;
            }

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);

                // Assert
                Assert.AreEqual(name, exportableAssessmentSection.Name);
                Assert.AreEqual(id, exportableAssessmentSection.Id);
                CollectionAssert.AreEqual(assessmentSection.ReferenceLine.Points, exportableAssessmentSection.Geometry);

                ExportableAssessmentSectionAssemblyResult exportableAssessmentSectionAssemblyResult = exportableAssessmentSection.AssessmentSectionAssembly;
                Assert.AreEqual(ExportableAssemblyMethod.BOI2A1, exportableAssessmentSectionAssemblyResult.ProbabilityAssemblyMethod);
                Assert.AreEqual(ExportableAssemblyMethod.BOI2B1, exportableAssessmentSectionAssemblyResult.AssemblyGroupAssemblyMethod);
                Assert.AreEqual(AssessmentSectionAssemblyGroup.APlus, exportableAssessmentSectionAssemblyResult.AssemblyGroup);
                Assert.AreEqual(0.14, exportableAssessmentSectionAssemblyResult.Probability);

                CollectionAssert.IsEmpty(exportableAssessmentSection.FailureMechanisms);

                Assert.AreEqual(1, exportableAssessmentSection.CombinedSectionAssemblies.Count());
                ExportableCombinedSectionAssembly exportableCombinedSectionAssembly = exportableAssessmentSection.CombinedSectionAssemblies.ElementAt(0);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Zero, exportableCombinedSectionAssembly.CombinedSectionAssemblyResult.AssemblyGroup);
                Assert.AreEqual(ExportableAssemblyMethod.BOI3C1, exportableCombinedSectionAssembly.CombinedSectionAssemblyResult.AssemblyGroupAssemblyMethod);
                Assert.AreEqual(0.0, exportableCombinedSectionAssembly.CombinedSectionAssemblyResult.FailureMechanismSection.StartDistance);
                Assert.AreEqual(1.0, exportableCombinedSectionAssembly.CombinedSectionAssemblyResult.FailureMechanismSection.EndDistance);
                CollectionAssert.IsEmpty(exportableCombinedSectionAssembly.FailureMechanismResults);
                Assert.AreEqual(ExportableAssemblyMethod.BOI3A1, exportableCombinedSectionAssembly.Section.AssemblyMethod);
                Assert.AreEqual(0.0, exportableCombinedSectionAssembly.Section.StartDistance);
                Assert.AreEqual(1.0, exportableCombinedSectionAssembly.Section.EndDistance);
            }
        }

        private static void AssertExportableFailureMechanisms(
            IEnumerable<ExportableFailureMechanism> exportableFailureMechanisms,
            AssessmentSection assessmentSection)
        {
            Assert.AreEqual(17, exportableFailureMechanisms.Count());

            AssertExportableFailureMechanism(assessmentSection.Piping,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(0));

            AssertExportableFailureMechanism(assessmentSection.GrassCoverErosionInwards,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(1));

            AssertExportableFailureMechanism(assessmentSection.MacroStabilityInwards,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(2));

            AssertExportableFailureMechanism(assessmentSection.Microstability,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(3));

            AssertExportableFailureMechanism(assessmentSection.StabilityStoneCover,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(4));

            AssertExportableFailureMechanism(assessmentSection.WaveImpactAsphaltCover,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(5));

            AssertExportableFailureMechanism(assessmentSection.WaterPressureAsphaltCover,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(6));

            AssertExportableFailureMechanism(assessmentSection.GrassCoverErosionOutwards,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(7));

            AssertExportableFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(8));

            AssertExportableFailureMechanism(assessmentSection.GrassCoverSlipOffInwards,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(9));

            AssertExportableFailureMechanism(assessmentSection.HeightStructures,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(10));

            AssertExportableFailureMechanism(assessmentSection.ClosingStructures,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(11));

            AssertExportableFailureMechanism(assessmentSection.PipingStructure,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(12));

            AssertExportableFailureMechanism(assessmentSection.StabilityPointStructures,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(13));

            AssertExportableFailureMechanism(assessmentSection.DuneErosion,
                                             ExportableFailureMechanismType.Generic,
                                             exportableFailureMechanisms.ElementAt(14));

            AssertExportableFailureMechanism(assessmentSection.SpecificFailureMechanisms.First(),
                                             ExportableFailureMechanismType.Specific,
                                             exportableFailureMechanisms.ElementAt(15));

            AssertExportableFailureMechanism(assessmentSection.SpecificFailureMechanisms.Last(),
                                             ExportableFailureMechanismType.Specific,
                                             exportableFailureMechanisms.ElementAt(16));
        }

        private static void AssertExportableFailureMechanism(IFailureMechanism<FailureMechanismSectionResult> failureMechanism,
                                                             ExportableFailureMechanismType expectedFailureMechanismType,
                                                             ExportableFailureMechanism actualExportableFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanismType, actualExportableFailureMechanism.FailureMechanismType);
            Assert.AreEqual(failureMechanism.Code, actualExportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssemblyResult = actualExportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(0.1, exportableFailureMechanismAssemblyResult.Probability);
            Assert.AreEqual(ExportableAssemblyMethod.BOI1A1, exportableFailureMechanismAssemblyResult.AssemblyMethod);

            Assert.AreEqual(failureMechanism.SectionResults.Count(), actualExportableFailureMechanism.SectionAssemblyResults.Count());
        }

        private static void AssertExportableFailureMechanismCombinedSectionAssemblyResults(IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> exportableFailureMechanismCombinedSectionAssemblyResults,
                                                                                           AssessmentSection assessmentSection)
        {
            Assert.AreEqual(17, exportableFailureMechanismCombinedSectionAssemblyResults.Count());

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.Piping,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(0));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.GrassCoverErosionInwards,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(1));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.MacroStabilityInwards,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(2));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.Microstability,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(3));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.StabilityStoneCover,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(4));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.WaveImpactAsphaltCover,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(5));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.WaterPressureAsphaltCover,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(6));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.GrassCoverErosionOutwards,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(7));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.GrassCoverSlipOffOutwards,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(8));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.GrassCoverSlipOffInwards,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(9));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.HeightStructures,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(10));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.ClosingStructures,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(11));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.PipingStructure,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(12));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.StabilityPointStructures,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(13));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.DuneErosion,
                                                                          ExportableFailureMechanismType.Generic,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(14));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.SpecificFailureMechanisms.First(),
                                                                          ExportableFailureMechanismType.Specific,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(15));

            AssertExportableFailureMechanismCombinedSectionAssemblyResult(assessmentSection.SpecificFailureMechanisms.Last(),
                                                                          ExportableFailureMechanismType.Specific,
                                                                          exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(16));
        }

        private static void AssertExportableFailureMechanismCombinedSectionAssemblyResult(IFailureMechanism failureMechanism,
                                                                                          ExportableFailureMechanismType expectedFailureMechanismType,
                                                                                          ExportableFailureMechanismCombinedSectionAssemblyResult actualExportableFailureMechanismCombinedSectionAssemblyResult)
        {
            Assert.AreEqual(failureMechanism.Code, actualExportableFailureMechanismCombinedSectionAssemblyResult.Code);
            Assert.AreEqual(expectedFailureMechanismType, actualExportableFailureMechanismCombinedSectionAssemblyResult.FailureMechanismType);
            Assert.AreEqual(failureMechanism.Name, actualExportableFailureMechanismCombinedSectionAssemblyResult.Name);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Dominant, actualExportableFailureMechanismCombinedSectionAssemblyResult.SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableAssemblyMethod.BOI3B1, actualExportableFailureMechanismCombinedSectionAssemblyResult.SectionAssemblyResult.AssemblyMethod);
        }
    }
}