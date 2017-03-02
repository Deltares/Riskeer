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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                ForeshoreProfiles =
                {
                    new TestForeshoreProfile()
                }
            };

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();

            // Call
            var context = new StabilityStoneCoverWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                failureMechanism.ForeshoreProfiles,
                assessmentSection);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext>(context);
            Assert.AreEqual(calculation.InputParameters, context.WrappedData);
            CollectionAssert.AreEqual(failureMechanism.ForeshoreProfiles, context.ForeshoreProfiles);
            CollectionAssert.AreEqual(hydraulicBoundaryDatabase.Locations, context.HydraulicBoundaryLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate test = () => new StabilityStoneCoverWaveConditionsInputContext(
                null,
                null,
                failureMechanism.ForeshoreProfiles,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("wrappedData", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ForeshoreProfilesNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var input = new WaveConditionsInput();

            // Call
            TestDelegate test = () => new StabilityStoneCoverWaveConditionsInputContext(
                input,
                new TestCalculation(), 
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var input = new WaveConditionsInput();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call

            TestDelegate test = () => new StabilityStoneCoverWaveConditionsInputContext(
                input,
                null, 
                failureMechanism.ForeshoreProfiles,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var input = new WaveConditionsInput();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call

            TestDelegate test = () => new StabilityStoneCoverWaveConditionsInputContext(
                input,
                new TestCalculation(), 
                failureMechanism.ForeshoreProfiles,
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void HydraulicBoundaryLocations_HydraulicBoundaryDatabaseNull_ReturnEmptyCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var input = new WaveConditionsInput();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var context = new StabilityStoneCoverWaveConditionsInputContext(
                input, 
                new TestCalculation(), 
                failureMechanism.ForeshoreProfiles, 
                assessmentSection);

            // Call
            IEnumerable<HydraulicBoundaryLocation> locations = context.HydraulicBoundaryLocations;

            // Assert
            CollectionAssert.IsEmpty(locations);
            mocks.VerifyAll();
        }
    }
}