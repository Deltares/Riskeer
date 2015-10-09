using DelftTools.Shell.Core;
using DelftTools.Utils;
using NUnit.Framework;
using Rhino.Mocks;

namespace Wti.Data.Test
{
    [TestFixture]
    public class WtiProjectTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var project = new WtiProject();

            // assert
            Assert.IsInstanceOf<INameable>(project);
            Assert.IsInstanceOf<IObservable>(project);
            Assert.AreEqual("WTI project", project.Name);
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToProject_ObserverIsNotified()
        {
            // setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var project = new WtiProject();
            project.Attach(observer);

            // call
            project.NotifyObservers();

            // assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_ObserverHasBeenDetached_ObserverShouldNotBeNotified()
        {
            // setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var project = new WtiProject();
            project.Attach(observer);
            project.Detach(observer);

            // call
            project.NotifyObservers();

            // assert
            mocks.VerifyAll(); // Expect no calls on observer
        }
    }
}