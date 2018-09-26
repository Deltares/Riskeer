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
    /// Class that represents a wave conditions calculation configuration.
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
        /// Gets or sets the name of the hydraulic boundary location of the calculation.
        /// </summary>
        public string HydraulicBoundaryLocationName { get; set; }

        /// <summary>
        /// Gets or sets the upper boundary of the revetment of the calculation.
        /// </summary>
        public double? UpperBoundaryRevetment { get; set; }

        /// <summary>
        /// Gets or sets the lower boundary of the revetment of the calculation.
        /// </summary>
        public double? LowerBoundaryRevetment { get; set; }

        /// <summary>
        /// Gets or sets the upper boundary of the water levels of the calculation.
        /// </summary>
        public double? UpperBoundaryWaterLevels { get; set; }

        /// <summary>
        /// Gets or sets the lower boundary of the water levels of the calculation.
        /// </summary>
        public double? LowerBoundaryWaterLevels { get; set; }

        /// <summary>
        /// Gets or sets the step size of the calculation.
        /// </summary>
        public ConfigurationWaveConditionsInputStepSize? StepSize { get; set; }

        /// <summary>
        /// Gets or sets the id of the foreshore profile of the calculation.
        /// </summary>
        public string ForeshoreProfileId { get; set; }

        /// <summary>
        /// Gets or sets the orientation of the calculation.
        /// </summary>
        public double? Orientation { get; set; }

        /// <summary>
        /// Gets or sets the wave reduction configuration.
        /// </summary>
        public WaveReductionConfiguration WaveReduction { get; set; }

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
                    throw new ArgumentNullException(nameof(value), @"Name is required for a calculation configuration.");
                }

                name = value;
            }
        }
    }
}