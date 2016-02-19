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
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.Forms.PresentationObjects;
using Ringtoets.Integration.Data;

namespace Ringtoets.HydraRing.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseContextTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            var pressentationObject = new HydraulicBoundaryDatabaseContext(hydraulicBoundaryDatabase, assessmentSectionBaseMock);

            // Assert
            Assert.IsInstanceOf<IObservable>(pressentationObject);
            Assert.AreSame(hydraulicBoundaryDatabase, pressentationObject.BoundaryDatabase);
            Assert.AreSame(assessmentSectionBaseMock, pressentationObject.BaseNode);
        }

        [Test]
        public void Constructor_AssesmentSectionBaseIsNull_ThrowArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            TestDelegate call = () => new HydraulicBoundaryDatabaseContext(hydraulicBoundaryDatabase, null);

            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None)[0];
            Assert.AreEqual("Het traject mag niet 'null' zijn.", customMessage);
        }

        [Test]
        public void NotifyObservers_ObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var presentationObject = new HydraulicBoundaryDatabaseContext(hydraulicBoundaryDatabase, assessmentSectionBaseMock);
            presentationObject.Attach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_ObserverDetached_NoCallOnObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var presentationObject = new HydraulicBoundaryDatabaseContext(hydraulicBoundaryDatabase, assessmentSectionBaseMock);
            presentationObject.Attach(observer);
            presentationObject.Detach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect not calls on 'observer'
        }
    }
}