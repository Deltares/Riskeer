using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Service.Comparers;

namespace Ringtoets.Integration.Service.Test.Comparers
{
    [TestFixture]
    public class AssessmentSectionMergeComparerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var comparer = new AssessmentSectionMergeComparer();

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionMergeComparer>(comparer);
        }

        [Test]
        public void Compare_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var comparer = new AssessmentSectionMergeComparer();

            // Call
            TestDelegate call = () => comparer.Compare(null, CreateAssessmentSection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Compare_OtherAssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var comparer = new AssessmentSectionMergeComparer();

            // Call
            TestDelegate call = () => comparer.Compare(CreateAssessmentSection(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("otherAssessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Compare_AssessmentSectionsHaveEquivalentReferenceLines_ReturnsTrue(bool hasReferenceLine)
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            AssessmentSection otherAssessmentSection = CreateAssessmentSection();

            if (!hasReferenceLine)
            {
                assessmentSection.ReferenceLine = null;
                otherAssessmentSection.ReferenceLine = null;
            }

            var comparer = new AssessmentSectionMergeComparer();

            // Call
            bool result = comparer.Compare(assessmentSection, otherAssessmentSection);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCaseSource(nameof(GetUnequivalentAssessmentSectionTestCases))]
        public void Compare_AssessmentSectionsNotEquivalent_ReturnsFalse(AssessmentSection otherAssessmentSection)
        {
            // Setup
            var comparer = new AssessmentSectionMergeComparer();

            // Call
            bool result = comparer.Compare(CreateAssessmentSection(), otherAssessmentSection);

            // Assert
            Assert.IsFalse(result);
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1, 1),
                new Point2D(1, 2)
            });
            return new AssessmentSection(AssessmentSectionComposition.Dike, 0.1, 0.025)
            {
                ReferenceLine = referenceLine
            };
        }

        private static IEnumerable<TestCaseData> GetUnequivalentAssessmentSectionTestCases()
        {
            foreach (ChangePropertyData<AssessmentSection> changeSingleDataProperty in ChangeSingleDataProperties())
            {
                AssessmentSection assessmentSection = CreateAssessmentSection();
                changeSingleDataProperty.ActionToChangeProperty(assessmentSection);
                yield return new TestCaseData(assessmentSection).SetName(changeSingleDataProperty.PropertyName);
            }
        }

        private static IEnumerable<ChangePropertyData<AssessmentSection>> ChangeSingleDataProperties()
        {
            var referenceLineDifferentPointCount = new ReferenceLine();
            referenceLineDifferentPointCount.SetGeometry(new[]
            {
                new Point2D(1, 1)
            });
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.ReferenceLine = referenceLineDifferentPointCount,
                                                                   "Referenceline different point count");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.ReferenceLine = null,
                                                                   "Referenceline null");

            var referenceLineDifferentPoint = new ReferenceLine();
            referenceLineDifferentPoint.SetGeometry(new[]
            {
                new Point2D(1, 1),
                new Point2D(1, 3)
            });
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.ReferenceLine = referenceLineDifferentPoint,
                                                                   "Referenceline different point");

            yield return new ChangePropertyData<AssessmentSection>(sec => sec.Id = "DifferentVersion",
                                                                   "Id");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.HydraulicBoundaryDatabase.Version = "DifferentVersion",
                                                                   "HydraulicBoundaryDataBase");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.FailureMechanismContribution.LowerLimitNorm = sec.FailureMechanismContribution.LowerLimitNorm - 0.05,
                                                                   "LowerLimitNorm");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.FailureMechanismContribution.SignalingNorm = sec.FailureMechanismContribution.SignalingNorm - 0.005,
                                                                   "SignalingNorm");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.FailureMechanismContribution.NormativeNorm = sec.FailureMechanismContribution.NormativeNorm == NormType.LowerLimit
                                                                                                                               ? NormType.Signaling
                                                                                                                               : NormType.LowerLimit,
                                                                   "NormType");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.ChangeComposition(AssessmentSectionComposition.DikeAndDune),
                                                                   "Composition");
        }
    }
}