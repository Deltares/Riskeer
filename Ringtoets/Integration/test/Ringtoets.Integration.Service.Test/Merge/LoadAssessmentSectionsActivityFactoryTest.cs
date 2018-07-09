using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Service;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Service.Merge;

namespace Ringtoets.Integration.Service.Test.Merge
{
    [TestFixture]
    public class LoadAssessmentSectionsActivityFactoryTest
    {
        [Test]
        public void CreateLoadAssessmentSectionsActivity_AssessmentSectionsOwnerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => LoadAssessmentSectionsActivityFactory.CreateLoadAssessmentSectionsActivity(null, provider, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("owner", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateLoadAssessmentSectionsActivity_AssessmentSectionsProviderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => LoadAssessmentSectionsActivityFactory.CreateLoadAssessmentSectionsActivity(new AssessmentSectionsOwner(),
                                                                                                                 null,
                                                                                                                 string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSectionProvider", exception.ParamName);
        }

        [Test]
        public void CreateLoadAssessmentSectionsActivity_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => LoadAssessmentSectionsActivityFactory.CreateLoadAssessmentSectionsActivity(new AssessmentSectionsOwner(),
                                                                                                                 provider,
                                                                                                                 null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateLoadAssessmentSectionsActivity_WithArguments_ReturnsActivityWithParametersSet()
        {
            // Setup
            const string filePath = "File\\Path";

            var owner = new AssessmentSectionsOwner();
            IEnumerable<AssessmentSection> expectedAssessmentSections = Enumerable.Empty<AssessmentSection>();

            var mocks = new MockRepository();
            var provider = mocks.Stub<IAssessmentSectionProvider>();
            provider.Expect(pr => pr.GetAssessmentSections(filePath)).Return(expectedAssessmentSections);
            mocks.ReplayAll();

            // Call
            Activity activity = LoadAssessmentSectionsActivityFactory.CreateLoadAssessmentSectionsActivity(owner, provider, filePath);

            // Assert
            Assert.IsInstanceOf<LoadAssessmentSectionsActivity>(activity);
            AssertLoadAssessmentSectionsActivity(activity, owner, expectedAssessmentSections);
            mocks.VerifyAll();
        }

        private static void AssertLoadAssessmentSectionsActivity(Activity activity,
                                                                 AssessmentSectionsOwner owner,
                                                                 IEnumerable<AssessmentSection> expectedAssessmentSections)
        {
            activity.Run();

            Assert.AreSame(expectedAssessmentSections, owner.AssessmentSections);
        }
    }
}