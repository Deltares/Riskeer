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
using Ringtoets.Common.IO;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Readers;

namespace Ringtoets.GrassCoverErosionInwards.IO.Readers
{
    /// <summary>
    /// Class that represents a grass cover erosion inwards calculation that is read via
    /// <see cref="GrassCoverErosionInwardsCalculationConfigurationReader"/>.
    /// </summary>
    public class ReadGrassCoverErosionInwardsCalculation : IConfigurationItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadGrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        /// <param name="constructionProperties">The container of the properties for the
        /// <see cref="ReadGrassCoverErosionInwardsCalculation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="constructionProperties"/>
        /// is <c>null</c>.</exception>
        public ReadGrassCoverErosionInwardsCalculation(ConstructionProperties constructionProperties)
        {
            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            Name = constructionProperties.Name;
            HydraulicBoundaryLocation = constructionProperties.HydraulicBoundaryLocation;
            DikeProfile = constructionProperties.DikeProfile;
            Orientation = constructionProperties.Orientation;
            DikeHeight = constructionProperties.DikeHeight;
            DikeHeightCalculationType = constructionProperties.DikeHeightCalculationType;
            UseBreakWater = constructionProperties.UseBreakWater;
            BreakWaterType = constructionProperties.BreakWaterType;
            BreakWaterHeight = constructionProperties.BreakWaterHeight;
            UseForeshore = constructionProperties.UseForeshore;
            CriticalFlowRateMean = constructionProperties.CriticalFlowRateMean;
            CriticalFlowRateStandardDeviation = constructionProperties.CriticalFlowRateStandardDeviation;
        }

        /// <summary>
        /// Gets the name of the hydraulic boundary location of the read grass cover erosion
        /// inwards calculation.
        /// </summary>
        public string HydraulicBoundaryLocation { get; }

        /// <summary>
        /// Gets the name of the dike profile of the read grass cover erosion inwards calculation.
        /// </summary>
        public string DikeProfile { get; }

        /// <summary>
        /// Gets the orientation of the grass cover erosion inwards calculation.
        /// </summary>
        public double? Orientation { get; }

        /// <summary>
        /// Gets the dike height of the grass cover erosion inwards calculation.
        /// </summary>
        public double? DikeHeight { get; }

        /// <summary>
        /// Gets the value for how the dike height should be calculated for the grass cover
        /// erosion inwards calculation.
        /// </summary>
        public ReadDikeHeightCalculationType? DikeHeightCalculationType { get; }

        /// <summary>
        /// Gets the value indicating if the break water for the grass cover erosion inwards
        /// calculation should be used.
        /// </summary>
        public bool? UseBreakWater { get; }

        /// <summary>
        /// Gets the type of break water for the grass cover erosion inwards calculation.
        /// </summary>
        public ReadBreakWaterType? BreakWaterType { get; }

        /// <summary>
        /// Gets the height of the break water for the grass cover erosion inwards calculation.
        /// </summary>
        public double? BreakWaterHeight { get; }

        /// <summary>
        /// Gets the value indicating if the foreshore for the grass cover erosion inwards
        /// calculation should be used.
        /// </summary>
        public bool? UseForeshore { get; }

        /// <summary>
        /// Gets the mean of the critical flow distribution for the grass cover erosion
        /// inwards calculation.
        /// </summary>
        public double? CriticalFlowRateMean { get; }

        /// <summary>
        /// Gets the standard deviation of the critical flow distribution for the grass
        /// cover erosion inwards calculation.
        /// </summary>
        public double? CriticalFlowRateStandardDeviation { get; }

        public string Name { get; }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="ReadGrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.Name"/>.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.HydraulicBoundaryLocation"/>.
            /// </summary>
            public string HydraulicBoundaryLocation { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.DikeProfile"/>.
            /// </summary>
            public string DikeProfile { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.Orientation"/>.
            /// </summary>
            public double? Orientation { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.DikeHeight"/>.
            /// </summary>
            public double? DikeHeight { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.DikeHeightCalculationType"/>.
            /// </summary>
            public ReadDikeHeightCalculationType? DikeHeightCalculationType { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.UseBreakWater"/>.
            /// </summary>
            public bool? UseBreakWater { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.BreakWaterType"/>.
            /// </summary>
            public ReadBreakWaterType? BreakWaterType { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.BreakWaterHeight"/>.
            /// </summary>
            public double? BreakWaterHeight { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.UseForeshore"/>.
            /// </summary>
            public bool? UseForeshore { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.CriticalFlowRateMean"/>.
            /// </summary>
            public double? CriticalFlowRateMean { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadGrassCoverErosionInwardsCalculation.CriticalFlowRateStandardDeviation"/>.
            /// </summary>
            public double? CriticalFlowRateStandardDeviation { get; set; }
        }
    }
}