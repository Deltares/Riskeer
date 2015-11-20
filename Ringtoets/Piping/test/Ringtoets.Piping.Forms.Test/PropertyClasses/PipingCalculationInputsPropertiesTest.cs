using System;
using Core.Common.Base;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;
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
            var pipingData = new PipingCalculationData
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

            var pipingData = new PipingCalculationData();
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

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            int numberProperties = 22;
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mocks.ReplayAll();

            var pipingData = new PipingCalculationData();
            pipingData.Attach(projectObserver);

            Random random = new Random(22);

            double waterVolumetricWeight = random.NextDouble();
            double upliftModelFactor = random.NextDouble();
            double piezometricHeadExit = random.NextDouble();
            double piezometricHeadPolder = random.NextDouble();
            double sellmeijerModelFactor = random.NextDouble();
            double sellmeijerReductionFactor = random.NextDouble();
            double sandParticlesVolumicWeight = random.NextDouble();
            double whitesDragCoefficient = random.NextDouble();
            double waterKinematicViscosity = random.NextDouble();
            double gravity = random.NextDouble();
            double meanDiameter70 = random.NextDouble();
            double beddingAngle = random.NextDouble();

            var dampingFactorExit = new LognormalDistribution();
            var phreaticLevelExit = new NormalDistribution();
            var thicknessCoverageLayer = new LognormalDistribution();
            var seepageLength = new LognormalDistribution();
            var diameter70 = new LognormalDistribution();
            var darcyPermeability = new LognormalDistribution();
            var thicknessAquiferLayer = new LognormalDistribution();

            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
            PipingSoilProfile soilProfile = new TestPipingSoilProfile();

            // Call
            new PipingCalculationInputsProperties
            {
                Data = new PipingCalculationInputs()
                {
                    PipingData = pipingData
                },
                Name = string.Empty,
                WaterVolumetricWeight = waterVolumetricWeight,
                UpliftModelFactor = upliftModelFactor,
                PiezometricHeadExit = piezometricHeadExit,
                DampingFactorExit = new LognormalDistributionDesignVariable(dampingFactorExit),
                PhreaticLevelExit = new NormalDistributionDesignVariable(phreaticLevelExit),
                PiezometricHeadPolder = piezometricHeadPolder,
                ThicknessCoverageLayer = new LognormalDistributionDesignVariable(thicknessCoverageLayer),
                SellmeijerModelFactor = sellmeijerModelFactor,
                SellmeijerReductionFactor = sellmeijerReductionFactor,
                SeepageLength = new LognormalDistributionDesignVariable(seepageLength),
                SandParticlesVolumicWeight = sandParticlesVolumicWeight,
                WhitesDragCoefficient = whitesDragCoefficient,
                Diameter70 = new LognormalDistributionDesignVariable(diameter70),
                DarcyPermeability = new LognormalDistributionDesignVariable(darcyPermeability),
                WaterKinematicViscosity = waterKinematicViscosity,
                Gravity = gravity,
                ThicknessAquiferLayer = new LognormalDistributionDesignVariable(thicknessAquiferLayer),
                MeanDiameter70 = meanDiameter70,
                BeddingAngle = beddingAngle,
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
            };

            // Assert
            Assert.AreEqual(string.Empty, pipingData.Name);
            Assert.AreEqual(waterVolumetricWeight, pipingData.WaterVolumetricWeight);
            Assert.AreEqual(upliftModelFactor, pipingData.UpliftModelFactor);
            Assert.AreEqual(piezometricHeadExit, pipingData.PiezometricHeadExit);
            Assert.AreEqual(dampingFactorExit, pipingData.DampingFactorExit);
            Assert.AreEqual(phreaticLevelExit, pipingData.PhreaticLevelExit);
            Assert.AreEqual(piezometricHeadPolder, pipingData.PiezometricHeadPolder);
            Assert.AreEqual(thicknessCoverageLayer, pipingData.ThicknessCoverageLayer);
            Assert.AreEqual(sellmeijerModelFactor, pipingData.SellmeijerModelFactor);
            Assert.AreEqual(sellmeijerReductionFactor, pipingData.SellmeijerReductionFactor);
            Assert.AreEqual(seepageLength, pipingData.SeepageLength);
            Assert.AreEqual(sandParticlesVolumicWeight, pipingData.SandParticlesVolumicWeight);
            Assert.AreEqual(whitesDragCoefficient, pipingData.WhitesDragCoefficient);
            Assert.AreEqual(diameter70, pipingData.Diameter70);
            Assert.AreEqual(darcyPermeability, pipingData.DarcyPermeability);
            Assert.AreEqual(waterKinematicViscosity, pipingData.WaterKinematicViscosity);
            Assert.AreEqual(gravity, pipingData.Gravity);
            Assert.AreEqual(thicknessAquiferLayer, pipingData.ThicknessAquiferLayer);
            Assert.AreEqual(meanDiameter70, pipingData.MeanDiameter70);
            Assert.AreEqual(beddingAngle, pipingData.BeddingAngle);
            Assert.AreEqual(surfaceLine, pipingData.SurfaceLine);
            Assert.AreEqual(soilProfile, pipingData.SoilProfile);

            mocks.VerifyAll();
        }
    }
}