using Core.Common.Base;
using NUnit.Framework;

using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingCalculationContextTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var presentationObject = new PipingCalculationContext();

            // Assert
            Assert.IsInstanceOf<IObservable>(presentationObject);
            Assert.IsNull(presentationObject.WrappedPipingCalculation);
            CollectionAssert.IsEmpty(presentationObject.AvailablePipingSurfaceLines);
            CollectionAssert.IsEmpty(presentationObject.AvailablePipingSoilProfiles);
        }

        [Test]
        public void NotifyObservers_HasPipingCalculationAndObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var presentationObject = new PipingCalculationContext
            {
                WrappedPipingCalculation = new PipingCalculation()
            };
            presentationObject.Attach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_HasPipingCalculationAndObserverDetached_NoCallsOnObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var presentationObject = new PipingCalculationContext
            {
                WrappedPipingCalculation = new PipingCalculation()
            };
            presentationObject.Attach(observer);
            presentationObject.Detach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect not calls on 'observer'
        }

        [Test]
        public void PipingCalculationNotifyObservers_AttachedOnPipingCalculationContext_ObserverNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var calculation = new PipingCalculation();
            var presentationObject = new PipingCalculationContext
            {
                WrappedPipingCalculation = calculation
            };
            presentationObject.Attach(observer);

            // Call
            calculation.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var inputs = new PipingCalculationContext
            {
                WrappedPipingCalculation = new PipingCalculation
                {
                    Output = new TestPipingOutput()
                }
            };

            // Call
            inputs.ClearOutput();

            // Assert
            Assert.IsNull(inputs.WrappedPipingCalculation.Output);
        }
    }
}