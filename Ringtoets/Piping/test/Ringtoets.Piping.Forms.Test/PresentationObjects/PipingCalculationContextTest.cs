using System;
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

            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles, pipingFailureMechanismMock);

            // Assert
            Assert.IsInstanceOf<IObservable>(presentationObject);
            Assert.IsInstanceOf<PipingContext<PipingCalculation>>(presentationObject);
            Assert.AreSame(calculation, presentationObject.WrappedData);
            Assert.AreSame(surfacelines, presentationObject.AvailablePipingSurfaceLines);
            Assert.AreSame(profiles, presentationObject.AvailablePipingSoilProfiles);
            Assert.AreSame(pipingFailureMechanismMock, presentationObject.PipingFailureMechanism);
        }

        [Test]
        public void ParameteredConstructor_PipingFailureMechanismIsNull_ThrowArgumentNullException()
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
            TestDelegate call = () => new PipingCalculationContext(calculation, surfacelines, profiles, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];
            Assert.AreEqual("Het piping faalmechanisme mag niet 'null' zijn.", customMessage);
        }

        [Test]
        public void NotifyObservers_HasPipingCalculationAndObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles, pipingFailureMechanismMock);
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

            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles, pipingFailureMechanismMock);
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

            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles, pipingFailureMechanismMock);
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

            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles, pipingFailureMechanismMock);

            // Call
            presentationObject.ClearOutput();

            // Assert
            Assert.IsNull(presentationObject.WrappedData.Output);
        }
    }
}