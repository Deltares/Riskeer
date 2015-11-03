using System;
using Core.Common.Base;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
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
            var random = new Random(22);

            const string name = "<very cool name>";
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var soilProfile = new PipingSoilProfile(String.Empty,random.NextDouble(), new []
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            });
            var pipingData = new PipingData
            {
                Name = name,
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };

            var properties = new PipingCalculationInputsProperties
            {
                Data = new PipingCalculationInputs
                {
                    PipingData = pipingData
                }
            };

            // Call & Assert
            Assert.AreEqual(name, properties.Name);

            Assert.AreEqual(pipingData.PhreaticLevelExit.Mean, properties.PhreaticLevelExit.Mean);
            Assert.AreEqual(pipingData.PhreaticLevelExit.StandardDeviation, properties.PhreaticLevelExit.StandardDeviation);

            Assert.AreEqual(pipingData.DampingFactorExit.Mean, properties.DampingFactorExit.Mean);
            Assert.AreEqual(pipingData.DampingFactorExit.StandardDeviation, properties.DampingFactorExit.StandardDeviation);

            Assert.AreEqual(pipingData.ThicknessCoverageLayer.Mean, properties.ThicknessCoverageLayer.Mean);
            Assert.AreEqual(pipingData.ThicknessCoverageLayer.StandardDeviation, properties.ThicknessCoverageLayer.StandardDeviation);

            Assert.AreEqual(pipingData.CriticalHeaveGradient.Mean, properties.CriticalHeaveGradient.Mean);
            Assert.AreEqual(pipingData.CriticalHeaveGradient.StandardDeviation, properties.CriticalHeaveGradient.StandardDeviation);

            Assert.AreEqual(pipingData.SeepageLength.Mean, properties.SeepageLength.Mean);
            Assert.AreEqual(pipingData.SeepageLength.StandardDeviation, properties.SeepageLength.StandardDeviation);

            Assert.AreEqual(pipingData.Diameter70.Mean, properties.Diameter70.Mean);
            Assert.AreEqual(pipingData.Diameter70.StandardDeviation, properties.Diameter70.StandardDeviation);

            Assert.AreEqual(pipingData.DarcyPermeability.Mean, properties.DarcyPermeability.Mean);
            Assert.AreEqual(pipingData.DarcyPermeability.StandardDeviation, properties.DarcyPermeability.StandardDeviation);

            Assert.AreEqual(pipingData.ThicknessAquiferLayer.Mean, properties.ThicknessAquiferLayer.Mean);
            Assert.AreEqual(pipingData.ThicknessAquiferLayer.StandardDeviation, properties.ThicknessAquiferLayer.StandardDeviation);

            Assert.AreEqual(pipingData.SandParticlesVolumicWeight.Mean, properties.SandParticlesVolumicWeight.Mean);
            Assert.AreEqual(pipingData.SandParticlesVolumicWeight.StandardDeviation, properties.SandParticlesVolumicWeight.StandardDeviation);
            Assert.AreEqual(pipingData.SandParticlesVolumicWeight.Shift, properties.SandParticlesVolumicWeight.Shift);

            Assert.AreEqual(0, properties.UpliftModelFactor);
            Assert.AreEqual(0, properties.PiezometricHeadExit);
            Assert.AreEqual(0, properties.PiezometricHeadPolder);
            Assert.AreEqual(0, properties.AssessmentLevel);
            Assert.AreEqual(0, properties.SellmeijerModelFactor);

            Assert.AreEqual(0.3, properties.SellmeijerReductionFactor);
            Assert.AreEqual(9.81, properties.Gravity);
            Assert.AreEqual(1.33e-6, properties.WaterKinematicViscosity);
            Assert.AreEqual(9.81, properties.WaterVolumetricWeight);
            Assert.AreEqual(0.25, properties.WhitesDragCoefficient);
            Assert.AreEqual(37, properties.BeddingAngle);
            Assert.AreEqual(2.08e-4, properties.MeanDiameter70);
            Assert.AreSame(surfaceLine, properties.SurfaceLine);
            Assert.AreSame(soilProfile, properties.SoilProfile);
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