// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.IO.Configurations;

namespace Riskeer.StabilityPointStructures.IO.Configurations
{
    /// <summary>
    /// Configuration of a stability point structures calculation.
    /// </summary>
    public class StabilityPointStructuresCalculationConfiguration : StructuresCalculationConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="StabilityPointStructuresCalculationConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public StabilityPointStructuresCalculationConfiguration(string name) : base(name) {}

        /// <summary>
        /// Gets or sets the stochast configuration for the area flow apertures of the structure.
        /// </summary>
        public StochastConfiguration AreaFlowApertures { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the bank width of the structure.
        /// </summary>
        public StochastConfiguration BankWidth { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the constructive strength of the linear load model of the structure.
        /// </summary>
        public StochastConfiguration ConstructiveStrengthLinearLoadModel { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the constructive strength of the quadratic load model of the structure.
        /// </summary>
        public StochastConfiguration ConstructiveStrengthQuadraticLoadModel { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the drain coefficient of the structure.
        /// </summary>
        public StochastConfiguration DrainCoefficient { get; set; }

        /// <summary>
        /// Gets or sets the evaluation level of the structure.
        /// </summary>
        public double? EvaluationLevel { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the failure collision energy of the structure.
        /// </summary>
        public StochastConfiguration FailureCollisionEnergy { get; set; }

        /// <summary>
        /// Gets or sets the failure probability of repairing a closure of the structure.
        /// </summary>
        public double? FailureProbabilityRepairClosure { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the flow velocity structure closable of the structure.
        /// </summary>
        public StochastConfiguration FlowVelocityStructureClosable { get; set; }

        /// <summary>
        /// Gets or sets the inflow model type of the structure.
        /// </summary>
        public ConfigurationStabilityPointStructuresInflowModelType? InflowModelType { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the inside water level of the structure.
        /// </summary>
        public StochastConfiguration InsideWaterLevel { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the inside water level failure construction of the structure.
        /// </summary>
        public StochastConfiguration InsideWaterLevelFailureConstruction { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the crest level of the structure.
        /// </summary>
        public StochastConfiguration LevelCrestStructure { get; set; }

        /// <summary>
        /// Gets or sets the leveling count of the structure.
        /// </summary>
        public int? LevellingCount { get; set; }

        /// <summary>
        /// Gets or sets the load schematization type of the structure.
        /// </summary>
        public ConfigurationStabilityPointStructuresLoadSchematizationType? LoadSchematizationType { get; set; }

        /// <summary>
        /// Gets or sets the probability of a secondary collision on the structure per leveling.
        /// </summary>
        public double? ProbabilityCollisionSecondaryStructure { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the mass of the ship.
        /// </summary>
        public StochastConfiguration ShipMass { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the velocity of the ship.
        /// </summary>
        public StochastConfiguration ShipVelocity { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the stability properties of the linear load model of the structure.
        /// </summary>
        public StochastConfiguration StabilityLinearLoadModel { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the stability properties of the quadratic load model of the structure.
        /// </summary>
        public StochastConfiguration StabilityQuadraticLoadModel { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the threshold height of the open weir of the structure.
        /// </summary>
        public StochastConfiguration ThresholdHeightOpenWeir { get; set; }

        /// <summary>
        /// Gets or sets the vertical distance of the structure.
        /// </summary>
        public double? VerticalDistance { get; set; }

        /// <summary>
        /// Gets or sets the factor for the storm duration for an open structure.
        /// </summary>
        public double? FactorStormDurationOpenStructure { get; set; }

        /// <summary>
        /// Gets or sets the volumic weight of water.
        /// </summary>
        public double? VolumicWeightWater { get; set; }
    }
}