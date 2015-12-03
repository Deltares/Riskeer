using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Test.Data
{
    [TestFixture]
    public class ProjectTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Call
            var project = new Project();

            // Assert
            Assert.IsInstanceOf<IObservable>(project);
            Assert.AreEqual("Project", project.Name);
            Assert.IsNull(project.Description);
            CollectionAssert.IsEmpty(project.Items);
        }

        [Test]
        public void NameConstructor_SetNameAndInitializeOtherProperties()
        {
            // Setup
            const string someName = "<Some name>";

            // Call
            var project = new Project(someName);

            // Assert
            Assert.IsInstanceOf<IObservable>(project);
            Assert.AreEqual(someName, project.Name);
            Assert.IsNull(project.Description);
            CollectionAssert.IsEmpty(project.Items);
        }

        [Test]
        public void AutomaticProperties_SetAndGettingValue_ShouldReturnSetValue()
        {
            // Setup
            const string niceProjectName = "Nice project name";
            const string nicerDescription = "Nicer description";

            // Call
            var project = new Project
            {
                Name = niceProjectName,
                Description = nicerDescription,
            };

            // Assert
            Assert.AreEqual(niceProjectName, project.Name);
            Assert.AreEqual(nicerDescription, project.Description);
        }

        [Test]
        public void NotifyObservers_WithObserverAttached_ObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Once();
            mocks.ReplayAll();

            var project = new Project();
            project.Attach(observerMock);

            // Call
            project.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_AttachedObserverHasBeenDetached_ObserverShouldNoLongerBeNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Once();
            mocks.ReplayAll();

            var project = new Project();
            project.Attach(observerMock);
            project.NotifyObservers();

            // Call
            project.Detach(observerMock);
            project.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }
    }
}