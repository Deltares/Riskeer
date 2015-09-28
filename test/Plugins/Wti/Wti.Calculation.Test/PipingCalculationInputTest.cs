using System;
using NUnit.Framework;

namespace Wti.Calculation.Test
{
    public class PipingCalculationInputTest
    {
        [Test]
        public void GivenSomeRandomValues_WhenPipingCalculationInputConstructedFromInput_ThenPropertiesAreSet()
        {
            var random = new Random(22);

            double volumetricWeightOfWaterValue = random.NextDouble();
            double modelFactorUpliftValue = random.NextDouble();
            double effectiveStressValue = random.NextDouble();
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

            var input = new PipingCalculationInput(
                volumetricWeightOfWaterValue,
                modelFactorUpliftValue,
                effectiveStressValue,
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
                beddingAngleValue);

            Assert.That(input.WaterVolumetricWeight, Is.EqualTo(volumetricWeightOfWaterValue));
            Assert.That(input.UpliftModelFactor, Is.EqualTo(modelFactorUpliftValue));
            Assert.That(input.EffectiveStress, Is.EqualTo(effectiveStressValue));
            Assert.That(input.AssessmentLevel, Is.EqualTo(hRiverValue));
            Assert.That(input.PiezometricHeadExit, Is.EqualTo(phiExitValue));
            Assert.That(input.DampingFactorExit, Is.EqualTo(rExitValue));
            Assert.That(input.PhreaticLevelExit, Is.EqualTo(hExitValue));
            Assert.That(input.PiezometricHeadPolder, Is.EqualTo(phiPolderValue));
            Assert.That(input.CriticalHeaveGradient, Is.EqualTo(ichValue));
            Assert.That(input.ThicknessCoverageLayer, Is.EqualTo(dTotalValue));
            Assert.That(input.SellmeijerModelFactor, Is.EqualTo(sellmeijerModelFactorValue));
            Assert.That(input.ReductionFactor, Is.EqualTo(reductionFactorValue));
            Assert.That(input.SeepageLength, Is.EqualTo(seepageLengthValue));
            Assert.That(input.SandParticlesVolumicWeight, Is.EqualTo(sandParticlesVolumicWeightValue));
            Assert.That(input.WhitesDragCoefficient, Is.EqualTo(whitesDragCoefficientValue));
            Assert.That(input.Diameter70, Is.EqualTo(diameter70Value));
            Assert.That(input.DarcyPermeability, Is.EqualTo(darcyPermeabilityValue));
            Assert.That(input.WaterKinematicViscosity, Is.EqualTo(waterKinematicViscosityValue));
            Assert.That(input.Gravity, Is.EqualTo(gravityValue));
            Assert.That(input.ThicknessAquiferLayer, Is.EqualTo(thicknessAquiferLayerValue));
            Assert.That(input.MeanDiameter70, Is.EqualTo(meanDiameter70Value));
            Assert.That(input.BeddingAngle, Is.EqualTo(beddingAngleValue));
        }
    }
}