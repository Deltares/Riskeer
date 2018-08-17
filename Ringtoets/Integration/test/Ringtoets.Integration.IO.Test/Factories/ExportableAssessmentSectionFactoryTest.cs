using System;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
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

            FailureMechanismTestHelper.AddSections(assessmentSection.Piping, random.Next(1, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;

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
                Assert.AreEqual(assessmentSection.Piping.Sections.Count(), piping.Sections.Count());
                Assert.AreEqual(assessmentSection.Piping.SectionResults.Count(), piping.SectionAssemblyResults.Count());

                CollectionAssert.IsEmpty(exportableAssessmentSection.FailureMechanismsWithoutProbability);
                Assert.IsNotNull(exportableAssessmentSection.CombinedSectionAssemblyResults);
            }
        }
    }
}