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
            var storeProject = mocks.StrictMock<IStoreProject>();
            mocks.ReplayAll();

            // Call
            var service = new LoadAssessmentSectionService(storeProject);

            // Assert
            Assert.IsInstanceOf<ILoadAssessmentSectionService>(service);
            mocks.VerifyAll();
        }

        [Test]
        public void LoadAssessmentSections_Always_SendsFilePathToLoadsProject()
        {
            // Setup
            const string filePath = "Some\\path";

            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(filePath)).Return(new RingtoetsProject());
            mocks.ReplayAll();

            var service = new LoadAssessmentSectionService(storeProject);

            // Call
            service.LoadAssessmentSections(filePath);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void LoadAssessmentSections_LoadingProjectSuccessful_ReturnsRingtoetsProject()
        {
            // Setup
            var project = new RingtoetsProject();

            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(null))
                        .IgnoreArguments()
                        .Return(project);
            mocks.ReplayAll();

            var service = new LoadAssessmentSectionService(storeProject);

            // Call
            IEnumerable<AssessmentSection> assessmentSections = service.LoadAssessmentSections(string.Empty);

            // Assert
            Assert.AreSame(project.AssessmentSections, assessmentSections);
            mocks.VerifyAll();
        }

        [Test]
        public void LoadAssessmentSections_LoadedProjectNull_ThrowsLoadAssessmentSectionException()
        {
            // Setup
            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(null))
                        .IgnoreArguments()
                        .Return(null);
            mocks.ReplayAll();

            var service = new LoadAssessmentSectionService(storeProject);

            // Call
            TestDelegate call = () => service.LoadAssessmentSections(string.Empty);

            // Assert
            Assert.Throws<LoadAssessmentSectionException>(call);
            mocks.VerifyAll();
        }

        [Test]
        public void LoadAssessmentSections_LoadingProjectThrowsException_ThrowsLoadAssessmentSectionExceptionAndLogsError()
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

            var service = new LoadAssessmentSectionService(storeProject);

            LoadAssessmentSectionException exception = null;

            // Call
            Action call = () =>
            {
                try
                {
                    service.LoadAssessmentSections(string.Empty);
                }
                catch (LoadAssessmentSectionException e)
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