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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.IO.Readers
{
    /// <summary>
    /// Class that represents a wave conditions calculation that is read via <see cref="WaveConditionsCalculationConfigurationReader"/>.
    /// </summary>
    public class ReadWaveConditionsCalculation : IReadConfigurationItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadWaveConditionsCalculation"/>.
        /// </summary>
        /// <param name="constructionProperties">The container of the properties for the <see cref="ReadWaveConditionsCalculation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="constructionProperties"/> is <c>null</c>.</exception>
        public ReadWaveConditionsCalculation(ConstructionProperties constructionProperties)
        {
            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            Name = constructionProperties.Name;
            HydraulicBoundaryLocation = constructionProperties.HydraulicBoundaryLocation;
            UpperBoundaryRevetment = constructionProperties.UpperBoundaryRevetment;
            LowerBoundaryRevetment = constructionProperties.LowerBoundaryRevetment;
            UpperBoundaryWaterLevels = constructionProperties.UpperBoundaryWaterLevels;
            LowerBoundaryWaterLevels = constructionProperties.LowerBoundaryWaterLevels;
            StepSize = constructionProperties.StepSize;
            ForeshoreProfile = constructionProperties.ForeshoreProfile;
            Orientation = constructionProperties.Orientation;
            UseBreakWater = constructionProperties.UseBreakWater;
            BreakWaterType = constructionProperties.BreakWaterType;
            BreakWaterHeight = constructionProperties.BreakWaterHeight;
            UseForeshore = constructionProperties.UseForeshore;
        }

        /// <summary>
        /// Gets the name of the hydraulic boundary location of the read calculation.
        /// </summary>
        public string HydraulicBoundaryLocation { get; }

        /// <summary>
        /// Gets the upper boundary of the revetment of the read calculation.
        /// </summary>
        public double? UpperBoundaryRevetment { get; }

        /// <summary>
        /// Gets the lower boundary of the revetment of the read calculation.
        /// </summary>
        public double? LowerBoundaryRevetment { get; }

        /// <summary>
        /// Gets the upper boundary of the water levels of the read calculation.
        /// </summary>
        public double? UpperBoundaryWaterLevels { get; }

        /// <summary>
        /// Gets the lower boundary of the water levels of the read calculation.
        /// </summary>
        public double? LowerBoundaryWaterLevels { get; }

        /// <summary>
        /// Gets the step size of the read calculation.
        /// </summary>
        public WaveConditionsInputStepSize? StepSize { get; }

        /// <summary>
        /// Gets the name of the foreshore profile of the read calculation.
        /// </summary>
        public string ForeshoreProfile { get; }

        /// <summary>
        /// Gets the orientation of the read calculation.
        /// </summary>
        public double? Orientation { get; }

        /// <summary>
        /// Gets whether the breakwater should be used for the read calculation.
        /// </summary>
        public bool? UseBreakWater { get; }

        /// <summary>
        /// Gets the breakwater type of the read calculation.
        /// </summary>
        public BreakWaterType? BreakWaterType { get; }

        /// <summary>
        /// Gets the breakwater height of the read calculation.
        /// </summary>
        public double? BreakWaterHeight { get; }

        /// <summary>
        /// Gets whether the foreshore should be used for the read calculation.
        /// </summary>
        public bool? UseForeshore { get; }

        public string Name { get; }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="ReadWaveConditionsCalculation"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.Name"/>.
            /// </summary>
            public string Name { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.HydraulicBoundaryLocation"/>.
            /// </summary>
            public string HydraulicBoundaryLocation { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.UpperBoundaryRevetment"/>.
            /// </summary>
            public double? UpperBoundaryRevetment { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.LowerBoundaryRevetment"/>.
            /// </summary>
            public double? LowerBoundaryRevetment { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.UpperBoundaryWaterLevels"/>.
            /// </summary>
            public double? UpperBoundaryWaterLevels { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.LowerBoundaryWaterLevels"/>.
            /// </summary>
            public double? LowerBoundaryWaterLevels { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.StepSize"/>.
            /// </summary>
            public WaveConditionsInputStepSize? StepSize{ internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.ForeshoreProfile"/>.
            /// </summary>
            public string ForeshoreProfile { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.Orientation"/>.
            /// </summary>
            public double? Orientation { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.UseBreakWater"/>.
            /// </summary>
            public bool? UseBreakWater { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.BreakWaterType"/>.
            /// </summary>
            public BreakWaterType? BreakWaterType { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.BreakWaterHeight"/>.
            /// </summary>
            public double? BreakWaterHeight { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadWaveConditionsCalculation.UseForeshore"/>.
            /// </summary>
            public bool? UseForeshore { internal get; set; }
        }
    }
}