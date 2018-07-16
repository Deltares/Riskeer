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
        public void Constructor_AssessmentSectionsOwnerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.StrictMock<ILoadAssessmentSectionService>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new LoadAssessmentSectionsActivity(null, provider, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSectionsOwner", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_loadAssessmentSectionServiceNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new LoadAssessmentSectionsActivity(new AssessmentSectionsOwner(), null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("loadAssessmentSectionService", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.StrictMock<ILoadAssessmentSectionService>();
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
            var provider = mocks.StrictMock<ILoadAssessmentSectionService>();
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
        public void Run_Always_SendsFilePathToGetAssessmentSections()
        {
            // Setup
            const string filePath = "File\\Path";

            var mocks = new MockRepository();
            var provider = mocks.StrictMock<ILoadAssessmentSectionService>();
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
            var provider = mocks.StrictMock<ILoadAssessmentSectionService>();
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
            var provider = mocks.StrictMock<ILoadAssessmentSectionService>();
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
            var provider = mocks.StrictMock<ILoadAssessmentSectionService>();
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