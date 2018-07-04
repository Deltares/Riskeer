using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
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
        public void Compare_ReferenceAssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionToCompare = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var comparer = new AssessmentSectionMergeComparer();

            // Call
            TestDelegate call = () => comparer.Compare(null, assessmentSectionToCompare);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("referenceAssessmentSection", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Compare_AssessmentSectionToCompareNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var referenceAssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var comparer = new AssessmentSectionMergeComparer();

            // Call
            TestDelegate call = () => comparer.Compare(referenceAssessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSectionToCompare", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Compare_AssessmentSectionsAreEqual_ReturnsTrue()
        {
            // Setup
            IAssessmentSection referenceAssessmentSection = CreateAssessmentSection();
            IAssessmentSection assessmentSectionToCompare = CreateAssessmentSection();

            var comparer = new AssessmentSectionMergeComparer();

            // Call
            bool result = comparer.Compare(referenceAssessmentSection, assessmentSectionToCompare);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCaseSource(nameof(GetUnequalTestCases))]
        public void Compare_AssessmentSectionsUnequalFailureMechanismContributions_ReturnsFalse(
            IAssessmentSection assessmentSection)
        {
            // Setup
            IAssessmentSection referenceAssessmentSection = CreateAssessmentSection();

            var comparer = new AssessmentSectionMergeComparer();

            // Call
            bool result = comparer.Compare(referenceAssessmentSection, assessmentSection);

            // Assert
            Assert.IsFalse(result);
        }

        private static IAssessmentSection CreateAssessmentSection()
        {
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new []
            {
                new Point2D(1, 1), 
                new Point2D(1, 2) 
            });
            return new TestAssessmentSection("Id", AssessmentSectionComposition.Dike)
            {
                ReferenceLine = referenceLine
            };
        }

        private static IEnumerable<TestCaseData> GetUnequalTestCases()
        {
            IAssessmentSection referenceAssessmentSection = CreateAssessmentSection();
            yield return new TestCaseData(new TestAssessmentSection("DifferentId", referenceAssessmentSection.Composition))
                .SetName("ID");
            yield return new TestCaseData(new TestAssessmentSection(referenceAssessmentSection.Id, AssessmentSectionComposition.DikeAndDune))
                .SetName("Composition");

            foreach (ChangePropertyData<IAssessmentSection> changeSingleDataProperty in ChangeSingleDataProperties())
            {
                IAssessmentSection assessmentSection = CreateAssessmentSection();
                changeSingleDataProperty.ActionToChangeProperty(assessmentSection);
                yield return new TestCaseData(assessmentSection).SetName(changeSingleDataProperty.PropertyName);
            }
        }

        private static IEnumerable<ChangePropertyData<IAssessmentSection>> ChangeSingleDataProperties()
        {
            var referenceLineDifferentPointCount = new ReferenceLine();
            referenceLineDifferentPointCount.SetGeometry(new[]
            {
                new Point2D(1, 1)
            });
            yield return new ChangePropertyData<IAssessmentSection>(sec => sec.ReferenceLine = referenceLineDifferentPointCount,
                                                                    "Referenceline different point count");
            yield return new ChangePropertyData<IAssessmentSection>(sec => sec.ReferenceLine = null,
                                                                    "Referenceline null");

            var referenceLineDifferentPoint = new ReferenceLine();
            referenceLineDifferentPoint.SetGeometry(new[]
            {
                new Point2D(1, 1),
                new Point2D(1, 3)
            });
            yield return new ChangePropertyData<IAssessmentSection>(sec => sec.ReferenceLine = referenceLineDifferentPoint,
                                                                    "Referenceline different point");

            yield return new ChangePropertyData<IAssessmentSection>(sec => sec.HydraulicBoundaryDatabase.Version = "DifferentVersion",
                                                                    "HydraulicBoundaryDataBase");
            yield return new ChangePropertyData<IAssessmentSection>(sec => sec.FailureMechanismContribution.LowerLimitNorm = sec.FailureMechanismContribution.LowerLimitNorm - 0.05,
                                                                    "LowerLimitNorm");
            yield return new ChangePropertyData<IAssessmentSection>(sec => sec.FailureMechanismContribution.SignalingNorm = sec.FailureMechanismContribution.SignalingNorm - 0.005,
                                                                    "SignalingNorm");
            yield return new ChangePropertyData<IAssessmentSection>(sec => sec.FailureMechanismContribution.NormativeNorm = sec.FailureMechanismContribution.NormativeNorm == NormType.LowerLimit
                                                                                                                                ? NormType.Signaling
                                                                                                                                : NormType.LowerLimit,
                                                                    "NormType");
        }

        private class TestAssessmentSection : IAssessmentSection
        {
            public TestAssessmentSection(string id, AssessmentSectionComposition composition)
            {
                Id = id;
                Composition = composition;
                FailureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 0, 0.1, 0.025);
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            }

            public IEnumerable<IObserver> Observers { get; }

            public void Attach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void Detach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void NotifyObservers()
            {
                throw new NotImplementedException();
            }

            public string Id { get; }
            public string Name { get; set; }
            public Comment Comments { get; }
            public AssessmentSectionComposition Composition { get; }
            public ReferenceLine ReferenceLine { get; set; }
            public FailureMechanismContribution FailureMechanismContribution { get; }
            public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; }
            public BackgroundData BackgroundData { get; }
            public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForFactorizedSignalingNorm { get; }
            public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForSignalingNorm { get; }
            public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForLowerLimitNorm { get; }
            public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForFactorizedLowerLimitNorm { get; }
            public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForFactorizedSignalingNorm { get; }
            public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForSignalingNorm { get; }
            public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForLowerLimitNorm { get; }
            public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForFactorizedLowerLimitNorm { get; }

            public IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                throw new NotImplementedException();
            }

            public void ChangeComposition(AssessmentSectionComposition newComposition)
            {
                throw new NotImplementedException();
            }
        }
    }
}