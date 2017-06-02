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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.IO.Configurations.Helpers
{
    /// <summary>
    /// Extension methods for converting <see cref="WaveConditionsInput"/> to <see cref="WaveConditionsCalculationConfiguration"/>.
    /// </summary>
    public static class WaveConditionsInputExtensions
    {
        /// <summary>
        /// Converts a <see cref="WaveConditionsInput"/> to <see cref="WaveConditionsCalculationConfiguration"/>.
        /// </summary>
        /// <param name="input">The input to convert.</param>
        /// <param name="calculationName">The name to use for the resulting configuration.</param>
        /// <returns>A new <see cref="WaveConditionsCalculationConfiguration"/> with values 
        /// taken from <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static WaveConditionsCalculationConfiguration ToConfiguration(this WaveConditionsInput input, string calculationName)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            var calculationConfiguration = new WaveConditionsCalculationConfiguration(calculationName)
            {
                HydraulicBoundaryLocation = input.HydraulicBoundaryLocation?.Name,
                LowerBoundaryRevetment = input.LowerBoundaryRevetment,
                UpperBoundaryRevetment = input.UpperBoundaryRevetment,
                LowerBoundaryWaterLevels = input.LowerBoundaryWaterLevels,
                UpperBoundaryWaterLevels = input.UpperBoundaryWaterLevels,
                Orientation = input.Orientation,
                StepSize = (ConfigurationWaveConditionsInputStepSize?) new ConfigurationWaveConditionsInputStepSizeConverter().ConvertFrom(input.StepSize)
            };
            SetConfigurationForeshoreProfileDependendProperties(calculationConfiguration, input);
            return calculationConfiguration;
        }

        /// <summary>
        /// Sets the <paramref name="configuration"/> using properties from <paramref name="input"/>, 
        /// when <see cref="WaveConditionsInput.ForeshoreProfile"/> is set.
        /// </summary>
        /// <param name="configuration">The configuration to update.</param>
        /// <param name="input">The wave conditions input to update from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        private static void SetConfigurationForeshoreProfileDependendProperties(WaveConditionsCalculationConfiguration configuration,
                                                                               WaveConditionsInput input)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.ForeshoreProfile == null)
            {
                return;
            }

            configuration.ForeshoreProfile = input.ForeshoreProfile?.Id;
            configuration.WaveReduction = new WaveReductionConfiguration
            {
                UseForeshoreProfile = input.UseForeshore,
                UseBreakWater = input.UseBreakWater,
                BreakWaterHeight = input.BreakWater.Height
            };

            if (Enum.IsDefined(typeof(BreakWaterType), input.BreakWater.Type))
            {
                configuration.WaveReduction.BreakWaterType = (ConfigurationBreakWaterType?)
                    new ConfigurationBreakWaterTypeConverter().ConvertFrom(input.BreakWater.Type);
            }
        }
    }
}