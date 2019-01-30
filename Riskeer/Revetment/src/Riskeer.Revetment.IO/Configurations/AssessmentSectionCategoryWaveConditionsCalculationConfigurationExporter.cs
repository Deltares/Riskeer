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

using System.Collections.Generic;
using Riskeer.Common.Data.Calculation;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.IO.Configurations.Converters;

namespace Riskeer.Revetment.IO.Configurations
{
    /// <summary>
    /// Exports an assessment section category wave conditions calculation configuration and stores it as an XML file.
    /// </summary>
    public class AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporter : WaveConditionsCalculationConfigurationExporter<
        AssessmentSectionCategoryWaveConditionsCalculationConfigurationWriter,
        AssessmentSectionCategoryWaveConditionsCalculationConfiguration,
        ICalculation<AssessmentSectionCategoryWaveConditionsInput>>
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporter"/>.
        /// </summary>
        public AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
            : base(calculations, filePath) {}

        protected override AssessmentSectionCategoryWaveConditionsCalculationConfigurationWriter CreateWriter(string filePath)
        {
            return new AssessmentSectionCategoryWaveConditionsCalculationConfigurationWriter(filePath);
        }

        protected override AssessmentSectionCategoryWaveConditionsCalculationConfiguration ToConfiguration(
            ICalculation<AssessmentSectionCategoryWaveConditionsInput> calculation)
        {
            var configuration = new AssessmentSectionCategoryWaveConditionsCalculationConfiguration(calculation.Name);
            SetConfigurationProperties(configuration, calculation);
            configuration.CategoryType = (ConfigurationAssessmentSectionCategoryType?) new ConfigurationAssessmentSectionCategoryTypeConverter()
                .ConvertFrom(calculation.InputParameters.CategoryType);
            return configuration;
        }
    }
}