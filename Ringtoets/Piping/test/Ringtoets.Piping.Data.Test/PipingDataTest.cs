using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test
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
            Assert.AreEqual("Piping", defaultConstructed.Name);

            Assert.IsInstanceOf<NormalDistribution>(defaultConstructed.PhreaticLevelExit);
            Assert.AreEqual(0, defaultConstructed.PhreaticLevelExit.Mean);
            Assert.AreEqual(1, defaultConstructed.PhreaticLevelExit.StandardDeviation);

            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.DampingFactorExit);
            Assert.AreEqual(1, defaultConstructed.DampingFactorExit.Mean);
            Assert.AreEqual(1, defaultConstructed.DampingFactorExit.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.ThicknessCoverageLayer);
            Assert.AreEqual(0, defaultConstructed.ThicknessCoverageLayer.Mean);
            Assert.AreEqual(1, defaultConstructed.ThicknessCoverageLayer.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.SeepageLength);
            Assert.AreEqual(0, defaultConstructed.SeepageLength.Mean);
            Assert.AreEqual(1, defaultConstructed.SeepageLength.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.Diameter70);
            Assert.AreEqual(0, defaultConstructed.Diameter70.Mean);
            Assert.AreEqual(1, defaultConstructed.Diameter70.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.DarcyPermeability);
            Assert.AreEqual(0, defaultConstructed.DarcyPermeability.Mean);
            Assert.AreEqual(1, defaultConstructed.DarcyPermeability.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.ThicknessAquiferLayer);
            Assert.AreEqual(0, defaultConstructed.ThicknessAquiferLayer.Mean);
            Assert.AreEqual(1, defaultConstructed.ThicknessAquiferLayer.StandardDeviation);

            Assert.AreEqual(0, defaultConstructed.PiezometricHeadExit);
            Assert.AreEqual(0, defaultConstructed.PiezometricHeadPolder);
            Assert.AreEqual(0, defaultConstructed.AssessmentLevel);
            Assert.IsNull(defaultConstructed.SurfaceLine);
            Assert.IsNull(defaultConstructed.SoilProfile);

            Assert.AreEqual(1.0, defaultConstructed.UpliftModelFactor);
            Assert.AreEqual(1, defaultConstructed.SellmeijerModelFactor);
            Assert.AreEqual(0.5, defaultConstructed.CriticalHeaveGradient);
            Assert.AreEqual(0.3, defaultConstructed.SellmeijerReductionFactor);
            Assert.AreEqual(9.81, defaultConstructed.Gravity);
            Assert.AreEqual(1.33e-6, defaultConstructed.WaterKinematicViscosity);
            Assert.AreEqual(10.0, defaultConstructed.WaterVolumetricWeight);
            Assert.AreEqual(16.5, defaultConstructed.SandParticlesVolumicWeight);
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
    }
}