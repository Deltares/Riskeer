using System;
using System.Collections.Generic;
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Service.Exceptions;
using Ringtoets.Integration.Service.Merge;

namespace Ringtoets.Integration.Service.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionProviderServiceTest
    {
        [Test]
        public void Constructor_ProjectStorageNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionProviderService(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("projectStorage", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var storeProject = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            // Call
            var provider = new AssessmentSectionProviderService(storeProject);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionProvider>(provider);
        }

        [Test]
        public void GetAssessmentSections_LoadingProjectSuccesful_ReturnsRingtoetsProject()
        {
            // Setup
            const string filePath = "Some path";
            var project = new RingtoetsProject();

            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(filePath)).Return(project);
            mocks.ReplayAll();

            var provider = new AssessmentSectionProviderService(storeProject);

            // Call
            IEnumerable<AssessmentSection> assessmentSections = provider.GetAssessmentSections(filePath);

            // Assert
            Assert.AreSame(project.AssessmentSections, assessmentSections);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentSections_LoadedProjectNull_ReturnsNull()
        {
            // Setup
            const string filePath = "Some path";

            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(filePath)).Return(null);
            mocks.ReplayAll();

            var provider = new AssessmentSectionProviderService(storeProject);

            // Call
            IEnumerable<AssessmentSection> assessmentSections = provider.GetAssessmentSections(filePath);

            // Assert
            Assert.IsNull(assessmentSections);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentSections_LoadingProjectThrowsException_ThrowsAssessmentSectionProviderExceptionAndLogsError()
        {
            // Setup
            const string filePath = "Some path";
            const string exceptionMessage = "StorageException";

            var storageException = new StorageException(exceptionMessage);

            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(filePath)).Throw(storageException);
            mocks.ReplayAll();

            var provider = new AssessmentSectionProviderService(storeProject);

            AssessmentSectionProviderException exception = null;

            // Call
            Action call = () =>
            {
                try
                {
                    provider.GetAssessmentSections(filePath);
                }
                catch (AssessmentSectionProviderException e)
                {
                    exception = e;
                }
            };

            // Assert
            var expectedLogMessage = new Tuple<string, LogLevelConstant>(exceptionMessage, LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage);
            Assert.AreEqual(storageException, exception.InnerException);
            Assert.AreEqual(storageException.Message, exception.Message);
            mocks.VerifyAll();
        }
    }
}