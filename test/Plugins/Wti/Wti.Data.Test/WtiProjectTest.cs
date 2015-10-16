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
            // Call
            var project = new WtiProject();

            // Assert
            Assert.IsInstanceOf<INameable>(project);
            Assert.IsInstanceOf<IObservable>(project);
            Assert.AreEqual("WTI project", project.Name);
        }

        [Test]
        public void InitializePipingFailureMechanism_WithoutFailureMechanismSet_SetNewFailureMechanism()
        {
            // Setup
            var project = new WtiProject();

            // Call
            project.InitializePipingFailureMechanism();

            // Assert
            Assert.NotNull(project.PipingFailureMechanism);

        }

        [Test]
        public void InitializePipingFailureMechanism_WithFailureMechanismSet_SetNewFailureMechanism()
        {
            // Setup
            var project = new WtiProject();
            project.InitializePipingFailureMechanism();
            PipingFailureMechanism notExpectedFailureMechanism = project.PipingFailureMechanism;

            // Precondition
            Assert.NotNull(notExpectedFailureMechanism);

            // Call
            project.InitializePipingFailureMechanism();

            // Assert
            Assert.AreNotSame(notExpectedFailureMechanism, project.PipingFailureMechanism);
        }

        [Test]
        public void ClearPipingFailureMechanism_WithFailureMechanismSet_FailureMechanismUnassigned()
        {
            // Setup
            var project = new WtiProject();
            project.InitializePipingFailureMechanism();
            PipingFailureMechanism notExpectedFailureMechanism = project.PipingFailureMechanism;

            // Precondition
            Assert.NotNull(notExpectedFailureMechanism);

            // Call
            project.ClearPipingFailureMechanism();

            // Assert
            Assert.IsNull(project.PipingFailureMechanism);
        }

        [Test]
        public void ClearPipingFailureMechanism_WithoutFailureMechanismSet_PipingFailureMechanismStillUnassigned()
        {
            // Setup
            var project = new WtiProject();

            // Precondition
            Assert.Null(project.PipingFailureMechanism);

            // Call
            project.ClearPipingFailureMechanism();

            // Assert
            Assert.IsNull(project.PipingFailureMechanism);
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToProject_ObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var project = new WtiProject();
            project.Attach(observer);

            // Call
            project.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_ObserverHasBeenDetached_ObserverShouldNotBeNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var project = new WtiProject();
            project.Attach(observer);
            project.Detach(observer);

            // Call
            project.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect no calls on observer
        }
    }
}