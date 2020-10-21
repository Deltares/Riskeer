// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Piping;
using Riskeer.HydraRing.Calculation.Data.Variables;
using Riskeer.HydraRing.Calculation.TestUtil;

namespace Riskeer.HydraRing.Calculation.Test.Data.Input.Piping
{
    [TestFixture]
    public class PipingCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;
            const double sectionLength = 1.1;
            const double phreaticLevelExitMean = 2.2;
            const double phreaticLevelExitStandardDeviation = 3.3;
            const double waterVolumetricWeight = 4.4;
            const double effectiveThicknessCoverageLayerMean = 5.5;
            const double effectiveThicknessCoverageLayerStandardDeviation = 6.6;
            const double saturatedVolumicWeightOfCoverageLayerMean = 7.7;
            const double saturatedVolumicWeightOfCoverageLayerStandardDeviation = 8.8;
            const double saturatedVolumicWeightOfCoverageLayerShift = 9.9;
            const double upliftModelFactorMean = 10.0;
            const double upliftModelFactorStandardDeviation = 11.1;
            const double dampingFactorExitMean = 12.2;
            const double dampingFactorExitStandardDeviation = 13.3;
            const double seepageLengthMean = 14.4;
            const double seepageLengthCoefficientOfVariation = 15.5;
            const double thicknessAquiferLayerMean = 16.6;
            const double thicknessAquiferLayerStandardDeviation = 17.7;
            const double sandParticlesVolumicWeight = 18.8;
            const double sellmeijerModelFactorMean = 19.9;
            const double sellmeijerModelFactorStandardDeviation = 20.0;
            const double beddingAngle = 21.1;
            const double whitesDragCoefficient = 22.2;
            const double waterKinematicViscosity = 23.3;
            const double darcyPermeabilityMean = 24.4;
            const double darcyPermeabilityCoefficientOfVariation = 25.5;
            const double diameter70Mean = 26.6;
            const double diameter70CoefficientOfVariation = 27.7;
            const double gravity = 28.8;
            const double criticalHeaveGradientMean = 29.9;
            const double criticalHeaveGradientStandardDeviation = 30.0;

            // Call
            var pipingCalculationInput = new PipingCalculationInput(
                hydraulicBoundaryLocationId, sectionLength, phreaticLevelExitMean, phreaticLevelExitStandardDeviation,
                waterVolumetricWeight, effectiveThicknessCoverageLayerMean, effectiveThicknessCoverageLayerStandardDeviation,
                saturatedVolumicWeightOfCoverageLayerMean, saturatedVolumicWeightOfCoverageLayerStandardDeviation,
                saturatedVolumicWeightOfCoverageLayerShift, upliftModelFactorMean, upliftModelFactorStandardDeviation,
                dampingFactorExitMean, dampingFactorExitStandardDeviation, seepageLengthMean, seepageLengthCoefficientOfVariation,
                thicknessAquiferLayerMean, thicknessAquiferLayerStandardDeviation, sandParticlesVolumicWeight,
                sellmeijerModelFactorMean, sellmeijerModelFactorStandardDeviation, beddingAngle, whitesDragCoefficient,
                waterKinematicViscosity, darcyPermeabilityMean, darcyPermeabilityCoefficientOfVariation, diameter70Mean,
                diameter70CoefficientOfVariation, gravity, criticalHeaveGradientMean, criticalHeaveGradientStandardDeviation);

            // Assert
            const int expectedCalculationTypeId = 1;
            const int expectedVariableId = 58;
            Assert.IsInstanceOf<ExceedanceProbabilityCalculationInput>(pipingCalculationInput);
            Assert.AreEqual(expectedCalculationTypeId, pipingCalculationInput.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, pipingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.Piping, pipingCalculationInput.FailureMechanismType);
            Assert.AreEqual(expectedVariableId, pipingCalculationInput.VariableId);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultPipingVariables().ToArray(), pipingCalculationInput.Variables.ToArray());
            Assert.IsNaN(pipingCalculationInput.Beta);

            HydraRingSection hydraRingSection = pipingCalculationInput.Section;
            Assert.AreEqual(1, hydraRingSection.SectionId);
            Assert.AreEqual(sectionLength, hydraRingSection.SectionLength);
            Assert.IsNaN(hydraRingSection.CrossSectionNormal);
        }

        private static IEnumerable<HydraRingVariable> GetDefaultPipingVariables()
        {
            yield return new NormalHydraRingVariable(42, HydraRingDeviationType.Standard, 2.2, 3.3);
            yield return new DeterministicHydraRingVariable(43, 4.4);
            yield return new LogNormalHydraRingVariable(44, HydraRingDeviationType.Standard, 5.5, 6.6);
            yield return new LogNormalHydraRingVariable(45, HydraRingDeviationType.Standard, 7.7, 8.8, 9.9);
            yield return new LogNormalHydraRingVariable(46, HydraRingDeviationType.Standard, 10.0, 11.1);
            yield return new LogNormalHydraRingVariable(47, HydraRingDeviationType.Standard, 12.2, 13.3);
            yield return new LogNormalHydraRingVariable(48, HydraRingDeviationType.Variation, 14.4, 15.5);
            yield return new LogNormalHydraRingVariable(49, HydraRingDeviationType.Standard, 16.6, 17.7);
            yield return new DeterministicHydraRingVariable(50, 18.8 + 4.4);
            yield return new LogNormalHydraRingVariable(51, HydraRingDeviationType.Standard, 19.9, 20.0);
            yield return new DeterministicHydraRingVariable(52, 21.1);
            yield return new DeterministicHydraRingVariable(53, 22.2);
            yield return new DeterministicHydraRingVariable(54, 23.3);
            yield return new LogNormalHydraRingVariable(55, HydraRingDeviationType.Variation, 24.4, 25.5);
            yield return new LogNormalHydraRingVariable(56, HydraRingDeviationType.Variation, 26.6, 27.7);
            yield return new DeterministicHydraRingVariable(58, 28.8);
            yield return new LogNormalHydraRingVariable(124, HydraRingDeviationType.Standard, 29.9, 30);
        }
    }
}