// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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
            const string filePath = "Some\\path";

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