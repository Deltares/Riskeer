﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Service.Exceptions;
using Riskeer.Integration.Service.Merge;

namespace Riskeer.Integration.Service.Test.Merge
{
    [TestFixture]
    public class LoadAssessmentSectionServiceTest
    {
        [Test]
        public void Constructor_ProjectStorageNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new LoadAssessmentSectionService(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
        public void LoadAssessmentSection_Always_SendsFilePathToLoadsProject()
        {
            // Setup
            const string filePath = "Some\\path";

            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(filePath)).Return(CreateProject());
            mocks.ReplayAll();

            var service = new LoadAssessmentSectionService(storeProject);

            // Call
            service.LoadAssessmentSection(filePath);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void LoadAssessmentSection_LoadingProjectSuccessful_ReturnsRiskeerProject()
        {
            // Setup
            RiskeerProject project = CreateProject();

            var mocks = new MockRepository();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Expect(sp => sp.LoadProject(null))
                        .IgnoreArguments()
                        .Return(project);
            mocks.ReplayAll();

            var service = new LoadAssessmentSectionService(storeProject);

            // Call
            AssessmentSection assessmentSection = service.LoadAssessmentSection(string.Empty);

            // Assert
            Assert.AreSame(project.AssessmentSection, assessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void LoadAssessmentSection_LoadedProjectNull_ThrowsLoadAssessmentSectionException()
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
            void Call() => service.LoadAssessmentSection(string.Empty);

            // Assert
            Assert.Throws<LoadAssessmentSectionException>(Call);
            mocks.VerifyAll();
        }

        [Test]
        public void LoadAssessmentSection_LoadingProjectThrowsException_ThrowsLoadAssessmentSectionExceptionAndLogsError()
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
            void Call()
            {
                try
                {
                    service.LoadAssessmentSection(string.Empty);
                }
                catch (LoadAssessmentSectionException e)
                {
                    exception = e;
                }
            }

            // Assert
            var expectedLogMessage = new Tuple<string, LogLevelConstant>(exceptionMessage, LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, expectedLogMessage);
            Assert.AreEqual(storageException, exception.InnerException);
            Assert.AreEqual(storageException.Message, exception.Message);
            mocks.VerifyAll();
        }

        private static RiskeerProject CreateProject()
        {
            var random = new Random(21);
            return new RiskeerProject(new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>()));
        }
    }
}