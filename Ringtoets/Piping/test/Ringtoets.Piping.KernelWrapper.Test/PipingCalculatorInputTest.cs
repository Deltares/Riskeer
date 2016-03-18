using System;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.KernelWrapper.Test
{
    public class PipingCalculatorInputTest
    {
        [Test]
        public void GivenSomeRandomValues_WhenPipingCalculationInputConstructedFromInput_ThenPropertiesAreSet()
        {
            var random = new Random(22);

            double volumetricWeightOfWaterValue = random.NextDouble();
            double saturatedVolumicWeightOfCoverageLayer = random.NextDouble();
            double modelFactorUpliftValue = random.NextDouble();
            double hRiverValue = random.NextDouble();
            double phiExit = random.NextDouble();
            double rExitValue = random.NextDouble();
            double hExitValue = random.NextDouble();
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
            var soilProfile = new PipingSoilProfile(string.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            }, 0);

            var input = new PipingCalculatorInput(
                volumetricWeightOfWaterValue,
                saturatedVolumicWeightOfCoverageLayer,
                modelFactorUpliftValue,
                hRiverValue,
                phiExit,
                rExitValue,
                hExitValue,
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
            Assert.AreEqual(saturatedVolumicWeightOfCoverageLayer, input.SaturatedVolumicWeightOfCoverageLayer);
            Assert.AreEqual(modelFactorUpliftValue, input.UpliftModelFactor);
            Assert.AreEqual(hRiverValue, input.AssessmentLevel);
            Assert.AreEqual(phiExit, input.PiezometricHeadExit);
            Assert.AreEqual(rExitValue, input.DampingFactorExit);
            Assert.AreEqual(hExitValue, input.PhreaticLevelExit);
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