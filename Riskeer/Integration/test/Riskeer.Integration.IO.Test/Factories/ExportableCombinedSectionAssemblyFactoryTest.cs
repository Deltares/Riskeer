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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.Util;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableCombinedSectionAssemblyFactoryTest
    {
        [Test]
        public void CreateExportableCombinedSectionAssemblyCollection_CombinedSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(
                null, new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("combinedSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void CreateExportableCombinedSectionAssemblyCollection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(
                Enumerable.Empty<CombinedFailureMechanismSectionAssemblyResult>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateExportableCombinedSectionAssemblyCollection_WithAssemblyResults_ReturnsExportableCombinedSectionAssemblyCollection(bool hasAssemblyGroupResults)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(0, 0),
                new Point2D(2, 2)
            });

            assessmentSection.SpecificFailurePaths.AddRange(new[]
            {
                new SpecificFailurePath
                {
                    Code = "Nieuw1"
                },
                new SpecificFailurePath
                {
                    Code = "Nieuw2"
                }
            });

            CombinedFailureMechanismSectionAssemblyResult[] assemblyResults =
            {
                CreateCombinedFailureMechanismSectionAssemblyResult(21, hasAssemblyGroupResults),
                CreateCombinedFailureMechanismSectionAssemblyResult(22, hasAssemblyGroupResults)
            };

            // Call
            IEnumerable<ExportableCombinedSectionAssembly> exportableCombinedSectionAssemblies =
                ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(assemblyResults, assessmentSection);

            // Assert
            AssertCombinedFailureMechanismSectionAssemblyResults(
                assemblyResults, exportableCombinedSectionAssemblies,
                assessmentSection.ReferenceLine, hasAssemblyGroupResults);
        }

        private static CombinedFailureMechanismSectionAssemblyResult CreateCombinedFailureMechanismSectionAssemblyResult(int seed, bool hasAssemblyGroupResults)
        {
            var random = new Random(seed);

            return new CombinedFailureMechanismSectionAssemblyResult(
                random.Next(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                {
                    Piping = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    GrassCoverErosionInwards = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    MacroStabilityInwards = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    Microstability = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    StabilityStoneCover = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    WaveImpactAsphaltCover = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    WaterPressureAsphaltCover = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    GrassCoverErosionOutwards = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    GrassCoverSlipOffOutwards = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    GrassCoverSlipOffInwards = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    HeightStructures = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    ClosingStructures = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    PipingStructure = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    StabilityPointStructures = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    DuneErosion = GetAssemblyGroup(random, hasAssemblyGroupResults),
                    SpecificFailurePaths = new[]
                    {
                        GetAssemblyGroup(random, hasAssemblyGroupResults),
                        GetAssemblyGroup(random, hasAssemblyGroupResults)
                    }
                });
        }

        private static FailureMechanismSectionAssemblyGroup? GetAssemblyGroup(Random random, bool hasAssemblyGroupResults)
        {
            return hasAssemblyGroupResults
                       ? random.NextEnumValue<FailureMechanismSectionAssemblyGroup>()
                       : (FailureMechanismSectionAssemblyGroup?) null;
        }

        private static void AssertCombinedFailureMechanismSectionAssemblyResults(IEnumerable<CombinedFailureMechanismSectionAssemblyResult> assemblyResults,
                                                                                 IEnumerable<ExportableCombinedSectionAssembly> exportableCombinedSectionAssemblies,
                                                                                 ReferenceLine referenceLine, bool hasAssemblyGroupResults)
        {
            int expectedNrOfSections = assemblyResults.Count();
            Assert.AreEqual(expectedNrOfSections, exportableCombinedSectionAssemblies.Count());

            for (var i = 0; i < expectedNrOfSections; i++)
            {
                CombinedFailureMechanismSectionAssemblyResult combinedFailureMechanismSectionAssemblyResult = assemblyResults.ElementAt(i);
                ExportableCombinedSectionAssembly exportableCombinedSectionAssembly = exportableCombinedSectionAssemblies.ElementAt(i);

                AssertExportableCombinedFailureMechanismSection(combinedFailureMechanismSectionAssemblyResult, exportableCombinedSectionAssembly.Section, referenceLine);
                AssertExportableCombinedFailureMechanismSectionResult(
                    combinedFailureMechanismSectionAssemblyResult, exportableCombinedSectionAssembly.Section, exportableCombinedSectionAssembly,
                    hasAssemblyGroupResults);
            }
        }

        private static void AssertExportableCombinedFailureMechanismSection(CombinedFailureMechanismSectionAssemblyResult expectedSection,
                                                                            ExportableCombinedFailureMechanismSection actualSection,
                                                                            ReferenceLine referenceLine)
        {
            IEnumerable<Point2D> expectedGeometry = FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(
                referenceLine,
                actualSection.StartDistance,
                actualSection.EndDistance).ToArray();
            CollectionAssert.IsNotEmpty(expectedGeometry);

            Assert.AreEqual(expectedSection.SectionStart, actualSection.StartDistance);
            Assert.AreEqual(expectedSection.SectionEnd, actualSection.EndDistance);
            CollectionAssert.AreEqual(expectedGeometry, actualSection.Geometry);
            Assert.AreEqual(ExportableAssemblyMethod.WBI3A1, actualSection.AssemblyMethod);
        }

        private static void AssertExportableCombinedFailureMechanismSectionResult(CombinedFailureMechanismSectionAssemblyResult expectedSection,
                                                                                  ExportableCombinedFailureMechanismSection actualSection,
                                                                                  ExportableCombinedSectionAssembly actualSectionResult,
                                                                                  bool hasAssemblyGroupResults)
        {
            Assert.AreSame(actualSection, actualSectionResult.Section);
            Assert.AreEqual(expectedSection.TotalResult, actualSectionResult.CombinedSectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableAssemblyMethod.WBI3C1, actualSectionResult.CombinedSectionAssemblyResult.AssemblyMethod);

            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismCombinedSectionResults = actualSectionResult.FailureMechanismResults;

            if (!hasAssemblyGroupResults)
            {
                CollectionAssert.IsEmpty(failureMechanismCombinedSectionResults);
                return;
            }

            Assert.AreEqual(17, failureMechanismCombinedSectionResults.Count());
            Assert.IsTrue(failureMechanismCombinedSectionResults.All(result => result.SectionAssemblyResult.AssemblyMethod == ExportableAssemblyMethod.WBI3B1));

            AssertSubSection(expectedSection.Piping, "STPH", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(0));
            AssertSubSection(expectedSection.GrassCoverErosionInwards, "GEKB", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(1));
            AssertSubSection(expectedSection.MacroStabilityInwards, "STBI", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(2));
            AssertSubSection(expectedSection.Microstability, "STMI", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(3));
            AssertSubSection(expectedSection.StabilityStoneCover, "ZST", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(4));
            AssertSubSection(expectedSection.WaveImpactAsphaltCover, "AGK", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(5));
            AssertSubSection(expectedSection.WaterPressureAsphaltCover, "AWO", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(6));
            AssertSubSection(expectedSection.GrassCoverErosionOutwards, "GEBU", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(7));
            AssertSubSection(expectedSection.GrassCoverSlipOffOutwards, "GABU", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(8));
            AssertSubSection(expectedSection.GrassCoverSlipOffInwards, "GABI", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(9));
            AssertSubSection(expectedSection.HeightStructures, "HTKW", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(10));
            AssertSubSection(expectedSection.ClosingStructures, "BSKW", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(11));
            AssertSubSection(expectedSection.PipingStructure, "PKW", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(12));
            AssertSubSection(expectedSection.StabilityPointStructures, "STKWp", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(13));
            AssertSubSection(expectedSection.DuneErosion, "DA", ExportableFailureMechanismType.Generic,
                             failureMechanismCombinedSectionResults.ElementAt(14));
            AssertSubSection(expectedSection.SpecificFailurePaths[0], "Nieuw1", ExportableFailureMechanismType.Specific,
                             failureMechanismCombinedSectionResults.ElementAt(15));
            AssertSubSection(expectedSection.SpecificFailurePaths[1], "Nieuw2", ExportableFailureMechanismType.Specific,
                             failureMechanismCombinedSectionResults.ElementAt(16));
        }

        private static void AssertSubSection(FailureMechanismSectionAssemblyGroup? subSectionGroup, string subSectionCode,
                                             ExportableFailureMechanismType failureMechanismType,
                                             ExportableFailureMechanismCombinedSectionAssemblyResult actualResult)
        {
            Assert.AreEqual(subSectionGroup, actualResult.SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(subSectionCode, actualResult.Code);
            Assert.AreEqual(failureMechanismType, actualResult.FailureMechanismType);
        }
    }
}