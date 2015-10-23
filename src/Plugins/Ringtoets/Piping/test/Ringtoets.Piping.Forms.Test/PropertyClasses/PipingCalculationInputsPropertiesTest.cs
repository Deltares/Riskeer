using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Wti.Data;
using Wti.Forms.PresentationObjects;
using Wti.Forms.PropertyClasses;

namespace Wti.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingCalculationInputsPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingCalculationInputsProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingCalculationInputs>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var pipingData = new PipingData
            {
                SurfaceLine = surfaceLine
            };

            var properties = new PipingCalculationInputsProperties
            {
                Data = new PipingCalculationInputs
                {
                    PipingData = pipingData
                }
            };

            // Call & Assert
            Assert.AreEqual(0, properties.CriticalHeaveGradient);
            Assert.AreEqual(0, properties.UpliftModelFactor);
            Assert.AreEqual(0, properties.PiezometricHeadExit);
            Assert.AreEqual(0, properties.PiezometricHeadPolder);
            Assert.AreEqual(0, properties.ThicknessCoverageLayer);
            Assert.AreEqual(0, properties.PhreaticLevelExit);
            Assert.AreEqual(0, properties.AssessmentLevel);
            Assert.AreEqual(0, properties.SellmeijerModelFactor);
            Assert.AreEqual(0, properties.SeepageLength);
            Assert.AreEqual(0, properties.Diameter70);
            Assert.AreEqual(0, properties.ThicknessAquiferLayer);
            Assert.AreEqual(0, properties.DarcyPermeability);

            Assert.AreEqual(1.0, properties.DampingFactorExit);
            Assert.AreEqual(0.3, properties.SellmeijerReductionFactor);
            Assert.AreEqual(16.5, properties.SandParticlesVolumicWeight);
            Assert.AreEqual(9.81, properties.Gravity);
            Assert.AreEqual(1.33e-6, properties.WaterKinematicViscosity);
            Assert.AreEqual(9.81, properties.WaterVolumetricWeight);
            Assert.AreEqual(0.25, properties.WhitesDragCoefficient);
            Assert.AreEqual(37, properties.BeddingAngle);
            Assert.AreEqual(2.08e-4, properties.MeanDiameter70);
            Assert.AreSame(surfaceLine, properties.SurfaceLine);
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingData = new PipingData();
            pipingData.Attach(projectObserver);

            var properties = new PipingCalculationInputsProperties
            {
                Data = new PipingCalculationInputs
                {
                    PipingData = pipingData
                }
            };

            // Call & Assert
            const double assessmentLevel = 0.12;
            properties.AssessmentLevel = assessmentLevel;
            Assert.AreEqual(assessmentLevel, pipingData.AssessmentLevel);
            mocks.VerifyAll();
        }
    }
}