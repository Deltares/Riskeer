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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionInwardsContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var target = new ObservableObject();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(target, failureMechanism, assessmentSectionStub);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableObject>>(context);
            Assert.AreSame(target, context.WrappedData);
            Assert.AreSame(assessmentSectionStub, context.AssessmentSection);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            CollectionAssert.IsEmpty(context.AvailableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var observableObject = new ObservableObject();

            // Call
            TestDelegate call = () => new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject, null, assessmentSectionStub);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var observableObject = new ObservableObject();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate call = () => new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AvailableHydraulicBoundaryLocations_HydraulicBoundaryDatabaseSet_ReturnsAllHydraulicBoundaryLocations()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "name", 1.1, 2.2));

            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var target = new ObservableObject();
            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(target, failureMechanism, assessmentSectionStub);

            // Call
            IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations = context.AvailableHydraulicBoundaryLocations;

            // Assert
            Assert.AreEqual(1, availableHydraulicBoundaryLocations.Count());
            Assert.AreEqual(hydraulicBoundaryDatabase.Locations, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        private class ObservableObject : Observable {}

        private class SimpleGrassCoverErosionInwardsContext<T> : GrassCoverErosionInwardsContext<T> where T : IObservable
        {
            public SimpleGrassCoverErosionInwardsContext(T target, GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(target, failureMechanism, assessmentSection) {}
        }
    }
}