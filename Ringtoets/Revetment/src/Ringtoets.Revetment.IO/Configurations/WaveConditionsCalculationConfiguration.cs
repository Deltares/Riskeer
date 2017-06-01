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

namespace Ringtoets.Revetment.IO.Configurations
{
    /// <summary>
    /// Class that represents a wave conditions calculation that is read via <see cref="WaveConditionsCalculationConfigurationReader"/>.
    /// </summary>
    public class WaveConditionsCalculationConfiguration : IConfigurationItem
    {
        private string name;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="WaveConditionsCalculationConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public WaveConditionsCalculationConfiguration(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the hydraulic boundary location of the read calculation.
        /// </summary>
        public string HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Gets the upper boundary of the revetment of the read calculation.
        /// </summary>
        public double? UpperBoundaryRevetment { get; set; }

        /// <summary>
        /// Gets the lower boundary of the revetment of the read calculation.
        /// </summary>
        public double? LowerBoundaryRevetment { get; set; }

        /// <summary>
        /// Gets the upper boundary of the water levels of the read calculation.
        /// </summary>
        public double? UpperBoundaryWaterLevels { get; set; }

        /// <summary>
        /// Gets the lower boundary of the water levels of the read calculation.
        /// </summary>
        public double? LowerBoundaryWaterLevels { get; set; }

        /// <summary>
        /// Gets the step size of the read calculation.
        /// </summary>
        public ConfigurationWaveConditionsInputStepSize? StepSize { get; set; }

        /// <summary>
        /// Gets the name of the foreshore profile of the read calculation.
        /// </summary>
        public string ForeshoreProfile { get; set; }

        /// <summary>
        /// Gets the orientation of the read calculation.
        /// </summary>
        public double? Orientation { get; set; }

        /// <summary>
        /// Gets whether the breakwater should be used for the read calculation.
        /// </summary>
        public bool? UseBreakWater { get; set; }

        /// <summary>
        /// Gets the breakwater type of the read calculation.
        /// </summary>
        public ConfigurationBreakWaterType? BreakWaterType { get; set; }

        /// <summary>
        /// Gets the breakwater height of the read calculation.
        /// </summary>
        public double? BreakWaterHeight { get; set; }

        /// <summary>
        /// Gets whether the foreshore should be used for the read calculation.
        /// </summary>
        public bool? UseForeshore { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="WaveConditionsCalculationConfiguration"/>.
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
                    throw new ArgumentNullException(nameof(value), @"Name is required for a structure calculation configuration.");
                }
                name = value;
            }
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="WaveConditionsCalculationConfiguration"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.Name"/>.
            /// </summary>
            public string Name { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.HydraulicBoundaryLocation"/>.
            /// </summary>
            public string HydraulicBoundaryLocation { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.UpperBoundaryRevetment"/>.
            /// </summary>
            public double? UpperBoundaryRevetment { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.LowerBoundaryRevetment"/>.
            /// </summary>
            public double? LowerBoundaryRevetment { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.UpperBoundaryWaterLevels"/>.
            /// </summary>
            public double? UpperBoundaryWaterLevels { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.LowerBoundaryWaterLevels"/>.
            /// </summary>
            public double? LowerBoundaryWaterLevels { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.StepSize"/>.
            /// </summary>
            public ConfigurationWaveConditionsInputStepSize? StepSize { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.ForeshoreProfile"/>.
            /// </summary>
            public string ForeshoreProfile { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.Orientation"/>.
            /// </summary>
            public double? Orientation { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.UseBreakWater"/>.
            /// </summary>
            public bool? UseBreakWater { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.BreakWaterType"/>.
            /// </summary>
            public ConfigurationBreakWaterType? BreakWaterType { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.BreakWaterHeight"/>.
            /// </summary>
            public double? BreakWaterHeight { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="WaveConditionsCalculationConfiguration.UseForeshore"/>.
            /// </summary>
            public bool? UseForeshore { internal get; set; }
        }
    }
}