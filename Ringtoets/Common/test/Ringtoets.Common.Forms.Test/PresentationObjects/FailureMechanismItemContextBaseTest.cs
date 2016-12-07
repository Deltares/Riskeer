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
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismItemContextBaseTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var observableStub = mockRepository.Stub<IObservable>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            var context = new SimpleFailureMechanismItemContext<IObservable, IFailureMechanism>(observableStub, failureMechanismStub, assessmentSectionStub);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IObservable>>(context);
            Assert.AreSame(observableStub, context.WrappedData);
            Assert.AreSame(failureMechanismStub, context.FailureMechanism);
            Assert.AreSame(assessmentSectionStub, context.AssessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var observableStub = mockRepository.Stub<IObservable>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleFailureMechanismItemContext<IObservable, IFailureMechanism>(observableStub, null, assessmentSectionStub);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observableStub = mockRepository.Stub<IObservable>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleFailureMechanismItemContext<IObservable, IFailureMechanism>(observableStub, failureMechanismStub, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void AvailableHydraulicBoundaryLocations_HydraulicBoundaryDatabaseSet_ReturnsAllHydraulicBoundaryLocations()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "name", 1.1, 2.2));

            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var observableStub = mockRepository.Stub<IObservable>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var context = new SimpleFailureMechanismItemContext<IObservable, IFailureMechanism>(observableStub, failureMechanismStub, assessmentSectionStub);

            // Call
            var availableHydraulicBoundaryLocations = context.AvailableHydraulicBoundaryLocations;

            // Assert
            Assert.AreEqual(1, availableHydraulicBoundaryLocations.Count());
            Assert.AreEqual(hydraulicBoundaryDatabase.Locations, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        private class SimpleFailureMechanismItemContext<TData, TFailureMechanism> : FailureMechanismItemContextBase<TData, TFailureMechanism>
            where TData : IObservable
            where TFailureMechanism : IFailureMechanism
        {
            public SimpleFailureMechanismItemContext(TData target, TFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(target, failureMechanism, assessmentSection) {}
        }
    }
}