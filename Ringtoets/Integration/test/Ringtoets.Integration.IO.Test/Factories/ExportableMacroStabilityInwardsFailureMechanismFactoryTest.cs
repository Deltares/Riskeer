using System;
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
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableMacroStabilityInwardsFailureMechanismFactoryTest
    {
        [Test]
        public void CreateExportableMacroStabilityInwardsFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () =>
                ExportableMacroStabilityInwardsFailureMechanismFactory.CreateExportableMacroStabilityInwardsFailureMechanism(null,
                                                                                                                             assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableMacroStabilityInwardsFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                ExportableMacroStabilityInwardsFailureMechanismFactory.CreateExportableMacroStabilityInwardsFailureMechanism(new MacroStabilityInwardsFailureMechanism(),
                                                                                                                             null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableMacroStabilityInwardsFailureMechanism_WithValidArguments_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> assemblyResult =
                    ExportableMacroStabilityInwardsFailureMechanismFactory.CreateExportableMacroStabilityInwardsFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Group, assemblyResult.FailureMechanismAssembly.AssemblyCategory);
                Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Probability, assemblyResult.FailureMechanismAssembly.Probability);
                Assert.AreEqual(ExportableFailureMechanismType.STBI, assemblyResult.Code);
                Assert.AreEqual(ExportableFailureMechanismGroup.Group2, assemblyResult.Group);

                ExportableFailureMechanismSectionTestHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, assemblyResult.Sections);
                AssertExportableFailureMechanismSectionResults(failureMechanismSectionAssemblyCalculator.SimpleAssessmentAssemblyOutput,
                                                               failureMechanismSectionAssemblyCalculator.DetailedAssessmentAssemblyOutput,
                                                               failureMechanismSectionAssemblyCalculator.TailorMadeAssessmentAssemblyOutput,
                                                               failureMechanismSectionAssemblyCalculator.CombinedAssemblyOutput,
                                                               assemblyResult.Sections,
                                                               assemblyResult.SectionAssemblyResults.Cast<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability>());
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
                                                                                            ExportableAssemblyMethod.WBI0E1,
                                                                                            actualResult.SimpleAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedDetailedAssembly,
                                                                                            ExportableAssemblyMethod.WBI0G5,
                                                                                            actualResult.DetailedAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedTailorMadeAssembly,
                                                                                            ExportableAssemblyMethod.WBI0T5,
                                                                                            actualResult.TailorMadeAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedCombinedAssembly,
                                                                                            ExportableAssemblyMethod.WBI1B1,
                                                                                            actualResult.CombinedAssembly);
        }
    }
}