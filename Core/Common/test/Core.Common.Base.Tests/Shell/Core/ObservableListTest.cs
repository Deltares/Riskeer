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
    }
}