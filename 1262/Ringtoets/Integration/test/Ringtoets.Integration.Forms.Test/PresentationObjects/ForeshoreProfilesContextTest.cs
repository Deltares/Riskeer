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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class ForeshoreProfilesContextTest
    {
        [Test]
        public void Constructor_ForeshoreProfilesNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ForeshoreProfilesContext(null, failureMechanism, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("wrappedData", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ParentFailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var foreshores = new ForeshoreProfileCollection();

            // Call
            TestDelegate call = () => new ForeshoreProfilesContext(foreshores, null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("parentFailureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ParentAssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var foreshores = new ForeshoreProfileCollection();

            // Call
            TestDelegate call = () => new ForeshoreProfilesContext(foreshores, failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("parentAssessmentSection", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidArgument_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var foreshores = new ForeshoreProfileCollection();

            // Call
            var context = new ForeshoreProfilesContext(foreshores, failureMechanism, assessmentSection);

            // Assert
            Assert.AreSame(foreshores, context.WrappedData);
            Assert.AreSame(failureMechanism, context.ParentFailureMechanism);
            Assert.AreSame(assessmentSection, context.ParentAssessmentSection);
        }
    }
}