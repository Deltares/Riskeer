// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
    public class ObservableListTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var observableList = new ObservableList<object>();

            // Assert
            Assert.IsInstanceOf<List<object>>(observableList);
            Assert.IsInstanceOf<IObservableEnumerable<object>>(observableList);
            CollectionAssert.IsEmpty(observableList.Observers);
        }

        [Test]
        public void Observers_WhenAttachingObserver_ContainsExpectedObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var testObservableList = new ObservableList<object>();
            var observer = mocks.Stub<IObserver>();
            testObservableList.Attach(observer);
            mocks.ReplayAll();

            // Call
            IEnumerable<IObserver> observers = testObservableList.Observers;

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

            var observableList = new ObservableList<object>();
            observableList.Attach(observer);

            // Call
            observableList.NotifyObservers();

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

            var observableList = new ObservableList<object>();
            observableList.Attach(observer);
            observableList.Detach(observer);

            // Call
            observableList.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void NotifyObservers_MultipleObserversDetachingOrAttachingOthers_NoUpdatesForAttachedAndDetachedObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observableList = new ObservableList<double>();

            var observer1 = mocks.Stub<IObserver>();
            var observer2 = mocks.Stub<IObserver>();
            var observer3 = mocks.Stub<IObserver>();
            var observer4 = mocks.Stub<IObserver>();
            var observer5 = mocks.Stub<IObserver>();
            var observer6 = mocks.Stub<IObserver>();

            observableList.Attach(observer1);
            observableList.Attach(observer2);
            observableList.Attach(observer3);
            observableList.Attach(observer4);
            observableList.Attach(observer6);

            observer1.Expect(o => o.UpdateObserver());
            observer2.Expect(o => o.UpdateObserver()).Do((Action) (() => observableList.Detach(observer3)));
            observer3.Expect(o => o.UpdateObserver()).Repeat.Never(); // A detached observer should no longer be updated
            observer4.Expect(o => o.UpdateObserver()).Do((Action) (() => observableList.Attach(observer5)));
            observer5.Expect(o => o.UpdateObserver()).Repeat.Never(); // An attached observer should not be updated too
            observer6.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            // Call
            observableList.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }
    }
}