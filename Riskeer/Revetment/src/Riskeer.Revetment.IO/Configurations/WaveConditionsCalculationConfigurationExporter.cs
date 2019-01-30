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
using System.Collections.Generic;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Export;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.IO.Configurations.Converters;

namespace Riskeer.Revetment.IO.Configurations
{
    /// <summary>
    /// Base implementation of an exporter for a wave conditions calculation configuration that stores it as an XML file.
    /// </summary>
    /// <typeparam name="TWaveConditionsCalculationConfigurationWriter">The type of writer to use.</typeparam>
    /// <typeparam name="TWaveConditionsCalculationConfiguration">The type of calculation configuration to export.</typeparam>
    /// <typeparam name="TCalculation">The type of calculation to use.</typeparam>
    public abstract class WaveConditionsCalculationConfigurationExporter<TWaveConditionsCalculationConfigurationWriter, TWaveConditionsCalculationConfiguration, TCalculation>
        : CalculationConfigurationExporter<TWaveConditionsCalculationConfigurationWriter, TCalculation, TWaveConditionsCalculationConfiguration>
        where TWaveConditionsCalculationConfigurationWriter : CalculationConfigurationWriter<TWaveConditionsCalculationConfiguration>
        where TWaveConditionsCalculationConfiguration : WaveConditionsCalculationConfiguration
        where TCalculation : class, ICalculation<WaveConditionsInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfigurationExporter{TWaveConditionsCalculationConfigurationWriter,TWaveConditionsCalculationConfiguration,TCalculation}"/>.
        /// </summary>
        /// <param name="calculations">The hierarchy of calculations to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        protected WaveConditionsCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
            : base(calculations, filePath) {}

        /// <summary>
        /// Sets the properties of a <see cref="TCalculation"/> to a <see cref="TWaveConditionsCalculationConfiguration"/>.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation configuration to set the properties for.</param>
        /// <param name="calculation">The calculation to get the properties of.</param>
        protected void SetConfigurationProperties(TWaveConditionsCalculationConfiguration calculationConfiguration, TCalculation calculation)
        {
            WaveConditionsInput input = calculation.InputParameters;
            calculationConfiguration.HydraulicBoundaryLocationName = input.HydraulicBoundaryLocation?.Name;
            calculationConfiguration.LowerBoundaryRevetment = input.LowerBoundaryRevetment;
            calculationConfiguration.UpperBoundaryRevetment = input.UpperBoundaryRevetment;
            calculationConfiguration.LowerBoundaryWaterLevels = input.LowerBoundaryWaterLevels;
            calculationConfiguration.UpperBoundaryWaterLevels = input.UpperBoundaryWaterLevels;
            calculationConfiguration.Orientation = input.Orientation;
            calculationConfiguration.StepSize = (ConfigurationWaveConditionsInputStepSize?) new ConfigurationWaveConditionsInputStepSizeConverter().ConvertFrom(input.StepSize);

            SetConfigurationForeshoreProfileDependentProperties(calculationConfiguration, input);
        }

        private static void SetConfigurationForeshoreProfileDependentProperties(WaveConditionsCalculationConfiguration configuration,
                                                                                WaveConditionsInput input)
        {
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