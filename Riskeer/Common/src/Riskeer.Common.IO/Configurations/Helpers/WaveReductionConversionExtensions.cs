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
using Riskeer.Common.Data;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Structures;

namespace Riskeer.Common.IO.Configurations.Helpers
{
    /// <summary>
    /// Extension methods for setting wave reduction properties of configuration elements.
    /// </summary>
    public static class WaveReductionConversionExtensions
    {
        /// <summary>
        /// Sets the <paramref name="configuration"/> using properties from <paramref name="input"/>, 
        /// when <see cref="StructuresInputBase{StructureBase}.ForeshoreProfile"/> is set.
        /// </summary>
        /// <typeparam name="T">The type of structure for <paramref name="input"/>.</typeparam>
        /// <param name="configuration">The configuration to update.</param>
        /// <param name="input">The structure input to update from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public static void SetConfigurationForeshoreProfileDependentProperties<T>(this StructuresCalculationConfiguration configuration,
                                                                                  StructuresInputBase<T> input) where T : StructureBase
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

            configuration.ForeshoreProfileId = input.ForeshoreProfile?.Id;
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