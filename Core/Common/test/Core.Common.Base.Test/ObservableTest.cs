// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

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
            var mocks = new MockRepository();
            var testObservable = new TestObservable();
            var observer = mocks.Stub<IObserver>();
            testObservable.Attach(observer);
            mocks.ReplayAll();

            // Call
            IEnumerable<IObserver> observers = testObservable.Observers;

            // Assert
            Assert.AreSame(observer, observers.Single());

            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_WithObserverAttached_ObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()); // Expect to be called once
            mocks.ReplayAll();

            var observable = new TestObservable();
            observable.Attach(observer);

            // Call
            observable.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObserver_AttachedObserverDetachedAgain_ObserverNoLongerNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observable = new TestObservable();
            observable.Attach(observer);
            observable.Detach(observer);

            // Call
            observable.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void NotifyObservers_MultipleObserversDetachingOrAttachingOthers_NoUpdatesForAttachedAndDetachedObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var testObservable = new TestObservable();

            var observer1 = mocks.Stub<IObserver>();
            var observer2 = mocks.Stub<IObserver>();
            var observer3 = mocks.Stub<IObserver>();
            var observer4 = mocks.Stub<IObserver>();
            var observer5 = mocks.Stub<IObserver>();
            var observer6 = mocks.Stub<IObserver>();

            testObservable.Attach(observer1);
            testObservable.Attach(observer2);
            testObservable.Attach(observer3);
            testObservable.Attach(observer4);
            testObservable.Attach(observer6);

            observer1.Expect(o => o.UpdateObserver());
            observer2.Expect(o => o.UpdateObserver()).Do((Action) (() => testObservable.Detach(observer3)));
            observer3.Expect(o => o.UpdateObserver()).Repeat.Never(); // A detached observer should no longer be updated
            observer4.Expect(o => o.UpdateObserver()).Do((Action) (() => testObservable.Attach(observer5)));
            observer5.Expect(o => o.UpdateObserver()).Repeat.Never(); // An attached observer should not be updated too
            observer6.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            // Call
            testObservable.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        private class TestObservable : Observable {}
    }
}