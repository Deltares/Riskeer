﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableWaterPressureAsphaltCoverFailureMechanismFactoryTest
    {
        [Test]
        public void CreateExportableFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                ExportableWaterPressureAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismNotRelevant_ReturnsDefaultExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism
            {
                IsRelevant = false
            };
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> exportableFailureMechanism =
                ExportableWaterPressureAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism);

            // Assert
            Assert.AreEqual(ExportableFailureMechanismType.AWO, exportableFailureMechanism.Code);
            Assert.AreEqual(ExportableFailureMechanismGroup.Group4, exportableFailureMechanism.Group);

            ExportableFailureMechanismAssemblyResult failureMechanismAssemblyResult = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(ExportableAssemblyMethod.WBI1A1, failureMechanismAssemblyResult.AssemblyMethod);
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.NotApplicable, failureMechanismAssemblyResult.AssemblyCategory);

            CollectionAssert.IsEmpty(exportableFailureMechanism.Sections);
            CollectionAssert.IsEmpty(exportableFailureMechanism.SectionAssemblyResults);
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismRelevant_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> exportableFailureMechanism =
                    ExportableWaterPressureAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism);

                // Assert
                Assert.AreEqual(ExportableFailureMechanismType.AWO, exportableFailureMechanism.Code);
                Assert.AreEqual(ExportableFailureMechanismGroup.Group4, exportableFailureMechanism.Group);

                ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssembly = exportableFailureMechanism.FailureMechanismAssembly;
                Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyCategoryGroupOutput, exportableFailureMechanismAssembly.AssemblyCategory);
                Assert.AreEqual(ExportableAssemblyMethod.WBI1A1, exportableFailureMechanismAssembly.AssemblyMethod);

                ExportableFailureMechanismSectionTestHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, exportableFailureMechanism.Sections);
                AssertExportableFailureMechanismSectionResults(failureMechanismSectionAssemblyCalculator.SimpleAssessmentAssemblyOutput.Group,
                                                               failureMechanismSectionAssemblyCalculator.TailorMadeAssemblyCategoryOutput.Value,
                                                               failureMechanismSectionAssemblyCalculator.CombinedAssemblyCategoryOutput.Value,
                                                               exportableFailureMechanism.Sections,
                                                               exportableFailureMechanism.SectionAssemblyResults.Cast<ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly>());
            }
        }

        private static void AssertExportableFailureMechanismSectionResults(FailureMechanismSectionAssemblyCategoryGroup expectedSimpleAssembly,
                                                                           FailureMechanismSectionAssemblyCategoryGroup expectedTailorMadeAssembly,
                                                                           FailureMechanismSectionAssemblyCategoryGroup expectedCombinedAssembly,
                                                                           IEnumerable<ExportableFailureMechanismSection> sections,
                                                                           IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly> results)
        {
            int expectedNrOfResults = sections.Count();
            Assert.AreEqual(expectedNrOfResults, results.Count());

            for (var i = 0; i < expectedNrOfResults; i++)
            {
                ExportableFailureMechanismSection section = sections.ElementAt(i);
                ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly result = results.ElementAt(i);

                AssertExportableFailureMechanismSectionResult(expectedSimpleAssembly,
                                                              expectedTailorMadeAssembly,
                                                              expectedCombinedAssembly,
                                                              section,
                                                              result);
            }
        }

        private static void AssertExportableFailureMechanismSectionResult(FailureMechanismSectionAssemblyCategoryGroup expectedSimpleAssembly,
                                                                          FailureMechanismSectionAssemblyCategoryGroup expectedTailorMadeAssembly,
                                                                          FailureMechanismSectionAssemblyCategoryGroup expectedCombinedAssembly,
                                                                          ExportableFailureMechanismSection expectedSection,
                                                                          ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly actualResult)
        {
            Assert.AreSame(expectedSection, actualResult.FailureMechanismSection);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSimpleAssembly,
                                                                                            ExportableAssemblyMethod.WBI0E1,
                                                                                            actualResult.SimpleAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedTailorMadeAssembly,
                                                                                            ExportableAssemblyMethod.WBI0T1,
                                                                                            actualResult.TailorMadeAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedCombinedAssembly,
                                                                                            ExportableAssemblyMethod.WBI0A1,
                                                                                            actualResult.CombinedAssembly);
        }
    }
}