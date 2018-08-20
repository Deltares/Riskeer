﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;
using Ringtoets.Integration.IO.TestUtil;
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableStabilityPointStructuresFailureMechanismFactoryTest
    {
        [Test]
        public void CreateExportableStabilityPointStructuresFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () =>
                ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableStabilityPointStructuresFailureMechanism(null,
                                                                                                                                   assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableStabilityPointStructuresFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableStabilityPointStructuresFailureMechanism(new StabilityPointStructuresFailureMechanism(),
                                                                                                                                   null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableStabilityPointStructuresFailureMechanism_WithFailureMechanismNotRelevant_ReturnsDefaultExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                IsRelevant = false
            };
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            var assessmentSection = new AssessmentSectionStub();

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism =
                ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableStabilityPointStructuresFailureMechanism(failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(ExportableFailureMechanismType.STKWp, exportableFailureMechanism.Code);
            Assert.AreEqual(ExportableFailureMechanismGroup.Group1, exportableFailureMechanism.Group);

            ExportableFailureMechanismAssemblyResultWithProbability failureMechanismAssemblyResult = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(ExportableAssemblyMethod.WBI1B1, failureMechanismAssemblyResult.AssemblyMethod);
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.NotApplicable, failureMechanismAssemblyResult.AssemblyCategory);
            Assert.AreEqual(0, failureMechanismAssemblyResult.Probability);

            CollectionAssert.IsEmpty(exportableFailureMechanism.Sections);
            CollectionAssert.IsEmpty(exportableFailureMechanism.SectionAssemblyResults);
        }

        [Test]
        public void CreateExportableStabilityPointStructuresFailureMechanism_WithFailureMechanismRelevant_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism =
                    ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableStabilityPointStructuresFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                Assert.AreEqual(ExportableFailureMechanismType.STKWp, exportableFailureMechanism.Code);
                Assert.AreEqual(ExportableFailureMechanismGroup.Group1, exportableFailureMechanism.Group);

                FailureMechanismAssembly calculatorOutput = failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput;
                ExportableFailureMechanismAssemblyResultWithProbability exportableFailureMechanismAssembly = exportableFailureMechanism.FailureMechanismAssembly;
                Assert.AreEqual(calculatorOutput.Group, exportableFailureMechanismAssembly.AssemblyCategory);
                Assert.AreEqual(calculatorOutput.Probability, exportableFailureMechanismAssembly.Probability);
                Assert.AreEqual(ExportableAssemblyMethod.WBI1B1, exportableFailureMechanismAssembly.AssemblyMethod);

                ExportableFailureMechanismSectionTestHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, exportableFailureMechanism.Sections);
                AssertExportableFailureMechanismSectionResults(failureMechanismSectionAssemblyCalculator.SimpleAssessmentAssemblyOutput,
                                                               failureMechanismSectionAssemblyCalculator.DetailedAssessmentAssemblyOutput,
                                                               failureMechanismSectionAssemblyCalculator.TailorMadeAssessmentAssemblyOutput,
                                                               failureMechanismSectionAssemblyCalculator.CombinedAssemblyOutput,
                                                               exportableFailureMechanism.Sections,
                                                               exportableFailureMechanism.SectionAssemblyResults.Cast<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability>());
            }
        }

        private static void AssertExportableFailureMechanismSectionResults(FailureMechanismSectionAssembly expectedSimpleAssembly,
                                                                           FailureMechanismSectionAssembly expectedDetailedAssembly,
                                                                           FailureMechanismSectionAssembly expectedTailorMadeAssembly,
                                                                           FailureMechanismSectionAssembly expectedCombinedAssembly,
                                                                           IEnumerable<ExportableFailureMechanismSection> sections,
                                                                           IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability> results)
        {
            int expectedNrOfResults = sections.Count();
            Assert.AreEqual(expectedNrOfResults, results.Count());

            for (var i = 0; i < expectedNrOfResults; i++)
            {
                ExportableFailureMechanismSection section = sections.ElementAt(i);
                ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability result = results.ElementAt(i);

                AssertExportableFailureMechanismSectionResult(expectedSimpleAssembly,
                                                              expectedDetailedAssembly,
                                                              expectedTailorMadeAssembly,
                                                              expectedCombinedAssembly,
                                                              section,
                                                              result);
            }
        }

        private static void AssertExportableFailureMechanismSectionResult(FailureMechanismSectionAssembly expectedSimpleAssembly,
                                                                          FailureMechanismSectionAssembly expectedDetailedAssembly,
                                                                          FailureMechanismSectionAssembly expectedTailorMadeAssembly,
                                                                          FailureMechanismSectionAssembly expectedCombinedAssembly,
                                                                          ExportableFailureMechanismSection expectedSection,
                                                                          ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability actualResult)
        {
            Assert.AreSame(expectedSection, actualResult.FailureMechanismSection);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSimpleAssembly,
                                                                                            ExportableAssemblyMethod.WBI0E3,
                                                                                            actualResult.SimpleAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedDetailedAssembly,
                                                                                            ExportableAssemblyMethod.WBI0G3,
                                                                                            actualResult.DetailedAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedTailorMadeAssembly,
                                                                                            ExportableAssemblyMethod.WBI0T3,
                                                                                            actualResult.TailorMadeAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedCombinedAssembly,
                                                                                            ExportableAssemblyMethod.WBI0A1,
                                                                                            actualResult.CombinedAssembly);
        }
    }
}