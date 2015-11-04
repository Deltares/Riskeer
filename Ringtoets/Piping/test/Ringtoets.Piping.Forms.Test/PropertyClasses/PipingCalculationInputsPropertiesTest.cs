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
                    IsAquifer = 1.0
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

            Assert.AreSame(pipingData.PhreaticLevelExit, properties.PhreaticLevelExit.Distribution);
            Assert.AreSame(pipingData.DampingFactorExit, properties.DampingFactorExit.Distribution);
            Assert.AreSame(pipingData.ThicknessCoverageLayer, properties.ThicknessCoverageLayer.Distribution);
            Assert.AreSame(pipingData.SeepageLength, properties.SeepageLength.Distribution);
            Assert.AreSame(pipingData.Diameter70, properties.Diameter70.Distribution);
            Assert.AreSame(pipingData.DarcyPermeability, properties.DarcyPermeability.Distribution);
            Assert.AreSame(pipingData.ThicknessAquiferLayer, properties.ThicknessAquiferLayer.Distribution);
            
            Assert.AreEqual(pipingData.UpliftModelFactor, properties.UpliftModelFactor);
            Assert.AreEqual(pipingData.PiezometricHeadExit, properties.PiezometricHeadExit);
            Assert.AreEqual(pipingData.PiezometricHeadPolder, properties.PiezometricHeadPolder);
            Assert.AreEqual(pipingData.AssessmentLevel, properties.AssessmentLevel);
            Assert.AreEqual(pipingData.SellmeijerModelFactor, properties.SellmeijerModelFactor);

            Assert.AreEqual(pipingData.CriticalHeaveGradient, properties.CriticalHeaveGradient);
            Assert.AreEqual(pipingData.SellmeijerReductionFactor, properties.SellmeijerReductionFactor);
            Assert.AreEqual(pipingData.Gravity, properties.Gravity);
            Assert.AreEqual(pipingData.WaterKinematicViscosity, properties.WaterKinematicViscosity);
            Assert.AreEqual(pipingData.WaterVolumetricWeight, properties.WaterVolumetricWeight);
            Assert.AreEqual(pipingData.SandParticlesVolumicWeight, properties.SandParticlesVolumicWeight);
            Assert.AreEqual(pipingData.WhitesDragCoefficient, properties.WhitesDragCoefficient);
            Assert.AreEqual(pipingData.BeddingAngle, properties.BeddingAngle);
            Assert.AreEqual(pipingData.MeanDiameter70, properties.MeanDiameter70);
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