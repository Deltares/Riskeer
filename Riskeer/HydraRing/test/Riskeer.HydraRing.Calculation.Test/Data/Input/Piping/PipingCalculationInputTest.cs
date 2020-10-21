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
            const double sectionLength = 22.2;
            const double phreaticLevelExitMean = 1.1;
            const double phreaticLevelExitStandardDeviation = 2.2;
            const double waterVolumetricWeight = 3.3;
            const double effectiveThicknessCoverageLayerMean = 4.4;
            const double effectiveThicknessCoverageLayerStandardDeviation = 5.5;
            const double saturatedVolumicWeightOfCoverageLayerMean = 6.6;
            const double saturatedVolumicWeightOfCoverageLayerStandardDeviation = 7.7;
            const double saturatedVolumicWeightOfCoverageLayerShift = 8.8;
            const double upliftModelFactorMean = 9.9;
            const double upliftModelFactorStandardDeviation = 10.0;
            const double dampingFactorExitMean = 11.1;
            const double dampingFactorExitStandardDeviation = 12.2;
            const double seepageLengthMean = 13.3;
            const double seepageLengthCoefficientOfVariation = 14.4;
            const double thicknessAquiferLayerMean = 15.5;
            const double thicknessAquiferLayerStandardDeviation = 16.6;
            const double sandParticlesVolumicWeight = 17.7;
            const double sellmeijerModelFactorMean = 18.8;
            const double sellmeijerModelFactorStandardDeviation = 19.9;
            const double beddingAngle = 20.0;
            const double whitesDragCoefficient = 21.1;
            const double waterKinematicViscosity = 22.2;
            const double darcyPermeabilityMean = 23.3;
            const double darcyPermeabilityCoefficientOfVariation = 24.4;
            const double diameter70Mean = 25.5;
            const double diameter70CoefficientOfVariation = 26.6;
            const double gravity = 27.7;
            const double criticalHeaveGradient = 28.8;

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
                diameter70CoefficientOfVariation, gravity, criticalHeaveGradient);

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
            yield return new NormalHydraRingVariable(42, HydraRingDeviationType.Standard, 1.1, 2.2);
            yield return new DeterministicHydraRingVariable(43, 3.3);
            yield return new LogNormalHydraRingVariable(44, HydraRingDeviationType.Standard, 4.4, 5.5);
            yield return new LogNormalHydraRingVariable(45, HydraRingDeviationType.Standard, 6.6, 7.7, 8.8);
            yield return new LogNormalHydraRingVariable(46, HydraRingDeviationType.Standard, 9.9, 10.0);
            yield return new LogNormalHydraRingVariable(47, HydraRingDeviationType.Standard, 11.1, 12.2);
            yield return new LogNormalHydraRingVariable(48, HydraRingDeviationType.Variation, 13.3, 14.4);
            yield return new LogNormalHydraRingVariable(49, HydraRingDeviationType.Standard, 15.5, 16.6);
            yield return new DeterministicHydraRingVariable(50, 17.7 + 3.3);
            yield return new LogNormalHydraRingVariable(51, HydraRingDeviationType.Standard, 18.8, 19.9);
            yield return new DeterministicHydraRingVariable(52, 20.0);
            yield return new DeterministicHydraRingVariable(53, 21.1);
            yield return new DeterministicHydraRingVariable(54, 22.2);
            yield return new LogNormalHydraRingVariable(55, HydraRingDeviationType.Variation, 23.3, 24.4);
            yield return new LogNormalHydraRingVariable(56, HydraRingDeviationType.Variation, 25.5, 26.6);
            yield return new DeterministicHydraRingVariable(58, 27.7);
            yield return new DeterministicHydraRingVariable(124, 28.8);
        }
    }
}