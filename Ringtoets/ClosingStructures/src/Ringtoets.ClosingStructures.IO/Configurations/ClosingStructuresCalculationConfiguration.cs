// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.ClosingStructures.IO.Configurations
{
    /// <summary>
    /// Configuration of a closing structure calculation.
    /// </summary>
    public class ClosingStructuresCalculationConfiguration : StructuresCalculationConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresCalculationConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ClosingStructuresCalculationConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public ClosingStructuresCalculationConfiguration(string name) : base(name) {}

        /// <summary>
        /// Gets or sets the stochast configuration for the inside water level for the structure.
        /// </summary>
        public StochastConfiguration InsideWaterLevel { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the drain coefficient for the structure.
        /// </summary>
        public StochastConfiguration DrainCoefficient { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the threshold height open weir for the structure.
        /// </summary>
        public StochastConfiguration ThresholdHeightOpenWeir { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the area flow of apertures for the structure.
        /// </summary>
        public StochastConfiguration AreaFlowApertures { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the level crest of structure not closing.
        /// </summary>
        public StochastConfiguration LevelCrestStructureNotClosing { get; set; }

        /// <summary>
        /// Gets or sets the number of identical apertures.
        /// </summary>
        public int? IdenticalApertures { get; set; }

        /// <summary>
        /// Gets or sets the factor of storm duration for an open structure.
        /// </summary>
        public double? FactorStormDurationOpenStructure { get; set; }

        /// <summary>
        /// Gets or sets the failure probability of an open structure.
        /// </summary>
        public double? FailureProbabilityOpenStructure { get; set; }

        /// <summary>
        /// Gets or sets the failure probability of reparation.
        /// </summary>
        public double? FailureProbabilityReparation { get; set; }

        /// <summary>
        /// Gets or sets the probability for an open structure before flooding.
        /// </summary>
        public double? ProbabilityOpenStructureBeforeFlooding { get; set; }

        /// <summary>
        /// Gets or sets the inflow model type.
        /// </summary>
        public ConfigurationClosingStructureInflowModelType? InflowModelType { get; set; }
    }
}