﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class WaveImpactAsphaltCoverContextTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var target = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            var context = new SimpleWaveImpactAsphaltCoverContext<IObservable>(target, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IObservable>>(context);
            Assert.AreSame(target, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(failureMechanism.ForeshoreProfiles, context.ForeshoreProfiles);
            CollectionAssert.IsEmpty(context.HydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var observableObject = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleWaveImpactAsphaltCoverContext<IObservable>(observableObject, null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var observableObject = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            TestDelegate call = () => new SimpleWaveImpactAsphaltCoverContext<IObservable>(observableObject, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void HydraulicBoundaryLocations_HydraulicBoundaryDatabaseSet_ReturnsAllHydraulicBoundaryLocations()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "name", 1.1, 2.2));

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            var target = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var context = new SimpleWaveImpactAsphaltCoverContext<IObservable>(target, failureMechanism, assessmentSection);

            // Call
            var availableHydraulicBoundaryLocations = context.HydraulicBoundaryLocations;

            // Assert
            Assert.AreSame(hydraulicBoundaryDatabase.Locations, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        private class SimpleWaveImpactAsphaltCoverContext<T> : WaveImpactAsphaltCoverContext<T> where T : IObservable
        {
            public SimpleWaveImpactAsphaltCoverContext(T target, WaveImpactAsphaltCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(target, failureMechanism, assessmentSection) {}
        }
    }
}