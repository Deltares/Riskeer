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
using Riskeer.Revetment.IO.Configurations;
using Riskeer.Revetment.IO.Configurations.Converters;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.IO.Configurations.Converters;

namespace Riskeer.StabilityStoneCover.IO.Configurations
{
    /// <summary>
    /// Exports a stability stone cover wave conditions calculation configuration and stores it as an XML file.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculationConfigurationExporter : WaveConditionsCalculationConfigurationExporter<StabilityStoneCoverWaveConditionsCalculationConfigurationWriter,
        StabilityStoneCoverWaveConditionsCalculationConfiguration, ICalculation<StabilityStoneCoverWaveConditionsInput>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsCalculationConfigurationExporter"/>.
        /// </summary>
        /// <param name="calculations">The hierarchy of calculations to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public StabilityStoneCoverWaveConditionsCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
            : base(calculations, filePath) {}

        protected override StabilityStoneCoverWaveConditionsCalculationConfigurationWriter CreateWriter(string filePath)
        {
            return new StabilityStoneCoverWaveConditionsCalculationConfigurationWriter(filePath);
        }

        protected override StabilityStoneCoverWaveConditionsCalculationConfiguration ToConfiguration(
            ICalculation<StabilityStoneCoverWaveConditionsInput> calculation)
        {
            var configuration = new StabilityStoneCoverWaveConditionsCalculationConfiguration(calculation.Name);
            SetConfigurationProperties(configuration, calculation);
            configuration.CategoryType = (ConfigurationAssessmentSectionCategoryType?) new ConfigurationAssessmentSectionCategoryTypeConverter()
                .ConvertFrom(calculation.InputParameters.CategoryType);
            configuration.CalculationType = (ConfigurationStabilityStoneCoverCalculationType?) new ConfigurationStabilityStoneCoverCalculationTypeConverter()
                .ConvertFrom(calculation.InputParameters.CalculationType);
            return configuration;
        }
    }
}