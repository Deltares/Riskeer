using System;
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
    public class LoadAssessmentSectionsActivityTest
    {
        [Test]
        public void Constructor_OwnerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new LoadAssessmentSectionsActivity(null, provider, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("owner", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ProviderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new LoadAssessmentSectionsActivity(new AssessmentSectionsOwner(), null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSectionProvider", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new LoadAssessmentSectionsActivity(new AssessmentSectionsOwner(), 
                                                                            provider, 
                                                                            null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
            mocks.VerifyAll();
        }
        
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            mocks.ReplayAll();

            var owner = new AssessmentSectionsOwner();

            // Call
            var activity = new LoadAssessmentSectionsActivity(owner, provider, string.Empty);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual("Inlezen van project", activity.Description);
            Assert.AreEqual(ActivityState.None, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        public void Run_Always_SendsFilePathToGetsAssessmentSections()
        {
            // Setup
            const string filePath = "File\\Path";

            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            provider.Expect(p => p.GetAssessmentSections(filePath)).Return(Enumerable.Empty<AssessmentSection>());
            mocks.ReplayAll();

            var owner = new AssessmentSectionsOwner();
            var activity = new LoadAssessmentSectionsActivity(owner, provider, filePath);

            // Call
            activity.Run();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Run_ProviderReturnsAssessmentSections_SetsActivityStateToExecutedAndSetsAssessmentSections()
        {
            // Setup
            IEnumerable<AssessmentSection> assessmentSections = Enumerable.Empty<AssessmentSection>();

            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            provider.Expect(p => p.GetAssessmentSections(null))
                    .IgnoreArguments()
                    .Return(assessmentSections);
            mocks.ReplayAll();

            var owner = new AssessmentSectionsOwner();
            var activity = new LoadAssessmentSectionsActivity(owner, provider, string.Empty);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Executed, activity.State);
            Assert.AreSame(assessmentSections, owner.AssessmentSections);
            mocks.VerifyAll();
        }

        [Test]
        public void Run_ProviderThrowsException_SetsActivityStateToFailedAndDoesNotSetAssessmentSections()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            provider.Expect(p => p.GetAssessmentSections(null))
                    .IgnoreArguments()
                    .Throw(new AssessmentSectionProviderException());
            mocks.ReplayAll();

            var owner = new AssessmentSectionsOwner();
            var activity = new LoadAssessmentSectionsActivity(owner, provider, string.Empty);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Failed, activity.State);
            Assert.IsNull(owner.AssessmentSections);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCancelledActivity_WhenFinishingActivity_ThenActivityStateSetToCancelledAndDoesNotSetAssessmentSections()
        {
            // Given
            IEnumerable<AssessmentSection> assessmentSections = Enumerable.Empty<AssessmentSection>();

            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            provider.Expect(p => p.GetAssessmentSections(null))
                    .IgnoreArguments()
                    .Return(assessmentSections);
            mocks.ReplayAll();

            var owner = new AssessmentSectionsOwner();
            var activity = new LoadAssessmentSectionsActivity(owner, provider, string.Empty);

            activity.Run();
            activity.Cancel();

            // When
            activity.Finish();

            // Assert
            Assert.AreEqual(ActivityState.Canceled, activity.State);
            Assert.IsNull(owner.AssessmentSections);
            mocks.VerifyAll();
        }
    }
}