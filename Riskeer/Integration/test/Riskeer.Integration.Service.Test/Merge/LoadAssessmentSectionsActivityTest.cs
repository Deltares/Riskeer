﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.Service;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Merge;
using Riskeer.Integration.Service.Exceptions;
using Riskeer.Integration.Service.Merge;

namespace Riskeer.Integration.Service.Test.Merge
{
    [TestFixture]
    public class LoadAssessmentSectionsActivityTest
    {
        [Test]
        public void Constructor_AssessmentSectionsOwnerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var service = mocks.StrictMock<ILoadAssessmentSectionService>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new LoadAssessmentSectionsActivity(null, service, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSectionOwner", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_LoadAssessmentSectionServiceNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new LoadAssessmentSectionsActivity(new AssessmentSectionOwner(), null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("loadAssessmentSectionService", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var service = mocks.StrictMock<ILoadAssessmentSectionService>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new LoadAssessmentSectionsActivity(new AssessmentSectionOwner(),
                                                                         service,
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
            var service = mocks.StrictMock<ILoadAssessmentSectionService>();
            mocks.ReplayAll();

            var owner = new AssessmentSectionOwner();

            // Call
            var activity = new LoadAssessmentSectionsActivity(owner, service, string.Empty);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual("Inlezen van project", activity.Description);
            Assert.AreEqual(ActivityState.None, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        public void Run_Always_SendsFilePathToGetAssessmentSections()
        {
            // Setup
            const string filePath = "File\\Path";

            var mocks = new MockRepository();
            var service = mocks.StrictMock<ILoadAssessmentSectionService>();
            service.Expect(p => p.LoadAssessmentSection(filePath)).Return(new AssessmentSection(AssessmentSectionComposition.Dike));
            mocks.ReplayAll();

            var owner = new AssessmentSectionOwner();
            var activity = new LoadAssessmentSectionsActivity(owner, service, filePath);

            // Call
            activity.Run();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Run_ServiceReturnsAssessmentSections_SetsActivityStateToExecutedAndSetsAssessmentSections()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mocks = new MockRepository();
            var service = mocks.StrictMock<ILoadAssessmentSectionService>();
            service.Expect(p => p.LoadAssessmentSection(null))
                   .IgnoreArguments()
                   .Return(assessmentSection);
            mocks.ReplayAll();

            var owner = new AssessmentSectionOwner();
            var activity = new LoadAssessmentSectionsActivity(owner, service, string.Empty);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Executed, activity.State);
            Assert.AreSame(assessmentSection, owner.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void Run_ServiceThrowsException_SetsActivityStateToFailedAndDoesNotSetAssessmentSections()
        {
            // Setup
            var mocks = new MockRepository();
            var service = mocks.StrictMock<ILoadAssessmentSectionService>();
            service.Expect(p => p.LoadAssessmentSection(null))
                   .IgnoreArguments()
                   .Throw(new LoadAssessmentSectionException());
            mocks.ReplayAll();

            var owner = new AssessmentSectionOwner();
            var activity = new LoadAssessmentSectionsActivity(owner, service, string.Empty);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Failed, activity.State);
            Assert.IsNull(owner.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCancelledActivity_WhenFinishingActivity_ThenActivityStateSetToCancelledAndDoesNotSetAssessmentSections()
        {
            // Given
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mocks = new MockRepository();
            var service = mocks.StrictMock<ILoadAssessmentSectionService>();
            service.Expect(p => p.LoadAssessmentSection(null))
                   .IgnoreArguments()
                   .Return(assessmentSection);
            mocks.ReplayAll();

            var owner = new AssessmentSectionOwner();
            var activity = new LoadAssessmentSectionsActivity(owner, service, string.Empty);

            activity.Run();
            activity.Cancel();

            // When
            activity.Finish();

            // Assert
            Assert.AreEqual(ActivityState.Canceled, activity.State);
            Assert.IsNull(owner.AssessmentSection);
            mocks.VerifyAll();
        }
    }
}