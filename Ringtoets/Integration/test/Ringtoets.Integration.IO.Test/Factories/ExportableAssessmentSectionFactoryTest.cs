using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;

namespace Ringtoets.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableAssessmentSectionFactoryTest
    {
        [Test]
        public void CreateExportableAssessmentSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableAssessmentSection_WithAssessmentSection_ReturnsExpectedValues()
        {
            // Setup
            const string name = "assessmentSectionName";

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            });

            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                Name = name,
                ReferenceLine = referenceLine
            };

            AddFailureMechanismSections(assessmentSection.Piping);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);

                // Assert
                Assert.AreEqual(name, exportableAssessmentSection.Name);
                CollectionAssert.AreEqual(referenceLine.Points, exportableAssessmentSection.Geometry);

                ExportableAssessmentSectionAssemblyResult exportableAssessmentSectionAssemblyResult = exportableAssessmentSection.AssessmentSectionAssembly;
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleAssessmentSectionCategoryGroupOutput, exportableAssessmentSectionAssemblyResult.AssemblyCategory);
                Assert.AreEqual(ExportableAssemblyMethod.WBI2C1, exportableAssessmentSectionAssemblyResult.AssemblyMethod);

                Assert.AreEqual(1, exportableAssessmentSection.FailureMechanismsWithProbability.Count());
                ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> piping = exportableAssessmentSection.FailureMechanismsWithProbability.First();
                Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Group, piping.FailureMechanismAssembly.AssemblyCategory);
                Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Probability, piping.FailureMechanismAssembly.Probability);
                Assert.AreEqual(ExportableFailureMechanismType.STPH, piping.Code);
                Assert.AreEqual(ExportableFailureMechanismGroup.Group2, piping.Group);

                AssertExportableFailureMechanismSections(assessmentSection.Piping.Sections, piping.Sections);
                AssertExportablePipingFailureMechanismSectionResults(failureMechanismSectionAssemblyCalculator.SimpleAssessmentAssemblyOutput,
                                                                     failureMechanismSectionAssemblyCalculator.DetailedAssessmentAssemblyOutput,
                                                                     failureMechanismSectionAssemblyCalculator.TailorMadeAssessmentAssemblyOutput,
                                                                     failureMechanismSectionAssemblyCalculator.CombinedAssemblyOutput,
                                                                     piping.Sections,
                                                                     piping.SectionAssemblyResults.Cast<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability>());

                CollectionAssert.IsEmpty(exportableAssessmentSection.FailureMechanismsWithoutProbability);
                Assert.IsNotNull(exportableAssessmentSection.CombinedSectionAssemblyResults);
            }
        }

        [Test]
        public void CreateExportableAssessmentSection_AssessmentSectionAssemblyCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                Name = "assessmentSectionName",
                ReferenceLine = new ReferenceLine()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);

                // Assert
                Assert.Throws<AssemblyException>(call);
            }
        }

        private static void AssertExportableFailureMechanismSections(IEnumerable<FailureMechanismSection> expectedSections,
                                                                     IEnumerable<ExportableFailureMechanismSection> actualSections)
        {
            int expectedNrOfSections = expectedSections.Count();
            Assert.AreEqual(expectedNrOfSections, actualSections.Count());
            for (var i = 0; i < expectedNrOfSections; i++)
            {
                FailureMechanismSection expectedSection = expectedSections.ElementAt(i);
                ExportableFailureMechanismSection actualSection = actualSections.ElementAt(i);

                AssertExportableFailureMechanismSection(expectedSection, actualSection);
            }
        }

        private static void AssertExportableFailureMechanismSection(FailureMechanismSection expectedSection,
                                                                    ExportableFailureMechanismSection actualSection)
        {
            Assert.IsNaN(actualSection.StartDistance);
            Assert.IsNaN(actualSection.EndDistance);
            CollectionAssert.AreEqual(expectedSection.Points, actualSection.Geometry);
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

            ExportableSectionAssemblyResultWithProbability actualSimpleAssembly = actualResult.SimpleAssembly;
            Assert.AreEqual(ExportableAssemblyMethod.WBI0E1, actualSimpleAssembly.AssemblyMethod);
            Assert.AreEqual(expectedSimpleAssembly.Group, actualSimpleAssembly.AssemblyCategory);
            Assert.AreEqual(expectedSimpleAssembly.Probability, actualSimpleAssembly.Probability);

            ExportableSectionAssemblyResultWithProbability actualDetailedAssembly = actualResult.DetailedAssembly;
            Assert.AreEqual(ExportableAssemblyMethod.WBI0G5, actualDetailedAssembly.AssemblyMethod);
            Assert.AreEqual(expectedDetailedAssembly.Group, actualDetailedAssembly.AssemblyCategory);
            Assert.AreEqual(expectedDetailedAssembly.Probability, actualDetailedAssembly.Probability);

            ExportableSectionAssemblyResultWithProbability actualTailorMadeAssembly = actualResult.TailorMadeAssembly;
            Assert.AreEqual(ExportableAssemblyMethod.WBI0T5, actualTailorMadeAssembly.AssemblyMethod);
            Assert.AreEqual(expectedTailorMadeAssembly.Group, actualTailorMadeAssembly.AssemblyCategory);
            Assert.AreEqual(expectedTailorMadeAssembly.Probability, actualTailorMadeAssembly.Probability);

            ExportableSectionAssemblyResultWithProbability actualCombinedResult = actualResult.CombinedAssembly;
            Assert.AreEqual(ExportableAssemblyMethod.WBI1B1, actualCombinedResult.AssemblyMethod);
            Assert.AreEqual(expectedCombinedAssembly.Group, actualCombinedResult.AssemblyCategory);
            Assert.AreEqual(expectedCombinedAssembly.Probability, actualCombinedResult.Probability);
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