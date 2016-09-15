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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
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
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Expect(asm => asm.HydraulicBoundaryDatabase).Return(null);
            var target = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            var context = new SimpleWaveImpactAsphaltCoverContext<IObservable>(target, failureMechanism, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IObservable>>(context);
            Assert.AreSame(target, context.WrappedData);
            Assert.AreSame(assessmentSectionMock, context.AssessmentSection);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            CollectionAssert.IsEmpty(context.HydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var observableObject = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleWaveImpactAsphaltCoverContext<IObservable>(observableObject, null, assessmentSectionMock);

            // Assert
            const string expectedMessage = "Het golfklappen op asfalt toetsspoor mag niet 'null' zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
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
            const string expectedMessage = "Het traject mag niet 'null' zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
            mockRepository.VerifyAll();
        }

        [Test]
        public void AvailableHydraulicBoundaryLocations_HydraulicBoundaryDatabaseSet_ReturnsAllHydraulicBoundaryLocations()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "name", 1.1, 2.2));

            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Expect(asm => asm.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase).Repeat.Twice();
            var target = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var context = new SimpleWaveImpactAsphaltCoverContext<IObservable>(target, failureMechanism, assessmentSectionMock);

            // Call
            var availableHydraulicBoundaryLocations = context.HydraulicBoundaryLocations;

            // Assert
            Assert.AreEqual(hydraulicBoundaryDatabase.Locations, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        private class SimpleWaveImpactAsphaltCoverContext<T> : WaveImpactAsphaltCoverContext<T> where T : IObservable
        {
            public SimpleWaveImpactAsphaltCoverContext(T target, WaveImpactAsphaltCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(target, failureMechanism, assessmentSection) {}
        }
    }
}