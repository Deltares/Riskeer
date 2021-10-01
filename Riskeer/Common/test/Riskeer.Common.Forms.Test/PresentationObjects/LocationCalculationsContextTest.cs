// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class LocationCalculationsContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var context = new TestLocationCalculationsContext(new object(), new ObservableList<IObservable>());

            // Assert
            Assert.IsInstanceOf<WrappedObjectContextBase<object>>(context);
            Assert.IsInstanceOf<IObservable>(context);
            Assert.IsEmpty(context.Observers);
        }

        [Test]
        public void GivenContext_WhenObserverAttached_ThenExpectedValues()
        {
            // Given
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            var context = new TestLocationCalculationsContext(new object(), new ObservableList<IObservable>());

            // When
            context.Attach(observer);

            // Then
            Assert.AreEqual(1, context.Observers.Count());
            Assert.AreSame(observer, context.Observers.ElementAt(0));
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenContext_WhenDetachingAttachedObserver_ThenExpectedValues()
        {
            // Given
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            var context = new TestLocationCalculationsContext(new object(), new ObservableList<IObservable>());

            context.Attach(observer);

            // When
            context.Detach(observer);

            // Then
            Assert.IsEmpty(context.Observers);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenContextWithObserverAttached_WhenNotifyingLocationCalculationsListToObserve_ThenObserverCorrectlyNotified()
        {
            // Given
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var locationCalculationsListToObserve = new ObservableList<IObservable>();
            var context = new TestLocationCalculationsContext(new object(), locationCalculationsListToObserve);

            context.Attach(observer);

            // When
            locationCalculationsListToObserve.NotifyObservers();

            // Then
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenContextWithObserverAttached_WhenNotifyingLocationCalculationsElementInListToObserve_ThenObserverCorrectlyNotified()
        {
            // Given
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var observable = new TestObservable();
            var locationCalculationsListToObserve = new ObservableList<IObservable>
            {
                observable
            };

            var context = new TestLocationCalculationsContext(new object(), locationCalculationsListToObserve);

            context.Attach(observer);

            // When
            observable.NotifyObservers();

            // Then
            mockRepository.VerifyAll();
        }

        private class TestLocationCalculationsContext : LocationCalculationsContext<object, IObservable>
        {
            public TestLocationCalculationsContext(object wrappedData, ObservableList<IObservable> locationCalculationsListToObserve) : base(wrappedData)
            {
                LocationCalculationsListToObserve = locationCalculationsListToObserve;
            }

            protected override ObservableList<IObservable> LocationCalculationsListToObserve { get; }
        }

        private class TestObservable : Observable {}
    }
}