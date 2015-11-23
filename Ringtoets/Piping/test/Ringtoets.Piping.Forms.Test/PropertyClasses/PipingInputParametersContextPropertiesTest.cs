﻿using System;

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
    public class PipingInputParametersContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingInputParametersContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingInputParametersContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(22);

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var soilProfile = new PipingSoilProfile(String.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            });
            var inputParameters = new PipingInputParameters
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };

            var properties = new PipingInputParametersContextProperties
            {
                Data = new PipingInputParametersContext{WrappedPipingInputParameters = inputParameters}
            };

            // Call & Assert
            Assert.AreSame(inputParameters.PhreaticLevelExit, properties.PhreaticLevelExit.Distribution);
            Assert.AreSame(inputParameters.DampingFactorExit, properties.DampingFactorExit.Distribution);
            Assert.AreSame(inputParameters.ThicknessCoverageLayer, properties.ThicknessCoverageLayer.Distribution);
            Assert.AreSame(inputParameters.SeepageLength, properties.SeepageLength.Distribution);
            Assert.AreSame(inputParameters.Diameter70, properties.Diameter70.Distribution);
            Assert.AreSame(inputParameters.DarcyPermeability, properties.DarcyPermeability.Distribution);
            Assert.AreSame(inputParameters.ThicknessAquiferLayer, properties.ThicknessAquiferLayer.Distribution);

            Assert.AreEqual(inputParameters.UpliftModelFactor, properties.UpliftModelFactor);
            Assert.AreEqual(inputParameters.PiezometricHeadExit, properties.PiezometricHeadExit);
            Assert.AreEqual(inputParameters.PiezometricHeadPolder, properties.PiezometricHeadPolder);
            Assert.AreEqual(inputParameters.AssessmentLevel, properties.AssessmentLevel);
            Assert.AreEqual(inputParameters.SellmeijerModelFactor, properties.SellmeijerModelFactor);

            Assert.AreEqual(inputParameters.CriticalHeaveGradient, properties.CriticalHeaveGradient);
            Assert.AreEqual(inputParameters.SellmeijerReductionFactor, properties.SellmeijerReductionFactor);
            Assert.AreEqual(inputParameters.Gravity, properties.Gravity);
            Assert.AreEqual(inputParameters.WaterKinematicViscosity, properties.WaterKinematicViscosity);
            Assert.AreEqual(inputParameters.WaterVolumetricWeight, properties.WaterVolumetricWeight);
            Assert.AreEqual(inputParameters.SandParticlesVolumicWeight, properties.SandParticlesVolumicWeight);
            Assert.AreEqual(inputParameters.WhitesDragCoefficient, properties.WhitesDragCoefficient);
            Assert.AreEqual(inputParameters.BeddingAngle, properties.BeddingAngle);
            Assert.AreEqual(inputParameters.MeanDiameter70, properties.MeanDiameter70);
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

            var inputParameters = new PipingInputParameters();
            inputParameters.Attach(projectObserver);

            var properties = new PipingInputParametersContextProperties
            {
                Data = new PipingInputParametersContext{WrappedPipingInputParameters = inputParameters}
            };

            // Call & Assert
            const double assessmentLevel = 0.12;
            properties.AssessmentLevel = assessmentLevel;
            Assert.AreEqual(assessmentLevel, inputParameters.AssessmentLevel);
            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            int numberProperties = 21;
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mocks.ReplayAll();

            var inputParameters = new PipingInputParameters();
            inputParameters.Attach(projectObserver);

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
            new PipingInputParametersContextProperties
            {
                Data = new PipingInputParametersContext{WrappedPipingInputParameters = inputParameters},
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
            Assert.AreEqual(waterVolumetricWeight, inputParameters.WaterVolumetricWeight);
            Assert.AreEqual(upliftModelFactor, inputParameters.UpliftModelFactor);
            Assert.AreEqual(piezometricHeadExit, inputParameters.PiezometricHeadExit);
            Assert.AreEqual(dampingFactorExit, inputParameters.DampingFactorExit);
            Assert.AreEqual(phreaticLevelExit, inputParameters.PhreaticLevelExit);
            Assert.AreEqual(piezometricHeadPolder, inputParameters.PiezometricHeadPolder);
            Assert.AreEqual(thicknessCoverageLayer, inputParameters.ThicknessCoverageLayer);
            Assert.AreEqual(sellmeijerModelFactor, inputParameters.SellmeijerModelFactor);
            Assert.AreEqual(sellmeijerReductionFactor, inputParameters.SellmeijerReductionFactor);
            Assert.AreEqual(seepageLength, inputParameters.SeepageLength);
            Assert.AreEqual(sandParticlesVolumicWeight, inputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(whitesDragCoefficient, inputParameters.WhitesDragCoefficient);
            Assert.AreEqual(diameter70, inputParameters.Diameter70);
            Assert.AreEqual(darcyPermeability, inputParameters.DarcyPermeability);
            Assert.AreEqual(waterKinematicViscosity, inputParameters.WaterKinematicViscosity);
            Assert.AreEqual(gravity, inputParameters.Gravity);
            Assert.AreEqual(thicknessAquiferLayer, inputParameters.ThicknessAquiferLayer);
            Assert.AreEqual(meanDiameter70, inputParameters.MeanDiameter70);
            Assert.AreEqual(beddingAngle, inputParameters.BeddingAngle);
            Assert.AreEqual(surfaceLine, inputParameters.SurfaceLine);
            Assert.AreEqual(soilProfile, inputParameters.SoilProfile);

            mocks.VerifyAll();
        }
    }
}