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
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.Data.Input.Piping
{
    /// <summary>
    /// Container of all data necessary for performing a piping calculation via Hydra-Ring.
    /// </summary>
    public class PipingCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly double phreaticLevelExitMean;
        private readonly double phreaticLevelExitStandardDeviation;
        private readonly double waterVolumetricWeight;
        private readonly double effectiveThicknessCoverageLayerMean;
        private readonly double effectiveThicknessCoverageLayerStandardDeviation;
        private readonly double saturatedVolumicWeightOfCoverageLayerMean;
        private readonly double saturatedVolumicWeightOfCoverageLayerStandardDeviation;
        private readonly double saturatedVolumicWeightOfCoverageLayerShift;
        private readonly double upliftModelFactorMean;
        private readonly double upliftModelFactorStandardDeviation;
        private readonly double dampingFactorExitMean;
        private readonly double dampingFactorExitStandardDeviation;
        private readonly double seepageLengthMean;
        private readonly double seepageLengthCoefficientOfVariation;
        private readonly double thicknessAquiferLayerMean;
        private readonly double thicknessAquiferLayerStandardDeviation;
        private readonly double sandParticlesVolumicWeight;
        private readonly double sellmeijerModelFactorMean;
        private readonly double sellmeijerModelFactorStandardDeviation;
        private readonly double beddingAngle;
        private readonly double whitesDragCoefficient;
        private readonly double waterKinematicViscosity;
        private readonly double darcyPermeabilityMean;
        private readonly double darcyPermeabilityCoefficientOfVariation;
        private readonly double diameter70Mean;
        private readonly double diameter70CoefficientOfVariation;
        private readonly double gravity;
        private readonly double criticalHeaveGradientMean;
        private readonly double criticalHeaveGradientStandardDeviation;

        private readonly bool hasCoverageLayer;

        /// <summary>
        /// Creates a new instance of the <see cref="PipingCalculationInput"/> class, taking into account the precense of a coverage layer.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="sectionLength">The length of the section.</param>
        /// <param name="phreaticLevelExitMean">The mean of the phreatic level at the exit point.</param>
        /// <param name="phreaticLevelExitStandardDeviation">The standard deviation of the phreatic level at the exit point.</param>
        /// <param name="waterVolumetricWeight">The volumetric weight of water.</param>
        /// <param name="effectiveThicknessCoverageLayerMean">The mean of the effective thickness of the coverage layers at the exit point.</param>
        /// <param name="effectiveThicknessCoverageLayerStandardDeviation">The standard deviation of the effective thickness of the coverage layers at the exit point.</param>
        /// <param name="saturatedVolumicWeightOfCoverageLayerMean">The mean of the volumic weight of the saturated coverage layer.</param>
        /// <param name="saturatedVolumicWeightOfCoverageLayerStandardDeviation">The standard deviation of the volumic weight of the saturated coverage layer.</param>
        /// <param name="saturatedVolumicWeightOfCoverageLayerShift">The shift of the volumic weight of the saturated coverage layer.</param>
        /// <param name="upliftModelFactorMean">The mean of the distribution used to account for uncertainty in the model for uplift.</param>
        /// <param name="upliftModelFactorStandardDeviation">The standard deviation of the distribution used to account for uncertainty in the model for uplift.</param>
        /// <param name="dampingFactorExitMean">The mean of the damping factor at the exit point.</param>
        /// <param name="dampingFactorExitStandardDeviation">The standard deviation of the damping factor at the exit point.</param>
        /// <param name="seepageLengthMean">The mean of the horizontal distance between entry and exit point.</param>
        /// <param name="seepageLengthCoefficientOfVariation">The coefficient of variation of the horizontal distance between entry and exit point.</param>
        /// <param name="thicknessAquiferLayerMean">The mean of the total thickness of the aquifer layers at the exit point.</param>
        /// <param name="thicknessAquiferLayerStandardDeviation">The standard deviation of the total thickness of the aquifer layers at the exit point.</param>
        /// <param name="sandParticlesVolumicWeight">The (lowerbound) volumic weight of sand grain material of a sand layer under water.</param>
        /// <param name="sellmeijerModelFactorMean">The mean of the distribution used to account for uncertainty in the model for Sellmeijer.</param>
        /// <param name="sellmeijerModelFactorStandardDeviation">The standard deviation of the distribution used to account for uncertainty in the model for Sellmeijer.</param>
        /// <param name="beddingAngle">The angle of the force balance representing the amount in which sand grains resist rolling.</param>
        /// <param name="whitesDragCoefficient">The White's drag coefficient.</param>
        /// <param name="waterKinematicViscosity">The kinematic viscosity of water at 10 °C.</param>
        /// <param name="darcyPermeabilityMean">The mean of the Darcy-speed with which water flows through the aquifer layer.</param>
        /// <param name="darcyPermeabilityCoefficientOfVariation">The coefficient of variation of the Darcy-speed with which water flows through the aquifer layer.</param>
        /// <param name="diameter70Mean">The mean of the sieve size through which 70% of the grains of the top part of the aquifer pass.</param>
        /// <param name="diameter70CoefficientOfVariation">The coefficient of variation of the sieve size through which 70% of the grains of the top part of the aquifer pass.</param>
        /// <param name="gravity">The gravitational acceleration.</param>
        /// <param name="criticalHeaveGradientMean">The mean of the critical exit gradient for heave.</param>
        /// <param name="criticalHeaveGradientStandardDeviation">The standard deviation of the critical exit gradient for heave.</param>
        public PipingCalculationInput(long hydraulicBoundaryLocationId,
                                      double sectionLength,
                                      double phreaticLevelExitMean, double phreaticLevelExitStandardDeviation,
                                      double waterVolumetricWeight,
                                      double effectiveThicknessCoverageLayerMean, double effectiveThicknessCoverageLayerStandardDeviation,
                                      double saturatedVolumicWeightOfCoverageLayerMean, double saturatedVolumicWeightOfCoverageLayerStandardDeviation,
                                      double saturatedVolumicWeightOfCoverageLayerShift,
                                      double upliftModelFactorMean, double upliftModelFactorStandardDeviation,
                                      double dampingFactorExitMean, double dampingFactorExitStandardDeviation,
                                      double seepageLengthMean, double seepageLengthCoefficientOfVariation,
                                      double thicknessAquiferLayerMean, double thicknessAquiferLayerStandardDeviation,
                                      double sandParticlesVolumicWeight,
                                      double sellmeijerModelFactorMean, double sellmeijerModelFactorStandardDeviation,
                                      double beddingAngle,
                                      double whitesDragCoefficient,
                                      double waterKinematicViscosity,
                                      double darcyPermeabilityMean, double darcyPermeabilityCoefficientOfVariation,
                                      double diameter70Mean, double diameter70CoefficientOfVariation,
                                      double gravity,
                                      double criticalHeaveGradientMean,
                                      double criticalHeaveGradientStandardDeviation)
            : base(hydraulicBoundaryLocationId)
        {
            hasCoverageLayer = true;

            Section = new HydraRingSection(1, sectionLength, double.NaN);
            this.phreaticLevelExitMean = phreaticLevelExitMean;
            this.phreaticLevelExitStandardDeviation = phreaticLevelExitStandardDeviation;
            this.waterVolumetricWeight = waterVolumetricWeight;
            this.effectiveThicknessCoverageLayerMean = effectiveThicknessCoverageLayerMean;
            this.effectiveThicknessCoverageLayerStandardDeviation = effectiveThicknessCoverageLayerStandardDeviation;
            this.saturatedVolumicWeightOfCoverageLayerMean = saturatedVolumicWeightOfCoverageLayerMean;
            this.saturatedVolumicWeightOfCoverageLayerStandardDeviation = saturatedVolumicWeightOfCoverageLayerStandardDeviation;
            this.saturatedVolumicWeightOfCoverageLayerShift = saturatedVolumicWeightOfCoverageLayerShift;
            this.upliftModelFactorMean = upliftModelFactorMean;
            this.upliftModelFactorStandardDeviation = upliftModelFactorStandardDeviation;
            this.dampingFactorExitMean = dampingFactorExitMean;
            this.dampingFactorExitStandardDeviation = dampingFactorExitStandardDeviation;
            this.seepageLengthMean = seepageLengthMean;
            this.seepageLengthCoefficientOfVariation = seepageLengthCoefficientOfVariation;
            this.thicknessAquiferLayerMean = thicknessAquiferLayerMean;
            this.thicknessAquiferLayerStandardDeviation = thicknessAquiferLayerStandardDeviation;
            this.sandParticlesVolumicWeight = sandParticlesVolumicWeight;
            this.sellmeijerModelFactorMean = sellmeijerModelFactorMean;
            this.sellmeijerModelFactorStandardDeviation = sellmeijerModelFactorStandardDeviation;
            this.beddingAngle = beddingAngle;
            this.whitesDragCoefficient = whitesDragCoefficient;
            this.waterKinematicViscosity = waterKinematicViscosity;
            this.darcyPermeabilityMean = darcyPermeabilityMean;
            this.darcyPermeabilityCoefficientOfVariation = darcyPermeabilityCoefficientOfVariation;
            this.diameter70Mean = diameter70Mean;
            this.diameter70CoefficientOfVariation = diameter70CoefficientOfVariation;
            this.gravity = gravity;
            this.criticalHeaveGradientMean = criticalHeaveGradientMean;
            this.criticalHeaveGradientStandardDeviation = criticalHeaveGradientStandardDeviation;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PipingCalculationInput"/> class, not taking into account the precense of a coverage layer.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="sectionLength">The length of the section.</param>
        /// <param name="phreaticLevelExitMean">The mean of the phreatic level at the exit point.</param>
        /// <param name="phreaticLevelExitStandardDeviation">The standard deviation of the phreatic level at the exit point.</param>
        /// <param name="waterVolumetricWeight">The volumetric weight of water.</param>
        /// <param name="upliftModelFactorMean">The mean of the distribution used to account for uncertainty in the model for uplift.</param>
        /// <param name="upliftModelFactorStandardDeviation">The standard deviation of the distribution used to account for uncertainty in the model for uplift.</param>
        /// <param name="dampingFactorExitMean">The mean of the damping factor at the exit point.</param>
        /// <param name="dampingFactorExitStandardDeviation">The standard deviation of the damping factor at the exit point.</param>
        /// <param name="seepageLengthMean">The mean of the horizontal distance between entry and exit point.</param>
        /// <param name="seepageLengthCoefficientOfVariation">The coefficient of variation of the horizontal distance between entry and exit point.</param>
        /// <param name="thicknessAquiferLayerMean">The mean of the total thickness of the aquifer layers at the exit point.</param>
        /// <param name="thicknessAquiferLayerStandardDeviation">The standard deviation of the total thickness of the aquifer layers at the exit point.</param>
        /// <param name="sandParticlesVolumicWeight">The (lowerbound) volumic weight of sand grain material of a sand layer under water.</param>
        /// <param name="sellmeijerModelFactorMean">The mean of the distribution used to account for uncertainty in the model for Sellmeijer.</param>
        /// <param name="sellmeijerModelFactorStandardDeviation">The standard deviation of the distribution used to account for uncertainty in the model for Sellmeijer.</param>
        /// <param name="beddingAngle">The angle of the force balance representing the amount in which sand grains resist rolling.</param>
        /// <param name="whitesDragCoefficient">The White's drag coefficient.</param>
        /// <param name="waterKinematicViscosity">The kinematic viscosity of water at 10 °C.</param>
        /// <param name="darcyPermeabilityMean">The mean of the Darcy-speed with which water flows through the aquifer layer.</param>
        /// <param name="darcyPermeabilityCoefficientOfVariation">The coefficient of variation of the Darcy-speed with which water flows through the aquifer layer.</param>
        /// <param name="diameter70Mean">The mean of the sieve size through which 70% of the grains of the top part of the aquifer pass.</param>
        /// <param name="diameter70CoefficientOfVariation">The coefficient of variation of the sieve size through which 70% of the grains of the top part of the aquifer pass.</param>
        /// <param name="gravity">The gravitational acceleration.</param>
        /// <param name="criticalHeaveGradientMean">The mean of the critical exit gradient for heave.</param>
        /// <param name="criticalHeaveGradientStandardDeviation">The standard deviation of the critical exit gradient for heave.</param>
        public PipingCalculationInput(long hydraulicBoundaryLocationId,
                                      double sectionLength,
                                      double phreaticLevelExitMean, double phreaticLevelExitStandardDeviation,
                                      double waterVolumetricWeight,
                                      double upliftModelFactorMean, double upliftModelFactorStandardDeviation,
                                      double dampingFactorExitMean, double dampingFactorExitStandardDeviation,
                                      double seepageLengthMean, double seepageLengthCoefficientOfVariation,
                                      double thicknessAquiferLayerMean, double thicknessAquiferLayerStandardDeviation,
                                      double sandParticlesVolumicWeight,
                                      double sellmeijerModelFactorMean, double sellmeijerModelFactorStandardDeviation,
                                      double beddingAngle,
                                      double whitesDragCoefficient,
                                      double waterKinematicViscosity,
                                      double darcyPermeabilityMean, double darcyPermeabilityCoefficientOfVariation,
                                      double diameter70Mean, double diameter70CoefficientOfVariation,
                                      double gravity,
                                      double criticalHeaveGradientMean,
                                      double criticalHeaveGradientStandardDeviation)
            : base(hydraulicBoundaryLocationId)
        {
            hasCoverageLayer = false;

            Section = new HydraRingSection(1, sectionLength, double.NaN);
            this.phreaticLevelExitMean = phreaticLevelExitMean;
            this.phreaticLevelExitStandardDeviation = phreaticLevelExitStandardDeviation;
            this.waterVolumetricWeight = waterVolumetricWeight;
            this.upliftModelFactorMean = upliftModelFactorMean;
            this.upliftModelFactorStandardDeviation = upliftModelFactorStandardDeviation;
            this.dampingFactorExitMean = dampingFactorExitMean;
            this.dampingFactorExitStandardDeviation = dampingFactorExitStandardDeviation;
            this.seepageLengthMean = seepageLengthMean;
            this.seepageLengthCoefficientOfVariation = seepageLengthCoefficientOfVariation;
            this.thicknessAquiferLayerMean = thicknessAquiferLayerMean;
            this.thicknessAquiferLayerStandardDeviation = thicknessAquiferLayerStandardDeviation;
            this.sandParticlesVolumicWeight = sandParticlesVolumicWeight;
            this.sellmeijerModelFactorMean = sellmeijerModelFactorMean;
            this.sellmeijerModelFactorStandardDeviation = sellmeijerModelFactorStandardDeviation;
            this.beddingAngle = beddingAngle;
            this.whitesDragCoefficient = whitesDragCoefficient;
            this.waterKinematicViscosity = waterKinematicViscosity;
            this.darcyPermeabilityMean = darcyPermeabilityMean;
            this.darcyPermeabilityCoefficientOfVariation = darcyPermeabilityCoefficientOfVariation;
            this.diameter70Mean = diameter70Mean;
            this.diameter70CoefficientOfVariation = diameter70CoefficientOfVariation;
            this.gravity = gravity;
            this.criticalHeaveGradientMean = criticalHeaveGradientMean;
            this.criticalHeaveGradientStandardDeviation = criticalHeaveGradientStandardDeviation;
        }

        public override HydraRingFailureMechanismType FailureMechanismType { get; } = HydraRingFailureMechanismType.Piping;

        public override int VariableId { get; } = 58;

        public override HydraRingSection Section { get; }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                yield return new NormalHydraRingVariable(42, HydraRingDeviationType.Standard, phreaticLevelExitMean,
                                                         phreaticLevelExitStandardDeviation);
                yield return new DeterministicHydraRingVariable(43, waterVolumetricWeight);

                if (hasCoverageLayer)
                {
                    yield return new LogNormalHydraRingVariable(44, HydraRingDeviationType.Standard, effectiveThicknessCoverageLayerMean,
                                                                effectiveThicknessCoverageLayerStandardDeviation);
                    yield return new LogNormalHydraRingVariable(45, HydraRingDeviationType.Standard, saturatedVolumicWeightOfCoverageLayerMean,
                                                                saturatedVolumicWeightOfCoverageLayerStandardDeviation,
                                                                saturatedVolumicWeightOfCoverageLayerShift);
                }
                else
                {
                    yield return new DeterministicHydraRingVariable(44, 0);
                    yield return new DeterministicHydraRingVariable(45, 0);
                }

                yield return new LogNormalHydraRingVariable(46, HydraRingDeviationType.Standard, upliftModelFactorMean,
                                                            upliftModelFactorStandardDeviation);
                yield return new LogNormalHydraRingVariable(47, HydraRingDeviationType.Standard, dampingFactorExitMean,
                                                            dampingFactorExitStandardDeviation);
                yield return new LogNormalHydraRingVariable(48, HydraRingDeviationType.Variation, seepageLengthMean,
                                                            seepageLengthCoefficientOfVariation);
                yield return new LogNormalHydraRingVariable(49, HydraRingDeviationType.Standard, thicknessAquiferLayerMean,
                                                            thicknessAquiferLayerStandardDeviation);
                yield return new DeterministicHydraRingVariable(50, sandParticlesVolumicWeight + waterVolumetricWeight);
                yield return new LogNormalHydraRingVariable(51, HydraRingDeviationType.Standard, sellmeijerModelFactorMean,
                                                            sellmeijerModelFactorStandardDeviation);
                yield return new DeterministicHydraRingVariable(52, beddingAngle);
                yield return new DeterministicHydraRingVariable(53, whitesDragCoefficient);
                yield return new DeterministicHydraRingVariable(54, waterKinematicViscosity);
                yield return new LogNormalHydraRingVariable(55, HydraRingDeviationType.Variation, darcyPermeabilityMean,
                                                            darcyPermeabilityCoefficientOfVariation);
                yield return new LogNormalHydraRingVariable(56, HydraRingDeviationType.Variation, diameter70Mean,
                                                            diameter70CoefficientOfVariation);
                yield return new DeterministicHydraRingVariable(58, gravity);
                yield return new LogNormalHydraRingVariable(124, HydraRingDeviationType.Standard, criticalHeaveGradientMean,
                                                            criticalHeaveGradientStandardDeviation);
            }
        }
    }
}