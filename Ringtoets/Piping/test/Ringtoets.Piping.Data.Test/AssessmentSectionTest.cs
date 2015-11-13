using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class AssessmentSectionTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var assessmentSection = new AssessmentSection();

            // Assert
            Assert.IsInstanceOf<IObservable>(assessmentSection);
            Assert.AreEqual("Toetstraject", assessmentSection.Name);
        }

        [Test]
        public void InitializePipingFailureMechanism_WithoutFailureMechanismSet_SetNewFailureMechanism()
        {
            // Setup
            var assessmentSection = new AssessmentSection();

            // Call
            assessmentSection.InitializePipingFailureMechanism();

            // Assert
            Assert.IsNotNull(assessmentSection.PipingFailureMechanism);
        }

        [Test]
        public void InitializePipingFailureMechanism_WithFailureMechanismSet_SetNewFailureMechanism()
        {
            // Setup
            var assessmentSection = new AssessmentSection();
            assessmentSection.InitializePipingFailureMechanism();
            PipingFailureMechanism notExpectedFailureMechanism = assessmentSection.PipingFailureMechanism;

            // Precondition
            Assert.IsNotNull(notExpectedFailureMechanism);

            // Call
            assessmentSection.InitializePipingFailureMechanism();

            // Assert
            Assert.AreNotSame(notExpectedFailureMechanism, assessmentSection.PipingFailureMechanism);
        }

        [Test]
        public void ClearPipingFailureMechanism_WithFailureMechanismSet_FailureMechanismUnassigned()
        {
            // Setup
            var assessmentSection = new AssessmentSection();
            assessmentSection.InitializePipingFailureMechanism();
            PipingFailureMechanism notExpectedFailureMechanism = assessmentSection.PipingFailureMechanism;

            // Precondition
            Assert.IsNotNull(notExpectedFailureMechanism);

            // Call
            assessmentSection.ClearPipingFailureMechanism();

            // Assert
            Assert.IsNull(assessmentSection.PipingFailureMechanism);
        }

        [Test]
        public void ClearPipingFailureMechanism_WithoutFailureMechanismSet_PipingFailureMechanismStillUnassigned()
        {
            // Setup
            var assessmentSection = new AssessmentSection();

            // Precondition
            Assert.IsNull(assessmentSection.PipingFailureMechanism);

            // Call
            assessmentSection.ClearPipingFailureMechanism();

            // Assert
            Assert.IsNull(assessmentSection.PipingFailureMechanism);
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToProject_ObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection();
            assessmentSection.Attach(observer);

            // Call
            assessmentSection.NotifyObservers();

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

            var assessmentSection = new AssessmentSection();
            assessmentSection.Attach(observer);
            assessmentSection.Detach(observer);

            // Call
            assessmentSection.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect no calls on observer
        }

        [Test]
        public void CanAddPipingFailureMechanism_NoPipingFailureMechanismAssigned_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSection();

            // Call
            var result = assessmentSection.CanAddPipingFailureMechanism();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanAddPipingFailureMechanism_PipingFailureMechanismInitialized_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSection();
            assessmentSection.InitializePipingFailureMechanism();

            // Call
            var result = assessmentSection.CanAddPipingFailureMechanism();

            // Assert
            Assert.IsFalse(result);
        }
    }
}