using Core.Common.Base.Service;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data.Merge;
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
            var activity = new AssessmentSectionProviderActivity(owner, provider);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
        }
    }
}