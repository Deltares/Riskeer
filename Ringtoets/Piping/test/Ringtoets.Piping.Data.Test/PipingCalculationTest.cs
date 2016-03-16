using System;

using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    public class PipingCalculationTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();
            var semiProbabilisticInputParameters = new SemiProbabilisticPipingInput();

            // Call
            var calculation = new PipingCalculation(generalInputParameters, semiProbabilisticInputParameters);

            // Assert
            Assert.IsInstanceOf<IPipingCalculationItem>(calculation);

            Assert.AreEqual("Nieuwe berekening", calculation.Name);

            Assert.AreEqual("Commentaar", calculation.Comments.Name);
            Assert.IsInstanceOf<PipingInput>(calculation.InputParameters);

            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.SemiProbabilisticOutput);
        }

        [Test]
        public void Constructor_GeneralPipingInputIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingCalculation(null, new SemiProbabilisticPipingInput());

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Constructor_SemiProbabilisticPipingInputIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingCalculation(new GeneralPipingInput(), null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Notify_SingleListenerAttached_ListenerIsNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            
            calculation.Attach(observer);

            // Call & Assert
            calculation.NotifyObservers();
        }

        [Test]
        public void Notify_SingleListenerAttachedAndDeattached_ListenerIsNotNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mockRepository.ReplayAll();

            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            
            calculation.Attach(observer);
            calculation.Detach(observer);

            // Call & Assert
            calculation.NotifyObservers();
        }

        [Test]
        public void Notify_TwoListenersAttached_BothAreNotified()
        {
            // Setup
            var observerA = mockRepository.StrictMock<IObserver>();
            observerA.Expect(o => o.UpdateObserver());

            var observerB = mockRepository.StrictMock<IObserver>();
            observerB.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            calculation.Attach(observerA);
            calculation.Attach(observerB);

            // Call & Assert
            calculation.NotifyObservers();
        }

        [Test]
        public void Notify_TwoListenersAttachedOneDetached_InvokedOnce()
        {
            // Setup
            var observerA = mockRepository.StrictMock<IObserver>();
            observerA.Expect(o => o.UpdateObserver()).Repeat.Never();

            var observerB = mockRepository.StrictMock<IObserver>();
            observerB.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            calculation.Attach(observerA);
            calculation.Attach(observerB);
            calculation.Detach(observerA);

            // Call & Assert
            calculation.NotifyObservers();
        }

        [Test]
        public void Detach_DetachNonAttachedObserver_ThrowsNoException()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();

            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            // Call & Assert
            calculation.Detach(observer);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var data = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                Output = new TestPipingOutput()
            };

            // Call
            data.ClearOutput();

            // Assert
            Assert.IsNull(data.Output);
        }

        [Test]
        public void HasOutput_OutputNull_ReturnsFalse()
        {
            // Setup
            var data = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                Output = null
            };

            // Call & Assert
            Assert.IsFalse(data.HasOutput);
        }

        [Test]
        public void HasOutput_OutputSet_ReturnsTrue()
        {
            // Setup
            var data = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                Output = new TestPipingOutput()
            };

            // Call & Assert
            Assert.IsTrue(data.HasOutput);
        }
    }
}