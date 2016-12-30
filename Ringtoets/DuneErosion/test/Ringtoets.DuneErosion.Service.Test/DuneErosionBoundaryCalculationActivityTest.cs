// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Service;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Activities;

namespace Ringtoets.DuneErosion.Service.Test
{
    [TestFixture]
    public class DuneErosionBoundaryCalculationActivityTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var duneLocation = new TestDuneLocation();
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            var activity = new DuneErosionBoundaryCalculationActivity(duneLocation, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<HydraRingActivityBase>(activity);
            Assert.AreEqual(duneLocation.Name, activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DuneLocationNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate test = () => new DuneErosionBoundaryCalculationActivity(null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("duneLocation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var duneLocation = new TestDuneLocation();

            // Call
            TestDelegate test = () => new DuneErosionBoundaryCalculationActivity(duneLocation, null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var duneLocation = new TestDuneLocation();
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate test = () => new DuneErosionBoundaryCalculationActivity(duneLocation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }
    }
}