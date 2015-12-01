using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Test.Shell.Core
{
    [TestFixture]
    public class ProjectTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // call
            var project = new Project();

            // assert
            Assert.IsInstanceOf<IObservable>(project);

            Assert.AreEqual("Project", project.Name);
            Assert.IsNull(project.Description);
            CollectionAssert.IsEmpty(project.Items);
        }

        [Test]
        public void NameConstructor_SetNameAndInitializeOtherProperties()
        {
            // setup
            const string someName = "<Some name>";

            // call
            var project = new Project(someName);

            // assert
            Assert.AreEqual(someName, project.Name);
            Assert.IsNull(project.Description);
            CollectionAssert.IsEmpty(project.Items);
        }

        [Test]
        public void AutomaticProperties_SetAndGettingValue_ShouldReturnSetValue()
        {
            // setup & Call
            const string niceProjectName = "Nice project name";
            const string nicerDescription = "Nicer description";

            var project = new Project
            {
                Name = niceProjectName,
                Description = nicerDescription,
            };

            // assert
            Assert.AreEqual(niceProjectName, project.Name);
            Assert.AreEqual(nicerDescription, project.Description);
        }

        [Test]
        public void NotifyObservers_WithObserverAttached_ObserverIsNotified()
        {
            // setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var project = new Project();
            project.Attach(observerMock);

            // call
            project.NotifyObservers();

            // assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_AttachedObserverHasBeenDetached_ObserverShouldNoLongerBeNotified()
        {
            // setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Once();
            mocks.ReplayAll();

            var project = new Project();
            project.Attach(observerMock);
            project.NotifyObservers();

            // call
            project.Detach(observerMock);
            project.NotifyObservers();

            // assert
            mocks.VerifyAll();
        }
    }
}