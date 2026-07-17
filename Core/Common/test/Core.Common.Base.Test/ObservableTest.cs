// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Core.Common.Base.Test
{
    [TestFixture]
    public class ObservableTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var observable = new TestObservable();

            // Assert
            Assert.IsInstanceOf<IObservable>(observable);
            CollectionAssert.IsEmpty(observable.Observers);
        }

        [Test]
        public void Observers_WhenAttachingObserver_ContainsExpectedObserver()
        {
            // Setup
            var testObservable = new TestObservable();
            var observer = Substitute.For<IObserver>();
            testObservable.Attach(observer);

            // Call
            IEnumerable<IObserver> observers = testObservable.Observers;

            // Assert
            Assert.AreSame(observer, observers.Single());
        }

        [Test]
        public void NotifyObservers_WithObserverAttached_ObserverIsNotified()
        {
            // Setup
            var observer = Substitute.For<IObserver>();

            var observable = new TestObservable();
            observable.Attach(observer);

            // Call
            observable.NotifyObservers();

            // Assert
            observer.Received().UpdateObserver(); // Expect to be called once
        }

        [Test]
        public void NotifyObserver_AttachedObserverDetachedAgain_ObserverNoLongerNotified()
        {
            // Setup
            var observer = Substitute.For<IObserver>();

            var observable = new TestObservable();
            observable.Attach(observer);
            observable.Detach(observer);

            // Call
            observable.NotifyObservers();

            // Assert
            observer.DidNotReceive().UpdateObserver(); // Expect no calls on 'observer'
        }

        [Test]
        public void NotifyObservers_MultipleObserversDetachingOrAttachingOthers_NoUpdatesForAttachedAndDetachedObservers()
        {
            // Setup
            var testObservable = new TestObservable();

            var observer1 = Substitute.For<IObserver>();
            var observer2 = Substitute.For<IObserver>();
            var observer3 = Substitute.For<IObserver>();
            var observer4 = Substitute.For<IObserver>();
            var observer5 = Substitute.For<IObserver>();
            var observer6 = Substitute.For<IObserver>();

            testObservable.Attach(observer1);
            testObservable.Attach(observer2);
            testObservable.Attach(observer3);
            testObservable.Attach(observer4);
            testObservable.Attach(observer6);

            observer2.When(x => x.UpdateObserver()).Do(_ => testObservable.Detach(observer3));
            observer4.When(x => x.UpdateObserver()).Do(_ => testObservable.Attach(observer5));

            // Call
            testObservable.NotifyObservers();

            // Assert
            observer1.Received().UpdateObserver();
            observer2.Received().UpdateObserver();
            observer3.DidNotReceive().UpdateObserver(); // A detached observer should no longer be updated
            observer4.Received().UpdateObserver();
            observer5.DidNotReceive().UpdateObserver(); // An attached observer should not be updated too
            observer6.Received().UpdateObserver();
        }

        private class TestObservable : Observable {}
    }
}