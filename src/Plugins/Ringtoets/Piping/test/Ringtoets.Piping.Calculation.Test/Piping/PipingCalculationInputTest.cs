﻿using System;
using NUnit.Framework;

using Ringtoets.Piping.Calculation.Piping;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.Test.Piping
{
    public class PipingCalculationInputTest
    {
        [Test]
        public void GivenSomeRandomValues_WhenPipingCalculationInputConstructedFromInput_ThenPropertiesAreSet()
        {
            var random = new Random(22);

            double volumetricWeightOfWaterValue = random.NextDouble();
            double modelFactorUpliftValue = random.NextDouble();
            double hRiverValue = random.NextDouble();
            double phiExitValue = random.NextDouble();
            double rExitValue = random.NextDouble();
            double hExitValue = random.NextDouble();
            double phiPolderValue = random.NextDouble();
            double ichValue = random.NextDouble();
            double dTotalValue = random.NextDouble();
            double sellmeijerModelFactorValue = random.NextDouble();
            double reductionFactorValue = random.NextDouble();
            double seepageLengthValue = random.NextDouble();
            double sandParticlesVolumicWeightValue = random.NextDouble();
            double whitesDragCoefficientValue = random.NextDouble();
            double diameter70Value = random.NextDouble();
            double darcyPermeabilityValue = random.NextDouble();
            double waterKinematicViscosityValue = random.NextDouble();
            double gravityValue = random.NextDouble();
            double thicknessAquiferLayerValue = random.NextDouble();
            double meanDiameter70Value = random.NextDouble();
            double beddingAngleValue = random.NextDouble();
            double exitPointXCoordinate = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var soilProfile = new PipingSoilProfile(string.Empty, random.NextDouble(), new []
            {
                new PipingSoilLayer(random.NextDouble()), 
            });

            var input = new PipingCalculationInput(
                volumetricWeightOfWaterValue,
                modelFactorUpliftValue,
                hRiverValue,
                phiExitValue,
                rExitValue,
                hExitValue,
                phiPolderValue,
                ichValue,
                dTotalValue,
                sellmeijerModelFactorValue,
                reductionFactorValue,
                seepageLengthValue,
                sandParticlesVolumicWeightValue,
                whitesDragCoefficientValue,
                diameter70Value,
                darcyPermeabilityValue,
                waterKinematicViscosityValue,
                gravityValue,
                thicknessAquiferLayerValue,
                meanDiameter70Value,
                beddingAngleValue,
                exitPointXCoordinate,
                surfaceLine, 
                soilProfile);

            Assert.AreEqual(volumetricWeightOfWaterValue, input.WaterVolumetricWeight);
            Assert.AreEqual(modelFactorUpliftValue, input.UpliftModelFactor);
            Assert.AreEqual(hRiverValue, input.AssessmentLevel);
            Assert.AreEqual(phiExitValue, input.PiezometricHeadExit);
            Assert.AreEqual(rExitValue, input.DampingFactorExit);
            Assert.AreEqual(hExitValue, input.PhreaticLevelExit);
            Assert.AreEqual(phiPolderValue, input.PiezometricHeadPolder);
            Assert.AreEqual(ichValue, input.CriticalHeaveGradient);
            Assert.AreEqual(dTotalValue, input.ThicknessCoverageLayer);
            Assert.AreEqual(sellmeijerModelFactorValue, input.SellmeijerModelFactor);
            Assert.AreEqual(reductionFactorValue, input.SellmeijerReductionFactor);
            Assert.AreEqual(seepageLengthValue, input.SeepageLength);
            Assert.AreEqual(sandParticlesVolumicWeightValue, input.SandParticlesVolumicWeight);
            Assert.AreEqual(whitesDragCoefficientValue, input.WhitesDragCoefficient);
            Assert.AreEqual(diameter70Value, input.Diameter70);
            Assert.AreEqual(darcyPermeabilityValue, input.DarcyPermeability);
            Assert.AreEqual(waterKinematicViscosityValue, input.WaterKinematicViscosity);
            Assert.AreEqual(gravityValue, input.Gravity);
            Assert.AreEqual(thicknessAquiferLayerValue, input.ThicknessAquiferLayer);
            Assert.AreEqual(meanDiameter70Value, input.MeanDiameter70);
            Assert.AreEqual(beddingAngleValue, input.BeddingAngle);
            Assert.AreEqual(exitPointXCoordinate, input.ExitPointXCoordinate);
            Assert.AreSame(surfaceLine, input.SurfaceLine);
            Assert.AreSame(soilProfile, input.SoilProfile);
        }
    }
}