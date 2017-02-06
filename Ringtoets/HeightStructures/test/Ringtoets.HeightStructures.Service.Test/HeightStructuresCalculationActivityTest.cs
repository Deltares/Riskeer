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
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.Service.Test
{
    [TestFixture]
    public class HeightStructuresCalculationActivityTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            // Call
            var activity = new HeightStructuresCalculationActivity(calculation, "", failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual(calculation.Name, activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            TestDelegate call = () => new HeightStructuresCalculationActivity(null, "", failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_HlcdFilepathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            // Call
            TestDelegate call = () => new HeightStructuresCalculationActivity(calculation, null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabaseFilePath", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new StructuresCalculation<HeightStructuresInput>();

            // Call
            TestDelegate call = () => new HeightStructuresCalculationActivity(calculation, "", null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            // Call
            TestDelegate call = () => new HeightStructuresCalculationActivity(calculation, "", failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }
    }
}