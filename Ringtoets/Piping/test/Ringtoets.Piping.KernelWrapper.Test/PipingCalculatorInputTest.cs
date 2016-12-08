// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.KernelWrapper.Test
{
    [TestFixture]
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
            double effectiveThicknessCoverageLayerValue = random.NextDouble();
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
            }, SoilProfileType.SoilProfile1D, 0);

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
                effectiveThicknessCoverageLayerValue,
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
            Assert.AreEqual(effectiveThicknessCoverageLayerValue, input.EffectiveThicknessCoverageLayer);
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