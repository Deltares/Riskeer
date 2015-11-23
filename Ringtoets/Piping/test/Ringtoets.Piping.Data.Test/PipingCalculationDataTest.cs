using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    public class PipingCalculationDataTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void DefaultConstructor_DefaultPropertyValuesAreSet()
        {
            // Call
            var defaultConstructed = new PipingCalculationData();

            // Assert
            Assert.AreEqual("Berekening", defaultConstructed.Name);

            Assert.AreEqual("Commentaar", defaultConstructed.Comments.Name);
            Assert.IsInstanceOf<PipingInputParameters>(defaultConstructed.InputParameters);

            Assert.IsFalse(defaultConstructed.HasOutput);
            Assert.IsNull(defaultConstructed.Output);
            Assert.AreEqual("Berekeningsverslag", defaultConstructed.CalculationReport.Name);
        }

        [Test]
        public void Notify_SingleListenerAttached_ListenerIsNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var pipingData = new PipingCalculationData();
            
            pipingData.Attach(observer);

            // Call & Assert
            pipingData.NotifyObservers();
        }

        [Test]
        public void Notify_SingleListenerAttachedAndDeattached_ListenerIsNotNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mockRepository.ReplayAll();

            var pipingData = new PipingCalculationData();
            
            pipingData.Attach(observer);
            pipingData.Detach(observer);

            // Call & Assert
            pipingData.NotifyObservers();
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

            var pipingData = new PipingCalculationData();

            pipingData.Attach(observerA);
            pipingData.Attach(observerB);

            // Call & Assert
            pipingData.NotifyObservers();
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

            var pipingData = new PipingCalculationData();

            pipingData.Attach(observerA);
            pipingData.Attach(observerB);
            pipingData.Detach(observerA);

            // Call & Assert
            pipingData.NotifyObservers();
        }

        [Test]
        public void Detach_DetachNonAttachedObserver_ThrowsNoException()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();

            var pipingData = new PipingCalculationData();

            // Call & Assert
            pipingData.Detach(observer);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var data = new PipingCalculationData
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
            var data = new PipingCalculationData
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
            var data = new PipingCalculationData
            {
                Output = new TestPipingOutput()
            };

            // Call & Assert
            Assert.IsTrue(data.HasOutput);
        }
    }
}