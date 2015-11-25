using System.Collections.Generic;

using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Common.Data;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var piping = new PipingFailureMechanism();

            // assert
            Assert.IsInstanceOf<IFailureMechanism>(piping);
            Assert.AreEqual("Vakindeling", piping.SectionDivisions.Name);
            CollectionAssert.IsEmpty(piping.SurfaceLines);
            Assert.IsInstanceOf<ObservableList<RingtoetsPipingSurfaceLine>>(piping.SurfaceLines);
            CollectionAssert.IsEmpty(piping.SoilProfiles);
            Assert.AreEqual(1, piping.Calculations.Count);
            Assert.AreEqual("Randvoorwaarden", piping.BoundaryConditions.Name);

            Assert.AreEqual("Oordeel", piping.AssessmentResult.Name);
        }

        [Test]
        public void Notify_SingleListenerAttached_ListenerIsNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();

            pipingFailureMechanism.Attach(observer);

            // Call & Assert
            pipingFailureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Notify_SingleListenerAttachedAndDeattached_ListenerIsNotNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mockRepository.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();

            pipingFailureMechanism.Attach(observer);
            pipingFailureMechanism.Detach(observer);

            // Call & Assert
            pipingFailureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
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

            var pipingFailureMechanism = new PipingFailureMechanism();

            pipingFailureMechanism.Attach(observerA);
            pipingFailureMechanism.Attach(observerB);

            // Call & Assert
            pipingFailureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
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

            var pipingFailureMechanism = new PipingFailureMechanism();

            pipingFailureMechanism.Attach(observerA);
            pipingFailureMechanism.Attach(observerB);
            pipingFailureMechanism.Detach(observerA);

            // Call & Assert
            pipingFailureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Detach_DetachNonAttachedObserver_ThrowsNoException()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();

            var pipingFailureMechanism = new PipingFailureMechanism();

            // Call & Assert
            pipingFailureMechanism.Detach(observer);
        }

        [Test]
        public void Calculations_AddPipingCalculation_ItemIsAddedToCollection()
        {
            // Setup
            var calculation = new PipingCalculation();

            var failureMechanism = new PipingFailureMechanism();

            // Call
            failureMechanism.Calculations.Add(calculation);

            // Assert
            CollectionAssert.Contains(failureMechanism.Calculations, calculation);
        }

        [Test]
        public void Calculations_RemovePipingCalculation_ItemIsRemovedFromCollection()
        {
            // Setup
            var calculation = new PipingCalculation();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Calculations.Add(calculation);

            // Call
            failureMechanism.Calculations.Remove(calculation);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.Calculations, calculation);
        }

        [Test]
        public void Calculations_AddPipingCalculationFolder_ItemIsAddedToCollection()
        {
            // Setup
            var folder = new PipingCalculationGroup();

            var failureMechanism = new PipingFailureMechanism();

            // Call
            failureMechanism.Calculations.Add(folder);

            // Assert
            CollectionAssert.Contains(failureMechanism.Calculations, folder);
        }

        [Test]
        public void Calculations_RemovePipingCalculationFolder_ItemIsRemovedFromCollection()
        {
            // Setup
            var folder = new PipingCalculationGroup();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Calculations.Add(folder);

            // Call
            failureMechanism.Calculations.Remove(folder);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.Calculations, folder);
        }
    }
}