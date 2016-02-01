using System;
using System.Linq;

using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.Gui.PropertyBag;

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
    public class PipingInputContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingInputContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingInputContext>>(properties);
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
            var inputParameters = new PipingInput
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>())
            };

            // Call & Assert
            Assert.AreSame(inputParameters.PhreaticLevelExit, properties.PhreaticLevelExitHeave.Distribution);
            Assert.AreSame(inputParameters.PhreaticLevelExit, properties.PhreaticLevelExitSellmeijer.Distribution);
            Assert.AreSame(inputParameters.PhreaticLevelExit, properties.PhreaticLevelExitUplift.Distribution);
            Assert.AreSame(inputParameters.DampingFactorExit, properties.DampingFactorExitUplift.Distribution);
            Assert.AreSame(inputParameters.DampingFactorExit, properties.DampingFactorExitHeave.Distribution);
            Assert.AreSame(inputParameters.ThicknessCoverageLayer, properties.ThicknessCoverageLayerHeave.Distribution);
            Assert.AreSame(inputParameters.ThicknessCoverageLayer, properties.ThicknessCoverageLayerSellmeijer.Distribution);
            Assert.AreSame(inputParameters.SeepageLength, properties.SeepageLength.Distribution);
            Assert.AreSame(inputParameters.Diameter70, properties.Diameter70.Distribution);
            Assert.AreSame(inputParameters.DarcyPermeability, properties.DarcyPermeability.Distribution);
            Assert.AreSame(inputParameters.ThicknessAquiferLayer, properties.ThicknessAquiferLayer.Distribution);

            Assert.AreEqual(inputParameters.UpliftModelFactor, properties.UpliftModelFactor);
            Assert.AreEqual(inputParameters.PiezometricHeadExit, properties.PiezometricHeadExitHeave);
            Assert.AreEqual(inputParameters.PiezometricHeadExit, properties.PiezometricHeadExitUplift);
            Assert.AreEqual(inputParameters.PiezometricHeadPolder, properties.PiezometricHeadPolderHeave);
            Assert.AreEqual(inputParameters.PiezometricHeadPolder, properties.PiezometricHeadPolderUplift);
            Assert.AreEqual(inputParameters.AssessmentLevel, properties.AssessmentLevelSellmeijer);
            Assert.AreEqual(inputParameters.AssessmentLevel, properties.AssessmentLevelUplift);
            Assert.AreEqual(inputParameters.SellmeijerModelFactor, properties.SellmeijerModelFactor);

            Assert.AreEqual(inputParameters.CriticalHeaveGradient, properties.CriticalHeaveGradient);
            Assert.AreEqual(inputParameters.SellmeijerReductionFactor, properties.SellmeijerReductionFactor);
            Assert.AreEqual(inputParameters.Gravity, properties.Gravity);
            Assert.AreEqual(inputParameters.WaterKinematicViscosity, properties.WaterKinematicViscosity);
            Assert.AreEqual(inputParameters.WaterVolumetricWeight, properties.WaterVolumetricWeightSellmeijer);
            Assert.AreEqual(inputParameters.WaterVolumetricWeight, properties.WaterVolumetricWeightUplift);
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

            var inputParameters = new PipingInput();
            inputParameters.Attach(projectObserver);

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>())
            };

            // Call & Assert
            const double assessmentLevel = 0.12;
            properties.AssessmentLevelSellmeijer = assessmentLevel;
            Assert.AreEqual(assessmentLevel, inputParameters.AssessmentLevel);
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

            var inputParameters = new PipingInput();
            inputParameters.Attach(projectObserver);

            Random random = new Random(22);

            double assessmentLevel = random.NextDouble();
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
            new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>()),
                AssessmentLevelSellmeijer = assessmentLevel,
                WaterVolumetricWeightUplift = waterVolumetricWeight,
                UpliftModelFactor = upliftModelFactor,
                PiezometricHeadExitUplift = piezometricHeadExit,
                DampingFactorExitHeave = new LognormalDistributionDesignVariable(dampingFactorExit),
                PhreaticLevelExitHeave = new NormalDistributionDesignVariable(phreaticLevelExit),
                PiezometricHeadPolderHeave = piezometricHeadPolder,
                ThicknessCoverageLayerSellmeijer = new LognormalDistributionDesignVariable(thicknessCoverageLayer),
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
            Assert.AreEqual(assessmentLevel, inputParameters.AssessmentLevel);
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