using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Primitives;

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
            var piping = new Piping();

            // assert
            Assert.IsInstanceOf<FailureMechanismBase>(piping);
            Assert.IsInstanceOf<GeneralPipingInput>(piping.GeneralInput);
            CollectionAssert.IsEmpty(piping.Sections);
            CollectionAssert.IsEmpty(piping.SurfaceLines);
            Assert.IsInstanceOf<List<RingtoetsPipingSurfaceLine>>(piping.SurfaceLines);
            CollectionAssert.IsEmpty(piping.StochasticSoilModels);
            Assert.IsInstanceOf<ObservableList<StochasticSoilModel>>(piping.StochasticSoilModels);
            Assert.AreEqual("Berekeningen", piping.CalculationsGroup.Name);
            Assert.IsEmpty(piping.CalculationsGroup.Children);
        }

        [Test]
        public void Notify_SingleListenerAttached_ListenerIsNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var pipingFailureMechanism = new Piping();

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

            var pipingFailureMechanism = new Piping();

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

            var pipingFailureMechanism = new Piping();

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

            var pipingFailureMechanism = new Piping();

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

            var pipingFailureMechanism = new Piping();

            // Call & Assert
            pipingFailureMechanism.Detach(observer);
        }

        [Test]
        public void Calculations_AddPipingCalculation_ItemIsAddedToCollection()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            var failureMechanism = new Piping();

            // Call
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Assert
            CollectionAssert.Contains(failureMechanism.CalculationsGroup.Children, calculation);
        }

        [Test]
        public void Calculations_RemovePipingCalculation_ItemIsRemovedFromCollection()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            var failureMechanism = new Piping();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Call
            failureMechanism.CalculationsGroup.Children.Remove(calculation);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.CalculationsGroup.Children, calculation);
        }

        [Test]
        public void Calculations_AddPipingCalculationGroup_ItemIsAddedToCollection()
        {
            // Setup
            var folder = new PipingCalculationGroup();

            var failureMechanism = new Piping();

            // Call
            failureMechanism.CalculationsGroup.Children.Add(folder);

            // Assert
            CollectionAssert.Contains(failureMechanism.CalculationsGroup.Children, folder);
        }

        [Test]
        public void Calculations_RemovePipingCalculationGroup_ItemIsRemovedFromCollection()
        {
            // Setup
            var folder = new PipingCalculationGroup();

            var failureMechanism = new Piping();
            failureMechanism.CalculationsGroup.Children.Add(folder);

            // Call
            failureMechanism.CalculationsGroup.Children.Remove(folder);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.CalculationsGroup.Children, folder);
        }
    }
}