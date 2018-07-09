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
    public class LoadAssessmentSectionServiceTest
    {
        [Test]
        public void Constructor_ProjectStorageNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new LoadAssessmentSectionService(null);

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
            var provider = new LoadAssessmentSectionService(storeProject);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionProvider>(provider);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentSections_Always_SendsFilePathToLoadsProject()
        {
            // Setup
            const string filePath = "Some path";

            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(filePath)).Return(new RingtoetsProject());
            mocks.ReplayAll();

            var provider = new LoadAssessmentSectionService(storeProject);

            // Call
            provider.GetAssessmentSections(filePath);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentSections_LoadingProjectSuccesful_ReturnsRingtoetsProject()
        {
            // Setup
            var project = new RingtoetsProject();

            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(null))
                        .IgnoreArguments()
                        .Return(project);
            mocks.ReplayAll();

            var provider = new LoadAssessmentSectionService(storeProject);

            // Call
            IEnumerable<AssessmentSection> assessmentSections = provider.GetAssessmentSections(string.Empty);

            // Assert
            Assert.AreSame(project.AssessmentSections, assessmentSections);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentSections_LoadedProjectNull_ThrowsAssessmentSectionProviderException()
        {
            // Setup
            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(null))
                        .IgnoreArguments()
                        .Return(null);
            mocks.ReplayAll();

            var provider = new LoadAssessmentSectionService(storeProject);

            // Call
            TestDelegate call = () => provider.GetAssessmentSections(string.Empty);

            // Assert
            Assert.Throws<AssessmentSectionProviderException>(call);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentSections_LoadingProjectThrowsException_ThrowsAssessmentSectionProviderExceptionAndLogsError()
        {
            // Setup
            const string exceptionMessage = "StorageException";
            var storageException = new StorageException(exceptionMessage);

            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(null))
                        .IgnoreArguments()
                        .Throw(storageException);
            mocks.ReplayAll();

            var provider = new LoadAssessmentSectionService(storeProject);

            AssessmentSectionProviderException exception = null;

            // Call
            Action call = () =>
            {
                try
                {
                    provider.GetAssessmentSections(string.Empty);
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