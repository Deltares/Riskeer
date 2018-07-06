using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Service;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Service.Exceptions;
using Ringtoets.Integration.Service.Merge;

namespace Ringtoets.Integration.Service.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionProviderActivityTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            mocks.ReplayAll();

            var owner = new AssessmentSectionsOwner();

            // Call
            var activity = new AssessmentSectionProviderActivity(owner, provider, string.Empty);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        [Test]
        public void Run_ProviderReturnsAssessmentSections_SetsActivityStateToExecutedAndSetsAssessmentSections()
        {
            // Setup
            const string filePath = "Path to file";
            IEnumerable<AssessmentSection> assessmentSections = Enumerable.Empty<AssessmentSection>();

            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            provider.Expect(p => p.GetAssessmentSections(filePath)).Return(assessmentSections);
            mocks.ReplayAll();

            var owner = new AssessmentSectionsOwner();
            var activity = new AssessmentSectionProviderActivity(owner, provider, filePath);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Executed, activity.State);
            Assert.AreSame(assessmentSections, owner.AssessmentSections);
        }

        [Test]
        public void Run_ProviderThrowsException_SetsActivityStateToFailedAndDoesNotSetAssessmentSections()
        {
            // Setup
            const string filePath = "Path to file";

            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            provider.Expect(p => p.GetAssessmentSections(filePath)).Throw(new AssessmentSectionProviderException());
            mocks.ReplayAll();

            var owner = new AssessmentSectionsOwner();
            var activity = new AssessmentSectionProviderActivity(owner, provider, filePath);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Failed, activity.State);
            Assert.IsNull(owner.AssessmentSections);
        }

        [Test]
        public void GivenCancelledActivity_WhenFinishingActivity_ThenActivityStateSetToCancelledAndDoesNotSetAssessmentSections()
        {
            // Given
            const string filePath = "Path to file";
            IEnumerable<AssessmentSection> assessmentSections = Enumerable.Empty<AssessmentSection>();

            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            provider.Expect(p => p.GetAssessmentSections(filePath)).Return(assessmentSections);
            mocks.ReplayAll();

            var owner = new AssessmentSectionsOwner();
            var activity = new AssessmentSectionProviderActivity(owner, provider, filePath);

            activity.Run();
            activity.Cancel();

            // When
            activity.Finish();

            // Assert
            Assert.AreEqual(ActivityState.Canceled, activity.State);
            Assert.IsNull(owner.AssessmentSections);
        }
    }
}