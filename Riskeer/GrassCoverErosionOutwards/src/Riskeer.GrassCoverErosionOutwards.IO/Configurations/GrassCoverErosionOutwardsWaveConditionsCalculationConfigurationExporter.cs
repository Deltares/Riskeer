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

using System.Collections.Generic;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.IO.Configurations.Converters;
using Ringtoets.Revetment.IO.Configurations;

namespace Riskeer.GrassCoverErosionOutwards.IO.Configurations
{
    /// <summary>
    /// Exports a grass cover erosion outwards wave conditions calculation configuration and stores it as an XML file.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationExporter : WaveConditionsCalculationConfigurationExporter<
        GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriter,
        GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration,
        GrassCoverErosionOutwardsWaveConditionsCalculation>
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationExporter"/>.
        /// </summary>
        public GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
            : base(calculations, filePath) {}

        protected override GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriter CreateWriter(string filePath)
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriter(filePath);
        }

        protected override GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration ToConfiguration(GrassCoverErosionOutwardsWaveConditionsCalculation calculation)
        {
            var configuration = new GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration(calculation.Name);
            SetConfigurationProperties(configuration, calculation);
            configuration.CategoryType = (ConfigurationGrassCoverErosionOutwardsCategoryType?) new ConfigurationGrassCoverErosionOutwardsCategoryTypeConverter()
                .ConvertFrom(calculation.InputParameters.CategoryType);
            return configuration;
        }
    }
}