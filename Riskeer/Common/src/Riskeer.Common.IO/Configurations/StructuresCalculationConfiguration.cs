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

using System;

namespace Riskeer.Common.IO.Configurations
{
    /// <summary>
    /// Configuration of a structure calculation.
    /// </summary>
    public abstract class StructuresCalculationConfiguration : IConfigurationItem
    {
        private string name;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresCalculationConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="StructuresCalculationConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        protected StructuresCalculationConfiguration(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the id of the structure.
        /// </summary>
        public string StructureId { get; set; }

        /// <summary>
        /// Gets or sets the name of the hydraulic boundary location.
        /// </summary>
        public string HydraulicBoundaryLocationName { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the storm duration.
        /// </summary>
        public StochastConfiguration StormDuration { get; set; }

        /// <summary>
        /// Gets or sets the structure's normal orientation.
        /// </summary>
        public double? StructureNormalOrientation { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the allowed level of storage increase.
        /// </summary>
        public StochastConfiguration AllowedLevelIncreaseStorage { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the storage structure area.
        /// </summary>
        public StochastConfiguration StorageStructureArea { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the flow width at bottom protection.
        /// </summary>
        public StochastConfiguration FlowWidthAtBottomProtection { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the critical overtopping discharge.
        /// </summary>
        public StochastConfiguration CriticalOvertoppingDischarge { get; set; }

        /// <summary>
        /// Gets or sets the failure probability of structure with erosion.
        /// </summary>
        public double? FailureProbabilityStructureWithErosion { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the flow width of apertures.
        /// </summary>
        public StochastConfiguration WidthFlowApertures { get; set; }

        /// <summary>
        /// Gets or sets the wave reduction configuration.
        /// </summary>
        public WaveReductionConfiguration WaveReduction { get; set; }

        /// <summary>
        /// Gets or sets the id of the foreshore profile.
        /// </summary>
        public string ForeshoreProfileId { get; set; }

        /// <summary>
        /// Gets or sets whether the illustration points should be calculated.
        /// </summary>
        public bool? ShouldIllustrationPointsBeCalculated { get; set; }

        /// <summary>
        /// Gets or sets the scenario.
        /// </summary>
        public ScenarioConfiguration Scenario { get; set; }

        /// <summary>
        /// Gets or sets the name of the calculation.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), @"Name is required for a calculation configuration.");
                }

                name = value;
            }
        }
    }
}