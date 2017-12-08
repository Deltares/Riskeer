﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StabilityStoneCoverContextTest
    {
        [Test]
        public void Constructor_WrappedDataNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate call = () => new SimpleStabilityStoneCoverContext(null, failureMechanism, assessmentSection);

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
            var observable = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleStabilityStoneCoverContext(observable, null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate call = () => new SimpleStabilityStoneCoverContext(observable, failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            var context = new SimpleStabilityStoneCoverContext(observable, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IObservable>>(context);
            Assert.AreSame(observable, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(failureMechanism.ForeshoreProfiles, context.ForeshoreProfiles);
            CollectionAssert.IsEmpty(context.HydraulicBoundaryLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void HydraulicBoundaryLocations_HydraulicBoundaryDatabaseSet_ReturnsAllHydraulicBoundaryLocations()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "name", 1.1, 2.2)
                }
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var target = mocks.StrictMock<IObservable>();

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new SimpleStabilityStoneCoverContext(target, failureMechanism, assessmentSection);

            // Call
            IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations = context.HydraulicBoundaryLocations;

            // Assert
            Assert.AreSame(hydraulicBoundaryDatabase.Locations, availableHydraulicBoundaryLocations);
            mocks.VerifyAll();
        }

        private class SimpleStabilityStoneCoverContext : StabilityStoneCoverContext<IObservable>
        {
            public SimpleStabilityStoneCoverContext(IObservable wrappedData,
                                                    StabilityStoneCoverFailureMechanism failureMechanism,
                                                    IAssessmentSection assessmentSection) :
                base(wrappedData, failureMechanism, assessmentSection) {}
        }
    }
}