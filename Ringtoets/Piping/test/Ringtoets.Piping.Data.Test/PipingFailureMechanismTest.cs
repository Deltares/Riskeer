﻿using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;

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
            CollectionAssert.IsEmpty(piping.SurfaceLines);
            Assert.IsInstanceOf<ObservableList<RingtoetsPipingSurfaceLine>>(piping.SurfaceLines);
            CollectionAssert.IsEmpty(piping.SoilProfiles);
            Assert.AreEqual(1, piping.Calculations.Count);
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
    }
}