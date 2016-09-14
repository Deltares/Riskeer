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
using Core.Common.Base.Geometry;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationInputContextTest
    {
        [Test]
        public void Constructor_CalculationGroupNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate call = () => new StabilityStoneCoverWaveConditionsCalculationInputContext(null, failureMechanism, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("wrappedData", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var input = new WaveConditionsInput();

            // Call
            TestDelegate call = () => new StabilityStoneCoverWaveConditionsCalculationInputContext(input, null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var input = new WaveConditionsInput();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate call = () => new StabilityStoneCoverWaveConditionsCalculationInputContext(input, failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(0, "A", 0, 0),
                    new HydraulicBoundaryLocation(1, "B", 1, 1)
                }
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var input = new WaveConditionsInput();

            // Call
            var context = new StabilityStoneCoverWaveConditionsCalculationInputContext(input, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<WaveConditionsInput>>(context);

            Assert.AreSame(input, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            CollectionAssert.AreEqual(hydraulicBoundaryDatabase.Locations, context.HydraulicBoundaryLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void AvailableForeshoreProfiles_Always_ReturnForeshoreProfilesOfFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                ForeshoreProfiles =
                {
                    new ForeshoreProfile(new Point2D(0, 0),
                                         new Point2D[0],
                                         null,
                                         new ForeshoreProfile.ConstructionProperties()),
                    new ForeshoreProfile(new Point2D(1, 1),
                                         new Point2D[0],
                                         null,
                                         new ForeshoreProfile.ConstructionProperties())
                }
            };

            var input = new WaveConditionsInput();

            // Call
            var context = new StabilityStoneCoverWaveConditionsCalculationInputContext(input, failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AreEqual(failureMechanism.ForeshoreProfiles, context.ForeshoreProfiles);
            mocks.VerifyAll();
        }

        [Test]
        public void AvailableHydraulicBoundaryLocations_AssessmentSectionHasNoDatabase_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = null;
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var input = new WaveConditionsInput();

            // Call
            var context = new StabilityStoneCoverWaveConditionsCalculationInputContext(input, failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.IsEmpty(context.HydraulicBoundaryLocations);
            mocks.VerifyAll();
        }
    }
}