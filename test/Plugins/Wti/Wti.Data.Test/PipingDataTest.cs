﻿using System;
using DelftTools.Shell.Core;
using NUnit.Framework;
using Rhino.Mocks;

namespace Wti.Data.Test
{
    public class PipingDataTest
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
            var defaultConstructed = new PipingData();

            // Assert
            Assert.AreEqual(0, defaultConstructed.CriticalHeaveGradient);
            Assert.AreEqual(0, defaultConstructed.UpliftModelFactor);
            Assert.AreEqual(0, defaultConstructed.PiezometricHeadExit);
            Assert.AreEqual(0, defaultConstructed.PiezometricHeadPolder);
            Assert.AreEqual(0, defaultConstructed.ThicknessCoverageLayer);
            Assert.AreEqual(0, defaultConstructed.PhreaticLevelExit);
            Assert.AreEqual(0, defaultConstructed.AssessmentLevel);
            Assert.AreEqual(0, defaultConstructed.SellmeijerModelFactor);
            Assert.AreEqual(0, defaultConstructed.SeepageLength);
            Assert.AreEqual(0, defaultConstructed.Diameter70);
            Assert.AreEqual(0, defaultConstructed.ThicknessAquiferLayer);
            Assert.AreEqual(0, defaultConstructed.DarcyPermeability);

            Assert.AreEqual(1.0, defaultConstructed.DampingFactorExit);
            Assert.AreEqual(0.3, defaultConstructed.SellmeijerReductionFactor);
            Assert.AreEqual(16.5, defaultConstructed.SandParticlesVolumicWeight);
            Assert.AreEqual(9.81, defaultConstructed.Gravity);
            Assert.AreEqual(1.33e-6, defaultConstructed.WaterKinematicViscosity);
            Assert.AreEqual(9.81, defaultConstructed.WaterVolumetricWeight);
            Assert.AreEqual(0.25, defaultConstructed.WhitesDragCoefficient);
            Assert.AreEqual(37, defaultConstructed.BeddingAngle);
            Assert.AreEqual(2.08e-4, defaultConstructed.MeanDiameter70);

            Assert.IsNull(defaultConstructed.Output);
        }

        [Test]
        public void Notify_SingleListenerAttached_ListenerIsNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var pipingData = new PipingData();
            
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

            var pipingData = new PipingData();
            
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

            var pipingData = new PipingData();

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

            var pipingData = new PipingData();

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

            var pipingData = new PipingData();

            // Call & Assert
            pipingData.Detach(observer);
        }

        [Test]
        public void Output_SetToNullWithListeners_ListenersNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var pipingData = new PipingData();
            pipingData.Attach(observer);

            // Call & Assert
            pipingData.Output = null;
        }

        [Test]
        public void Output_SetToNewOutputWithListeners_ListenersNotified()
        {
            // Setup
            var random = new Random(22);
            var pipingData = new PipingData();

            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            pipingData.Attach(observer);
            mockRepository.ReplayAll();

            // Call & Assert
            pipingData.Output = new PipingOutput(
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble()
            );
        }
    }
}