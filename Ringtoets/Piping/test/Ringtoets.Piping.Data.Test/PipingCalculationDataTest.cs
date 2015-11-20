using System;

using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data.Probabilistics;

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

            Assert.IsInstanceOf<NormalDistribution>(defaultConstructed.PhreaticLevelExit);
            Assert.AreEqual(0, defaultConstructed.PhreaticLevelExit.Mean);
            Assert.AreEqual(1, defaultConstructed.PhreaticLevelExit.StandardDeviation);

            double defaultLogNormalMean = Math.Exp(-0.5);
            double defaultLogNormalStandardDev = Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1));
            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.DampingFactorExit);
            Assert.AreEqual(1, defaultConstructed.DampingFactorExit.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, defaultConstructed.DampingFactorExit.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.ThicknessCoverageLayer);
            Assert.AreEqual(defaultLogNormalMean, defaultConstructed.ThicknessCoverageLayer.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, defaultConstructed.ThicknessCoverageLayer.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.SeepageLength);
            Assert.AreEqual(defaultLogNormalMean, defaultConstructed.SeepageLength.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, defaultConstructed.SeepageLength.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.Diameter70);
            Assert.AreEqual(defaultLogNormalMean, defaultConstructed.Diameter70.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, defaultConstructed.Diameter70.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.DarcyPermeability);
            Assert.AreEqual(defaultLogNormalMean, defaultConstructed.DarcyPermeability.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, defaultConstructed.DarcyPermeability.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(defaultConstructed.ThicknessAquiferLayer);
            Assert.AreEqual(defaultLogNormalMean, defaultConstructed.ThicknessAquiferLayer.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, defaultConstructed.ThicknessAquiferLayer.StandardDeviation);

            Assert.AreEqual(0, defaultConstructed.PiezometricHeadExit);
            Assert.AreEqual(0, defaultConstructed.PiezometricHeadPolder);
            Assert.AreEqual(0, defaultConstructed.AssessmentLevel);
            Assert.IsNull(defaultConstructed.SurfaceLine);
            Assert.IsNull(defaultConstructed.SoilProfile);

            Assert.AreEqual(1.0, defaultConstructed.UpliftModelFactor);
            Assert.AreEqual(1, defaultConstructed.SellmeijerModelFactor);
            Assert.AreEqual(0.3, defaultConstructed.CriticalHeaveGradient);
            Assert.AreEqual(0.3, defaultConstructed.SellmeijerReductionFactor);
            Assert.AreEqual(9.81, defaultConstructed.Gravity);
            Assert.AreEqual(1.33e-6, defaultConstructed.WaterKinematicViscosity);
            Assert.AreEqual(10.0, defaultConstructed.WaterVolumetricWeight);
            Assert.AreEqual(16.5, defaultConstructed.SandParticlesVolumicWeight);
            Assert.AreEqual(0.25, defaultConstructed.WhitesDragCoefficient);
            Assert.AreEqual(37, defaultConstructed.BeddingAngle);
            Assert.AreEqual(2.08e-4, defaultConstructed.MeanDiameter70);

            Assert.AreEqual("Commentaar", defaultConstructed.Comments.Name);
            Assert.AreEqual("Berekeningsverslag", defaultConstructed.CalculationReport.Name);

            Assert.IsNull(defaultConstructed.Output);
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