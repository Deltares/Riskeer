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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class DikeProfilesContextTest
    {
        [Test]
        public void Constructor_ValidValues_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            var context = new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<WrappedObjectContextBase<DikeProfileCollection>>(context);
            Assert.AreSame(failureMechanism.DikeProfiles, context.WrappedData);
            Assert.AreSame(failureMechanism, context.ParentFailureMechanism);
            Assert.AreSame(assessmentSection, context.ParentAssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ObservableListIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate call = () => new DikeProfilesContext(null, failureMechanism, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("wrappedData", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var dikeProfiles = new DikeProfileCollection();

            // Call
            TestDelegate call = () => new DikeProfilesContext(dikeProfiles, null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("parentFailureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate call = () => new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("parentAssessmentSection", paramName);
        }
    }
}