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

using System.Xml;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.IO.Configurations.Converters;

namespace Riskeer.Revetment.IO.Configurations
{
    /// <summary>
    /// Writer for calculations that contain <see cref="AssessmentSectionCategoryWaveConditionsInput"/> as input,
    /// to XML format.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of the configuration</typeparam>
    public class AssessmentSectionCategoryWaveConditionsCalculationConfigurationWriter<TConfiguration>
        : WaveConditionsCalculationConfigurationWriter<TConfiguration>
        where TConfiguration : AssessmentSectionCategoryWaveConditionsCalculationConfiguration
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionCategoryWaveConditionsCalculationConfigurationWriter{TConfiguration}"/>.
        /// </summary>
        public AssessmentSectionCategoryWaveConditionsCalculationConfigurationWriter(string filePath)
            : base(filePath) {}

        protected override void WriteWaveConditionsSpecificParameters(
            XmlWriter writer, TConfiguration configuration)
        {
            if (!configuration.CategoryType.HasValue)
            {
                return;
            }

            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();
            writer.WriteElementString(WaveConditionsCalculationConfigurationSchemaIdentifiers.CategoryType,
                                      converter.ConvertToInvariantString(configuration.CategoryType.Value));
        }
    }
}