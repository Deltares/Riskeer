using System;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
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

            // Call
            ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);

            // Assert
            Assert.AreEqual(name, exportableAssessmentSection.Name);
            CollectionAssert.AreEqual(referenceLine.Points, exportableAssessmentSection.Geometry);
            CollectionAssert.IsEmpty(exportableAssessmentSection.FailureMechanismsWithProbability);
            CollectionAssert.IsEmpty(exportableAssessmentSection.FailureMechanismsWithoutProbability);
            Assert.IsNotNull(exportableAssessmentSection.CombinedSectionAssemblyResults);
            Assert.IsNotNull(exportableAssessmentSection.AssessmentSectionAssembly);
        }
    }
}