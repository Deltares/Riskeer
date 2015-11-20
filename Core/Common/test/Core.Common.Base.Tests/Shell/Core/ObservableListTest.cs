using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Tests.Shell.Core
{
    [TestFixture]
    public class ObservableListTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var list = new ObservableList<object>();

            // Assert
            Assert.IsInstanceOf<List<object>>(list);
            Assert.IsInstanceOf<IObservable>(list);
        }

        [Test]
        public void NotifyObservers_WithObserverAttached_ObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()); // Expect to be called once
            mocks.ReplayAll();

            var list = new ObservableList<object>();
            list.Attach(observer);

            // Call
            list.NotifyObservers();

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

            var list = new ObservableList<object>();
            list.Attach(observer);
            list.Detach(observer);

            // Call
            list.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void NotifyObservers_MultipleObserversDetachingOrAttachingOthers_NoUpdatesForAttachedAndDetachedObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var testObservable = new ObservableList<double>();

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

            observer1.Expect(o => o.UpdateObserver()).Repeat.Once();
            observer2.Expect(o => o.UpdateObserver()).Do((Action)(() => testObservable.Detach(observer3))).Repeat.Once();
            observer3.Expect(o => o.UpdateObserver()).Repeat.Never(); // A detached observer should no longer be updated
            observer4.Expect(o => o.UpdateObserver()).Do((Action)(() => testObservable.Attach(observer5))).Repeat.Once();
            observer5.Expect(o => o.UpdateObserver()).Repeat.Never(); // An attached observer should not be updated too
            observer6.Expect(o => o.UpdateObserver()).Repeat.Once();

            mocks.ReplayAll();

            // Call
            testObservable.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }
    }
}