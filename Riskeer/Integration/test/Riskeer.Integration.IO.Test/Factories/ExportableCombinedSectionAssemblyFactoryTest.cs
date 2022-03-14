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
                null, new ReferenceLine());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("combinedSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void CreateExportableCombinedSectionAssemblyCollection_ReferenceLineNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(
                Enumerable.Empty<CombinedFailureMechanismSectionAssemblyResult>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateExportableCombinedSectionAssemblyCollection_WithAssemblyResults_ReturnsExportableCombinedSectionAssemblyCollection(bool hasAssemblyGroupResults)
        {
            // Setup
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0, 0),
                new Point2D(2, 2)
            });
            CombinedFailureMechanismSectionAssemblyResult[] assemblyResults =
            {
                CreateCombinedFailureMechanismSectionAssemblyResult(21, hasAssemblyGroupResults),
                CreateCombinedFailureMechanismSectionAssemblyResult(22, hasAssemblyGroupResults)
            };

            // Call
            IEnumerable<ExportableCombinedSectionAssembly> exportableCombinedSectionAssemblies =
                ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(assemblyResults, referenceLine);

            // Assert
            AssertCombinedFailureMechanismSectionAssemblyResults(assemblyResults, exportableCombinedSectionAssemblies, referenceLine, hasAssemblyGroupResults);
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
                    DuneErosion = GetAssemblyGroup(random, hasAssemblyGroupResults)
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

            Assert.AreEqual(15, failureMechanismCombinedSectionResults.Count());
            Assert.IsTrue(failureMechanismCombinedSectionResults.All(result => result.SectionAssemblyResult.AssemblyMethod == ExportableAssemblyMethod.WBI3B1));

            Assert.AreEqual(expectedSection.Piping, failureMechanismCombinedSectionResults.ElementAt(0).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.STPH, failureMechanismCombinedSectionResults.ElementAt(0).Code);

            Assert.AreEqual(expectedSection.GrassCoverErosionInwards, failureMechanismCombinedSectionResults.ElementAt(1).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.GEKB, failureMechanismCombinedSectionResults.ElementAt(1).Code);

            Assert.AreEqual(expectedSection.MacroStabilityInwards, failureMechanismCombinedSectionResults.ElementAt(2).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.STBI, failureMechanismCombinedSectionResults.ElementAt(2).Code);

            Assert.AreEqual(expectedSection.Microstability, failureMechanismCombinedSectionResults.ElementAt(3).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.STMI, failureMechanismCombinedSectionResults.ElementAt(3).Code);

            Assert.AreEqual(expectedSection.StabilityStoneCover, failureMechanismCombinedSectionResults.ElementAt(4).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.ZST, failureMechanismCombinedSectionResults.ElementAt(4).Code);

            Assert.AreEqual(expectedSection.WaveImpactAsphaltCover, failureMechanismCombinedSectionResults.ElementAt(5).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.AGK, failureMechanismCombinedSectionResults.ElementAt(5).Code);

            Assert.AreEqual(expectedSection.WaterPressureAsphaltCover, failureMechanismCombinedSectionResults.ElementAt(6).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.AWO, failureMechanismCombinedSectionResults.ElementAt(6).Code);

            Assert.AreEqual(expectedSection.GrassCoverErosionOutwards, failureMechanismCombinedSectionResults.ElementAt(7).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.GEBU, failureMechanismCombinedSectionResults.ElementAt(7).Code);

            Assert.AreEqual(expectedSection.GrassCoverSlipOffOutwards, failureMechanismCombinedSectionResults.ElementAt(8).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.GABU, failureMechanismCombinedSectionResults.ElementAt(8).Code);

            Assert.AreEqual(expectedSection.GrassCoverSlipOffInwards, failureMechanismCombinedSectionResults.ElementAt(9).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.GABI, failureMechanismCombinedSectionResults.ElementAt(9).Code);

            Assert.AreEqual(expectedSection.HeightStructures, failureMechanismCombinedSectionResults.ElementAt(10).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.HTKW, failureMechanismCombinedSectionResults.ElementAt(10).Code);

            Assert.AreEqual(expectedSection.ClosingStructures, failureMechanismCombinedSectionResults.ElementAt(11).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.BSKW, failureMechanismCombinedSectionResults.ElementAt(11).Code);

            Assert.AreEqual(expectedSection.PipingStructure, failureMechanismCombinedSectionResults.ElementAt(12).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.PKW, failureMechanismCombinedSectionResults.ElementAt(12).Code);

            Assert.AreEqual(expectedSection.StabilityPointStructures, failureMechanismCombinedSectionResults.ElementAt(13).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.STKWp, failureMechanismCombinedSectionResults.ElementAt(13).Code);

            Assert.AreEqual(expectedSection.DuneErosion, failureMechanismCombinedSectionResults.ElementAt(14).SectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(ExportableFailureMechanismType.DA, failureMechanismCombinedSectionResults.ElementAt(14).Code);
        }
    }
}