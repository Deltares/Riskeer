using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;

namespace Ringtoets.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableCombinedSectionAssemblyCollectionFactoryTest
    {
        [Test]
        public void CreateExportableCombinedSectionAssemblyCollection_CombinedSectionAssemblyResultssNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableCombinedSectionAssemblyCollectionFactory.CreateExportableCombinedSectionAssemblyCollection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void CreateExportableCombinedSectionAssemblyCollection_WithAssemblyResults_ReturnsExportableCombinedSectionAssemblyCollection()
        {
            // Setup
            CombinedFailureMechanismSectionAssemblyResult[] assemblyResults =
            {
                CreateCombinedFailureMechanismSectionAssemblyResult(21),
                CreateCombinedFailureMechanismSectionAssemblyResult(22)
            };

            // Call
            ExportableCombinedSectionAssemblyCollection exportableCombinedSectionAssembly =
                ExportableCombinedSectionAssemblyCollectionFactory.CreateExportableCombinedSectionAssemblyCollection(assemblyResults);

            // Assert
            AssertCombinedFailureMechanismSectionAssemblyResults(assemblyResults,
                                                                 exportableCombinedSectionAssembly);
        }

        private static CombinedFailureMechanismSectionAssemblyResult CreateCombinedFailureMechanismSectionAssemblyResult(int seed)
        {
            var random = new Random(seed);

            return new CombinedFailureMechanismSectionAssemblyResult(random.NextDouble(),
                                                                     random.NextDouble(),
                                                                     random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                     new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                                                                     {
                                                                         Piping = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         GrassCoverErosionInwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         MacroStabilityInwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         MacroStabilityOutwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         Microstability = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         StabilityStoneCover = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         WaveImpactAsphaltCover = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         WaterPressureAsphaltCover = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         GrassCoverErosionOutwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         GrassCoverSlipOffOutwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         GrassCoverSlipOffInwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         HeightStructures = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         ClosingStructures = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         PipingStructure = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         StabilityPointStructures = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         StrengthStabilityLengthwiseConstruction = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         DuneErosion = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                         TechnicalInnovation = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
                                                                     });
        }

        private static void AssertCombinedFailureMechanismSectionAssemblyResults(IEnumerable<CombinedFailureMechanismSectionAssemblyResult> expectedSections,
                                                                                 ExportableCombinedSectionAssemblyCollection collection)
        {
            int expectedNrOfSections = expectedSections.Count();
            Assert.AreEqual(expectedNrOfSections, collection.Sections.Count());
            Assert.AreEqual(expectedNrOfSections, collection.CombinedSectionAssemblyResults.Count());

            for (var i = 0; i < expectedNrOfSections; i++)
            {
                CombinedFailureMechanismSectionAssemblyResult expectedSection = expectedSections.ElementAt(i);
                ExportableCombinedFailureMechanismSection actualSection = collection.Sections.ElementAt(i);
                ExportableCombinedSectionAssembly actualSectionResult = collection.CombinedSectionAssemblyResults.ElementAt(i);

                AssertExportableCombinedFailureMechanismSection(expectedSection, actualSection);
                AssertExportableCombinedFailureMechanismSectionResult(actualSectionResult, actualSection, expectedSection);
            }
        }

        private static void AssertExportableCombinedFailureMechanismSection(CombinedFailureMechanismSectionAssemblyResult expectedSection,
                                                                            ExportableCombinedFailureMechanismSection actualSection)
        {
            Assert.AreEqual(expectedSection.SectionStart, actualSection.StartDistance);
            Assert.AreEqual(expectedSection.SectionEnd, actualSection.EndDistance);
            CollectionAssert.IsEmpty(actualSection.Geometry);
            Assert.AreEqual(ExportableAssemblyMethod.WBI3A1, actualSection.AssemblyMethod);
        }

        private static void AssertExportableCombinedFailureMechanismSectionResult(ExportableCombinedSectionAssembly expectedSectionResult,
                                                                                  ExportableCombinedFailureMechanismSection expectedSection,
                                                                                  CombinedFailureMechanismSectionAssemblyResult actualCombinedSectionAssemblyResult)
        {
            Assert.AreSame(expectedSection, expectedSectionResult.Section);
            Assert.AreEqual(actualCombinedSectionAssemblyResult.TotalResult, expectedSectionResult.CombinedSectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableAssemblyMethod.WBI3C1, expectedSectionResult.CombinedSectionAssemblyResult.AssemblyMethod);

            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismCombinedSectionResults = expectedSectionResult.FailureMechanismResults;
            Assert.AreEqual(18, failureMechanismCombinedSectionResults.Count());
            Assert.IsTrue(failureMechanismCombinedSectionResults.All(result => result.SectionAssemblyResult.AssemblyMethod == ExportableAssemblyMethod.WBI3B1));

            Assert.AreEqual(actualCombinedSectionAssemblyResult.Piping, failureMechanismCombinedSectionResults.ElementAt(0).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STPH, failureMechanismCombinedSectionResults.ElementAt(0).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.GrassCoverErosionInwards, failureMechanismCombinedSectionResults.ElementAt(1).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.GEKB, failureMechanismCombinedSectionResults.ElementAt(1).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.MacroStabilityInwards, failureMechanismCombinedSectionResults.ElementAt(2).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STBI, failureMechanismCombinedSectionResults.ElementAt(2).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.MacroStabilityOutwards, failureMechanismCombinedSectionResults.ElementAt(3).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STBU, failureMechanismCombinedSectionResults.ElementAt(3).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.Microstability, failureMechanismCombinedSectionResults.ElementAt(4).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STMI, failureMechanismCombinedSectionResults.ElementAt(4).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.StabilityStoneCover, failureMechanismCombinedSectionResults.ElementAt(5).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.ZST, failureMechanismCombinedSectionResults.ElementAt(5).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.WaveImpactAsphaltCover, failureMechanismCombinedSectionResults.ElementAt(6).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.AGK, failureMechanismCombinedSectionResults.ElementAt(6).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.WaterPressureAsphaltCover, failureMechanismCombinedSectionResults.ElementAt(7).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.AWO, failureMechanismCombinedSectionResults.ElementAt(7).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.GrassCoverErosionOutwards, failureMechanismCombinedSectionResults.ElementAt(8).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.GEBU, failureMechanismCombinedSectionResults.ElementAt(8).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.GrassCoverSlipOffOutwards, failureMechanismCombinedSectionResults.ElementAt(9).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.GABU, failureMechanismCombinedSectionResults.ElementAt(9).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.GrassCoverSlipOffInwards, failureMechanismCombinedSectionResults.ElementAt(10).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.GABI, failureMechanismCombinedSectionResults.ElementAt(10).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.HeightStructures, failureMechanismCombinedSectionResults.ElementAt(11).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.HTKW, failureMechanismCombinedSectionResults.ElementAt(11).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.ClosingStructures, failureMechanismCombinedSectionResults.ElementAt(12).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.BSKW, failureMechanismCombinedSectionResults.ElementAt(12).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.PipingStructure, failureMechanismCombinedSectionResults.ElementAt(13).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.PKW, failureMechanismCombinedSectionResults.ElementAt(13).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.StabilityPointStructures, failureMechanismCombinedSectionResults.ElementAt(14).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STKWp, failureMechanismCombinedSectionResults.ElementAt(14).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.StrengthStabilityLengthwiseConstruction, failureMechanismCombinedSectionResults.ElementAt(15).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STKWl, failureMechanismCombinedSectionResults.ElementAt(15).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.DuneErosion, failureMechanismCombinedSectionResults.ElementAt(16).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.DA, failureMechanismCombinedSectionResults.ElementAt(16).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.TechnicalInnovation, failureMechanismCombinedSectionResults.ElementAt(17).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.INN, failureMechanismCombinedSectionResults.ElementAt(17).Code);
        }
    }
}