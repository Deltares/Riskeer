using System;
using DelftTools.Shell.Core;
using NUnit.Framework;

namespace Wti.Data.Test
{
    public class PipingDataTest
    {
        [Test]
        public void DefaultConstructor_DefaultPropertyValuesAreSet()
        {
            // call
            var defaultConstructed = new PipingData();

            // assert
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
            // setup
            var called = false;
            var observer = new PipingDataTestObserver
            {
                OnUpdate = () => called = true
            };

            var pipingData = new PipingData();
            
            pipingData.Attach(observer);

            // call
            pipingData.NotifyObservers();

            // assert
            Assert.IsTrue(called);
        }

        [Test]
        public void Notify_SingleListenerAttachedAndDeattached_ListenerIsNotNotified()
        {
            // setup
            var called = false;
            var observer = new PipingDataTestObserver
            {
                OnUpdate = () => called = true
            };

            var pipingData = new PipingData();
            
            pipingData.Attach(observer);
            pipingData.Detach(observer);

            // call
            pipingData.NotifyObservers();

            // assert
            Assert.IsFalse(called);
        }

        [Test]
        public void Notify_TwoListenersAttached_BothAreNotified()
        {
            // setup
            var calledTimes = 0;
            var observerA = new PipingDataTestObserver
            {
                OnUpdate = () => calledTimes++
            };
            var observerB = new PipingDataTestObserver
            {
                OnUpdate = () => calledTimes++
            };

            var pipingData = new PipingData();

            pipingData.Attach(observerA);
            pipingData.Attach(observerB);

            // call
            pipingData.NotifyObservers();

            // assert
            Assert.AreEqual(2, calledTimes);
        }

        [Test]
        public void Notify_TwoListenersAttachedOneDetached_InvokedOnce()
        {
            // setup
            var calledTimes = 0;
            var observerA = new PipingDataTestObserver
            {
                OnUpdate = () => calledTimes++
            };
            var observerB = new PipingDataTestObserver
            {
                OnUpdate = () => calledTimes++
            };

            var pipingData = new PipingData();

            pipingData.Attach(observerA);
            pipingData.Attach(observerB);
            pipingData.Detach(observerA);

            // call
            pipingData.NotifyObservers();

            // assert
            Assert.AreEqual(1, calledTimes);
        }

        [Test]
        public void Detach_DetachNonAttachedObserver_ThrowsNoException()
        {
            // setup
            var observer = new PipingDataTestObserver();

            var pipingData = new PipingData();

            // call & assert
            pipingData.Detach(observer);
        }

        [Test]
        public void Output_SetToNullWithListeners_ListenersNotified()
        {
            // Setup
            var called = false;
            var observer = new PipingDataTestObserver
            {
                OnUpdate = () => called = true
            };
            var pipingData = new PipingData();
            pipingData.Attach(observer);

            // Call
            pipingData.Output = null;

            // Assert
            Assert.IsTrue(called);
        }

        [Test]
        public void Output_SetToNewOutputWithListeners_ListenersNotified()
        {
            // Setup
            var random = new Random(22);
            var called = false;
            var observer = new PipingDataTestObserver
            {
                OnUpdate = () => called = true
            };
            var pipingData = new PipingData();
            pipingData.Attach(observer);

            // Call
            pipingData.Output = new PipingOutput(
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble()
            );

            // Assert
            Assert.IsTrue(called);
        }
        
        class PipingDataTestObserver : IObserver
        {
            public Action OnUpdate;

            public void UpdateObserver()
            {
                if (OnUpdate != null)
                {
                    OnUpdate();
                }
            }
        } 
    }
}