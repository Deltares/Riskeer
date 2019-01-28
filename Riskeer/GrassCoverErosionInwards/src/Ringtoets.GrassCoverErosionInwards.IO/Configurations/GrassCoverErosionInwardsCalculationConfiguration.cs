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

namespace Ringtoets.GrassCoverErosionInwards.IO.Configurations
{
    /// <summary>
    /// Configuration of a grass cover erosion inwards calculation.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationConfiguration : IConfigurationItem
    {
        private string name;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="GrassCoverErosionInwardsCalculationConfiguration"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>
        /// is <c>null</c>.</exception>
        public GrassCoverErosionInwardsCalculationConfiguration(string name)
        {
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
        /// Gets or sets the value for how the dike height should be calculated for the grass cover
        /// erosion inwards calculation.
        /// </summary>
        public ConfigurationHydraulicLoadsCalculationType? DikeHeightCalculationType { get; set; }

        /// <summary>
        /// Gets or sets the value for how the overtopping rate should be calculated for the grass 
        /// cover erosion inwards calculation.
        /// </summary>
        public ConfigurationHydraulicLoadsCalculationType? OvertoppingRateCalculationType { get; set; }

        /// <summary>
        /// Gets or sets if the illustration points should be calculated for Overtopping Output.
        /// </summary>
        public bool? ShouldOvertoppingOutputIllustrationPointsBeCalculated { get; set; }

        /// <summary>
        /// Gets or sets if the illustration points should be calculated for Dike Height.
        /// </summary>
        public bool? ShouldDikeHeightIllustrationPointsBeCalculated { get; set; }

        /// <summary>
        /// Gets or sets if the illustration points should be calculated for Overtopping Flow.
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
        /// Gets or sets the name of the grass cover erosion inwards calculation.
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