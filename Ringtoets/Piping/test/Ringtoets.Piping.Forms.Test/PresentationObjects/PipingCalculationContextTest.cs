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
            // Setup
            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();

            // Call
            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles);

            // Assert
            Assert.IsInstanceOf<IObservable>(presentationObject);
            Assert.IsInstanceOf<PipingContext<PipingCalculation>>(presentationObject);
            Assert.AreSame(calculation, presentationObject.WrappedData);
            Assert.AreSame(surfacelines, presentationObject.AvailablePipingSurfaceLines);
            Assert.AreSame(profiles, presentationObject.AvailablePipingSoilProfiles);
        }

        [Test]
        public void NotifyObservers_HasPipingCalculationAndObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();

            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles);
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

            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();

            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles);
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

            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();

            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles);
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
            // Setup
            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation
            {
                Output = new TestPipingOutput()
            };

            // Call
            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles);

            // Call
            presentationObject.ClearOutput();

            // Assert
            Assert.IsNull(presentationObject.WrappedData.Output);
        }
    }
}