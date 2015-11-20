﻿using Core.Common.Base;
using NUnit.Framework;

using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingCalculationInputsTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var presentationObject = new PipingCalculationInputs();

            // Assert
            Assert.IsInstanceOf<IObservable>(presentationObject);
            Assert.IsNull(presentationObject.PipingData);
            CollectionAssert.IsEmpty(presentationObject.AvailablePipingSurfaceLines);
        }

        [Test]
        public void NotifyObservers_HasPipingDataAndObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var presentationObject = new PipingCalculationInputs
            {
                PipingData = new PipingCalculationData()
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

            var presentationObject = new PipingCalculationInputs
            {
                PipingData = new PipingCalculationData()
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

            var pipingData = new PipingCalculationData();
            var presentationObject = new PipingCalculationInputs
            {
                PipingData = pipingData
            };
            presentationObject.Attach(observer);

            // Call
            pipingData.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var inputs = new PipingCalculationInputs
            {
                PipingData = new PipingCalculationData
                {
                    Output = new TestPipingOutput()
                }
            };

            // Call
            inputs.ClearOutput();

            // Assert
            Assert.IsNull(inputs.PipingData.Output);
        }
    }
}