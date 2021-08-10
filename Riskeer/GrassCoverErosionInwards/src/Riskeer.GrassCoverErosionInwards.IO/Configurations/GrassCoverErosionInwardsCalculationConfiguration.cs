// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.IO.Configurations;

namespace Riskeer.GrassCoverErosionInwards.IO.Configurations
{
    /// <summary>
    /// Configuration of a grass cover erosion inwards calculation.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationConfiguration : IConfigurationItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the grass cover erosion inwards calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public GrassCoverErosionInwardsCalculationConfiguration(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the hydraulic boundary location of the read grass cover erosion
        /// inwards calculation.
        /// </summary>
        public string HydraulicBoundaryLocationName { get; set; }

        /// <summary>
        /// Gets or sets the id of the dike profile of the read grass cover erosion inwards calculation.
        /// </summary>
        public string DikeProfileId { get; set; }

        /// <summary>
        /// Gets or sets the orientation of the grass cover erosion inwards calculation.
        /// </summary>
        public double? Orientation { get; set; }

        /// <summary>
        /// Gets or sets the dike height of the grass cover erosion inwards calculation.
        /// </summary>
        public double? DikeHeight { get; set; }

        /// <summary>
        /// Gets or sets whether the illustration points should be calculated for overtopping output.
        /// </summary>
        public bool? ShouldOvertoppingOutputIllustrationPointsBeCalculated { get; set; }

        /// <summary>
        /// Gets or sets whether the dike height should be calculated.
        /// </summary>
        public bool? ShouldDikeHeightBeCalculated { get; set; }

        /// <summary>
        /// Gets or sets the target probability for dike height.
        /// </summary>
        public double? DikeHeightTargetProbability { get; set; }

        /// <summary>
        /// Gets or sets whether the illustration points should be calculated for dike height.
        /// </summary>
        public bool? ShouldDikeHeightIllustrationPointsBeCalculated { get; set; }

        /// <summary>
        /// Gets or sets whether the overtopping rate should be calculated.
        /// </summary>
        public bool? ShouldOvertoppingRateBeCalculated { get; set; }

        /// <summary>
        /// Gets or sets the target probability of overtopping rate.
        /// </summary>
        public double? OvertoppingRateTargetProbability { get; set; }

        /// <summary>
        /// Gets or sets whether the illustration points should be calculated for overtopping rate.
        /// </summary>
        public bool? ShouldOvertoppingRateIllustrationPointsBeCalculated { get; set; }

        /// <summary>
        /// Gets or sets the wave reduction configuration.
        /// </summary>
        public WaveReductionConfiguration WaveReduction { get; set; }

        /// <summary>
        /// Gets or sets the critical flow distribution for the grass cover erosion inwards calculation.
        /// </summary>
        public StochastConfiguration CriticalFlowRate { get; set; }

        /// <summary>
        /// Gets or sets the configuration of the scenario of the grass cover erosion inwards calculation.
        /// </summary>
        public ScenarioConfiguration Scenario { get; set; }

        /// <summary>
        /// Gets the name of the grass cover erosion inwards calculation.
        /// </summary>
        public string Name { get; }
    }
}