using Core.Common.Base;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingInputContextTest
    {
        [Test]
        public void DefaultConstructpr_ExpectedValues()
        {
            // Call
            var context = new PipingInputContext();

            // Assert
            Assert.IsInstanceOf<IObservable>(context);
            Assert.IsNull(context.WrappedPipingInput);
            CollectionAssert.IsEmpty(context.AvailablePipingSurfaceLines);
            CollectionAssert.IsEmpty(context.AvailablePipingSoilProfiles);
        }

        [Test]
        public void NotifyObservers_HasPipingDataAndObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var presentationObject = new PipingInputContext
            {
                WrappedPipingInput = new PipingInput()
            };
            presentationObject.Attach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_HasPipingDataAndObserverDetached_NoCallsOnObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var presentationObject = new PipingInputContext
            {
                WrappedPipingInput = new PipingInput()
            };
            presentationObject.Attach(observer);
            presentationObject.Detach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect not calls on 'observer'
        }

        [Test]
        public void PipingDataNotifyObservers_AttachedOnPipingCalculationInputs_ObserverNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingData = new PipingInput();
            var presentationObject = new PipingInputContext
            {
                WrappedPipingInput = pipingData
            };
            presentationObject.Attach(observer);

            // Call
            pipingData.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }
    }
}