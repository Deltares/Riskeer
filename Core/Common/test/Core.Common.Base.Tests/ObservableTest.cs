using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Tests
{
    [TestFixture]
    public class ObservableTest
    {
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

            observer1.Expect(o => o.UpdateObserver()).Repeat.Once();
            observer2.Expect(o => o.UpdateObserver()).Do((Action) (() => testObservable.Detach(observer3))).Repeat.Once();
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

        private class TestObservable : Observable
        {
            
        }
    }
}
