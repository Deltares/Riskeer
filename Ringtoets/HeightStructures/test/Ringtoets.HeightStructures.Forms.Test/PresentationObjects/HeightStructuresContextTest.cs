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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HeightStructures.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class HeightStructuresContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var target = new ObservableObject();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            var context = new SimpleHeightStructuresContext<ObservableObject>(target, failureMechanism, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableObject>>(context);
            Assert.AreSame(target, context.WrappedData);
            Assert.AreSame(assessmentSectionMock, context.AssessmentSection);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var observableObject = new ObservableObject();

            // Call
            TestDelegate call = () => new SimpleHeightStructuresContext<ObservableObject>(observableObject, null, assessmentSectionMock);

            // Assert
            const string expectedMessage = "Het hoogte kunstwerk toetsspoor mag niet 'null' zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var observableObject = new ObservableObject();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            TestDelegate call = () => new SimpleHeightStructuresContext<ObservableObject>(observableObject, failureMechanism, null);

            // Assert
            const string expectedMessage = "Het traject mag niet 'null' zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void AvailableHydraulicBoundaryLocations_HydraulicBoundaryDatabaseSet_ReturnsAllHydraulicBoundaryLocations()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "name", 1.1, 2.2));

            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mockRepository.ReplayAll();

            var target = new ObservableObject();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var context = new SimpleHeightStructuresContext<ObservableObject>(target, failureMechanism, assessmentSectionStub);

            // Call
            var availableHydraulicBoundaryLocations = context.AvailableHydraulicBoundaryLocations;

            // Assert
            Assert.AreEqual(1, availableHydraulicBoundaryLocations.Count());
            Assert.AreEqual(hydraulicBoundaryDatabase.Locations, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        private class ObservableObject : Observable {}

        private class SimpleHeightStructuresContext<T> : HeightStructuresContext<T> where T : IObservable
        {
            public SimpleHeightStructuresContext(T target, HeightStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(target, failureMechanism, assessmentSection) {}
        }
    }
}