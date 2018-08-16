using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAssessmentSectionTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly = CreateAssessmentSectionAssembly();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();
            IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults = Enumerable.Empty<ExportableCombinedSectionAssembly>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(null,
                                                                      geometry,
                                                                      assessmentSectionAssembly,
                                                                      failureMechanismsWithProbability,
                                                                      failureMechanismsWithoutProbability,
                                                                      combinedSectionAssemblyResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Setup
            ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly = CreateAssessmentSectionAssembly();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();
            IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults = Enumerable.Empty<ExportableCombinedSectionAssembly>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      null,
                                                                      assessmentSectionAssembly,
                                                                      failureMechanismsWithProbability,
                                                                      failureMechanismsWithoutProbability,
                                                                      combinedSectionAssemblyResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionAssemblyNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();
            IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults = Enumerable.Empty<ExportableCombinedSectionAssembly>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      geometry,
                                                                      null,
                                                                      failureMechanismsWithProbability,
                                                                      failureMechanismsWithoutProbability,
                                                                      combinedSectionAssemblyResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSectionAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsWithProbabilityNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly = CreateAssessmentSectionAssembly();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();
            IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults = Enumerable.Empty<ExportableCombinedSectionAssembly>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      geometry,
                                                                      assessmentSectionAssembly,
                                                                      null,
                                                                      failureMechanismsWithoutProbability,
                                                                      combinedSectionAssemblyResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismsWithProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsWithoutProbabilityNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly = CreateAssessmentSectionAssembly();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults = Enumerable.Empty<ExportableCombinedSectionAssembly>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      geometry,
                                                                      assessmentSectionAssembly,
                                                                      failureMechanismsWithProbability,
                                                                      null,
                                                                      combinedSectionAssemblyResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismsWithoutProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly = CreateAssessmentSectionAssembly();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();

            // Call
            TestDelegate call = () => new ExportableAssessmentSection(string.Empty,
                                                                      geometry,
                                                                      assessmentSectionAssembly,
                                                                      failureMechanismsWithProbability,
                                                                      failureMechanismsWithoutProbability,
                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("Valid name")]
        public void Constructor_ExpectedValues(string name)
        {
            // Setup
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly = CreateAssessmentSectionAssembly();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> failureMechanismsWithProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> failureMechanismsWithoutProbability =
                Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>();
            IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults = Enumerable.Empty<ExportableCombinedSectionAssembly>();

            // Call
            var assessmentSection = new ExportableAssessmentSection(name,
                                                                    geometry,
                                                                    assessmentSectionAssembly,
                                                                    failureMechanismsWithProbability,
                                                                    failureMechanismsWithoutProbability,
                                                                    combinedSectionAssemblyResults);

            // Assert
            Assert.AreEqual(name, assessmentSection.Name);
            Assert.AreSame(geometry, assessmentSection.Geometry);
            Assert.AreSame(assessmentSectionAssembly, assessmentSection.AssessmentSectionAssembly);
            Assert.AreSame(failureMechanismsWithProbability, assessmentSection.FailureMechanismsWithProbability);
            Assert.AreSame(failureMechanismsWithoutProbability, assessmentSection.FailureMechanismsWithoutProbability);
            Assert.AreSame(combinedSectionAssemblyResults, assessmentSection.CombinedSectionAssemblyResults);
        }

        private static ExportableAssessmentSectionAssemblyResult CreateAssessmentSectionAssembly()
        {
            var random = new Random(21);
            return new ExportableAssessmentSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                 random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());
        }
    }
}