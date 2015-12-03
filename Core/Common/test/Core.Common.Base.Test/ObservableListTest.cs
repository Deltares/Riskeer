using System;
using System.Collections.Generic;
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
            Assert.IsInstanceOf<IObservable>(observableList);
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

            observer1.Expect(o => o.UpdateObserver()).Repeat.Once();
            observer2.Expect(o => o.UpdateObserver()).Do((Action)(() => observableList.Detach(observer3))).Repeat.Once();
            observer3.Expect(o => o.UpdateObserver()).Repeat.Never(); // A detached observer should no longer be updated
            observer4.Expect(o => o.UpdateObserver()).Do((Action)(() => observableList.Attach(observer5))).Repeat.Once();
            observer5.Expect(o => o.UpdateObserver()).Repeat.Never(); // An attached observer should not be updated too
            observer6.Expect(o => o.UpdateObserver()).Repeat.Once();

            mocks.ReplayAll();

            // Call
            observableList.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }
    }
}