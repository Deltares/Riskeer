using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;
using Ringtoets.Integration.IO.TestUtil;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportablePipingFailureMechanismFactoryTest
    {
        [Test]
        public void CreateExportablePipingFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => ExportablePipingFailureMechanismFactory.CreateExportablePipingFailureMechanism(null,
                                                                                                                     assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportablePipingFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportablePipingFailureMechanismFactory.CreateExportablePipingFailureMechanism(new PipingFailureMechanism(),
                                                                                                                     null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportablePipingFailureMechanism_WithValidArguments_ReturnsExportableFailureMechanism()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            AddFailureMechanismSections(failureMechanism);

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> assemblyResult =
                    ExportablePipingFailureMechanismFactory.CreateExportablePipingFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Group, assemblyResult.FailureMechanismAssembly.AssemblyCategory);
                Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Probability, assemblyResult.FailureMechanismAssembly.Probability);
                Assert.AreEqual(ExportableFailureMechanismType.STPH, assemblyResult.Code);
                Assert.AreEqual(ExportableFailureMechanismGroup.Group2, assemblyResult.Group);

                ExportableFailureMechanismSectionTestHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, assemblyResult.Sections);
                AssertExportablePipingFailureMechanismSectionResults(failureMechanismSectionAssemblyCalculator.SimpleAssessmentAssemblyOutput,
                                                                     failureMechanismSectionAssemblyCalculator.DetailedAssessmentAssemblyOutput,
                                                                     failureMechanismSectionAssemblyCalculator.TailorMadeAssessmentAssemblyOutput,
                                                                     failureMechanismSectionAssemblyCalculator.CombinedAssemblyOutput,
                                                                     assemblyResult.Sections,
                                                                     assemblyResult.SectionAssemblyResults.Cast<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability>());
            }
        }

        private static void AssertExportablePipingFailureMechanismSectionResults(FailureMechanismSectionAssembly expectedSimpleAssembly,
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

                AssertExportablePipingFailureMechanismSectionResult(expectedSimpleAssembly,
                                                                    expectedDetailedAssembly,
                                                                    expectedTailorMadeAssembly,
                                                                    expectedCombinedAssembly,
                                                                    section,
                                                                    result);
            }
        }

        private static void AssertExportablePipingFailureMechanismSectionResult(FailureMechanismSectionAssembly expectedSimpleAssembly,
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

        private static void AddFailureMechanismSections(IFailureMechanism failureMechanism)
        {
            const int numberOfSections = 3;

            var startPoint = new Point2D(-1, -1);
            var endPoint = new Point2D(15, 15);
            double endPointStepsX = (endPoint.X - startPoint.X) / numberOfSections;
            double endPointStepsY = (endPoint.Y - startPoint.Y) / numberOfSections;

            var sections = new List<FailureMechanismSection>();
            for (var i = 1; i <= numberOfSections; i++)
            {
                endPoint = new Point2D(startPoint.X + endPointStepsX, startPoint.Y + endPointStepsY);
                sections.Add(new FailureMechanismSection(i.ToString(),
                                                         new[]
                                                         {
                                                             startPoint,
                                                             endPoint
                                                         }));
                startPoint = endPoint;
            }

            FailureMechanismTestHelper.SetSections(failureMechanism, sections);
        }
    }
}