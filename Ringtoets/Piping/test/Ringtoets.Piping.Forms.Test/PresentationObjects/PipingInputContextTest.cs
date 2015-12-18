using System.Linq;

using Core.Common.Base;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingInputContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var pipingInput = new PipingInput();
            var surfaceLines = new[] { new RingtoetsPipingSurfaceLine() };
            var profiles = new[] { new TestPipingSoilProfile() };
            
            // Call
            var context = new PipingInputContext(pipingInput, surfaceLines, profiles);

            // Assert
            Assert.IsInstanceOf<PipingContext<PipingInput>>(context);
            Assert.AreSame(pipingInput, context.WrappedData);
            CollectionAssert.AreEqual(surfaceLines, context.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(profiles, context.AvailablePipingSoilProfiles);
        }

        [Test]
        public void NotifyObservers_HasPipingInputAndObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var presentationObject = new PipingInputContext(new PipingInput(),
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<PipingSoilProfile>());
            presentationObject.Attach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_HasPipingInputAndObserverDetached_NoCallsOnObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var presentationObject = new PipingInputContext(new PipingInput(),
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<PipingSoilProfile>());
            presentationObject.Attach(observer);
            presentationObject.Detach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect not calls on 'observer'
        }

        [Test]
        public void PipingInputNotifyObservers_AttachedOnPipingCalculationContext_ObserverNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingInput = new PipingInput();
            var presentationObject = new PipingInputContext(pipingInput,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<PipingSoilProfile>());
            presentationObject.Attach(observer);

            // Call
            pipingInput.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }
    }
}