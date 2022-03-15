// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Assembly;
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

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);

                // Assert
                Assert.AreEqual(name, exportableAssessmentSection.Name);
                Assert.AreEqual(id, exportableAssessmentSection.Id);
                CollectionAssert.AreEqual(assessmentSection.ReferenceLine.Points, exportableAssessmentSection.Geometry);

                ExportableAssessmentSectionAssemblyResult exportableAssessmentSectionAssemblyResult = exportableAssessmentSection.AssessmentSectionAssembly;
                Assert.AreEqual(ExportableAssemblyMethod.WBI2B1, exportableAssessmentSectionAssemblyResult.AssemblyMethod);
                Assert.AreEqual(AssessmentSectionAssemblyGroup.APlus, exportableAssessmentSectionAssemblyResult.AssemblyGroup);
                Assert.AreEqual(0.14, exportableAssessmentSectionAssemblyResult.Probability);

                AssertExportableFailureMechanismsWithoutProbability(exportableAssessmentSection.FailureMechanisms,
                                                                    assessmentSection);

                CollectionAssert.IsEmpty(exportableAssessmentSection.CombinedSectionAssemblies);
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
            
            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
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
                Assert.AreEqual(ExportableAssemblyMethod.WBI2B1, exportableAssessmentSectionAssemblyResult.AssemblyMethod);
                Assert.AreEqual(AssessmentSectionAssemblyGroup.APlus, exportableAssessmentSectionAssemblyResult.AssemblyGroup);
                Assert.AreEqual(0.14, exportableAssessmentSectionAssemblyResult.Probability);

                CollectionAssert.IsEmpty(exportableAssessmentSection.FailureMechanisms);
                CollectionAssert.IsEmpty(exportableAssessmentSection.CombinedSectionAssemblies);
            }
        }

        private static void AssertExportableFailureMechanismsWithoutProbability(
            IEnumerable<ExportableFailureMechanism> exportableFailureMechanisms,
            AssessmentSection assessmentSection)
        {
            Assert.AreEqual(15, exportableFailureMechanisms.Count());

            AssertExportableFailureMechanism(ExportableFailureMechanismType.STPH,
                                             assessmentSection.Piping,
                                             exportableFailureMechanisms.ElementAt(0));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.STBI,
                                             assessmentSection.MacroStabilityInwards,
                                             exportableFailureMechanisms.ElementAt(1));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.GEKB,
                                             assessmentSection.GrassCoverErosionInwards,
                                             exportableFailureMechanisms.ElementAt(2));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.HTKW,
                                             assessmentSection.HeightStructures,
                                             exportableFailureMechanisms.ElementAt(3));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.BSKW,
                                             assessmentSection.ClosingStructures,
                                             exportableFailureMechanisms.ElementAt(4));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.STKWp,
                                             assessmentSection.StabilityPointStructures,
                                             exportableFailureMechanisms.ElementAt(5));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.ZST,
                                             assessmentSection.StabilityStoneCover,
                                             exportableFailureMechanisms.ElementAt(6));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.AGK,
                                             assessmentSection.WaveImpactAsphaltCover,
                                             exportableFailureMechanisms.ElementAt(7));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.GEBU,
                                             assessmentSection.GrassCoverErosionOutwards,
                                             exportableFailureMechanisms.ElementAt(8));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.DA,
                                             assessmentSection.DuneErosion,
                                             exportableFailureMechanisms.ElementAt(9));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.STMI,
                                             assessmentSection.Microstability,
                                             exportableFailureMechanisms.ElementAt(10));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.GABU,
                                             assessmentSection.GrassCoverSlipOffOutwards,
                                             exportableFailureMechanisms.ElementAt(11));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.GABI,
                                             assessmentSection.GrassCoverSlipOffInwards,
                                             exportableFailureMechanisms.ElementAt(12));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.PKW,
                                             assessmentSection.PipingStructure,
                                             exportableFailureMechanisms.ElementAt(13));

            AssertExportableFailureMechanism(ExportableFailureMechanismType.AWO,
                                             assessmentSection.WaterPressureAsphaltCover,
                                             exportableFailureMechanisms.ElementAt(14));
        }

        private static void AssertExportableFailureMechanism(ExportableFailureMechanismType expectedFailureMechanismCode,
                                                             IFailurePath<FailureMechanismSectionResult> failureMechanism,
                                                             ExportableFailureMechanism actualExportableFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanismCode, actualExportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssemblyResult = actualExportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(0, exportableFailureMechanismAssemblyResult.Probability);
            Assert.IsFalse(exportableFailureMechanismAssemblyResult.IsManual);

            Assert.AreEqual(failureMechanism.SectionResults.Count(), actualExportableFailureMechanism.SectionAssemblyResults.Count());
        }
    }
}